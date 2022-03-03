﻿using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    /// <summary>
    /// Implement on a FeatureDefinition to be able to change the min / max values on ability checks
    /// </summary>
    public interface IChangeAbilityCheck
    {
        public int MinAbilityCheck(
            RulesetCharacter character,
            int baseBonus,
            int rollModifier,
            string abilityScoreName,
            string proficiencyName,
            List<RuleDefinitions.TrendInfo> advantageTrends,
            List<RuleDefinitions.TrendInfo> modifierTrends);
    }
}
