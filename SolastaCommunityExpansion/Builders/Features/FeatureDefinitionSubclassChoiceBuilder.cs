﻿using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionSubclassChoiceBuilder : BaseDefinitionBuilder<FeatureDefinitionSubclassChoice>
    {
        public FeatureDefinitionSubclassChoiceBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionSubclassChoiceBuilder(string name, Guid namespaceGuid, string category = null)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionSubclassChoiceBuilder(FeatureDefinitionSubclassChoice original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionSubclassChoiceBuilder(FeatureDefinitionSubclassChoice original, string name, Guid namespaceGuid, string category = null)
            : base(original, name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionSubclassChoiceBuilder SetFilterByDeity(bool requireDeity)
        {
            Definition.SetFilterByDeity(requireDeity);
            return this;
        }

        public FeatureDefinitionSubclassChoiceBuilder SetSubclassSuffix(string subclassSuffix)
        {
            Definition.SetSubclassSuffix(subclassSuffix);
            return this;
        }
    }
}