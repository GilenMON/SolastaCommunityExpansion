﻿using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal sealed class PrimalChampionBuilder : FeatureDefinitionBuilder<PrimalChampion, PrimalChampionBuilder>
    {
        private const string PrimalChampionName = "ZSPrimalChampion";
        private const string PrimalChampionGuid = "118a5ea1-8a19-4bee-9db1-7a2464c8e7b5";

        private PrimalChampionBuilder(string name, string guid) : base(name, guid)
        {
            Definition.GuiPresentation.Description = "Feature/&PrimalChampionDescription";
            Definition.GuiPresentation.Title = "Feature/&PrimalChampionTitle";
        }

        private static PrimalChampion CreateAndAddToDB(string name, string guid)
        {
            return new PrimalChampionBuilder(name, guid).AddToDB();
        }

        internal static readonly PrimalChampion PrimalChampion =
            CreateAndAddToDB(PrimalChampionName, PrimalChampionGuid);
    }

    internal sealed class PrimalChampion : FeatureDefinition, IFeatureDefinitionCustomCode
    {
        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            ModifyAttributeAndMax(hero, AttributeDefinitions.Strength, 4);
            ModifyAttributeAndMax(hero, AttributeDefinitions.Constitution, 4);

            hero.RefreshAll();
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            ModifyAttributeAndMax(hero, AttributeDefinitions.Strength, -4);
            ModifyAttributeAndMax(hero, AttributeDefinitions.Constitution, -4);

            hero.RefreshAll();
        }

        private static void ModifyAttributeAndMax(RulesetCharacterHero hero, string attributeName, int amount)
        {
            RulesetAttribute attribute = hero.GetAttribute(attributeName);
            attribute.BaseValue += amount;
            attribute.MaxValue += amount;
            attribute.MaxEditableValue += amount;
            attribute.Refresh();

            hero.AbilityScoreIncreased?.Invoke(hero, attributeName, amount, amount);
        }
    }
}
