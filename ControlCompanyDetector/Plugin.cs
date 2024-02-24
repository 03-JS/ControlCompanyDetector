using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using ControlCompanyDetector.Logic;
using ControlCompanyDetector.Patches;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;

namespace ControlCompanyDetector
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "JS03.ControlCompanyDetector";
        private const string modName = "Control Company Detector";
        private const string modVersion = "3.3.3";

        // Plugin detection related
        public static List<string> keywords;
        public static bool canHostDetectEnemySpawning;
        public static PluginInfo problematicPluginInfo;

        // Config related
        // public static ConfigEntry<string> bepinexPathEntry;
        public static ConfigEntry<bool> ignoreFriendlyLobbies;
        public static ConfigEntry<bool> showInfoMessage;
        public static ConfigEntry<bool> showControlCompanyLobbiesOnly;
        public static ConfigEntry<bool> hideControlCompanyLobbies;
        public static ConfigEntry<bool> detectEnemySpawning;
        public static ConfigEntry<bool> detectMaskedSpawning;
        public static ConfigEntry<bool> detectEnemySpawningAsHost;

        private readonly Harmony harmony = new Harmony(modGUID);
        private static Plugin Instance;
        internal static ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            GenerateConfigValues();
            GenerateKeywords();

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Control Company Detector has started");

            if (hideControlCompanyLobbies.Value)
            {
                mls.LogWarning("Lobbies hosting Control Company will be hidden");
            }
            else
            {
                if (showControlCompanyLobbiesOnly.Value)
                {
                    mls.LogWarning("Only lobbies hosting Control Company will be shown");
                }
                else
                {
                    mls.LogWarning("All lobbies will be shown");
                }
            }

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
            showInfoMessage = Config.Bind(
                "Hosting", // Config section
                "Show info message", // Key of this config
                true, // Default value
                "Set this to false if you want to hide the additional info message that can appear when hosting a lobby" // Description
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
                "Should the mod be able to detect if an enemy has been spawned by the host / a player? (Only works with indoor enemies)" // Description
            );

            detectMaskedSpawning = Config.Bind(
                "Spawn detection", // Config section
                "Detect Masked enemy spawning", // Key of this config
                true, // Default value
                "Should the mod be able to detect if a Masked enemy has been spawned by the host / a player?\n" +
                "Disable this if you're using mods that alter how Masked enemies spawn" // Description
            );

            detectEnemySpawningAsHost = Config.Bind(
                "Spawn detection", // Config section
                "Detect enemy spawning as host", // Key of this config
                true, // Default value
                "Should the mod be able to detect if an enemy has been spawned by another player when hosting a lobby? (Only works if Detect enemy spawning is enabled)" // Description
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

        public static void GetLoadedMods()
        {
            canHostDetectEnemySpawning = true;
            Dictionary<string, PluginInfo> Mods = Chainloader.PluginInfos;
            mls.LogInfo("Getting currently loaded mods...");
            foreach (PluginInfo info in Mods.Values)
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

        public static bool UserHasCC()
        {
            Dictionary<string, PluginInfo> Mods = Chainloader.PluginInfos;
            mls.LogInfo("Checking for Control Company...");
            foreach (PluginInfo info in Mods.Values)
            {
                if (info.Metadata.GUID.Equals("ControlCompany.ControlCompany"))
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
