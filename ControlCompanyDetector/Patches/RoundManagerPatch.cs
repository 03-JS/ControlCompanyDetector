using ControlCompanyDetector.Logic;
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
        internal static bool hasEnemyCountIncreased;
        internal static EnemyAI spawnedEnemy;

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        static void PatchUpdate()
        {
            if (Plugin.detectEnemyControl.Value && !StartOfRound.Instance.IsHost)
            {
                if (!StartOfRound.Instance.IsClientFriendsWithHost())
                {
                    DetectEnemyControl();
                }
                else if (!Plugin.ignoreFriendlyLobbies.Value)
                {
                    DetectEnemyControl();
                }
            }
        }

        internal static void DetectEnemyControl()
        {
            EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
            enemyCount = enemies.Length;

            if (enemyCount > previousEnemyCount)
            {
                if (!hasEnemyCountIncreased)
                {
                    DetectIndoorsEnemyControl(enemies);
                }
                hasEnemyCountIncreased = true;
                previousOpenVentCount = EnemyVentPatch.openVentCount;
            }
            else
            {
                hasEnemyCountIncreased = false;
            }
            previousEnemyCount = enemyCount;
        }

        internal static void DetectIndoorsEnemyControl(EnemyAI[] enemies)
        {
            spawnedEnemy = enemies[0];
            // Debug.Log("Spawned enemy name: " + spawnedEnemy.name);
            if (!spawnedEnemy.enemyType.isOutsideEnemy && !spawnedEnemy.name.ToUpper().Contains("DRESSGIRL"))
            {
                // Plugin.LogInfoMLS("openVentCount: " + EnemyVentPatch.openVentCount);
                if (EnemyVentPatch.openVentCount <= previousOpenVentCount)
                {
                    DisplayEnemyMessage("The host has spawned a " + FormatEnemyName(spawnedEnemy.name));
                }
            }
        }

        internal static void DisplayEnemyMessage(string message)
        {
            Plugin.LogWarnMLS("An enemy has been manually spawned");
            Detector.SendUITip("WARNING:", message, false);
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
            if (enemyName.Contains("LASSO"))
            {
                enemyName = "Lasso Man";
            }
            return enemyName;
        }
    }
}
