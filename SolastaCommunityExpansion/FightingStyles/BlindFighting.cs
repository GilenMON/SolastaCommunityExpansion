﻿using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;

namespace SolastaCommunityExpansion.FightingStyles
{
    internal class BlindFighting : AbstractFightingStyle
    {
        private CustomizableFightingStyle instance;

        internal override List<FeatureDefinitionFightingStyleChoice> GetChoiceLists()
        {
            return new List<FeatureDefinitionFightingStyleChoice>() {
                DatabaseHelper.FeatureDefinitionFightingStyleChoices.FightingStyleChampionAdditional,
                DatabaseHelper.FeatureDefinitionFightingStyleChoices.FightingStyleFighter,
                DatabaseHelper.FeatureDefinitionFightingStyleChoices.FightingStylePaladin,
                DatabaseHelper.FeatureDefinitionFightingStyleChoices.FightingStyleRanger,};
        }

        internal override FightingStyleDefinition GetStyle()
        {
            if (instance == null)
            {
                GuiPresentationBuilder gui = new GuiPresentationBuilder("FightingStyle/&BlindFightingTitle", "FightingStyle/&BlindFightingDescription");
                gui.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.RangerShadowTamer.GuiPresentation.SpriteReference);
                CustomizableFightingStyleBuilder builder = new CustomizableFightingStyleBuilder("BlindFightingStlye", "a0df0cb6-640f-494e-b752-b746fa79bede",
                    new List<FeatureDefinition>() { DatabaseHelper.FeatureDefinitionSenses.SenseBlindSight2 },
                    gui.Build());
                instance = builder.AddToDB();
            }
            return instance;
        }
    }
}
