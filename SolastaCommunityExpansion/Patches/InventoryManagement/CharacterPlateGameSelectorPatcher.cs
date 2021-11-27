﻿using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    internal static class CharacterPlateGameSelectorPatcher
    {
        [HarmonyPatch(typeof(CharacterPlateGameSelector), "OnPointerClick")]
        internal static class CharacterPlateGameSelector_OnPointerClick
        {
            internal static void Prefix()
            {
                Models.InventoryManagementContenxt.ResetDropdowns(filterDropdown: true, sortDropdown: false);
            }
        }
    }
}
