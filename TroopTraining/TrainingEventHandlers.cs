using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;

namespace TrainingTweak
{
    public static class TrainingEventHandlers
    {
        private static int ExecuteDailyTraining(Hero hero)
        {
            if (hero.IsPrisoner)
            {
                return 0;
            }    

            var partyMembers = hero.PartyBelongedTo.MemberRoster;
            int totalXp = 0;
            int baseXpGain = 0;
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

            // For each member of the hero's party
            for (int idx = 0; idx < partyMembers.Count; idx++)
            {
                var curMember = partyMembers.GetCharacterAtIndex(idx);
                // If member is a troop, and low enough tier to be trained
                if (!curMember.IsHero && curMember.Tier <= maxTierTrained)
                {
                    int numNotWounded = partyMembers.GetElementNumber(idx)
                        - partyMembers.GetElementWoundedNumber(idx);
                    double levelDiffMult = 1.0;
                    if (Settings.Instance.LevelDifferenceMultiplierMultiple >= 1.0)
                    {
                        levelDiffMult = 1.0 + Math.Max(0, (hero.Level - curMember.Level))
                            / (double)Settings.Instance.LevelDifferenceMultiplierMultiple;
                    }
                    double xpPerTroop = levelDiffMult * baseXpGain;
                    int xpForCurElement = (int)Math.Round(numNotWounded * xpPerTroop);

                    partyMembers.AddXpToTroopAtIndex(xpForCurElement, idx);
                    totalXp += xpForCurElement;
                }
            }

            if (Settings.Instance.TrainingXPToLeadershipXP > 0)
            {
                // Give hero leadership xp
                hero.AddSkillXp(DefaultSkills.Leadership,
                    (int)Math.Ceiling((double) totalXp / Settings.Instance.TrainingXPToLeadershipXP));
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
                    $"Training total xp gain: {totalXp}"));
            }
        }
    }
}
