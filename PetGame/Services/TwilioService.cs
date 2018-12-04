using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace PetGame.Services
{
    // see: https://www.twilio.com/docs/sms/quickstart/csharp-dotnet-core

    /// <summary>
    ///     Service that abstracts the usage of the Twilio API library.
    /// </summary>
    public class TwilioService
    {
        // private configuration defined by environment variables
        private readonly string AccountSid;
        private readonly string AuthToken;
        private readonly string PhoneNum;
        private readonly string DebugNum;

        // phone # being sent from
        private readonly PhoneNumber FromPhone;
        // debug number to send to
        private readonly PhoneNumber ToDebug;

        // toggles the state of this service
        // default disabled
        private readonly bool Enable;

        const string ENV_VAR_ACCOUNTSID = "PETGAME_TWILIO_ACCOUNTSID";
        const string ENV_VAR_AUTHTOKEN = "PETGAME_TWILIO_AUTHTOKEN";
        // the twilio phone # we are sending from
        const string ENV_VAR_PHONENUM = "PETGAME_TWILIO_PHONENUM";
        // the debug phone #
        // free trial of twilio requires that each phone number is verified beforehand
        // so we may have to only send messages to the debug phone number instead
        const string ENV_VAR_DEBUGNUM = "PETGAME_TWILIO_DEBUG";
        // enables this service, set to "True", "true", "on", "enable" to be enabled
        const string ENV_VAR_ENABLE = "PETGAME_TWILIO_ENABLE";

        public TwilioService()
        {
            // check if service is enabled
            var enableStr = Environment.GetEnvironmentVariable(ENV_VAR_ENABLE);
            Enable = !string.IsNullOrWhiteSpace(enableStr) && (enableStr == "True" || enableStr == "true" || enableStr == "on" || enableStr == "enable");

            // if not enabled, don't bother doing anything else
            if (!Enable)
                return;

            // get configuration variables from environment
            AccountSid = Environment.GetEnvironmentVariable(ENV_VAR_ACCOUNTSID);
            AuthToken = Environment.GetEnvironmentVariable(ENV_VAR_AUTHTOKEN);
            PhoneNum = Environment.GetEnvironmentVariable(ENV_VAR_PHONENUM);
            DebugNum = Environment.GetEnvironmentVariable(ENV_VAR_DEBUGNUM);
            
            // validate these
            if (string.IsNullOrWhiteSpace(AccountSid) || string.IsNullOrWhiteSpace(AuthToken) || string.IsNullOrWhiteSpace(PhoneNum) || string.IsNullOrWhiteSpace(DebugNum))
            {
                throw new InvalidOperationException("One or more of the Twilio environment variables were not set correctly.");
            }

            // convert strings to PhoneNumbers
            FromPhone = new PhoneNumber(PhoneNum);
            ToDebug = new PhoneNumber(DebugNum);
        }

        /// <summary>
        ///     Sends a message to the given phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number of the user to send message to.</param>
        /// <param name="message">The message to send</param>
        /// <param name="useDebugNumber">If enabled, ignores the phoneNumber param and always sends the message to the debug number.</param>
        /// <exception cref="ArgumentNullException">Thrown if the given message is null or whitespace.</exception>
        public void SendMessage(string phoneNumber, string message, bool useDebugNumber = false)
        {
            // ensure enabled
            if (!Enable)
                return;

            PhoneNumber to;
            if (useDebugNumber)
                to = DebugNum;
            else if (!string.IsNullOrWhiteSpace(phoneNumber))
                to = new PhoneNumber(phoneNumber);
            else
                // don't do anything 
                return;

            // validate the message
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(paramName: nameof(message), message: "The supplied message may not be null or whitespace.");

            // init the client
            TwilioClient.Init(AccountSid, AuthToken);
            // create the message and send it
            MessageResource.Create(to, from: FromPhone, body: message);
        }
    }
}
