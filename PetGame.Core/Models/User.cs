using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PetGame.Models
{
    /// <summary>
    ///     Represents information about a user.
    /// </summary>
    public class User
    {
        /// <summary>
        ///     Regular expression for valid usernames. All usernames must pass this validation.
        ///     Allows for names betwen [2, 50] chars (inclusive) with the supported characters.
        /// </summary>
        public const string UsernameRegex = @"^[^\s]([$@._/-?!0-9a-zA-Z]){2,50}[^\s]$";

        /// <summary>
        ///     A user's unique identifier.
        /// </summary>
        public ulong UserId { get; set; }
        // yes, I am being optimistic by making the UserId an unsigned long
            
        /// <summary>
        ///     A user's name.
        ///     This must not be null or whitespace, and must be less than 32 characters in length.
        ///     It should match the regex defined by <see cref="UsernameRegex"/>
        /// </summary>
        public string Username
        {
            get => _Username;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value), "Usernames may not be null or whitespace.");
                }
                if (!Regex.IsMatch(value, UsernameRegex))
                {
                    throw new ArgumentException("The username must match the username requirements.", nameof(value));
                }
                _Username = value;
            }
        }
        private string _Username;

        /// <summary>
        ///     Represents a user's hash value.
        /// </summary>
        /// <remarks>
        ///     We will use a HMAC SHA512 password hash method.
        ///     See https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hmacsha512
        ///     This byte array will be 64 bytes in length.
        /// </remarks>
        public byte[] PasswordHash { get; set; }

        /// <summary>
        ///     Represents the unique key that is used when checking this user's HMAC SHA512 password hash.
        ///     This should be randomly-generated and unique for this user, and also combined with a constant 
        ///     and known key stored elsewhere.
        ///     This information should be kept very private.
        /// </summary>
        /// <remarks>
        ///     This byte array will be 256 bytes in length.
        /// </remarks>
        public byte[] HMACKey { get; set; }
    }
}
