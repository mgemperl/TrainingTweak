using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace TrainingTweak
{
    public static class Strings
    {
        public const string XpPlaceholder = "[xp]";

        private static string FetchString(string id, 
            string backup = "")
        {
            string str;
            TextObject text;

            if (Module.CurrentModule.GlobalTextManager.TryGetText(
                    id, null, out text))
            {
                str = text.ToString();
            }
            else
            {
                str = (backup.Length > 0)
                    ? backup
                    : $"MISSING_STRING[{id}]";
            }

            return str;
        }

        // In-Game strings
        public static string DailyTrainingMessage => FetchString(
            "str_tt_daily_training_message", 
            "Total xp gained from training: [xp]");
        public static string UpgradesAvailableMessage => FetchString(
            "str_tt_upgrades_available_message", 
            "Some troops are ready to upgrade.");

        // Debug and error messages
        public static string DebugSettingGroup => FetchString(
            "str_tt_debug_group", "Debug");
        public static string EnableDebugModeDisplay => FetchString(
            "str_tt_enable_debug_mode_display",
            "Enable Debug Mode");
        public static string EnableDebugModeHint => FetchString(
            "str_tt_enable_debug_mode_hint",
            "Whether this mod displays potential errors it finds while running.");
        public static string TrainingOverrideFatalErrorMessage => FetchString(
            "str_tt_training_fatal_error_message",
            "Training Tweak's training features have encountered an error and are stopping, " +
            "but you may continue playing without them. Re-enabling native training.");
        public static string FatalErrorDisclaimer => FetchString(
            "str_tt_fatal_error_disclaimer",
            "Note: This was not necessarily caused by Training Tweak, it just " +
            "encountered an issue that prevented it from functioning properly.");
        public static string WarningMessageHeader  => FetchString(
            "str_tt_warning_message_header",
            "Training Tweak may have detected a corrupted game state.");
        public static string WarningDisclaimer  => FetchString(
            "str_tt_warning_disclaimer",
            "Note: This was likely not caused by Training Tweak, it just " +
            "detected a potential source of errors in the game state.");
        public static string DebugModeNote  => FetchString(
            "str_tt_debug_mode_note",
            "You may disable these debugging notices in the mod options menu " +
            "by disabling Debug Mode.");
        public static string NullPartyDetected => FetchString(
            "str_tt_null_party_detected",
            "Detected null party in the game state.");
        public static string NullMemberRosterDetected => FetchString(
            "str_tt_null_member_roster_detected",
            "Detected null member roster for party");
        public static string NullCharacterDetected  => FetchString(
            "str_tt_null_character_detected",
            "Detected null member character for party");
        public static string NullTownDetected  => FetchString(
            "str_tt_null_town_detected",
            "Detected null town for garrison");
        public static string NullHeroDetected  => FetchString(
            "str_tt_null_hero_detected",
            "Detected hero with null hero object in party");
        public static string NullCharacter  => FetchString(
            "str_tt_null_character",
            "Null Character");
        public static string HeroNotInPartyDetected => FetchString(
            "str_tt_hero_not_in_party_detected",
            "Hero doesn't consider itself a member of its party");
        public static string PartyLeaderHeader => FetchString(
            "str_tt_party_leader_header",
            "Party Leader");
        public static string SettlementHeader => FetchString(
            "str_tt_settlement_header",
            "Settlement");
        public static string HeroHeader  => FetchString(
            "str_tt_hero_header", 
            "Hero");
        public static string FinancialSolutionsPatchFailed => FetchString(
            "str_tt_financial_solutions_patches_failed",
            "Training Tweak mod's Financial Solutions patches have failed. " +
            "Continuing with Financial Solutions disabled.");
        public static string DisableNativeTrainingPatchFailed => FetchString(
            "str_tt_disable_native_training_failed",
            "Training Tweak mod failed to disable native training. Native training " +
            "and Training Tweak's training will both run in parallel, likely resulting in " +
            "excessive xp gain.");
        public static string ConfigFileFailed => FetchString(
            "str_tt_config_file_failed",
            "Training Tweak failed to load config file. Using default settings.");
    }
}
