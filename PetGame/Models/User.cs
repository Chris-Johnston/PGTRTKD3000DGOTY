using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetGame.Models
{
    /// <summary>
    ///     Represents information about a user.
    /// </summary>
    public class User
    {
        /// <summary>
        ///     A user's unique identifier.
        /// </summary>
        public ulong UserId { get; set; }
        // yes, I am being optimistic by making the UserId an unsigned long

        //TODO: Define a Regex for checking validity of usernames.

        /// <summary>
        ///     A user's name.
        ///     This must not be null or whitespace, and must be less than 32 characters in length.
        ///     It should match a-zA-Z0-9!?.
        /// </summary>
        public string Username { get; set; }

        //TODO: Define a regex for minimum (plaintext) password requirements.

        /// <summary>
        ///     Represents a user's hash value.
        /// </summary>
        /// <remarks>
        ///     We will use a HMAC SHA512 password hash method.
        ///     See https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hmacsha512
        /// </remarks>
        public byte[] PasswordHash { get; set; }

        /// <summary>
        ///     Represents the unique key that is used when checking this user's HMAC SHA512 password hash.
        ///     This should be randomly-generated and unique for this user, and also combined with a constant 
        ///     and known key stored elsewhere.
        ///     This information should be kept very private.
        /// </summary>
        /// <remarks>
        ///     This byte array will be 64 bytes in length.
        /// </remarks>
        public byte[] HMACKey { get; set; }
    }
}
