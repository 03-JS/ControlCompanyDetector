using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using ControlCompanyDetector.Patches;
using HarmonyLib;

namespace ControlCompanyDetector
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "JS03.ControlCompanyDetector";
        private const string modName = "Control Company Detector";
        private const string modVersion = "3.1.0";

        // Config related
        // public static ConfigEntry<string> bepinexPathEntry;
        public static ConfigEntry<bool> ignoreFriendlyLobbies;
        public static ConfigEntry<bool> detectEnemyControl;
        public static ConfigEntry<bool> showInfoMessage;
        public static ConfigEntry<bool> hideControlCompanyLobbies;

        private readonly Harmony harmony = new Harmony(modGUID);
        private static Plugin Instance;
        internal static ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            ignoreFriendlyLobbies = Config.Bind(
                "General", // Config section
                "Ignore Friend Lobbies", // Key of this config
                true, // Default value
                "Should the mod completely ignore lobbies created by friends?" // Description
            );

            detectEnemyControl = Config.Bind(
                "General", // Config section
                "Detect Enemy Control", // Key of this config
                true, // Default value
                "Should the mod be able to detect if an enemy is being controlled?\n(Only works with indoor enemies)" // Description
            );

            showInfoMessage = Config.Bind(
                "General", // Config section
                "Show info message", // Key of this config
                true, // Default value
                "Set this to false if you want to hide the info message that appears when you join a friend's lobby and Ignore Friendly Lobbies is set to true" // Description
            );

            hideControlCompanyLobbies = Config.Bind(
                "General", // Config section
                "Hide Control Company Lobbies", // Key of this config
                false, // Default value
                "Hides lobbies hosting Control Company" // Description
            );

            //bepinexPathEntry = Config.Bind(
            //        "Critical", // Config section
            //        "BepInEx Directory", // Key of this config
            //        "Paste your BepInEx folder here", // Default value
            //        "For the mod to work, you need to paste your BepInEx folder here." + // Description
            //        "\nTo find it inside Thunderstore / r2modman go to Settings > Browse profile folder > BepInEx." +
            //        "\nOnce you're there just copy and paste the path to the folder."
            //);

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Control Company Detector has started");

            if (hideControlCompanyLobbies.Value)
            {
                mls.LogWarning("Lobbies hosting Control Company will be hidden");
            }
            else
            {
                mls.LogWarning("Lobbies hosting Control Company will be shown");
            }

            // Network.RegisterAll();

            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(LobbyListPatch));
            harmony.PatchAll(typeof(RoundManagerPatch));
            harmony.PatchAll(typeof(EnemyVentPatch));
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
