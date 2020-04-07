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
        public static void DailyTrainingTickHandler(MBCampaignEvent campaignEvent, 
            params object[] delegateParams)
        {
            var hero = Hero.MainHero; // Only apply to player... for now
            var partyMembers = hero.PartyBelongedTo.MemberRoster;
            int totalXp = 0;
            int baseXpGain = 0;
            int maxTierTrained = 0;
            string perkName = "";

            // If hero has Raise The Meek perk
            if (hero.GetPerkValue(DefaultPerks.Leadership.RaiseTheMeek))
            {
                baseXpGain = Campaign.Current.Models.PartyTrainingModel
                    .GetTroopPerksXp(DefaultPerks.Leadership.RaiseTheMeek);
                maxTierTrained = Settings.Instance.RaiseTheMeekMaxTierTrained;
                perkName = DefaultPerks.Leadership.RaiseTheMeek.Name.ToString();
            }
            // If hero has Combat Tips perk
            else if (hero.GetPerkValue(DefaultPerks.Leadership.CombatTips))
            {
                baseXpGain = Campaign.Current.Models.PartyTrainingModel
                    .GetTroopPerksXp(DefaultPerks.Leadership.CombatTips);
                maxTierTrained = int.MaxValue;
                perkName = DefaultPerks.Leadership.CombatTips.Name.ToString();
            }
            // If player has neither perk, do nothing
            else
            {
                return;
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
                    double levelDiffMult = 1.0 + Math.Max(0, (hero.Level - curMember.Level))
                        / (double) Settings.Instance.LevelDifferenceMultiplierMultiple;
                    double xpPerTroop = levelDiffMult * baseXpGain;
                    int xpForCurElement = (int) Math.Round(numNotWounded * xpPerTroop);

                    partyMembers.AddXpToTroopAtIndex(xpForCurElement, idx);
                    totalXp += xpForCurElement;
                }
            }

            // Display training results to player
            InformationManager.DisplayMessage(new InformationMessage(
                $"{perkName} total xp gain: {totalXp}"));
        }
    }
}
