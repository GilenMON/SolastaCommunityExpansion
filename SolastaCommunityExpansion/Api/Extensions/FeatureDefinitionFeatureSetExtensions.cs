using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Text;
using TA.AI;
using TA;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using  static  ActionDefinitions ;
using  static  TA . AI . DecisionPackageDefinition ;
using  static  TA . AI . DecisionDefinition ;
using  static  RuleDefinitions ;
using  static  BanterDefinitions ;
using  static  Gui ;
using  static  BestiaryDefinitions ;
using  static  CursorDefinitions ;
using  static  AnimationDefinitions ;
using  static  CharacterClassDefinition ;
using  static  CreditsGroupDefinition ;
using  static  CampaignDefinition ;
using  static  GraphicsCharacterDefinitions ;
using  static  GameCampaignDefinitions ;
using  static  TooltipDefinitions ;
using  static  BaseBlueprint ;
using  static  MorphotypeElementDefinition ;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(FeatureDefinitionFeatureSet))]
    public static partial class FeatureDefinitionFeatureSetExtensions
    {
        public static T SetDefaultSelection<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionFeatureSet
        {
            entity.SetField("defaultSelection", value);
            return entity;
        }

        public static T SetEnumerateInDescription<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionFeatureSet
        {
            entity.SetField("enumerateInDescription", value);
            return entity;
        }

        public static T SetHasRacialAffinity<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionFeatureSet
        {
            entity.SetField("hasRacialAffinity", value);
            return entity;
        }

        public static T SetMode<T>(this T entity, FeatureDefinitionFeatureSet.FeatureSetMode value)
            where T : FeatureDefinitionFeatureSet
        {
            entity.SetField("mode", value);
            return entity;
        }

        public static T SetUniqueChoices<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionFeatureSet
        {
            entity.SetField("uniqueChoices", value);
            return entity;
        }
    }
}