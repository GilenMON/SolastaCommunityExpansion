﻿using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static FeatureDefinitionAutoPreparedSpells;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionAutoPreparedSpellsBuilder : BaseDefinitionBuilder<FeatureDefinitionAutoPreparedSpells>
    {
        public FeatureDefinitionAutoPreparedSpellsBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionAutoPreparedSpellsBuilder(string name, Guid namespaceGuid, string category = null)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionAutoPreparedSpellsBuilder(FeatureDefinitionAutoPreparedSpells original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionAutoPreparedSpellsBuilder(FeatureDefinitionAutoPreparedSpells original, string name, Guid namespaceGuid, string category = null)
            : base(original, name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionAutoPreparedSpellsBuilder SetPreparedSpellGroups(params AutoPreparedSpellsGroup[] autospelllists)
        {
            return SetPreparedSpellGroups(autospelllists.AsEnumerable());
        }

        public FeatureDefinitionAutoPreparedSpellsBuilder SetPreparedSpellGroups(IEnumerable<AutoPreparedSpellsGroup> autospelllists)
        {
            Definition.AutoPreparedSpellsGroups.SetRange(autospelllists);
            return this;
        }

        public FeatureDefinitionAutoPreparedSpellsBuilder SetCharacterClass(CharacterClassDefinition castingClass)
        {
            Definition.SetSpellcastingClass(castingClass);
            return this;
        }

        public FeatureDefinitionAutoPreparedSpellsBuilder SetAffinityRace(CharacterRaceDefinition castingRace)
        {
            Definition.SetAffinityRace(castingRace);
            return this;
        }

        /**
         * This tag is used to create a tooltip:
         * this.autoPreparedTitle.Text = string.Format("Screen/&{0}SpellTitle", autoPreparedTag);
	     * this.autoPreparedTooltip.Content = string.Format("Screen/&{0}SpellDescription", autoPreparedTag);
         */
        public FeatureDefinitionAutoPreparedSpellsBuilder SetAutoTag(string tag)
        {
            Definition.SetAutopreparedTag(tag);
            return this;
        }

        public FeatureDefinitionAutoPreparedSpellsBuilder SetSpellcastingClass(CharacterClassDefinition characterClass)
        {
            Definition.SetSpellcastingClass(characterClass);
            return this;
        }
    }

    public static class AutoPreparedSpellsGroupBuilder
    {
        public static AutoPreparedSpellsGroup Build(int classLevel, params SpellDefinition[] spellnames)
        {
            return Build(classLevel, spellnames.AsEnumerable());
        }

        public static AutoPreparedSpellsGroup Build(int classLevel, IEnumerable<SpellDefinition> spellnames)
        {
            return new AutoPreparedSpellsGroup
            {
                ClassLevel = classLevel,
                SpellsList = spellnames.ToList()
            };
        }
    }
}