using System;
using System.Collections.Generic;
using System.Text;

namespace PetGame.Core
{
    /// <summary>
    ///     Exception that is thrown when a user's password is insecure.
    ///     This will happen when it does not meet the minimum password requirements.
    /// </summary>
    public class InsecurePasswordException : Exception
    {
        public InsecurePasswordException() : base() { }
        public InsecurePasswordException(string message) : base(message) { }
    }
}
