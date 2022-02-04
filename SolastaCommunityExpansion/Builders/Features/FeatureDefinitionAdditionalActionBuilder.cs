﻿using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionAdditionalActionBuilder : BaseDefinitionBuilder<FeatureDefinitionAdditionalAction>
    {
        public FeatureDefinitionAdditionalActionBuilder(string name, string guid) : base(name, guid)
        {
        }

        public FeatureDefinitionAdditionalActionBuilder(string name, Guid namespaceGuid, string category = null)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionAdditionalActionBuilder(FeatureDefinitionAdditionalAction original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionAdditionalActionBuilder(FeatureDefinitionAdditionalAction original, string name, Guid namespaceGuid, string category = null)
            : base(original, name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionAdditionalActionBuilder SetActionType(ActionDefinitions.ActionType actionType)
        {
            Definition.SetActionType(actionType);
            return this;
        }

        public FeatureDefinitionAdditionalActionBuilder SetMaxAttacksNumber(int maxAttacksNumber)
        {
            Definition.SetMaxAttacksNumber(maxAttacksNumber);
            return this;
        }

        public FeatureDefinitionAdditionalActionBuilder SetTriggerCondition(RuleDefinitions.AdditionalActionTriggerCondition triggerCondition)
        {
            Definition.SetTriggerCondition(triggerCondition);
            return this;
        }

        /**
         * The list of actions which are forbidden to use
         */
        public FeatureDefinitionAdditionalActionBuilder SetForbiddenActions(params ActionDefinitions.Id[] forbiddenActions)
        {
            return SetForbiddenActions(forbiddenActions.AsEnumerable());
        }

        public FeatureDefinitionAdditionalActionBuilder SetForbiddenActions(IEnumerable<ActionDefinitions.Id> forbiddenActions)
        {
            Definition.ForbiddenActions.SetRange(forbiddenActions);
            return this;
        }

        /**
         * The list of actions which are individually greenlighted
         */
        public FeatureDefinitionAdditionalActionBuilder SetAuthorizedActions(params ActionDefinitions.Id[] authorizedActions)
        {
            return SetAuthorizedActions(authorizedActions.AsEnumerable());
        }

        public FeatureDefinitionAdditionalActionBuilder SetAuthorizedActions(IEnumerable<ActionDefinitions.Id> authorizedActions)
        {
            Definition.AuthorizedActions.SetRange(authorizedActions);
            return this;
        }

        /**
         * The list of the only actions which are authorized (when non empty)
         */
        public FeatureDefinitionAdditionalActionBuilder SetRestrictedActions(params ActionDefinitions.Id[] restrictedActions)
        {
            return SetRestrictedActions(restrictedActions.AsEnumerable());
        }

        public FeatureDefinitionAdditionalActionBuilder SetRestrictedActions(IEnumerable<ActionDefinitions.Id> restrictedActions)
        {
            Definition.RestrictedActions.SetRange(restrictedActions);
            return this;
        }
    }
}