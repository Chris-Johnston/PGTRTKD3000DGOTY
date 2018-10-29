using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PetGame.Models;

namespace PetGame.Util
{
    public class Cryptography
    {
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
            user.HMACKey = new byte[256];
            gen.GetBytes(user.HMACKey);
        }

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
                using (var sreader = new MemoryStream(Encoding.ASCII.GetBytes(plainTextPassword)))
                {
                    // set the new password hash
                    user.PasswordHash = hmac.ComputeHash(sreader);
                }
            }
        }
    }
}
