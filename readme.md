# About Skua

Skua is the successor to RBot (originally made by "[rodit](https://github.com/rodit/RBot)"), now remade and rebranded by Breno_Henrike, with the help of Lord Exelot, and a handful of scripters. It is a Third Party Client made by the people mentioned above. It also has many "features" and quirks. Overall, it will make this glorified flash game on steroids a piece of cake.

## How to use

[Usage Guide](./usage.md)

## Do we store information online?

The *only* things that get recorded are: the auto-generated number **(not your actual game user ID)** to identify you, the number of scripts run (stopped & started), and the start and stop timestamps. This can be completely opted out of when first running a script, or you can edit the text file ***“DataCollectionSettings”*** in your `Documents\Skua > DataCollectionSettings.txt`. If you make it look as shown below, it will send absolutely nothing 👍

```txt
UserID: null
genericDataConsent: false
scriptNameConsent: false
stopTimeConsent: false
```

## For Account Manager

Your **Account Info** will be stored only in your **local appdata** and never shown anywhere, nor in a text file. We **DO NOT** store it online because we intended to make an account manager with **no database**.

## What do we use this data for?

To keep track of what bots are run, how often, how long, and just how popular some bots are.

## Some examples of the types of scripts Skua has

- **Story scripts** found in the `Story` folder.
- **Merge scripts** found in the `Other > MergeShops` folder.
- **Farming scripts** found in the `Farm` folder. These include, but are not limited to, Gold, Experience, Class Points, and Reputation.
- **Faction-specific** (nation/legion/etc) can be found in their respective folders.
- Specific tools such as **Butler** (a follow and kill [doesn't support quests]), "ChooseBestGear" (a script that will look at your inv, and equip the appropriate setting for the race type you select.), BuyOut ( will either buy **all/non-ac/ac** (will prompt due to ACs) from a specified shop)
- **Core Script Files** are not meant to be run.
- **0ScriptName.cs** are basically "Do everything required for this script."
- If you wanted to have a new farming script that doesn't exist, though, please request it
in the discord

### [Skua Discord](https://discord.com/invite/CKKbk2zr3p) Join the community and get help with Skua

### For questions or help go to the [#skua-help](https://discord.com/channels/1090693457586176013/1090741396970938399) channel



## Skua Developers

Skua developers need your support to improve Skua. You can donate or sponsor us by clicking the PayPal link below. Thank you for your support.

### [purple/SharpTheNightmare](https://www.paypal.com/ncp/payment/RHG8F84AUXFKY)

### [Breno Henrike's PayPal](https://www.paypal.com/donate?hosted_button_id=QVQ4Q7XSH9VBY)

### [Lord Exelot's PayPal](www.paypal.me/LordExelot)

## Cast

- **Breno Henrike**, the artist of Skua.
- **Lord Exelot**, the scripts manager and Skua Discord owner.
- **Tato** the script writer, is also the writer of this about content.
- **Skua Heroes*,* the script makers and helpers.
- **Boaters** are the ones who sail overnight using Skua and help the Skua team to improve, thanks to their feedback and suggestions **which is you**.

## [Build Guide](./BUILD.md) **Ignore this if you are not a developer**
