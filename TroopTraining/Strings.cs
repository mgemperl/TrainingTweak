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

        // Settings group names
        public static string XpMultipliersSettingGroup => FetchString(
            "str_tt_xp_multipliers_group", "Training Xp Multipliers");
        public static string TierLimitsSettingGroup => FetchString(
            "str_tt_tier_limits_group", "Tier Limits");
        public static string FinancialSolutionsSettingGroup => FetchString(
            "str_tt_financial_solutions_group", "Financial Solutions");
        public static string BaseTrainingSettingGroup => FetchString(
            "str_tt_base_training_group", "Basic Training");
        public static string GeneralSettingGroup => FetchString(
            "str_tt_general_group", "General");

        // Xp Multipliers
        public static string PlayerPartyTrainingMultDisplay => FetchString(
            "str_tt_player_party_training_mult_display",
            "Player Party Training Xp Multiplier");
        public static string PlayerPartyTrainingMultHint => FetchString(
            "str_tt_player_party_training_mult_hint",
            "Multiplier for all xp this mod gives to the player's party.");
        public static string PlayerClanPartyTrainingMultDisplay => FetchString(
            "str_tt_player_clan_party_training_mult_display",
            "Player Clan Party Training Xp Multiplier");
        public static string PlayerClanPartyTrainingMultHint => FetchString(
            "str_tt_player_clan_party_training_mult_hint",
            "Multiplier for all xp this mod gives to AI parties in the player's clan.");
        public static string PlayerClanGarrisonTrainingMultDisplay => FetchString(
            "str_tt_player_clan_garrison_training_mult_display",
            "Player Clan Garrison Training Xp Multiplier");
        public static string PlayerClanGarrisonTrainingMultHint => FetchString(
            "str_tt_player_clan_garrison_training_mult_hint",
            "Multiplier for all xp this mod gives to garrisons of player-owned settlements.");
        public static string NonPlayerClanPartyTrainingMultDisplay => FetchString(
            "str_tt_non_player_clan_party_training_mult_display",
            "Non-Player Clan Party Training Xp Multiplier");
        public static string NonPlayerClanPartyTrainingMultHint => FetchString(
            "str_tt_non_player_clan_party_training_mult_hint",
            "Multiplier for all xp this mod gives to AI parties not in the player's clan.");
        public static string NonPlayerClanGarrisonTrainingMultDisplay => FetchString(
            "str_tt_non_player_clan_garrison_training_mult_display",
            "Non-Player Clan Garrison Training Xp Multiplier");
        public static string NonPlayerClanGarrisonTrainingMultHint => FetchString(
            "str_tt_non_player_clan_garrison_training_mult_hint",
            "Multiplier for all xp this mod gives to garrisons of settlements not owned by the player.");

        // Tier Limits
        public static string RaiseTheMeekTierDisplay => FetchString(
            "str_tt_raise_the_meek_tier_display",
            "Raise The Meek Max Tier Trained");
        public static string RaiseTheMeekTierHint => FetchString(
            "str_tt_raise_the_meek_tier_hint",
            "Max tier of troops in parties that will be trained by the 'Raise The Meek' perk.");

        // In-Game strings
        public static string DailyTrainingMessage => FetchString(
            "str_tt_daily_training_message", 
            "Total xp gained from training: [xp]");
        public static string UpgradesAvailableMessage => FetchString(
            "str_tt_upgrades_available_message", 
            "Some troops are ready to upgrade.");

        // Debug and error messages
        public static string FatalErrorMessage => FetchString(
            "str_tt_fatal_error_message",
            "Training Tweak has encountered an error and is stopping. You may " +
            "continue playing without Training Tweak, but your game state may " +
            "already be corrupted.");
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
        public static string FinancialSolutionsFailed => FetchString(
            "str_tt_financial_solutions_failed",
            "Training tweak mod failed to apply Financial Solutions patches. " +
            "Continuing without them.");
        public static string ConfigFileFailed => FetchString(
            "str_tt_config_file_failed",
            "Training Tweak failed to load config file. Using default settings.");
        public static string SettingsRegistrationFailed => FetchString(
            "str_tt_settings_registration_failed",
            "Training Tweak failed to register settings with MCM, so its " +
            "mod options menu will not appear. Using default settings.");
    }
}
