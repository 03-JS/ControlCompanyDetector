using System.Collections;
using System.Linq;
using ControlCompanyDetector.Logic;
using HarmonyLib;
using LobbyCompatibility.Features;
using LobbyCompatibility.Models;
using Steamworks.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Image = UnityEngine.UI.Image;
namespace ControlCompanyDetector.Patches
{
    [HarmonyPatch(typeof(LobbySlot))]
    internal class LobbySlotPatch
    {
        private static LobbyDiff lobbyDiff;
        private static Color outline;
        private static Color joinHighlight;
        private static Color text;
        private static Color slot;
        private static Color customTextColor;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void AwakePatch(LobbySlot __instance)
        {
            if (__instance != null)
            {
                CoroutineManager.StartCoroutine(ModifySlot(__instance));
            }
        }

        internal static IEnumerator ModifySlot(LobbySlot lobbySlot)
        {
            yield return new WaitForEndOfFrame();
            lobbyDiff = LobbyHelper.GetLobbyDiff(lobbySlot.thisLobby);
            AddCCPrefix(lobbySlot);
            if (Plugin.highlightCCLobbies.Value)
            {
                ChangeHighlightColor(Plugin.lobbyHighlightColor.Value);
                CreateCustomSlot(lobbySlot, outline, joinHighlight, text, slot, customTextColor, Plugin.clientHasRBL);
            }
            else if (Plugin.clientHasRBL && Plugin.clientHasCCFilter)
            {
                RectTransform[] rectTransforms = MonoBehaviour.FindObjectsOfType<RectTransform>();
                foreach (RectTransform rectTransform in rectTransforms)
                {
                    if (rectTransform.gameObject.name.Equals("cc"))
                    {
                        rectTransform.localPosition = new Vector3(60f, -12.5f, -7f);
                        rectTransform.sizeDelta = new Vector2(220f, 24f);
                    }
                }
            }
        }

        internal static void AddCCPrefix(LobbySlot lobbySlot)
        {
            bool challenge = ((Object)((Component)lobbySlot).transform).name.Contains("Challenge");
            string lobbyName = lobbySlot.thisLobby.GetData("name");
            if (lobbyDiff.PluginDiffs.Any(diff => diff.ServerVersion != null))
            {
                if (lobbyDiff.PluginDiffs.Any(diff => diff.GUID == "ControlCompany.ControlCompany"))
                {
                    if (challenge)
                    {
                        lobbySlot.LobbyName.SetText("[CC] " + lobbyName);
                        return;
                    }
                    string prefix = Plugin.showCCLobbyPrefix.Value ? "[CC] " : "";
                    lobbySlot.LobbyName.SetText(prefix + lobbyName);
                }
            }
            else if (lobbyName.Contains('\u200b'))
            {
                if (challenge)
                {
                    lobbySlot.LobbyName.SetText("[CC] " + lobbyName);
                    return;
                }
                string prefix = Plugin.showCCLobbyPrefix.Value ? "[CC] " : "";
                lobbySlot.LobbyName.SetText(prefix + lobbyName);
            }
        }

        internal static void CreateCustomSlot(LobbySlot lobbySlot, Color outline, Color joinHighlight, Color text, Color slot, Color customTextColor, bool placeLeft)
        {
            if (!((Object)((Component)lobbySlot).transform).name.Contains("Challenge"))
            {
                string lobbyName = lobbySlot.thisLobby.GetData("name");
                if (lobbyDiff.PluginDiffs.Any(diff => diff.ServerVersion != null))
                {
                    if (lobbyDiff.PluginDiffs.Any(diff => diff.GUID == "ControlCompany.ControlCompany"))
                    {
                        CreateSlotComponents(lobbySlot, placeLeft);
                    }
                }
                else if (lobbyName.Contains('\u200b'))
                {
                    CreateSlotComponents(lobbySlot, placeLeft);
                }
            }
        }

