using ControlCompanyDetector.Logic;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.UIElements.UIR.Implementation.UIRStylePainter;

namespace ControlCompanyDetector.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch : MonoBehaviour
    {
        internal static int enemyCount;
        internal static int previousEnemyCount;
        internal static int previousOpenVentCount;
        public static int maskDeaths;
        internal static int previousMaskDeaths;
        internal static EnemyAI spawnedEnemy;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void DisplayHostOnlyMsg()
        {
            if (!Plugin.canHostDetectEnemySpawning && Plugin.showInfoMessage.Value && Plugin.detectEnemySpawningAsHost.Value)
            {
                CoroutineManager.StartCoroutine(Detector.SendDelayedUITip("Control Company Detector:", "<size=15>Detect enemy spawning as host has been disabled because you have the following mod installed:</size>\n" + Plugin.problematicPluginInfo.Metadata.Name + "", false, 3.5f));
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PatchUpdate()
        {
            if (!StartOfRound.Instance.IsHost)
            {
                StartSpawnDetection();
            }
            else if (Plugin.detectEnemySpawningAsHost.Value && Plugin.canHostDetectEnemySpawning)
            {
                StartSpawnDetection();
            }
        }

        internal static void StartSpawnDetection()
        {
            if (Plugin.detectEnemySpawning.Value)
            {
                if (!StartOfRound.Instance.IsClientFriendsWithHost())
                {
                    DetectEnemySpawning();
                }
                else if (StartOfRound.Instance.IsHost)
                {
                    DetectEnemySpawning();
                }
                else if (!Plugin.ignoreFriendlyLobbies.Value)
                {
                    DetectEnemySpawning();
                }
            }
        }

        internal static void DetectEnemySpawning()
        {
            EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
            enemyCount = enemies.Length;

            if (enemyCount > previousEnemyCount)
            {
                spawnedEnemy = enemies[0];
                DetectIndoorsEnemySpawning();
                if (Plugin.detectMaskedSpawning.Value)
                {
                    DetectMaskedEnemySpawning();
                    previousMaskDeaths = maskDeaths;
                }
                previousOpenVentCount = EnemyVentPatch.openVentCount;
            }

            previousEnemyCount = enemyCount;
        }

        internal static void DetectIndoorsEnemySpawning()
        {
            // Debug.Log("Spawned enemy name: " + spawnedEnemy.name);
            if (IsEnemyTypeValid())
            {
                // Plugin.LogInfoMLS("openVentCount: " + EnemyVentPatch.openVentCount);
                if (EnemyVentPatch.openVentCount <= previousOpenVentCount)
                {
                    DisplayEnemyMessage();
                }
            }
        }

        internal static void DetectMaskedEnemySpawning()
        {
            // Debug.Log("Spawned enemy name: " + spawnedEnemy.name);
            if (spawnedEnemy.GetType() == typeof(MaskedPlayerEnemy))
            {
                // Plugin.LogInfoMLS("openVentCount: " + EnemyVentPatch.openVentCount);
                // Plugin.LogInfoMLS("deaths: " + maskDeaths);
                // Plugin.LogInfoMLS("previousDeaths: " + previousMaskDeaths);
                if (EnemyVentPatch.openVentCount <= previousOpenVentCount && previousMaskDeaths == maskDeaths)
                {
                    DisplayEnemyMessage();
                }
            }
        }

        internal static void DisplayEnemyMessage()
        {
            Plugin.LogWarnMLS("An enemy has spawned in an abnormal manner");
            if (Detector.hostHasCC)
            {
                Detector.SendUITip("WARNING:", "The host has spawned a " + FormatEnemyName(spawnedEnemy), false);
            }
            else if (StartOfRound.Instance.IsHost)
            {
                Detector.SendUITip("WARNING:", "A " + FormatEnemyName(spawnedEnemy) + " has been spawned by a player", false);
            }
        }

        internal static string FormatEnemyName(EnemyAI enemy)
        {
            if (enemy.GetType() == typeof(FlowermanAI))
            {
                return "Bracken";
            }
            if (enemy.GetType() == typeof(SpringManAI))
            {
                return "Coil-Head";
            }
            if (enemy.GetType() == typeof(BlobAI))
            {
                return "Hygrodere";
            }
            if (enemy.GetType() == typeof(PufferAI))
            {
                return "Spore Lizard";
            }
            if (enemy.GetType() == typeof(CrawlerAI))
            {
                return "Thumper";
            }
            if (enemy.GetType() == typeof(HoarderBugAI))
            {
                int randomNumber = UnityEngine.Random.Range(0, 4);
                return randomNumber == 0 ? "Yippee Bug" : "Hoarding Bug";
            }
            if (enemy.GetType() == typeof(NutcrackerEnemyAI))
            {
                return "Nutcracker";
            }
            if (enemy.GetType() == typeof(CentipedeAI))
            {
                return "Snare Flea";
            }
            if (enemy.GetType() == typeof(SandSpiderAI))
            {
                return "Bunker Spider";
            }
            if (enemy.GetType() == typeof(JesterAI))
            {
                return "Jester";
            }
            if (enemy.GetType() == typeof(MaskedPlayerEnemy))
            {
                return "Masked Employee";
            }
            if (enemy.GetType() == typeof(LassoManAI))
            {
                return "Lasso Man";
            }
            return "[REDACTED]";
        }

        internal static bool IsEnemyTypeValid()
        {
            return spawnedEnemy.GetType() == typeof(FlowermanAI) || spawnedEnemy.GetType() == typeof(SpringManAI)
                || spawnedEnemy.GetType() == typeof(BlobAI) || spawnedEnemy.GetType() == typeof(PufferAI)
                || spawnedEnemy.GetType() == typeof(CrawlerAI) || spawnedEnemy.GetType() == typeof(HoarderBugAI)
                || spawnedEnemy.GetType() == typeof(NutcrackerEnemyAI) || spawnedEnemy.GetType() == typeof(CentipedeAI)
                || spawnedEnemy.GetType() == typeof(SandSpiderAI) || spawnedEnemy.GetType() == typeof(JesterAI)
                || spawnedEnemy.GetType() == typeof(LassoManAI);
        }
    }
}
