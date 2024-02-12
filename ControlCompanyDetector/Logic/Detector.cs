using BepInEx;
using BepInEx.Bootstrap;
using LC_API.GameInterfaceAPI.Features;
using LC_API.Networking;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using ControlCompanyDetector.Patches;
using static UnityEngine.Scripting.GarbageCollector;
using System;

namespace ControlCompanyDetector.Logic
{
    internal static class Detector
    {
        private static Dictionary<string, PluginInfo> Mods = new Dictionary<string, PluginInfo>();
        private static int previousLastCCLine;

        public static void StartDetection()
        {
            HUDManagerPatch.displayTip = false;
            if (Player.LocalPlayer == null)
            {
                Mods = Chainloader.PluginInfos;
                Plugin.LogInfoMLS("Starting detection...");
                Network.Broadcast("LC_API_ReqGUID");
                CoroutineManager.StartCoroutine(ReadBepinLog());
            }
        }

        //[NetworkMessage("CCD_SendMods")]
        internal static void SendUITip(/*ulong senderId*/ string header, string message)
        {
            // Player player = Player.Get(senderId);
            HUDManagerPatch.displayTip = true;
            Player.LocalPlayer.QueueTip(header, message, 5f, 0, true, false, "LC_Tip1");
        }

        // [NetworkMessage("LC_APISendMods")]
        public static IEnumerator ReadBepinLog()
        {
            Plugin.LogInfoMLS("Waiting to read log file...");

            yield return new WaitForSeconds(4.5f);
            
            bool shouldContinue = Detector.CheckLocalPlugins();

            if (!Player.LocalPlayer.IsHost && shouldContinue)
            {
                Plugin.LogInfoMLS("Reading log file");
                string bepinexPath = Plugin.bepinexPathEntry.Value;
                DirectoryInfo bepinexDirectory = new DirectoryInfo(bepinexPath);
                Plugin.LogInfoMLS("BepInEx folder: " + bepinexDirectory);
                FileInfo[] ogFiles = bepinexDirectory.GetFiles("LogOutput.log");
                Plugin.LogInfoMLS("Log file: " + ogFiles[0].FullName);
                Plugin.LogInfoMLS("Copying file...");
                File.Copy(ogFiles[0].FullName, bepinexPath + "/LogCopy.log", true);
                FileInfo[] filesToEdit = bepinexDirectory.GetFiles("LogCopy.log");
                if (filesToEdit.Length > 0)
                {
                    // int lastLC_APIline = FindLastLineOccurrence("responded with these mods", filesToEdit);
                    int lastCCline = FindLastLineOccurrence("ControlCompany.ControlCompany", filesToEdit);
                    if (previousLastCCLine != lastCCline)
                    {
                        Plugin.LogWarnMLS("Control Company has been detected");
                        Detector.SendUITip("WARNING:", $"{Player.HostPlayer.Username} is using Control Company");
                    }
                    previousLastCCLine = lastCCline;
                }
            }
        }

        internal static bool CheckLocalPlugins()
        {
            bool shouldContinue = true;
            foreach (PluginInfo info in Mods.Values)
            {
                if (info.Metadata.GUID.Equals("ControlCompany.ControlCompany"))
                {
                    shouldContinue = false;
                    Plugin.LogWarnMLS("Control Company detected on client, stopping corroutine. Please do not use Control Company and this mod together");
                    Detector.SendUITip("ERROR:", "Please do not use Control Company and Control Company Detector together");
                }
            }
            return shouldContinue;
        }

        internal static int FindLastLineOccurrence(string targetLine, FileInfo[] filesToEdit)
        {
            int lastLineOccurrence = 0;
            using (StreamReader reader = filesToEdit[0].OpenText())
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(targetLine))
                    {
                        lastLineOccurrence++;
                    }
                }
            }
            return lastLineOccurrence;
        }
    }
}
