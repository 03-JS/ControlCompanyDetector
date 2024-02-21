using ControlCompanyDetector.Logic;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PatchUpdate()
        {
            if (!StartOfRound.Instance.IsHost)
            {
                StartSpawnDetection();
            }
            else if (Plugin.detectEnemySpawningAsHost.Value)
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
            if (!spawnedEnemy.enemyType.isOutsideEnemy && !spawnedEnemy.name.ToUpper().Contains("DRESSGIRL"))
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
            if (spawnedEnemy.name.ToUpper().Contains("MASK"))
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
                Detector.SendUITip("WARNING:", "The host has spawned a " + FormatEnemyName(spawnedEnemy.name), false);
            }
            else if (StartOfRound.Instance.IsHost)
            {
                Detector.SendUITip("WARNING:", "A " + FormatEnemyName(spawnedEnemy.name) + " has been spawned by a player", false);
            }
        }

        internal static string FormatEnemyName(string enemyName)
        {
            enemyName = enemyName.ToUpper();
            if (enemyName.Contains("FLOWER"))
            {
                enemyName = "Bracken";
            }
            if (enemyName.Contains("SPRING"))
            {
                enemyName = "Coil-Head";
            }
            if (enemyName.Contains("BLOB"))
            {
                enemyName = "Hygrodere";
            }
            if (enemyName.Contains("PUFFER"))
            {
                enemyName = "Spore Lizard";
            }
            if (enemyName.Contains("CRAWLER"))
            {
                enemyName = "Thumper";
            }
            if (enemyName.Contains("BUG"))
            {
                int randomNumber = UnityEngine.Random.Range(0, 4);
                enemyName = randomNumber == 0 ? "Yippee Bug" : "Hoarding Bug";
            }
            if (enemyName.Contains("NUT"))
            {
                enemyName = "Nutcracker";
            }
            if (enemyName.Contains("CENTIPEDE"))
            {
                enemyName = "Snare Flea";
            }
            if (enemyName.Contains("SPIDER"))
            {
                enemyName = "Bunker Spider";
            }
            if (enemyName.Contains("JESTER"))
            {
                enemyName = "Jester";
            }
            if (enemyName.Contains("MASK"))
            {
                enemyName = "Masked Employee";
            }
            if (enemyName.Contains("LASSO"))
            {
                enemyName = "Lasso Man";
            }
            return enemyName;
        }
    }
}
