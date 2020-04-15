using System;
using TaleWorlds.CampaignSystem;

namespace TrainingTweak.CampaignBehaviors
{
    public class GarrisonTrainingBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(
                this, TrainGarrison);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void TrainGarrison(Settlement settlement)
        {
            // If not a town or castle, do nothing
            if (settlement?.Town == null || !settlement.IsTown)
            {
                return;
            }

            Town town = settlement.Town;
            // If town has an active garrison
            if (town.GarrisonParty != null && town.GarrisonParty.IsActive)
            {
                // Get configured multiplier for this garrison
                double multiplier = (settlement.OwnerClan == Hero.MainHero.Clan)
                    ? Settings.Instance.PlayerClanGarrisonTrainingXpMultiplier
                    : Settings.Instance.NonPlayerClanGarrisonTrainingXpMultiplier;

                // Get base xp gain for this garrison
                var trainingModel = Campaign.Current.Models.DailyTroopXpBonusModel;
                double xpPerTroop = trainingModel.CalculateDailyTroopXpBonus(town)
                    * trainingModel.CalculateGarrisonXpBonusMultiplier(town)
                    * multiplier;

                // If training this garrison, and garrison has member list
                if (xpPerTroop > 0 && town.GarrisonParty.MemberRoster != null)
                {
                    var members = town.GarrisonParty.MemberRoster;

                    // For each group in the garrison
                    for (int idx = 0; idx < town.GarrisonParty.MemberRoster.Count; idx++)
                    {
                        int numInGroup = members.GetElementNumber(idx);
                        int numUpgradeable = members.GetElementCopyAtIndex(idx)
                            .NumberReadyToUpgrade;

                        // If there are troops in group to train
                        if (numUpgradeable < numInGroup)
                        {
                            int numNotTrained = 0;
                            // Remove wounded from training if configured to
                            if (!Settings.Instance.WoundedReceiveTraining)
                            {
                                numNotTrained = members.GetElementWoundedNumber(idx);
                            }
                            // Remove upgradeable from training if configured to
                            if (!Settings.Instance.UpgradeableReceiveTraining)
                            {
                                // Allow wounded troops to be considered the upgradeable ones
                                numNotTrained = Math.Max(numNotTrained, numUpgradeable);
                            }

                            int xpForCurGroup = (int) Math.Ceiling(
                                (numInGroup - numNotTrained) * xpPerTroop);
                            // Add xp to current troop group
                            if (xpForCurGroup > 0)
                            {
                                members.AddXpToTroopAtIndex(xpForCurGroup, idx);
                            }
                        }
                    }
                }
            }
        }
    }
}
