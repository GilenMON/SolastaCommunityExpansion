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
    [TargetType(typeof(TravelJournalDefinition))]
    public static partial class TravelJournalDefinitionExtensions
    {
        public static System.Collections.Generic.List<TravelStyleDuplet> GetStyleDuplets<T>(this T entity)
            where T : TravelJournalDefinition
        {
            return entity.GetField<System.Collections.Generic.List<TravelStyleDuplet>>("styleDuplets");
        }

        public static T SetIndentWidth<T>(this T entity, System.Single value)
            where T : TravelJournalDefinition
        {
            entity.SetField("indentWidth", value);
            return entity;
        }

        public static T SetLineHeight<T>(this T entity, System.Single value)
            where T : TravelJournalDefinition
        {
            entity.SetField("lineHeight", value);
            return entity;
        }

        public static T SetLineSpacing<T>(this T entity, System.Single value)
            where T : TravelJournalDefinition
        {
            entity.SetField("lineSpacing", value);
            return entity;
        }

        public static T SetMinHeight<T>(this T entity, System.Single value)
            where T : TravelJournalDefinition
        {
            entity.SetField("minHeight", value);
            return entity;
        }

        public static T SetWordSpacing<T>(this T entity, System.Single value)
            where T : TravelJournalDefinition
        {
            entity.SetField("wordSpacing", value);
            return entity;
        }
    }
}