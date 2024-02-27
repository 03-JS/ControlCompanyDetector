using ControlCompanyDetector.Logic;
using HarmonyLib;
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

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void DisplayHostOnlyMsg()
        {
            if (!Plugin.canHostDetectEnemySpawning && Plugin.showInfoMessage.Value && Plugin.detectEnemySpawningAsHost.Value)
            {
                CoroutineManager.StartCoroutine(Detector.SendDelayedUITip("Control Company Detector:", "<size=15>Detect enemy spawning as host has been disabled because you have the following mod installed:</size>\n" + Plugin.problematicPluginInfo.Metadata.Name + "", false, 3.5f));
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
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
                if (!Detector.clientIsFriendsWithHost)
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
            // Plugin.LogInfoMLS("Spawned enemy name: " + spawnedEnemy.name);
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
                Detector.SendUITip("WARNING:", "The host has spawned " + FormatEnemyName(spawnedEnemy, false), false);
            }
            else if (StartOfRound.Instance.IsHost)
            {
                Detector.SendUITip("WARNING:", FormatEnemyName(spawnedEnemy, true) + " has been spawned by a player", false);
            }
        }

        internal static string FormatEnemyName(EnemyAI enemy, bool upper)
        {
            string prefix = upper ? "A " : "a ";
            if (enemy.GetType() == typeof(FlowermanAI))
            {
                return prefix + "Bracken";
            }
            if (enemy.GetType() == typeof(SpringManAI))
            {
                return prefix + "Coil-Head";
            }
            if (enemy.GetType() == typeof(BlobAI))
            {
                return prefix + "Hygrodere";
            }
            if (enemy.GetType() == typeof(PufferAI))
            {
                return prefix + "Spore Lizard";
            }
            if (enemy.GetType() == typeof(CrawlerAI))
            {
                return prefix + "Thumper";
            }
            if (enemy.GetType() == typeof(HoarderBugAI))
            {
                int randomNumber = Random.Range(0, 4);
                return randomNumber == 0 ? prefix + "Yippee Bug" : prefix + "Hoarding Bug";
            }
            if (enemy.GetType() == typeof(NutcrackerEnemyAI))
            {
                return prefix + "Nutcracker";
            }
            if (enemy.GetType() == typeof(CentipedeAI))
            {
                return prefix + "Snare Flea";
            }
            if (enemy.GetType() == typeof(SandSpiderAI))
            {
                return prefix + "Bunker Spider";
            }
            if (enemy.GetType() == typeof(JesterAI))
            {
                return prefix + "Jester";
            }
            if (enemy.GetType() == typeof(MaskedPlayerEnemy))
            {
                return prefix + "Masked Employee";
            }
            if (enemy.GetType() == typeof(LassoManAI))
            {
                return prefix + "Lasso Man";
            }
            if (enemy.GetType() == typeof(TestEnemy))
            {
                prefix = upper ? "An " : "an ";
                return prefix + "Obunga";
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
                || spawnedEnemy.GetType() == typeof(LassoManAI) || spawnedEnemy.GetType() == typeof(TestEnemy);
        }
    }
}
