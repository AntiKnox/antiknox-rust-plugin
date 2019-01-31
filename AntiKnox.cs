using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Oxide.Plugins
{

    [Info("AntiKnox", "AntiKnox", "1.0.0")]
    [Description("Disallow VPNs from accessing your server.")]
    class AntiKnox : RustPlugin
    {

        private string authKey;
        private bool loginWarningShown;

        protected override void LoadConfig()
        {
            base.LoadConfig();

            authKey = (string)Config["Key"];
        }

        void Loaded()
        {
            tryPrintSetupGuide();
        }

        protected override void LoadDefaultConfig()
        {
            Config["Key"] = "<PUT YOUR ANTIKNOX KEY HERE>";
            authKey = null;

            SaveConfig();
        }

        private void tryPrintSetupGuide()
        {
            if (authKey == null || authKey.Length != 64)
            {
                authKey = null;
                printSetupGuide();
            }
        }

        private void printSetupGuide()
        {
            var warning = @"
Welcome to AntiKnox!

Before you can begin filtering VPN connections, you will need
to set your 64-character auth key. Enter the following command
to secure your server:

antiknox.setkey your64characterlonglicensekeyfromyourdashboard

Alternatively, you can edit the config file 'AntiKnox.json'
and set the 'Key' to your auth key manually.

If you don't have a license key yet, sign up for a free account
at https://www.antiknox.net to obtain one.
";

            foreach (var line in warning.Split('\n'))
            {
                PrintWarning(line);
            }
        }

        private void printSetupSuccess()
        {
            var message = @"
Nice - you're all set! Welcome to AntiKnox!

From now on, AntiKnox will guard your Rust server from VPNs, proxies
and other IP hiders. You've taken a big step in increasing your server's
security and playability. Buh-bye, criminals!

Should you require assistance, you can reach out to us at:
https://www.antiknox.com/dashboard
";

            foreach (var line in message.Split('\n'))
            {
                PrintWarning(line);
            }
        }

        object CanClientLogin(Network.Connection connection)
        {
            // If the auth key is not defined we allow the connection.
            if (authKey == null || authKey.Length != 64)
            {
                // If this is the first player logging in, we send a helpful
                // message indicating we're not set up yet.
                if (!loginWarningShown)
                {
                    printSetupGuide();
                }

                return true;
            }

            var host = connection.ipaddress.Split(':')[0];

            var req = UnityWebRequest.Get("https://api.antiknox.net/lookup/" + host + "?auth=" + authKey);
            var state = req.SendWebRequest();

            while (!state.isDone)
            {
                ;
            }

            if (req.isNetworkError || req.isHttpError)
            {
                Puts(req.error);
            }
            else
            {
                var responseData = req.downloadHandler.text;
                var result = JsonConvert.DeserializeObject<AntiKnoxResult>(responseData);

                if (result.match)
                {
                    PrintWarning("Disallowing IP " + host + " because it's an anonymous IP address.");
                    return "AntiKnox: You're not allowed to connect with a VPN or proxy.";
                }
            }

            return true;
        }

        [ConsoleCommand("antiknox.setkey")]
        private void cmdSetKey(ConsoleSystem.Arg arg)
        {
            if (arg.Args == null || arg.Args.Length < 1)
            {
                PrintError("Usage: antiknox.setkey [64-character-auth-key]");
                return;
            }

            var key = arg.Args[0].Trim();
            if (key.Length != 64)
            {
                PrintError("The auth key must be 64 characters. See the dashboard for your key:");
                PrintError("https://ww.antiknox.net/dashboard");
                return;
            }

            var hadKey = authKey != null && authKey.Length == 64;
            Config["Key"] = authKey = key;
            SaveConfig();

            Puts("Your auth key has been updated.");

            // Print the welcome message once the user has set everything up:
            if (!hadKey)
            {
                printSetupSuccess();
            }
        }

        [System.Serializable]
        class AntiKnoxResult
        {
            public bool match;
        }

    }

}
