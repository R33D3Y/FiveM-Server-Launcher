# FiveM-Server-Launcher
Just a simple Server Launcher to help host a FiveM server; built around our and other server use.

## Installation/First-Time Use
### Directory Selection
When you first boot up the Launcher it will present a screen to enter the Server Directory and Server Config Directory.

1. Enter the Server Directory or click the '...' button to use the FolderDialog selector.
2. Enter the Server Config Directory or click the '...' button to use the FileDialog selector.
3. Click 'Okay' to save changes.

Clicking Cancel will save any changes made and close the Launcher.
To change either the Server Directory or Server Config Directory after clicking 'Okay', simply click the cog Icon on the Launcher.

### Restart Schedule
To access the Restart Schedule click the button on the left labeled 'Show Restart Schedule & CMD/Node Scirpts' and select the 'Restarts' tab.

1. Click the green plus button in the bottom right of the panel.
2. Alter the data under 'Schedule 0'; this is your first schedule.
   - Change the HH:MM time.
   - Change the MM Server Warning time.
   - Change wether the schedule will Restart/Stop/Start the server.
3. Tick 'Restart Schedule' to make sure these changes take effect when the HH:MM is right.

### Node/CMD Scripts
The Launcher will run a CMD.exe on Server Start; data entered here will be attached to this CMD.
To access the CMD/Node Scripts click the button on the left labeled 'Show Restart Schedule & CMD/Node Scirpts' and select the 'CMD/Node Scripts' tab.

1. Click the green plus button in the bottom right of the panel.
2. Alter the data under 'CMD Command 0'; this is your first CMD/Node Script.
   - Enter the Script Directory or click the '...' button to use the FolderDialog selector.
   - Enter the Arguments for the script to run.
3. Tick 'Enable On Server Start' to make sure these changes take effect when the Server is started.

### SQL Backups
-- Blah

### Resource Management
-- Blah

## Daily Use
