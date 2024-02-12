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
        private const string modVersion = "2.0.0";

        // Config related
        // public static ConfigEntry<string> bepinexPathEntry;

        private readonly Harmony harmony = new Harmony(modGUID);
        private static Plugin Instance;
        internal static ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

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

            // Network.RegisterAll();

            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(HUDManagerPatch));
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
