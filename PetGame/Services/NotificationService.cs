using Newtonsoft.Json;
using PetGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace PetGame.Services
{
    // see: https://www.twilio.com/docs/sms/quickstart/csharp-dotnet-core

    /// <summary>
    ///     Service that notifies users.
    /// </summary>
    public class NotificationService
    {
        // private configuration defined by environment variables
        private readonly string AccountSid;
        private readonly string AuthToken;
        private readonly string PhoneNum;
        private readonly string DebugNum;
        private readonly bool OnlyDebugNum;

        // phone # being sent from
        private readonly PhoneNumber FromPhone;
        // debug number to send to
        private readonly PhoneNumber ToDebug;

        // toggles the state of this service
        // default disabled
        private readonly bool TwilioEnable;
        private readonly bool DiscordEnable;

        private readonly string DiscordWebhook;

        const string ENV_VAR_ACCOUNTSID = "PETGAME_TWILIO_ACCOUNTSID";
        const string ENV_VAR_AUTHTOKEN = "PETGAME_TWILIO_AUTHTOKEN";
        // the twilio phone # we are sending from
        const string ENV_VAR_PHONENUM = "PETGAME_TWILIO_PHONENUM";
        // the debug phone #
        // free trial of twilio requires that each phone number is verified beforehand
        // so we may have to only send messages to the debug phone number instead
        const string ENV_VAR_DEBUGNUM = "PETGAME_TWILIO_DEBUGNUM";
        // enables this service, set to "True" to be enabled
        const string ENV_VAR_ENABLE = "PETGAME_TWILIO_ENABLE";
        // forces twilio to only use the debug phone number, set to "True" to be enabled
        const string ENV_VAR_DEBUGONLY = "PETGAME_TWILIO_DEBUGONLY";
        // if "True", this service should use discord webhooks
        const string ENV_VAR_USEWEBHOOK = "PETGAME_DISCORD_USEWEBHOOK";
        // the url to the discord webhook
        const string ENV_VAR_WEBHOOK = "PETGAME_DISCORD_WEBHOOK";

        public NotificationService()
        {
            // check if service is enabled
            var enableStr = Environment.GetEnvironmentVariable(ENV_VAR_ENABLE);
            TwilioEnable = !string.IsNullOrWhiteSpace(enableStr) && (enableStr == "True");

            var discordEnable = Environment.GetEnvironmentVariable(ENV_VAR_USEWEBHOOK);
            DiscordEnable = !string.IsNullOrWhiteSpace(discordEnable) && (discordEnable == "True");

            // if not enabled, don't bother doing anything else
            if (TwilioEnable)
            {
                OnlyDebugNum = Environment.GetEnvironmentVariable(ENV_VAR_DEBUGONLY) == "True";
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
                // if invalid, let it throw
                FromPhone = new PhoneNumber(PhoneNum);
                ToDebug = new PhoneNumber(DebugNum);
            }

            if (DiscordEnable)
            {
                // don't validate this
                DiscordWebhook = Environment.GetEnvironmentVariable(ENV_VAR_WEBHOOK);
            }
        }

        /// <summary>
        ///     Sends a message to the given phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number of the user to send message to.</param>
        /// <param name="message">The message to send</param>
        /// <param name="useDebugNumber">If enabled, ignores the phoneNumber param and always sends the message to the debug number.</param>
        /// <exception cref="ArgumentNullException">Thrown if the given message is null or whitespace.</exception>
        public void SendMessage(string phoneNumber, string message)
        {
            // ensure enabled
            if (!TwilioEnable)
                return;
            // use the debug number if enforced
            PhoneNumber to;
            if (OnlyDebugNum)
                to = DebugNum;
            // otherwise get the number from input
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
            try
            {
                MessageResource.Create(to, from: FromPhone, body: message);
            }
            catch (Twilio.Exceptions.ApiException e)
            {
                // can be thrown if the phone number is not valid
                // fail silently
            }
        }

        /// <summary>
        ///     Sends a message using discord webhooks
        /// </summary>
        /// <param name="message"></param>
        public void SendDiscordWebhookNotification(string message)
        {
            // perform some basic sanitization, to prevent users injecting @everyone's
            // so pad @'s with zero width space
            var msg = SanitizeForPings(message);

            using (var client = new HttpClient())
            {
                // https://discordapp.com/developers/docs/resources/webhook#execute-webhook
                // inline anon type that matches the params of the discord webhook execute endpoint
                var content = new { content = msg, username = "PGTRTKD3000DGOTY", avatar_url = "https://raw.githubusercontent.com/Chris-Johnston/PGTRTKD3000DGOTY/master/PetRaceTurbo30xx.png" };
                var httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
                var uri = new Uri(DiscordWebhook);
                // post, ignore the result
                var result = client.PostAsync(uri, httpContent).Result;
            }
        }

        const char zwsp = '​';

        private string SanitizeForPings(string message)
            => message.Replace("@", $"@{zwsp}");

        public void SendDiscordNotifyHighScore(long rank, int score, Pet pet, User owner)
        {
            if (!DiscordEnable) return;
            if (pet == null || owner == null) return;

            string petImageUrl = $"http://pgtrtkd3000dgoty.fun/api/image/{pet.PetImageId}";

            using (var client = new HttpClient())
            {
                // https://discordapp.com/developers/docs/resources/webhook#execute-webhook
                // inline anon type that matches the params of the discord webhook execute endpoint
                var content = new { content = " ", username = "PGTRTKD3000DGOTY", avatar_url = petImageUrl,
                    embeds = new[] {
                        new { title = "New High Score!", url = "http://pgtrtkd3000dgoty.fun/leaderboard", image = new { url = petImageUrl },
                            color = 0x4ef442,
                            author = new { name = "PGTRTKD3000DGOTY(tm)", url = "http://pgtrtkd3000dgoty.fun", icon_url = "https://raw.githubusercontent.com/Chris-Johnston/PGTRTKD3000DGOTY/master/PetRaceTurbo30xx.png" },
                            description =
                        $"Wow! **{SanitizeForPings(owner.Username)}** just placed **#{rank}** on the leaderboard with a score of **{score}** with their pet **{SanitizeForPings(pet.Name)}**" }
                    }
                };
                var httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
                if (!string.IsNullOrWhiteSpace(DiscordWebhook))
                {
                    var uri = new Uri(DiscordWebhook);
                    // post, ignore the result
                    var result = client.PostAsync(uri, httpContent).Result;
                }
            }
        }
    }
}
