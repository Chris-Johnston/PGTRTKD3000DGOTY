using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using PetGame.Models;

namespace PetGame.Util
{
    /// <summary>
    ///     Static methods that are used for Cryptographic actions.
    /// </summary>
    public static class Cryptography
    {
        const int TokenSize = 256;
        const int UserTokenSize = 64;

        /// <summary>
        ///     Verifies the given plain-text password with a 
        ///     <see cref="User"/> to ensure that the hashes
        ///     match.
        /// </summary>
        /// <returns>
        ///     True if the passwords matched, false otherwise.
        /// </returns>
        /// <param name="plaintextPassword">
        ///     The user's plaintext password, as they typed it into the form.
        ///     (Should we hash this client-side?)
        /// </param>
        /// <param name="user">
        ///     A reference to the <see cref="User"/> instance that this
        ///     user is trying to authenticate as.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if either the user or password parameters are null.
        /// </exception>
        public static bool VerifyUserPassword(User user, string plaintextPassword)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "The user parameter may not be null.");
            if (string.IsNullOrWhiteSpace(plaintextPassword))
                throw new ArgumentNullException(nameof(plaintextPassword), "The password may not be null or whitespace.");
            // create a new HMAC SHA512 from the user's unique HMAC key
            using (HMACSHA512 hmac = new HMACSHA512(user.HMACKey))
            {
                // create a reader for the pw string
                using (var sreader = new MemoryStream(Encoding.ASCII.GetBytes(plaintextPassword)))
                {
                    var result = hmac.ComputeHash(sreader);
                    return CryptographicCompare(user.PasswordHash, result);
                }
            }
        }

        /// <summary>
        ///     Makes a new <see cref="UserToken"/> for the given user.
        /// </summary>
        /// <param name="user">
        ///     The user to create the token for.
        /// </param>
        /// <returns>
        ///      A new UserToken.
        /// </returns>
        public static UserToken MakeUserToken(User user)
        {
            var ut = new UserToken()
            {
                UserId = user.UserId,
                Created = DateTime.Now,
                LastUsed = DateTime.Now
            };

            // generate a new token
            var bytes = new byte[UserTokenSize];
            var gen = RandomNumberGenerator.Create();
            // get a bunch of random bytes
            gen.GetBytes(bytes);
            // encode as a string
            ut.Token = Convert.ToBase64String(bytes);
            // return this value
            return ut;
        }

        public static bool CryptographicCompare(byte[] a, byte[] b)
        {
            // don't bother with null arrays
            if (a == null || b == null)
                return false;
            bool eq = true;
            for (int i = 0; i < a.Length; i++)
            {
                if (i < b.Length)
                {
                    if (a[i] != b[i])
                        eq = false;
                }
            }
            return eq && a.Length == b.Length;
        }

        public static void SetUserHMACKey(User user)
        {
            // fill the HMACKey with 256 random bytes
            var gen = RandomNumberGenerator.Create();
            user.HMACKey = new byte[TokenSize];
            gen.GetBytes(user.HMACKey);
        }

        /// <summary>
        ///     Updates a user's HMAC Key and password hash
        /// </summary>
        public static void SetUserPassword(User user, string plainTextPassword)
        {
            // check parameters
            if (user == null)
                throw new ArgumentNullException(nameof(user), "The user parameter may not be null.");
            if (string.IsNullOrEmpty(plainTextPassword))
                throw new ArgumentNullException(nameof(plainTextPassword), "The plaintext password parameter may not be null.");

            // set a new hmac key for the user as well
            SetUserHMACKey(user);

            using (HMACSHA512 hmac = new HMACSHA512(user.HMACKey))
            {
                // create a reader for the pw string
                using (var sreader = new MemoryStream(Encoding.UTF8.GetBytes(plainTextPassword)))
                {
                    // set the new password hash
                    user.PasswordHash = hmac.ComputeHash(sreader);
                }
            }
        }
    }
}
