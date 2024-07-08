using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using ControlCompanyDetector.Patches;
using HarmonyLib;
using LobbyCompatibility;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using System.Collections.Generic;
using System.Linq;

namespace ControlCompanyDetector
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
    [LobbyCompatibility(CompatibilityLevel.ClientOnly, VersionStrictness.None)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "JS03.ControlCompanyDetector";
        private const string modName = "Control Company Detector";
        private const string modVersion = "4.0.2";

        public static string[] colors;

        // Plugin detection related
        public static List<string> keywords;
        public static bool canHostDetectEnemySpawning;
        public static bool clientHasCCFilter;
        public static bool clientHasRBL;
        public static BepInEx.PluginInfo problematicPluginInfo;

        // Config related
        // public static ConfigEntry<string> bepinexPathEntry;
        public static ConfigEntry<bool> ignoreFriendlyLobbies;
        // public static ConfigEntry<bool> showInfoMessage;
        public static ConfigEntry<bool> showCCLobbyPrefix;
        public static ConfigEntry<bool> highlightCCLobbies;
        public static ConfigEntry<bool> showControlCompanyLobbiesOnly;
        public static ConfigEntry<bool> hideControlCompanyLobbies;
        public static ConfigEntry<bool> detectEnemySpawning;
        public static ConfigEntry<bool> detectMaskedSpawning;
        public static ConfigEntry<bool> detectEnemySpawningAsHost;
        public static ConfigEntry<string> lobbyHighlightColor;
        public static ConfigEntry<bool> sendChatMessage;

        private readonly Harmony harmony = new Harmony(modGUID);
        private static Plugin Instance;
        internal static ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            GenerateColors();
            GenerateConfigValues();
            GenerateKeywords();

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Control Company Detector has started");

            PatchStuff();
        }

        internal void PatchStuff()
        {
            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(LobbyListPatch));
            harmony.PatchAll(typeof(LobbySlotPatch));
            harmony.PatchAll(typeof(RoundManagerPatch));
            harmony.PatchAll(typeof(EnemyVentPatch));
            harmony.PatchAll(typeof(MaskedEnemyPatch));
            harmony.PatchAll(typeof(MenuManagerPatch));
        }

        internal void GenerateConfigValues()
        {
            /*
            showInfoMessage = Config.Bind(
                "Hosting", // Config section
                "Show info message", // Key of this config
                true, // Default value
                "Set this to false if you want to hide the additional info message that can appear when hosting a lobby" // Description
            );
            */

            showCCLobbyPrefix = Config.Bind(
                "Public Lobbies", // Config section
                "Show CC lobby prefix", // Key of this config
                true, // Default value
                "Lobbies that are certain to use Control Company will have the [CC] prefix in the lobby list\n" +
                "NOTE: Challenge moon lobbies that have Control Company will always have the [CC] prefix" // Description
            );

            highlightCCLobbies = Config.Bind(
                "Public Lobbies", // Config section
                "Highlight Control Company lobbies", // Key of this config
                true, // Default value
                "Lobbies that are certain to use Control Company will be highlighted in the lobby list" // Description
            );

            lobbyHighlightColor = Config.Bind(
                "Public Lobbies", // Config section
                "Lobby highlight color", // Key of this config
                "DEFAULT", // Default value
                new ConfigDescription("Changes the color of highlighted lobbies", new AcceptableValueList<string>(colors)) // Description
            );

            hideControlCompanyLobbies = Config.Bind(
                "Public Lobbies", // Config section
                "Hide Control Company lobbies", // Key of this config
                false, // Default value
                "Hides lobbies hosting Control Company" // Description
            );

            showControlCompanyLobbiesOnly = Config.Bind(
                "Public Lobbies", // Config section
                "Only show Control Company lobbies", // Key of this config
                false, // Default value
                "Only shows lobbies hosting Control Company" // Description
            );

            ignoreFriendlyLobbies = Config.Bind(
                "Private Lobbies", // Config section
                "Ignore friend lobbies", // Key of this config
                true, // Default value
                "Should the mod completely ignore lobbies created by friends?" // Description
            );

            detectEnemySpawning = Config.Bind(
                "Spawn detection", // Config section
                "Detect enemy spawning", // Key of this config
                true, // Default value
                "Should the mod be able to detect if an enemy has been spawned by the host? (Only works with indoor enemies)" // Description
            );

            detectMaskedSpawning = Config.Bind(
                "Spawn detection", // Config section
                "Detect Masked enemy spawning", // Key of this config
                true, // Default value
                "Should the mod be able to detect if a Masked enemy has been spawned by the host?\n" +
                "Disable this if you're using mods that alter how Masked enemies spawn" // Description
            );

            detectEnemySpawningAsHost = Config.Bind(
                "Spawn detection", // Config section
                "Detect enemy spawning as host", // Key of this config
                true, // Default value
                "Should the mod be able to detect if an enemy has been spawned by another player when hosting a lobby? (Only works if Detect enemy spawning is enabled)" // Description
            );

            sendChatMessage = Config.Bind(
                "Text Chat", // Config section
                "Send chat message", // Key of this config
                false, // Default value
                "Sends a message in the chat to let others know when a lobby has Control Company" // Description
            );
        }

        internal static void GenerateKeywords()
        {
            keywords = new List<string> {
                "BrutalCompany",
                "HullBreaker",
                "MoreMonsters",
                "MonstersPlus",
                "SavageCompany",
                "Lethal_Company_Variables",
                "LethalHoardingSwarm",
                "TitaniumTurbine.EnemySpawner",
                "Waffle.MaskedWaves"
            };
        }

        internal static void GenerateColors()
        {
            colors = new string[]{
                "DEFAULT",
                "RANDOM",
                // "RED",
                "GREEN",
                "LIME",
                "BLUE",
                // "ORANGE",
                "CYAN",
                "PINK",
                "PURPLE",
                // "MAGENTA",
                "YELLOW",
                "GRAY",
                "GREY",
                "MAROON"
            };
        }

        public static void CheckProblematicMods()
        {
            canHostDetectEnemySpawning = true;
            Dictionary<string, BepInEx.PluginInfo> Mods = Chainloader.PluginInfos;
            mls.LogInfo("Getting currently loaded mods...");
            foreach (BepInEx.PluginInfo info in Mods.Values)
            {
                foreach (string key in Plugin.keywords)
                {
                    if (info.Metadata.GUID.Contains(key))
                    {
                        mls.LogWarning("A mod that alters how enemies spawn has been detected!");
                        canHostDetectEnemySpawning = false;
                        problematicPluginInfo = info;
                        return;
                    }
                }
            }
        }

        public static bool UserHasMod(string modGUID)
        {
            Dictionary<string, BepInEx.PluginInfo> mods = Chainloader.PluginInfos;
            mls.LogInfo("Checking for " + modGUID + "...");
            foreach (BepInEx.PluginInfo info in mods.Values)
            {
                if (info.Metadata.GUID.Equals(modGUID))
                {
                    return true;
                }
            }
            return false;
        }

        public static void LogInfoMLS(string info)
        {
            mls.LogInfo(info);
        }

        public static void LogWarnMLS(string warn)
        {
            mls.LogWarning(warn);
        }
    }
}
