## Table of Contents


- [Features](#features)
- [Commands](#commands)
- [Configuration](#configuration)
- [Acknowledgments](#acknowledgments)

## Features

This mod can be used to allow only known player to access an otherwhise open server. 
If not present, the mod will create the configuration files on startup

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
