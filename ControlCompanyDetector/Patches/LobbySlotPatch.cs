﻿using System.Collections;
using ControlCompanyDetector.Logic;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace ControlCompanyDetector.Patches
{
    [HarmonyPatch(typeof(LobbySlot))]
    internal class LobbySlotPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void AwakePatch(LobbySlot __instance)
        {
            if (__instance != null)
            {
                CoroutineManager.StartCoroutine(ModifySlot(__instance));
            }
        }

        private static IEnumerator ModifySlot(LobbySlot lobbySlot)
        {
            yield return new WaitForEndOfFrame();
            if (Plugin.highlightCCLobbies.Value)
            {
                Color outline = GetColorFromRGBA(158, 49, 72, 255);
                Color joinHighlight = GetColorFromRGBA(210, 0, 72, 255);
                Color text = GetColorFromRGBA(205, 71, 100, 255);
                Color slot = GetColorFromRGBA(158, 49, 72, 64);
                // Color customTextColor = GetColorFromRGBA(183, 60, 89, 255);
                Color customTextColor = GetColorFromRGBA(150, 60, 89, 255 / 2);
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

        internal static void CreateCustomSlot(LobbySlot lobbySlot, Color outline, Color joinHighlight, Color text, Color slot, Color customTextColor, bool placeLeft)
        {
            bool challenge = ((Object)((Component)lobbySlot).transform).name.Contains("Challenge");
            string lobbyName = lobbySlot.thisLobby.GetData("name");
            string prefix = Plugin.showCCLobbyPrefix.Value ? "[CC] " : "";
            if (lobbyName.Contains('\u200b'))
            {
                if (challenge)
                {
                    lobbySlot.LobbyName.SetText("[CC] " + lobbyName);
                    return;
                }
                lobbySlot.LobbyName.SetText(prefix + lobbyName);
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
        }

        private static Color GetColorFromRGBA(int r, int b, int g, int a)
        {
            return new Color((float)r / 255f, (float)b / 255f, (float)g / 255f, (float)a / 255f);
        }

    }
}
