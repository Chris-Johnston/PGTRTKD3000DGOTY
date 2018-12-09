# Pet GTR Turbo Kart Drift 3000 _!!!_ Deluxe Game of The Year Edition™

A game for CSS 475 (databases class).

[![Build status](https://ci.appveyor.com/api/projects/status/85yu79oweov3xyrc/branch/master?svg=true)](https://ci.appveyor.com/project/Chris-Johnston/pgtrtkd3000dgoty/branch/master)

<img width="200" src="PetRaceTurbo30xx.png" alt="Logo image." />

## Building

Building and running the application is done entirely in Visual Studio. All typescript files
get compiled into regular js automatically.

1. Make sure you have `npm` installed. This is bundled with Node JS, so you'll have to install that
too.

2. Install gulp: `npm i -g gulp gulp-cli less gulp-less` (This may require sudo/admin)


## Local Configuration

In your `PetGame` project configuration, add the following environment variables.

| Key | Value | Notes |
| --- | ----- | ----- |
| `PETGAME_DB_CONNECTION_STRING` | `Server=localhost;Database=PetGame;Trusted_Connection=True;` | Database connection string. Requires username and password if not hosted locally (Azure). |
| `PETGAME_TWILIO_ENABLE` | `True` or `False` | Enables/disables the Twilio service. |
| `PETGAME_TWILIO_ACCOUNTSID` | `(secret)` | Twilio Account SID. |
| `PETGAME_TWILIO_AUTHTOKEN` | `(secret)` | Twilio Auth Token. |
| `PETGAME_TWILIO_PHONENUM` | `+1XXXYYYZZZZ` | Twilio phone number for sending messages. |
| `PETGAME_TWILIO_DEBUGNUM` | `+1XXXYYYZZZZ` | Phone number for sending debug messages. |
| `PETGAME_TWILIO_DEBUGONLY` | `True` or `False` | If True, **all** messages will only be sent to the debug phone number. |
| `PETGAME_DISCORD_USEWEBHOOK` | `True` or `False` | If True, messages will be sent with the supplied webhook. |
| `PETGAME_DISCORD_WEBHOOK` | `(secret)` | The webhook url to send messages to. |

This will point the ASP.NET connections to your local database.
