using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetGame.Core;
using PetGame.Models;
using PetGame.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PetGame
{
    public class LoginService
    {
        private readonly SqlManager sqlManager;

        public LoginService(SqlManager sqlManager)
        {
            this.sqlManager = sqlManager;
        }

        // get only the username and user id that owns this token
        // return null if invalid or out of date

        /// <summary>
        ///     Gets the <see cref="User"/> that owns 
        ///     the given token, if the token is valid and not expired.
        /// </summary>
        /// <param name="token">
        ///     The user token to check.
        /// </param>
        /// <returns>
        ///     A new <see cref="User"/> if the token was valid, 
        ///     or null if invalid.
        ///     Sensitive properties like the password hash and 
        ///     HMAC key are not included
        /// </returns>
        private User GetUserFromToken(string token)
        {
            // validation of the token
            if (string.IsNullOrWhiteSpace(token))
                return null;

            ulong userid = 0;
            using (var conn = sqlManager.EstablishDataConnection)
            {
                // 0 is error case
                ulong usertokenid = 0;

                var cmd = conn.CreateCommand();
                // get the user id and user token id from the UserToken token
                // where the token has been used in the last 3 days
                cmd.CommandText =
                    @"SELECT UserId, UserTokenId FROM UserToken WHERE Token = @Token AND GETDATE() <= DATEADD(day, 3, GETDATE());";
                // add the token parameter
                cmd.Parameters.AddWithValue("@Token", token);

                using (var reader = cmd.ExecuteReader())
                {
                    // read the results
                    while (reader.Read())
                    {
                        userid = (ulong)reader.GetInt64(0);
                        usertokenid = (ulong)reader.GetInt64(1);
                    }
                }

                // query returned no results, or invalid
                if (userid == 0 && usertokenid == 0)
                {
                    // return null for no results
                    return null;
                }

                // update the last used time
                var updatecmd = conn.CreateCommand();
                updatecmd.CommandText =
                    @"UPDATE UserToken SET LastUsed = GETDATE() WHERE UserTokenId = @UserTokenId;";
                updatecmd.Parameters.AddWithValue("@UserTokenId", usertokenid.ToString());
                updatecmd.ExecuteNonQuery();

                // todo delete all UserToken that are out of date
            }

            return GetUserById(userid);
        }

        /// <summary>
        ///     Gets a new <see cref="User"/> for the given ID.
        /// </summary>
        /// <param name="id">
        ///     The user's id to get.
        ///     0 is invalid.
        /// </param>
        /// <returns>
        ///     A new User object if found, null otherwise.
        /// </returns>
        private User GetUserById(ulong id)
        {
            // todo add more input validation to GetUserById
            if (id <= 0) return null;

            User ret = null;

            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"SELECT UserId, Username FROM [User] WHERE UserId = @UserId;";
                cmd.Parameters.AddWithValue("@UserId", id.ToString());

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        ret = new User();
                        ret.UserId = id;
                        ret.Username = r.GetString(1);
                    }
                }
            }
            return ret;
        }   

        /// <summary>
        ///     Gets the User from the HttpContext.User, if 
        ///     there is a User logged in.
        /// </summary>
        /// <param name="contextClaimsPrincipal"></param>
        /// <returns>
        ///     A <see cref="User"/> for the given 
        ///     claims, or null if invalid or out of date
        /// </returns>
        public User GetUserFromContext(ClaimsPrincipal contextClaimsPrincipal)
        {
            string z = null;
            if (contextClaimsPrincipal.HasClaim(x => x.Type == ClaimTypes.UserData))
            {
                var claim = contextClaimsPrincipal.Claims.First(x => x.Type == ClaimTypes.UserData);
                z = claim.Value;
            }

            return GetUserFromToken(z);
        }

        /// <summary>
        ///     Handles the logic for getting the user token from
        ///     the supplied credentials.
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <returns></returns>
        public UserToken GetUserToken(ControllerBase controllerContext)
        {
            // create LoginModel from the request
            UserLoginModel data = new UserLoginModel();

            // if request data is from the from, populate LoginModel from the form
            if (controllerContext.Request.HasFormContentType)
            {
                data.username = controllerContext.Request.Form["username"];
                data.password = controllerContext.Request.Form["password"];
            }
            else
            {
                // get the LoginModel passed in the request body
                if (controllerContext.Request.ContentType == "application/json")
                {
                    StringWriter s = new StringWriter();
                    using (var sr = new StreamReader(controllerContext.Request.Body))
                    {
                        s.Write(sr.ReadToEnd());
                    }
                    data = JsonConvert.DeserializeObject<UserLoginModel>(s.ToString());
                }
            }

            if (data == null)
                throw new ArgumentNullException(nameof(data), "The login data may not be null.");

            //TODO: Run username and password against regex validation rules
            if (string.IsNullOrWhiteSpace(data.username))
                throw new ArgumentException("Neither the Username or Password may be null or whitespace.");
            if (string.IsNullOrWhiteSpace(data.password))
                throw new ArgumentException("Neither the Username or Password may be null or whitespace.");

            var user = CheckUsernamePassword(data.username, data.password);
            // username and password matches
            if (user != null)
            {
                return MakeUserToken(controllerContext, user);
            }
            return null;
        }

        /// <summary>
        ///     Checks if the given username and password are valid.
        /// </summary>
        /// <returns>
        ///     A User object for this user if the password matches, null otherwise.
        /// </returns>
        private User CheckUsernamePassword(string username, string password)
        {
            // get the User for the given username
            var user = GetUser(username);
            if (user != null && Cryptography.VerifyUserPassword(user, password))
            {
                return user;
            }
            return null;
        }

        private User GetUser(string username)
        {
            User ret = null;
            using (var sql = sqlManager.EstablishDataConnection)
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText =
@"SELECT TOP 1 UserId, Username, PasswordHash, HMACKey FROM [User] WHERE Username = @Username;";
                cmd.Parameters.AddWithValue("@Username", username);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret = new User();
                        ret.UserId = (ulong)reader.GetInt64(0);
                        ret.Username = reader.GetString(1);
                        ret.PasswordHash = reader.GetSqlBytes(2).Value;
                        ret.HMACKey = reader.GetSqlBytes(3).Value;
                    }

                    reader.Close();
                }
            }
            return ret;
        }

        private void InsertToken(UserToken userToken)
        {
            if (userToken == null) return;

            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"INSERT INTO UserToken (UserId, Token, LastUsed, Created) VALUES (@UserId, @Token, GETDATE(), GETDATE());";
                cmd.Parameters.AddWithValue("@UserId", userToken.UserId.ToString());
                cmd.Parameters.AddWithValue("@Token", userToken.Token);
                
                cmd.ExecuteNonQuery();
            }
        }

        private UserToken MakeUserToken(ControllerBase controllerContext, User user)
        {
            // get a user token for this suer
            var ut = Cryptography.MakeUserToken(user);

            // insert a new user token
            InsertToken(ut);

            // TODO: re-use tokens that are already valid instead of just inserting new ones
            // TODO: Consider tracking user agent with user tokens
            controllerContext.Response.StatusCode = 200;

            // delete the cookie
            controllerContext.Response.Cookies.Delete("auth_token");

            // set up cookies
            controllerContext.Response.Cookies.Append("auth_token", ut.Token, new Microsoft.AspNetCore.Http.CookieOptions()
            {
                HttpOnly = true
            });

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var userid = new ClaimsIdentity(claims, "auth_token");
            var pr = new ClaimsPrincipal(userid);

            controllerContext.HttpContext.SignInAsync(pr).Wait();
            return ut;
        }

        /// <summary>
        /// Creates and inserts a new user with the given username and password into the database.
        /// </summary>
        private User InsertNewUser(string username, string plaintextPassword)
        {
            // TODO apply validation to username and password
            User u = new User()
            {
                Username = username
            };

            Cryptography.SetUserPassword(u, plaintextPassword);

            // insert this user, and get the id
            using (var s = sqlManager.EstablishDataConnection)
            {
                var cmd = s.CreateCommand();
                cmd.CommandText = "INSERT INTO [User] (Username, PasswordHash, HMACKey) OUTPUT INSERTED.UserID VALUES (@Username, @PasswordHash, @HMACKey);";
                cmd.Parameters.AddWithValue("@Username", u.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", u.PasswordHash);
                cmd.Parameters.AddWithValue("@HMACKey", u.HMACKey);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        u.UserId = (ulong)reader.GetInt64(0);
                    }
                    reader.Close();
                }
            }

            // return the new User object
            return u;
        }

        /// <summary>
        /// Registers a new user with the supplied credentials, and returns it's usertoken.
        /// </summary>
        /// <returns></returns>
        public UserToken RegisterNewUser(ControllerBase controllerContext)
        {
            // create LoginModel from the request
            UserLoginModel data = new UserLoginModel();

            // if request data is from the from, populate LoginModel from the form
            if (controllerContext.Request.HasFormContentType)
            {
                data.username = controllerContext.Request.Form["username"];
                data.password = controllerContext.Request.Form["password"];
            }
            else
            {
                // get the LoginModel passed in the request body
                if (controllerContext.Request.ContentType == "application/json")
                {
                    StringWriter s = new StringWriter();
                    using (var sr = new StreamReader(controllerContext.Request.Body))
                    {
                        s.Write(sr.ReadToEnd());
                    }
                    data = JsonConvert.DeserializeObject<UserLoginModel>(s.ToString());
                }
            }

            if (data == null)
                throw new ArgumentNullException(nameof(data), "The login data may not be null.");

            //TODO: Run username and password against regex validation rules
            if (string.IsNullOrWhiteSpace(data.username))
                throw new ArgumentException("Neither the Username or Password may be null or whitespace.");
            if (string.IsNullOrWhiteSpace(data.password))
                throw new ArgumentException("Neither the Username or Password may be null or whitespace.");

            // register a new user
            var user = InsertNewUser(data.username, data.password);

            // return the user token for this user
            return MakeUserToken(controllerContext, user);
        }
    }
}
