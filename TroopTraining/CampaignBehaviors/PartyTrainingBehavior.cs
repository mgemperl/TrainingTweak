using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TrainingTweak.CampaignBehaviors
{
    public class PartyTrainingBehavior : CampaignBehaviorBase
    {
        private static bool _trainingDisabled = false;
        private static HashSet<string> _reported = new HashSet<string>();

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(
                this, SafeHandleDailyTraining);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void SafeHandleDailyTraining(MobileParty party)
        {
            try
            {
                if (!_trainingDisabled)
                {
                    HandleDailyTraining(party);
                }
            }
            catch (Exception exc)
            {
                _trainingDisabled = true;
                Settings.Instance.EnableGarrisonTraining = false;
                Settings.Instance.EnableTrainingPerkOverrides = false;
                Settings.Instance.EnableBaseTraining = false;
                Util.Warning(
                    $"{Strings.TrainingOverrideFatalErrorMessage}\n\n" +
                    $"{Strings.FatalErrorDisclaimer}",
                    exc: exc);
            }
        }

        
    }
}
