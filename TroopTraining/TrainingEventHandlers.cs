using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace TrainingTweak
{
    public static class TrainingEventHandlers
    {
        private static int ExecuteDailyTraining(Hero hero)
        {
            if (hero?.PartyBelongedTo?.MemberRoster == null)
            {
                return 0;
            }    

            var partyMembers = hero.PartyBelongedTo.MemberRoster;
            int totalXp = 0;
            double baseXpGain = 0;
            int maxTierTrained = 0;

            // If hero has Raise The Meek perk
            if (hero.GetPerkValue(DefaultPerks.Leadership.RaiseTheMeek))
            {
                baseXpGain = Campaign.Current.Models.PartyTrainingModel
                    .GetTroopPerksXp(DefaultPerks.Leadership.RaiseTheMeek);
                maxTierTrained = Math.Max(0, Settings.Instance.RaiseTheMeekMaxTierTrained);
            }
            // If hero has Combat Tips perk
            else if (hero.GetPerkValue(DefaultPerks.Leadership.CombatTips))
            {
                baseXpGain = Campaign.Current.Models.PartyTrainingModel
                    .GetTroopPerksXp(DefaultPerks.Leadership.CombatTips);
                maxTierTrained = int.MaxValue;
            }
            // If player has neither perk, do nothing
            else
            {
                return 0;
            }

            // Apply xp multiplier
            baseXpGain *= Math.Max(0, Settings.Instance.TrainingXpMultiplier);

            // For each group in the hero's party
            for (int idx = 0; idx < partyMembers.Count; idx++)
            {
                var curMember = partyMembers.GetCharacterAtIndex(idx);
                // If member is a troop, and low enough tier to be trained
                if (!curMember.IsHero && curMember.Tier <= maxTierTrained)
                {
                    int numInGroup = partyMembers.GetElementNumber(idx);
                    int numNotTrained = 0;

                    if (!Settings.Instance.WoundedReceiveTraining)
                    {
                        numNotTrained = partyMembers.GetElementWoundedNumber(idx);
                    }
                    if (!Settings.Instance.UpgradeableReceiveTraining)
                    {
                        numNotTrained = Math.Max(numNotTrained, 
                            partyMembers.GetElementCopyAtIndex(idx).NumberReadyToUpgrade);
                    }

                    numInGroup -= numNotTrained;

                    double levelDiffMult = 1.0;
                    if (Settings.Instance.LevelDifferenceMultiple >= 1.0)
                    {
                        levelDiffMult = 1.0 + Math.Max(0, (hero.Level - curMember.Level))
                            / (double)Settings.Instance.LevelDifferenceMultiple;
                    }
                    double xpPerTroop = levelDiffMult * baseXpGain;
                    int xpForCurElement = (int)Math.Round(numInGroup * xpPerTroop);

                    partyMembers.AddXpToTroopAtIndex(xpForCurElement, idx);
                    totalXp += xpForCurElement;
                }
            }

            if (Settings.Instance.TrainingXpPerLeadershipXp > 0)
            {
                // Give hero leadership xp
                hero.AddSkillXp(DefaultSkills.Leadership,
                    (int)Math.Ceiling((double) totalXp 
                        / Settings.Instance.TrainingXpPerLeadershipXp));
            }

            return totalXp;
        }

        public static void DailyTrainingTickHandler(MBCampaignEvent campaignEvent, 
            params object[] delegateParams)
        {
            var hero = Hero.MainHero; // Only apply to player... for now
            int totalXp = ExecuteDailyTraining(hero);

            if (totalXp > 0)
            {
                // Display training results to player
                InformationManager.DisplayMessage(new InformationMessage(
                    $"Total xp gained from training: {totalXp}"));

                // If there are troops ready to upgrade
                if (Hero.MainHero?.PartyBelongedTo?.MemberRoster != null
                    && !Hero.MainHero.PartyBelongedTo.MemberRoster
                        .Where(elem => elem.NumberReadyToUpgrade > 0)
                        .IsEmpty())
                {
                    InformationManager.DisplayMessage(new InformationMessage(
                        "Some troops are ready to upgrade."));
                }
            }
        }
    }
}
