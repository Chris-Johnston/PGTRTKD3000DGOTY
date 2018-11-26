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

        /// <summary>
        ///     Handles the logic for getting the user token from
        ///     the supplied credentials.
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <returns></returns>
        public UserToken GetUserToken(ControllerBase controllerContext)
        {
            // create LoginModel from the request
            LoginModel data = new LoginModel();

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
                    data = JsonConvert.DeserializeObject<LoginModel>(s.ToString());
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

            // TODO, re-use tokens and upload them into the SQL server
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
            LoginModel data = new LoginModel();

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
                    data = JsonConvert.DeserializeObject<LoginModel>(s.ToString());
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
