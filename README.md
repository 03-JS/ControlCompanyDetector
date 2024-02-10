# Control Company Detector

## Description

A mod that gives you an in-game warning when a lobby owner is using Control Company.

Both parties need to have LC_API installed for the mod to work.

Please don't try to use Control Company and this mod together, as it will lead to errors and the mod not working.

## How to configure and use:

For the mod to work, you need to tell it what the path to your current BepInEx folder is.

All you need to do is follow these very simple steps:

- **If you're using Thunderstore or r2modman**:

1. After downloading and installing the mod, open Lethal Company and then close it once you're in the main menu
2. Go to Settings and click on "Browse profile folder"
3. Go inside the folder named "BepInEx" and copy the full file path
4. Go to Config Editor and look for "JS03.ControlCompanyDetector.cfg"
5. Click on Edit Config
6. Paste the BepInEx folder path you just copied in the BepInEx Directory field

- **If you're not using Thunderstore or r2modman**:

1. After downloading and installing the mod, open Lethal Company and then close it once you're in the main menu
2. Go into your Lethal Company game directory
3. Go inside the folder named "BepInEx" and copy the full file path
4. Inside the BepInEx folder, go into a folder named "config" and look for "JS03.ControlCompanyDetector.cfg"
5. Open the file with your preferred text editor and paste the BepInEx folder path you just copied after the = symbol. It should look something like this:
```
BepInEx Directory = Paste your BepInEx folder here
```

And that's it! The mod should now work as intended
