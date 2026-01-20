using System;
using GameNetcodeStuff;
using HarmonyLib;
using OoLunar.StaminaUI;
using TMPro;
using UnityEngine;

namespace OoLunar.StaminaUI.Patches
{
    [HarmonyPatch, LethalPatch]
    internal class HUDManagerPatch : MonoBehaviour
    {
        private static bool _instantiating = true;
        private static TextMeshProUGUI? _hudPercentagesText;

        private static Color ParseHexColor(string hex)
        {
            // Remove # if present
            string cleanHex = hex.TrimStart('#');

            // Validate hex string length
            if (cleanHex.Length != 6 && cleanHex.Length != 8)
            {
                StaminaUIPlugin.StaticLogger?.LogWarning($"Invalid hex color format: {hex}. Expected 6 or 8 characters. Using default red color.");
                return new Color(1f, 0f, 0f, 1f);
            }

            try
            {
                // Parse hex to RGB/RGBA
                int r = Convert.ToInt32(cleanHex.Substring(0, 2), 16);
                int g = Convert.ToInt32(cleanHex.Substring(2, 2), 16);
                int b = Convert.ToInt32(cleanHex.Substring(4, 2), 16);
                int a = cleanHex.Length == 8 ? Convert.ToInt32(cleanHex.Substring(6, 2), 16) : 255;

                // Convert to Unity Color format (0-1 range)
                return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
            }
            catch (Exception ex)
            {
                StaminaUIPlugin.StaticLogger?.LogWarning($"Failed to parse hex color '{hex}': {ex.Message}. Using default red color.");
                return new Color(1f, 0f, 0f, 1f);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartOfRound), "SceneManager_OnLoadComplete1")]
        public static void CreateHudPercentages()
        {
            if (!_instantiating)
            {
                return;
            }

            GameObject val = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner/WeightUI");
            GameObject val2 = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner");
            GameObject val3 = Instantiate(val, val2.transform);
            val3.name = "HPSP";

            // Find the child of the instantiated object and set its position.
            GameObject _hudPercentages = val3.transform.GetChild(0).gameObject;
            _hudPercentages.GetComponent<RectTransform>().anchoredPosition = new Vector2(-45f, 10f);

            // Stylize the text.
            _hudPercentagesText = _hudPercentages.GetComponent<TextMeshProUGUI>();
            string hexColor = StaminaUIPlugin.TextColorHex?.Value ?? "FF0000";
            _hudPercentagesText.faceColor = ParseHexColor(hexColor);
            _hudPercentagesText.fontSize = 12f;
            _hudPercentagesText.margin = new Vector4(0f, -36f, 100f, 0f);
            _hudPercentagesText.alignment = (TextAlignmentOptions)260;
            _hudPercentagesText.text = "";
            _instantiating = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
        public static void UnInstantiate() => _instantiating = true;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(HUDManager), "Update")]
        public static void Update()
        {
            PlayerControllerB? playerController = GameNetworkManager.Instance.localPlayerController;
            if (playerController == null || _instantiating || _hudPercentagesText == null)
            {
                return;
            }

            float health = Mathf.RoundToInt(playerController.health);
            int sprint = Math.Max(Mathf.RoundToInt(((playerController.sprintMeter * 100f) - 10f) / 90f * 100f), 0);
            _hudPercentagesText.text = $"{health}\n\n\n\n{sprint}%";
        }
    }
}