        private static void CreateSlotComponents(LobbySlot lobbySlot, bool placeLeft)
        {
            Image outlineImage = lobbySlot.transform.Find("Outline")?.GetComponent<Image>();
            Image joinButtonSelectionHighlight = lobbySlot.transform.Find("JoinButton/SelectionHighlight")?.GetComponent<Image>();
            if ((bool)outlineImage)
            {
                outlineImage.color = outline;
            }
            if ((bool)joinButtonSelectionHighlight)
            {
                joinButtonSelectionHighlight.color = joinHighlight;
            }
            TextMeshProUGUI serverNameText = lobbySlot.LobbyName;
            TextMeshProUGUI numPlayersText = lobbySlot.playerCount;
            serverNameText.color = text;
            numPlayersText.color = text;
            Image lobbySlotImage = lobbySlot.GetComponent<Image>();
            lobbySlotImage.color = slot;
            MonoBehaviour.Destroy(GameObject.Find("cc"));
            GameObject rectTransformGO = new GameObject("CCSlot", typeof(RectTransform));
            RectTransform rectTransform = rectTransformGO.GetComponent<RectTransform>();
            TextMeshProUGUI ccText = rectTransform.gameObject.AddComponent<TextMeshProUGUI>();
            ccText.fontSize = 21f;
            ccText.text = "CONTROL COMPANY";
            ccText.alignment = TextAlignmentOptions.MidlineLeft;
            ccText.font = numPlayersText.font;
            ccText.color = customTextColor;
            rectTransform.SetParent(lobbySlot.transform);
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.localScale = new Vector3(0.8497399f, 0.8497399f, 0.8497399f);
            rectTransform.localPosition = new Vector3(180f, -15f, -7f);
            if (placeLeft)
            {
                rectTransform.localPosition = new Vector3(60f, -15f, -7f);
            }
            rectTransform.sizeDelta = new Vector2(170.5f, 24f);
        }

        private static Color GetColorFromRGBA(float r, float b, float g, float a)
        {
            return new Color(r / 255f, b / 255f, g / 255f, a / 255f);
        }

