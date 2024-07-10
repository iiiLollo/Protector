## Table of Contents


- [Features](#features)
- [Commands](#commands)
- [Configuration](#configuration)
- [Acknowledgments](#acknowledgments)

## Features

This mod can be used to allow only known player to access an otherwhise open server. 
If not present, the mod will create the configuration files on startup

The mod will check if a connecting user steamID is in the whitelist file, if not, it will prevent the user from accessing the server.
Additionally, is possible to add/remove users from the whitelist file while the server is running.
If the option "GateKeeperKickPlayer" as been enabled, the mod will kick any user from the server that has been removed from the whitelist.

The whitelist file can be reloaded on a live server in multiple ways:
- GateKeeperFileWatcherEnabled will enable a file whatcher what will reload the file on change (this doesn't work on Wine installations).
- GateKeeperUpdateEnabled will enable a time based reload of the file.
- Using the ".Protector reload" command, an Admin user can ask the mod to reload the list.

## Commands

### Protector Commands
- `.Protector list`
  - Returns the list of current whitelisted clients.
- `.Protector reload`
  - Forces the reload of the whitelist file
 
## Configuration

### Main configuration file 
The main configuration file is the following: BepInEx\config\Protector.cfg

- **Whitelist File**: `GateKeeperWhitelistFile` (String, default: \BepInEx\config\Protector\WhiteList.txt)  
  Path to the WhiteList file.
- **Whitelist File watcher**: `GateKeeperFileWatcherEnabled` (bool, default: true)  
  Enable or disable hot monitor of configuration file, if this is set to true, chaning the file will update the whitelist without having to restart the server. Please be aware that this setting doesn't work if the server is running in wine
- **Background update**: `GateKeeperUpdateEnabled` (bool, default: true) 
  Enable background processor. If this is set to true, the whitelist will be reaload periodically (to be used in place of the watcher)
- **Background update delay**: `GateKeeperUpdateInterval` (Int32, default: 9999) 
  IF the Background process is enabled, this is the delay in minutes between executions.
- **Kick Player**: `GateKeeperKickPlayer` (bool, default: false) 
  If set to true, players removed from the whitelist will be kicked from the server
  
### Whitelist configuration file 
The default location for the Whitelis file is main configuration file is \BepInEx\config\Protector\WhiteList.txt
Add all Whitelisted steamID to this file.


## Acknowledgments
- This mod is inspired by the Guardian2 mod for V Rising: https://github.com/jokeruarwentto/Guardian2/
- The mod structure is based on the https://raw.githubusercontent.com/mfoltz/Sanguis
