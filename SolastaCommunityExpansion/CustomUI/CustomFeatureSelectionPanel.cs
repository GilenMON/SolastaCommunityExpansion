using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModKit;
using SolastaCommunityExpansion.Models;
using SolastaModApi;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SolastaCommunityExpansion.CustomUI
{
    public class CustomFeatureSelectionPanel : CharacterStagePanel
    {
        private static CustomFeatureSelectionPanel _instance;

        public static CharacterStagePanel Get(GameObject[] prefabs, Transform parent)
        {
            if (_instance == null)
            {
                var gameObject = Gui.GetPrefabFromPool(prefabs[8], parent);

                var spells = gameObject.GetComponent<CharacterStageSpellSelectionPanel>();
                _instance = gameObject.AddComponent<CustomFeatureSelectionPanel>();
                _instance.Setup(gameObject, spells, prefabs);
            }

            return _instance;
        }

        #region Fields from CharacterStageSpellSelectionPanel

        private CharacterStageSpellSelectionPanel spellsPanel; //TODO: do we need it?

        private RectTransform spellsByLevelTable;
        private GameObject spellsByLevelPrefab;
        private ScrollRect spellsScrollRect;
        private RectTransform learnStepsTable;
        private GameObject learnStepPrefab;
        private AssetReferenceSprite backdropReference;
        private Image backdrop;
        private AnimationCurve curve;
        private RectTransform levelButtonsTable;
        private GameObject levelButtonPrefab;
        private GuiLabel stageTitleLabel;
        private GuiLabel righrFeaturesLabel;
        private GuiLabel rightFeaturesDescription;

        #endregion

        private void Setup(GameObject o, CharacterStageSpellSelectionPanel spells, GameObject[] prefabs)
        {
            spellsPanel = spells;
            _instance.SetField("stageDefinition", spells.StageDefinition);

            spellsByLevelTable = spells.GetField<RectTransform>("spellsByLevelTable");
            spellsByLevelPrefab = spells.GetField<GameObject>("spellsByLevelPrefab");
            spellsScrollRect = spells.GetField<ScrollRect>("spellsScrollRect");
            learnStepsTable = spells.GetField<RectTransform>("learnStepsTable");
            learnStepPrefab = spells.GetField<GameObject>("learnStepPrefab");
            backdropReference = spells.GetField<AssetReferenceSprite>("backdropReference");
            backdrop = spells.GetField<Image>("backdrop");
            curve = spells.GetField<AnimationCurve>("curve");
            levelButtonsTable = spells.GetField<RectTransform>("levelButtonsTable");
            levelButtonPrefab = spells.GetField<GameObject>("levelButtonPrefab");
            stageTitleLabel = spellsPanel.RectTransform.FindChildRecursive("ChoiceTitle").GetComponent<GuiLabel>();
            righrFeaturesLabel = spellsPanel.RectTransform.FindChildRecursive("SpellsInfoTitle").GetComponent<GuiLabel>();
            rightFeaturesDescription = spellsPanel.RectTransform.FindChildRecursive("ProficienciesIntroDescription").GetComponent<GuiLabel>();
        }

        private const float ScrollDuration = 0.3f;
        private const float SpellsByLevelMargin = 10.0f;

        //TODO: add proper translation strings
        public override string Name => "CustomFeatureSelection";
        public override string Title => "Custom Feature Panel Title";
        public override string Description => "Custom Feature Panel Description";
        private bool IsFinalStep => this.currentLearnStep == this.allTags.Count;

        private int currentLearnStep;
        private List<string> allTags = new ();
        private bool wasClicked;

        public override void SetScrollSensitivity(float scrollSensitivity)
        {
            this.spellsScrollRect.scrollSensitivity = -scrollSensitivity;
        }

        public override IEnumerator Load()
        {
            Main.Log($"[ENDER] CUSTOM Load");

            yield return base.Load();
            IRuntimeService runtimeService = ServiceRepository.GetService<IRuntimeService>();
            runtimeService.RuntimeLoaded += this.RuntimeLoaded;
        }

        public override IEnumerator Unload()
        {
            Main.Log($"[ENDER] CUSTOM Unload");

            IRuntimeService runtimeService = ServiceRepository.GetService<IRuntimeService>();
            runtimeService.RuntimeLoaded -= this.RuntimeLoaded;

            // this.allCantrips.Clear();
            // this.allSpells.Clear();
            yield return base.Unload();
        }

        private void RuntimeLoaded(Runtime runtime)
        {
            //TODO: collect any relevant info we need
            // SpellDefinition[] allSpellDefinitions = DatabaseRepository.GetDatabase<SpellDefinition>().GetAllElements();
            // this.allCantrips = new List<SpellDefinition>();
            // this.allSpells = new Dictionary<int, List<SpellDefinition>>();
            //
            // foreach (SpellDefinition spellDefinition in allSpellDefinitions)
            // {
            //     if (spellDefinition.SpellLevel == 0)
            //     {
            //         this.allCantrips.Add(spellDefinition);
            //     }
            //     else
            //     {
            //         if (!this.allSpells.ContainsKey(spellDefinition.SpellLevel))
            //         {
            //             this.allSpells.Add(spellDefinition.SpellLevel, new List<SpellDefinition>());
            //         }
            //
            //         this.allSpells[spellDefinition.SpellLevel].Add(spellDefinition);
            //     }
            // }
        }

        public override void UpdateRelevance()
        {
            RulesetCharacterHero hero = this.currentHero;
            CharacterHeroBuildingData heroBuildingData = hero.GetHeroBuildingData();

            //TODO: check if we have any custom features for selection

            this.IsRelevant = true;
        }

        public override void EnterStage()
        {
            Main.Log($"[ENDER] CUSTOM EnterStage '{this.StageDefinition}'");

            //TODO: add proper localization
            stageTitleLabel.TMP_Text.text = "Select Features";
            righrFeaturesLabel.TMP_Text.text = "Features";
            rightFeaturesDescription.TMP_Text.text = "Select features";
            
            this.currentLearnStep = 0;

            this.CollectTags();

            this.OnEnterStageDone();
        }

        protected override void OnBeginShow(bool instant = false)
        {
            base.OnBeginShow(instant);

            this.backdrop.sprite = Gui.LoadAssetSync<Sprite>(this.backdropReference);

            this.CommonData.CharacterStatsPanel.Show(CharacterStatsPanel.ArmorClassFlag |
                                                     CharacterStatsPanel.InitiativeFlag | CharacterStatsPanel.MoveFlag |
                                                     CharacterStatsPanel.ProficiencyFlag |
                                                     CharacterStatsPanel.HitPointMaxFlag |
                                                     CharacterStatsPanel.HitDiceFlag);

            this.BuildLearnSteps();
            this.spellsScrollRect.normalizedPosition = Vector2.zero;

            this.OnPreRefresh();
            this.RefreshNow();
        }

        protected override void OnEndHide()
        {
            for (int i = 0; i < this.spellsByLevelTable.childCount; i++)
            {
                Transform child = this.spellsByLevelTable.GetChild(i);
                if (child.gameObject.activeSelf)
                {
                    SpellsByLevelGroup group = child.GetComponent<SpellsByLevelGroup>();
                    group.CustomUnbind();
                }
            }

            Gui.ReleaseChildrenToPool(this.spellsByLevelTable);
            Gui.ReleaseChildrenToPool(this.learnStepsTable);
            Gui.ReleaseChildrenToPool(this.levelButtonsTable);

            base.OnEndHide();

            if (this.backdrop.sprite != null)
            {
                Gui.ReleaseAddressableAsset(this.backdrop.sprite);
                this.backdrop.sprite = null;
            }
        }

        protected override void Refresh()
        {
            RulesetCharacterHero hero = this.currentHero;
            CharacterHeroBuildingData heroBuildingData = hero.GetHeroBuildingData();
            Main.Log($"[ENDER] Refresh - start");
            string currentTag = string.Empty;
            for (int i = 0; i < this.learnStepsTable.childCount; i++)
            {
                Transform child = this.learnStepsTable.GetChild(i);

                if (!child.gameObject.activeSelf)
                {
                    continue;
                }

                LearnStepItem stepItem = child.GetComponent<LearnStepItem>();

                LearnStepItem.Status status;
                if (i == this.currentLearnStep)
                {
                    status = LearnStepItem.Status.InProgress;
                }
                else if (i == this.currentLearnStep - 1)
                {
                    status = LearnStepItem.Status.Previous;
                }
                else
                {
                    status = LearnStepItem.Status.Locked;
                }
                    
                stepItem.CustomRefresh(status);

                if (status == LearnStepItem.Status.InProgress)
                {
                    currentTag = stepItem.Tag;
                }
            }
            Main.Log($"[ENDER] Refresh - steps refreshed");

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.learnStepsTable);

            string lastTag = currentTag;
            if (this.IsFinalStep)
            {
                lastTag = this.allTags[this.allTags.Count - 1];
            }

            int requiredSpellGroups = 1;
            while (this.spellsByLevelTable.childCount < requiredSpellGroups)
            {
                Gui.GetPrefabFromPool(this.spellsByLevelPrefab, this.spellsByLevelTable);
            }
            Main.Log($"[ENDER] Refresh - spell groups counted");

            float totalWidth = 0;
            float lastWidth = 0;
            HorizontalLayoutGroup layout = this.spellsByLevelTable.GetComponent<HorizontalLayoutGroup>();
            layout.padding.left = (int)SpellsByLevelMargin;
            
            Main.Log($"[ENDER] Refresh - start configuring spell groups");

            for (int i = 0; i < this.spellsByLevelTable.childCount; i++)
            {
                Transform child = this.spellsByLevelTable.GetChild(i);
                child.gameObject.SetActive(i < requiredSpellGroups);
                if (i < requiredSpellGroups)
                {
                    var group = child.GetComponent<SpellsByLevelGroup>();
                    int spellLevel = i;

                    group.Selected = true;

                    //TODO: create own wrapper for this
                    // group.BindLearning(this.CharacterBuildingService, spellFeature.SpellListDefinition,
                    //     spellFeature.RestrictedSchools, spellLevel, this.OnSpellBoxSelectedCb, knownSpells,
                    //     unlearnedSpells, lastTag, group.Selected, this.IsSpellUnlearnStep(this.currentLearnStep));
                    var arcanum = DatabaseHelper.GetDefinition<FeatureDefinitionFeatureSet>($"ClassWarlockMysticArcanumSetLevel11", null);
                    var prey = DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHunterHuntersPrey;
                    
                    group.CustomFeatureBind(arcanum, new List<FeatureDefinition>(), lastTag, group.Selected, this.IsUnlearnStep(this.currentLearnStep), this.OnFeatureSelected);

                    lastWidth = group.RectTransform.rect.width + layout.spacing;
                    totalWidth += lastWidth;
                }
            }
            Main.Log($"[ENDER] Refresh - finished configuring spell groups");

            // Compute manually the table width, adding a reserve of fluff for the scrollview
            totalWidth += this.spellsScrollRect.GetComponent<RectTransform>().rect.width - lastWidth;
            this.spellsByLevelTable.sizeDelta = new Vector2(totalWidth, this.spellsByLevelTable.sizeDelta.y);

            // Spell Level Buttons
            while (this.levelButtonsTable.childCount < requiredSpellGroups)
            {
                Gui.GetPrefabFromPool(this.levelButtonPrefab, this.levelButtonsTable);
            }

            // Bind the required group, once for each spell level
            for (int spellLevel = 0; spellLevel < requiredSpellGroups; spellLevel++)
            {
                Transform child = this.levelButtonsTable.GetChild(spellLevel);
                child.gameObject.SetActive(true);
                var button = child.GetComponent<SpellLevelButton>();
                button.CustomBind(spellLevel, this.LevelSelected);
            }
            Main.Log($"[ENDER] Refresh - finished configuring buttons");

            // Hide remaining useless groups
            for (int i = requiredSpellGroups; i < this.levelButtonsTable.childCount; i++)
            {
                Transform child = this.levelButtonsTable.GetChild(i);
                child.gameObject.SetActive(false);
            }
            Main.Log($"[ENDER] Refresh - finished hiding buttons");

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.spellsByLevelTable);

            base.Refresh();
        }

        private void OnFeatureSelected(SpellBox spellbox)
        {
            //TODO: implement
        }

        private bool IsUnlearnStep(int step)
        {
            //TODO: implement
            return false;
        }


        public override bool CanProceedToNextStage(out string failureString)
        {
            Main.Log($"[ENDER] CUSTOM CanProceedToNextStage");
            failureString = string.Empty;

            if (!this.IsFinalStep)
            {
                failureString = Gui.Localize("Stage/&SpellSelectionStageFailLearnSpellsDescription");
                return false;
            }

            failureString = "Not implementes yet";
            return false;
        }

        public void MoveToNextLearnStep()
        {
            this.currentLearnStep++;

            this.LevelSelected(0);

            this.OnPreRefresh();
            this.RefreshNow();
        }

        public void MoveToPreviousLearnStep(bool refresh = true, Action onDone = null)
        {
            IHeroBuildingCommandService heroBuildingCommandService =
                ServiceRepository.GetService<IHeroBuildingCommandService>();

            if (this.currentLearnStep > 0)
            {
                if (!this.IsFinalStep)
                {
                    this.ResetLearnings(this.currentLearnStep);
                }

                this.currentLearnStep--;
                this.ResetLearnings(this.currentLearnStep);
                if (this.IsFeatureUnlearnStep(this.currentLearnStep))
                {
                    heroBuildingCommandService.AcknowledgePreviousCharacterBuildingCommandLocally(() =>
                    {
                        this.CollectTags();
                        this.BuildLearnSteps();
                    });
                }
            }

            heroBuildingCommandService.AcknowledgePreviousCharacterBuildingCommandLocally(() =>
            {
                this.LevelSelected(0);
                this.OnPreRefresh();
                this.RefreshNow();
                this.ResetWasClickedFlag();
            });
        }

        private bool IsFeatureUnlearnStep(int step)
        {
            return false;
        }

        private void CollectTags()
        {
            //TODO: collect all relevant groups of feature selections
            allTags.SetRange("TestGroup1");
        }

        private void BuildLearnSteps()
        {
            Main.Log($"[ENDER] BuildLearnSteps");

            // Register all steps
            if (this.allTags != null && this.allTags.Count > 0)
            {
                while (this.learnStepsTable.childCount < this.allTags.Count)
                {
                    Gui.GetPrefabFromPool(this.learnStepPrefab, this.learnStepsTable);
                }

                for (int i = 0; i < this.learnStepsTable.childCount; i++)
                {
                    Transform child = this.learnStepsTable.GetChild(i);

                    if (i < this.allTags.Count)
                    {
                        child.gameObject.SetActive(true);
                        LearnStepItem learnStepItem = child.GetComponent<LearnStepItem>();
                        learnStepItem.CustomBind(i, this.allTags[i], 
                            this.OnLearnBack, 
                            this.OnLearnReset,
                             this.OnLearnAuto
                        );
                        // Bind(i, this.allTags[i], HeroDefinitions.PointsPoolType.Cantrip,
                        //     this.OnLearnBack, this.OnLearnReset,
                        //     this.OnLearnAuto);
                    }
                    else
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }

        public override void CancelStage()
        {
            int stepNumber = this.currentLearnStep;
            if (this.IsFinalStep)
            {
                stepNumber--;
            }

            for (int i = stepNumber; i >= 0; i--)
            {
                this.ResetLearnings(i);
            }

            IHeroBuildingCommandService heroBuildingCommandService =
                ServiceRepository.GetService<IHeroBuildingCommandService>();
            heroBuildingCommandService.AcknowledgePreviousCharacterBuildingCommandLocally(this.OnCancelStageDone);
        }

        public void OnLearnBack()
        {
            if (this.wasClicked)
            {
                return;
            }

            this.wasClicked = true;

            this.MoveToPreviousLearnStep(true, this.ResetWasClickedFlag);
        }

        public void OnLearnReset()
        {
            if (this.wasClicked)
            {
                return;
            }

            this.wasClicked = true;

            if (this.IsFinalStep)
            {
                this.currentLearnStep = this.allTags.Count - 1;
            }

            this.ResetLearnings(this.currentLearnStep,
                () =>
                {
                    this.OnPreRefresh();
                    this.RefreshNow();
                    this.ResetWasClickedFlag();
                });
        }

        private void ResetLearnings(int stepNumber, Action onDone = null)
        {
            RulesetCharacterHero hero = this.currentHero;
            var heroBuildingCommandService = ServiceRepository.GetService<IHeroBuildingCommandService>();

            string tag = this.allTags[stepNumber];

            //TODO: implement resetting of gained/forgotten features for specific step

            //
            // if (this.IsCantripStep(stepNumber))
            // {
            //     heroBuildingCommandService.UnacquireCantrips(hero, tag);
            // }
            //
            // if (this.IsSpellUnlearnStep(stepNumber))
            // {
            //     heroBuildingCommandService.UndoUnlearnSpells(hero, tag);
            // }
            //
            // if (this.IsSpellStep(stepNumber))
            // {
            //     heroBuildingCommandService.UnacquireSpells(hero, tag);
            // }

            heroBuildingCommandService.AcknowledgePreviousCharacterBuildingCommandLocally(() => onDone?.Invoke());
        }

        #region UI helpers

        private void ResetWasClickedFlag()
        {
            wasClicked = false;
        }

        public void LevelSelected(int level)
        {
            this.StartCoroutine(this.BlendToLevelGroup(level));
        }

        private IEnumerator BlendToLevelGroup(int level)
        {
            float duration = ScrollDuration;
            SpellsByLevelGroup group = this.spellsByLevelTable.GetChild(0).GetComponent<SpellsByLevelGroup>();
            foreach (Transform child in this.spellsByLevelTable)
            {
                SpellsByLevelGroup spellByLevelGroup = child.GetComponent<SpellsByLevelGroup>();
                if (spellByLevelGroup.SpellLevel == level)
                {
                    group = spellByLevelGroup;
                }
            }

            float initialX = this.spellsByLevelTable.anchoredPosition.x;
            float finalX = -group.RectTransform.anchoredPosition.x + SpellsByLevelMargin;

            while (duration > 0)
            {
                this.spellsByLevelTable.anchoredPosition = new Vector2(
                    Mathf.Lerp(initialX, finalX, this.curve.Evaluate((ScrollDuration - duration) / ScrollDuration)), 0);
                duration -= Gui.SystemDeltaTime;
                yield return null;
            }

            this.spellsByLevelTable.anchoredPosition = new Vector2(finalX, 0);
        }

        #endregion


        #region autoselect stuff

        public override void AutotestAutoValidate() => this.OnLearnAuto();

        public void OnLearnAuto()
        {
            if (this.wasClicked)
            {
                return;
            }

            this.wasClicked = true;

            this.OnLearnAutoImpl();
        }

        private void OnLearnAutoImpl(System.Random rng = null)
        {
            //TODO: implement auto-selection of stuff
        }

        #endregion
    }

    internal static class LearnStepItemExtension
    {
        
        public static void CustomBind(this LearnStepItem instance, int rank,
            string tag,
            LearnStepItem.ButtonActivatedHandler onBackOneStepActivated,
            LearnStepItem.ButtonActivatedHandler onResetActivated,
            LearnStepItem.ButtonActivatedHandler onAutoSelectActivated)
        {
            instance.Tag = tag;
            instance.PoolType = HeroDefinitions.PointsPoolType.Irrelevant;
            instance.SetField("rank", rank);
            instance.SetField("ignoreAvailable", false);
            instance.SetField("autoLearnAvailable", false);
            string header = Gui.Localize($"Title-{tag}");
            instance.GetField<GuiLabel>("headerLabelActive").Text = header;
            instance.GetField<GuiLabel>("headerLabelInactive").Text = header;
            instance.OnBackOneStepActivated = onBackOneStepActivated;
            instance.OnResetActivated = onResetActivated;
            instance.OnAutoSelectActivated = onAutoSelectActivated;
        }

        public static void CustomRefresh(this LearnStepItem instance, LearnStepItem.Status status)
        {
            int remainingPoints = 2;//poolOfTypeAndTag.RemainingPoints;
            int maxPoints = 5;//poolOfTypeAndTag.MaxPoints;
            var ignoreAvailable = instance.GetField<bool>("ignoreAvailable");
            var choiceLabel = instance.GetField<GuiLabel>("choicesLabel");
            var activeGroup = instance.GetField<RectTransform>("activeGroup");
            var inactiveGroup = instance.GetField<RectTransform>("inactiveGroup");
            var autoButton = instance.GetField<Button>("autoButton");
            var resetButton = instance.GetField<Button>("resetButton");
            
            activeGroup.gameObject.SetActive(status == LearnStepItem.Status.InProgress);
            inactiveGroup.gameObject.SetActive( status !=  LearnStepItem.Status.InProgress);
            instance.GetField<Image>("activeBackground").gameObject.SetActive(status != LearnStepItem.Status.Locked);
            instance.GetField<Image>("inactiveBackground").gameObject.SetActive(status == LearnStepItem.Status.Locked);
            instance.GetField<Button>("backOneStepButton").gameObject.SetActive(status == LearnStepItem.Status.Previous);
            resetButton.gameObject.SetActive(status == LearnStepItem.Status.InProgress);
            autoButton.gameObject.SetActive(status == LearnStepItem.Status.InProgress && !ignoreAvailable);
            instance.GetField<Button>("ignoreButton").gameObject.SetActive(ignoreAvailable);

            if (status == LearnStepItem.Status.InProgress)
            {
                int current = maxPoints - remainingPoints;
                instance.GetField<GuiLabel>("pointsLabelActive").Text = Gui.FormatCurrentOverMax(current, maxPoints);
                instance.GetField<Image>("remainingPointsGaugeActive").fillAmount = (float) current /  maxPoints;
                choiceLabel.Text = "Label-" + instance.Tag;
                LayoutRebuilder.ForceRebuildLayoutImmediate(choiceLabel.RectTransform);
                activeGroup.sizeDelta = new Vector2(activeGroup.sizeDelta.x, (float) (choiceLabel.RectTransform.rect.height - choiceLabel.RectTransform.anchoredPosition.y + 12.0));
                instance.RectTransform.sizeDelta = activeGroup.sizeDelta;
                resetButton.interactable = remainingPoints < maxPoints;
                autoButton.interactable = instance.GetField<bool>("autoLearnAvailable") && remainingPoints > 0;
            }
            else
            {
                int current = maxPoints - remainingPoints;
                instance.GetField<GuiLabel>("pointsLabelInactive").Text = Gui.FormatCurrentOverMax(current, maxPoints);
                instance.GetField<Image>("remainingPointsGaugeInactive").fillAmount =  (float) current / maxPoints;
                instance.RectTransform.sizeDelta = inactiveGroup.sizeDelta;
                instance.GetField<Button>("backOneStepButton").interactable = true;
            }
        }
    }

    internal static class SpellLevelButtonExtension
    {
        public static void CustomBind(this SpellLevelButton instance,
            int level, SpellLevelButton.LevelSelectedHandler levelSelected)
        {
            instance.Level = level;
            instance.LevelSelected = levelSelected;
            instance.GetField<GuiLabel>("label").Text = $"{level}";
        }
    }

    internal static class SpellsByLevelGroupExtensions
    {
        public static RectTransform GetSpellsTable(this SpellsByLevelGroup instance)
        {
            return instance.GetField<RectTransform>("spellsTable");
        }

        public static void CustomFeatureBind(this SpellsByLevelGroup instance, FeatureDefinitionFeatureSet featureSet,
            List<FeatureDefinition> unlearned, string spellTag,
            bool canAcquireSpells,
            bool unlearn, SpellBox.SpellBoxChangedHandler spellBoxChanged)
        {
            instance.name = $"Feature[{featureSet.Name}]";

            // instance.CommonBind( null, unlearn ? SpellBox.BindMode.Unlearn : SpellBox.BindMode.Learning, spellBoxChanged, new List<SpellDefinition>(), null, new List<SpellDefinition>(), new List<SpellDefinition>(), "");
            var allFeatures = featureSet.FeatureSet;
            //TODO: implement proper sorting
            //allSpells.Sort((IComparer<SpellDefinition>) instance);

            var spellsTable = instance.GetSpellsTable();
            var spellPrefab = instance.GetField<GameObject>("spellPrefab");

            while (spellsTable.childCount < allFeatures.Count)
            {
                Gui.GetPrefabFromPool(spellPrefab, spellsTable);
            }

            GridLayoutGroup component1 = spellsTable.GetComponent<GridLayoutGroup>();
            component1.constraintCount = Mathf.Max(3, Mathf.CeilToInt(allFeatures.Count / 4f));
            for (int index = 0; index < allFeatures.Count; ++index)
            {
                var feature = allFeatures[index];
                spellsTable.GetChild(index).gameObject.SetActive(true);
                var box = spellsTable.GetChild(index).GetComponent<SpellBox>();
                bool isUnlearned = unlearned != null && unlearned.Contains(feature);
                SpellBox.BindMode bindMode = unlearn ? SpellBox.BindMode.Unlearn : SpellBox.BindMode.Learning;

                // box.Bind(guiSpellDefinition1, null, false, null, isUnlearned, bindMode, spellBoxChanged);
                box.CustomFeatureBind(feature, isUnlearned, bindMode, spellBoxChanged);
            }

            //disable unneeded spell boxes
            for (int count = allFeatures.Count; count < spellsTable.childCount; ++count)
                spellsTable.GetChild(count).gameObject.SetActive(false);

            float x = (float)((double)component1.constraintCount * (double)component1.cellSize.x +
                              (double)(component1.constraintCount - 1) * (double)component1.spacing.x);
            spellsTable.sizeDelta = new Vector2(x, spellsTable.sizeDelta.y);
            instance.RectTransform.sizeDelta = new Vector2(x, instance.RectTransform.sizeDelta.y);

            instance.GetField<SlotStatusTable>("slotStatusTable")
                .Bind(null, 1, null, false); //TODO: change spell level label

            if (unlearn)
            {
                instance.RefreshUnlearning(allFeatures, unlearned, spellTag, canAcquireSpells);
            }
            else
            {
                instance.RefreshLearning(allFeatures, unlearned, spellTag, canAcquireSpells);
            }
        }
        
        public static void RefreshLearning(this SpellsByLevelGroup instance,
            //ICharacterBuildingService characterBuildingService,
            List<FeatureDefinition> knownFeatures,
            List<FeatureDefinition> unlearnedFetures,
            string spellTag,
            bool canAcquireFeatures)
        {
            // CharacterHeroBuildingData heroBuildingData = characterBuildingService.CurrentLocalHeroCharacter?.GetHeroBuildingData();
            foreach (Transform transform in instance.GetSpellsTable())
            {
                if (transform.gameObject.activeSelf)
                {
                    SpellBox box = transform.GetComponent<SpellBox>();
                    var boxFeature = box.GetFeature();
                    bool selected = Global.ActiveLevelUpHeroHasFeature(boxFeature);
                    bool isUnlearned = unlearnedFetures != null && unlearnedFetures.Contains(boxFeature);
                    bool canLearn = knownFeatures.Contains(boxFeature) && !selected;

                    var knownNames = String.Join(", ", knownFeatures.Select(f=>f.Name));
                    Main.Log($"[ENDER] SB.refreshLearning '{boxFeature.Name}', canAcquireFeatures: {canAcquireFeatures}, selected: {selected}, known: [{knownNames}]");
                    
                    if (canAcquireFeatures)
                        box.RefreshLearningInProgress(canLearn && !isUnlearned, selected);
                    else
                        box.RefreshLearningInactive((canLearn || selected) && !isUnlearned);
                }
            }
        }
        
        public static void RefreshUnlearning(this SpellsByLevelGroup instance,
            List<FeatureDefinition> knownFeatures,
            List<FeatureDefinition> unlearnedSpells,
            string spellTag,
            bool canUnlearnSpells)
        {
            foreach (Transform transform in instance.GetSpellsTable())
            {
                if (transform.gameObject.activeSelf)
                {
                    SpellBox box = transform.GetComponent<SpellBox>();
                    var boxFeature = box.GetFeature();
                    bool isUnlearned = unlearnedSpells != null && unlearnedSpells.Contains(boxFeature);
                    bool canUnlearn = knownFeatures.Contains(boxFeature) && !isUnlearned;
                    if (canUnlearnSpells)
                        box.RefreshUnlearnInProgress(canUnlearnSpells && canUnlearn, isUnlearned);
                    else
                        box.RefreshUnlearnInactive(isUnlearned);
                }
            }
        }

        public static void CustomUnbind(this SpellsByLevelGroup instance)
        {
            instance.SpellRepertoire = null;
            var spellsTable = instance.GetSpellsTable();
            foreach (Component component in spellsTable)
                component.GetComponent<SpellBox>().CustomUnbind();
            Gui.ReleaseChildrenToPool(spellsTable);
            instance.GetField<SlotStatusTable>("slotStatusTable").Unbind(); //TODO: probably would need custom unbind  
        }
    }

    internal static class SpellBoxExtensions
    {
        private static readonly Dictionary<SpellBox, FeatureDefinition> Features = new();

        public static FeatureDefinition GetFeature(this SpellBox box)
        {
            return Features.GetValueOrDefault(box);
        }

        public static void CustomFeatureBind(this SpellBox instance, FeatureDefinition feature, bool unlearned,
            SpellBox.BindMode bindMode, SpellBox.SpellBoxChangedHandler spellBoxChanged)
        {
            Features.AddOrReplace(instance, feature);

            //instance.GuiSpellDefinition = guiSpellDefinition;

            //instance.Caster = null; //TODO: do we need to set it to null?
            var image = instance.GetField<Image>("spellImage");

            instance.SetField("bindMode", bindMode);
            instance.SpellBoxChanged = spellBoxChanged;
            instance.GetField<GuiLabel>("titleLabel").Text = feature.GuiPresentation.Title;
            // instance.GuiSpellDefinition.SetupSprite(instance.spellImage, (object) null);
            // instance.GuiSpellDefinition.SetupTooltip((ITooltip) instance.tooltip, null);
            instance.SetField("hovered", false);
            instance.SetField("ritualSpell", false);
            instance.SetField("autoPrepared", false);
            instance.SetField("unlearnedSpell", unlearned);
            image.color = Color.white;
            SetupSprite(image, feature.GuiPresentation);
            SetupTooltip(instance.GetField<GuiTooltip>("tooltip"), feature);
            instance.transform.localScale = new Vector3(1f, 1f, 1f);

            GuiModifier component =
                instance.GetField<RectTransform>("availableToLearnGroup").GetComponent<GuiModifier>();
            component.ForwardStartDelay = Random.Range(0.0f, component.Duration);

            instance.SetField("prepared", false);
            instance.SetField("canLearn", false);
            instance.SetField("selectedToLearn", false);
            instance.SetField("canPrepare", false);
            instance.SetField("known", false);
            instance.SetField("canUnlearn", false);

            instance.name = feature.Name;
        }

        public static void SetupSprite(Image imageComponent, GuiPresentation presentation)
        {
            if (imageComponent.sprite != null)
            {
                Gui.ReleaseAddressableAsset(imageComponent.sprite);
                imageComponent.sprite = null;
            }

            if (presentation.SpriteReference != null && presentation.SpriteReference.RuntimeKeyIsValid())
            {
                imageComponent.gameObject.SetActive(true);
                imageComponent.sprite = Gui.LoadAssetSync<Sprite>(presentation.SpriteReference);
            }
            else
                imageComponent.gameObject.SetActive(false);
        }

        public static void SetupTooltip(GuiTooltip tooltip, FeatureDefinition feature)
        {
            if (feature is FeatureDefinitionPower power)
            {
                var gui = ServiceRepository.GetService<IGuiWrapperService>().GetGuiPowerDefinition(power.Name);
                gui.SetupTooltip(tooltip);
            }
            else
            {
                tooltip.TooltipClass = "";
                tooltip.Content = feature.GuiPresentation.Description;
                tooltip.Context = null;
                tooltip.DataProvider = null;
            }
        }

        public static void CustomUnbind(this SpellBox instance)
        {
            Features.Remove(instance);
            instance.Unbind();
        }
    }
}