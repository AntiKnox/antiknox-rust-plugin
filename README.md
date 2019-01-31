# Rust AntiKnox plugin
Protect your Rust game server from VPNs and other IP hiders.

## Installation
This plugin requires [the uMod framework](https://umod.org/games/rust) to be installed on the dedicated Rust server you 
want to protect. If you haven't installed the uMod framework on your server yet, go ahead and install that first. 
The official uMod website has information on how to install it.

Once you have uMod installed, open the `plugins` folder inside your Rust server folder 
(for example `C:\steamcmd\rust_server\oxide\plugins`) and put the `AntiKnox.cs` file in it.

If the server is running, it should show you that it's now enabling the AntiKnox extension. If the
server was not yet running, now is the moment to start the Rust server.

## Getting your key
To use AntiKnox, you first need a (free) acount. Head over to https://www.antiknox.net and create one. 

Head over to your [AntiKnox dashboard](https://www.antiknox.net/dashboard) and copy your licence key on 
the right. You're going to need this in the next step.

## Finishing up
Once you have your key from the steps above, head over to the Rust server console. Run the following command:

`antiknox.setkey your-64-character-long-key-goes-here`

For example:

`antiknox.setkey aabbccddeeff00112233445566778899aabbccddeeff00112233445566778899`

All set! Your Rust server will now verify incoming connections with AntiKnox before letting them in. 
