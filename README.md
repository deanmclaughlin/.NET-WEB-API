# Files

## ./.gitignore and ./.gitattributes
Used for control of Git.

## ./Program.cs
Contains the entry point ("start") for the application:
- Configures the application
- Configures authentication
- Configures routing
- Starts the application

## ./appsettings.json
Contains general configuration.

## ./appsettings.Development.json
Contains development configuration.

## ./Properties/launchSettings.json
Contains web server launch settings.

## ./wwwroot/
Contains all the "traditional" HTML/CSS/JS/etc files.

## ./Controllers/
Contains the logic for dealing with your Models (Create, Read, Update, Delete).

## ./Models/
Contains the classes that map onto database objects, as well as any not-mapped classes.

## ./Views/
Contains the logic for translating your data (from Controllers) to viewable pages.