        internal static void ChangeHighlightColor(string preset)
        {
            if (preset.Equals("RANDOM"))
            {
                int randomNumber = Random.Range(2, Plugin.colors.Length);
                preset = Plugin.colors[randomNumber];
            }
            switch (preset)
            {
                //case "RED":
                //    outline = GetColorFromRGBA(230, 0, 0, 255);
                //    joinHighlight = GetColorFromRGBA(255, 0, 0, 255);
                //    text = GetColorFromRGBA(255, 0, 0, 255);
                //    slot = GetColorFromRGBA(105, 0, 0, 64);
                //    customTextColor = GetColorFromRGBA(190, 0, 0, 255 / 2);
                //    break;
                case "BLUE":
                    outline = GetColorFromRGBA(0, 0, 255, 255);
                    joinHighlight = GetColorFromRGBA(50, 50, 255, 255);
                    text = GetColorFromRGBA(100, 100, 255, 255);
                    slot = GetColorFromRGBA(0, 0, 125, 64);
                    customTextColor = GetColorFromRGBA(0, 0, 175, 255 / 2);
                    break;
                case "GREEN":
                    outline = GetColorFromRGBA(0, 155, 0, 255);
                    joinHighlight = GetColorFromRGBA(0, 165, 0, 255);
                    text = GetColorFromRGBA(0, 175, 0, 255);
                    slot = GetColorFromRGBA(0, 90, 0, 64);
                    customTextColor = GetColorFromRGBA(0, 120, 0, 255 / 2);
                    break;
                case "LIME":
                    outline = GetColorFromRGBA(0, 235, 0, 235);
                    joinHighlight = GetColorFromRGBA(20, 235, 20, 255);
                    text = GetColorFromRGBA(70, 235, 70, 255);
                    slot = GetColorFromRGBA(0, 125, 0, 64);
                    customTextColor = GetColorFromRGBA(0, 175, 0, 255 / 2);
                    break;
                //case "ORANGE":
                //    outline = GetColorFromRGBA(255, 102, 0, 255);
                //    joinHighlight = GetColorFromRGBA(255, 133, 51, 255);
                //    text = GetColorFromRGBA(255, 148, 77, 255);
                //    slot = GetColorFromRGBA(153, 61, 0, 64);
                //    customTextColor = GetColorFromRGBA(180, 80, 0, 255 / 2);
                //    break;
                case "CYAN":
                    outline = GetColorFromRGBA(0, 235, 235, 235);
                    joinHighlight = GetColorFromRGBA(72, 235, 235, 255);
                    text = GetColorFromRGBA(98, 235, 235, 255);
                    slot = GetColorFromRGBA(0, 130, 130, 64);
                    customTextColor = GetColorFromRGBA(0, 170, 170, 255 / 2);
                    break;
                case "PINK":
                    outline = GetColorFromRGBA(232, 136, 220, 255);
                    joinHighlight = GetColorFromRGBA(232, 145, 222, 255);
                    text = GetColorFromRGBA(233, 161, 223, 255);
                    slot = GetColorFromRGBA(252, 156, 240, 15);
                    customTextColor = GetColorFromRGBA(252, 165, 242, 255 / 4);
                    break;
                case "PURPLE":
                    outline = GetColorFromRGBA(255, 35, 255, 255);
                    joinHighlight = GetColorFromRGBA(255, 85, 255, 255);
                    text = GetColorFromRGBA(255, 90, 255, 255);
                    slot = GetColorFromRGBA(145, 0, 145, 64);
                    customTextColor = GetColorFromRGBA(190, 0, 190, 255 / 2);
                    break;
                //case "MAGENTA":
                //    outline = GetColorFromRGBA(235, 0, 114, 255);
                //    joinHighlight = GetColorFromRGBA(235, 30, 150, 255);
                //    text = GetColorFromRGBA(235, 40, 140, 255);
                //    slot = GetColorFromRGBA(150, 0, 90, 64);
                //    customTextColor = GetColorFromRGBA(175, 0, 100, 255 / 2);
                //    break;
                case "YELLOW":
                    outline = GetColorFromRGBA(235, 235, 0, 255);
                    joinHighlight = GetColorFromRGBA(235, 235, 75, 255);
                    text = GetColorFromRGBA(235, 235, 85, 255);
                    slot = GetColorFromRGBA(125, 125, 0, 64);
                    customTextColor = GetColorFromRGBA(165, 165, 0, 255 / 2);
                    break;
                case "GRAY":
                case "GREY":
                    outline = GetColorFromRGBA(128, 128, 128, 255);
                    joinHighlight = GetColorFromRGBA(158, 158, 158, 255);
                    text = GetColorFromRGBA(168, 168, 168, 255);
                    slot = GetColorFromRGBA(75, 75, 75, 64);
                    customTextColor = GetColorFromRGBA(100, 100, 100, 255 / 2);
                    break;
                case "MAROON":
                    outline = GetColorFromRGBA(158, 49, 72, 255);
                    joinHighlight = GetColorFromRGBA(210, 0, 72, 255);
                    text = GetColorFromRGBA(205, 71, 100, 255);
                    slot = GetColorFromRGBA(158, 49, 72, 64);
                    customTextColor = GetColorFromRGBA(150, 60, 89, 255 / 2);
                    break;
                default:
                    outline = GetColorFromRGBA(158, 49, 72, 255);
                    joinHighlight = GetColorFromRGBA(210, 0, 72, 255);
                    text = GetColorFromRGBA(205, 71, 100, 255);
                    slot = GetColorFromRGBA(158, 49, 72, 64);
                    customTextColor = GetColorFromRGBA(150, 60, 89, 255 / 2);
                    break;
            }
        }
    }
}
