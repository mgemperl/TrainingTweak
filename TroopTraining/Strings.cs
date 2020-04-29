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

        /*
        public static void LoadStrings(string localizationFolderPath,
            GameTextManager textManager)
        {
            List<string> fileNames = new List<string>();

            if (Directory.Exists(localizationFolderPath))
            {
                fileNames = Directory.EnumerateFiles(localizationFolderPath, "*.xml")
                    .Select(path => Path.GetFileName(path))
                    .ToList();
            }

            // If language files exist
            if (fileNames.Count > 0)
            {
                string filePath;
                // If localization for user language and country is present
                if (fileNames.Contains(CultureInfo.CurrentCulture.Name + ".xml"))
                {
                    filePath = Path.Combine(localizationFolderPath, fileNames
                        .Find(fileName => fileName == CultureInfo.CurrentCulture.Name + ".xml"));
                }
                // If localization for user language is present
                else if (!fileNames.Where(fileName => MatchesUserLanguage(fileName))
                    .IsEmpty())
                {
                    filePath = Path.Combine(localizationFolderPath, fileNames
                        .Find(fileName => MatchesUserLanguage(fileName)));
                }
                // If only one language is available
                else if (fileNames.Count == 1)
                {
                    filePath = Path.Combine(localizationFolderPath, fileNames.First());
                }
                else
                {
                    // Use en-US
                    filePath = Path.Combine(localizationFolderPath, "en-US.xml");
                }

                //strings = ReadLanguageFile(filePath);
                textManager.LoadGameTexts(filePath);
            }
        }

        private static bool MatchesUserLanguage(string fileName)
        {
            string[] splitCulture = CultureInfo.CurrentCulture.Name.Split('-');
            string userLanguage = string.Join("", splitCulture.Take(splitCulture.Length - 1));

            splitCulture = fileName.Split('-');
            string fileLanguage = string.Join("", splitCulture.Take(
                Math.Min(1, splitCulture.Length - 1)));

            return userLanguage == fileLanguage;
        }
        */

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

        public static string DailyTrainingMessage
        {
            get
            {
                return FetchString("str_tt_daily_training_message",
                    "TEST Total xp gained from training: [xp]");
            }
        }
        public static string UpgradesAvailableMessage 
        {
            get
            {
                return FetchString("str_tt_upgrades_available_message",
                    "Some troops are ready to upgrade.");
            }
        }

        public static string FatalErrorMessage
        {
            get
            {
                return FetchString("str_tt_fatal_error_message",
                    "Training Tweak has encountered an error and is stopping. You may " +
                    "continue playing without Training Tweak, but your game state may " +
                    "already be corrupted.");
            }
        }
        public static string FatalErrorDisclaimer
        {
            get
            {
                return FetchString("str_tt_fatal_error_disclaimer",
                    "Note: This was not necessarily caused by Training Tweak, it just " +
                    "encountered an issue that prevented it from functioning properly.");
            }
        }
        public static string WarningMessageHeader 
        {
            get
            {
                return FetchString("str_tt_warning_message_header",
                    "Training Tweak may have detected a corrupted game state.");
            }
        }
        public static string WarningDisclaimer 
        {
            get
            {
                return FetchString("str_tt_warning_disclaimer",
                    "Note: This was likely not caused by Training Tweak, it just " +
                    "detected a potential source of errors in the game state.");
            }
        }
        public static string DebugModeNote 
        {
            get
            {
                return FetchString("str_tt_debug_mode_note",
                    "You may disable these debugging notices in the mod options menu " +
                    "by disabling Debug Mode.");
            }
        }
        public static string NullPartyDetected
        {
            get
            {
                return FetchString("str_tt_null_party_detected",
                    "Detected null party in the game state.");
            }
        }
        public static string NullMemberRosterDetected
        {
            get
            {
                return FetchString("str_tt_null_member_roster_detected",
                    "Detected null member roster for party");
            }
        }
        public static string NullCharacterDetected 
        {
            get
            {
                return FetchString("str_tt_null_character_detected",
                    "Detected null member character for party");
            }
        }
        public static string NullTownDetected 
        {
            get
            {
                return FetchString("str_tt_null_town_detected",
                    "Detected null town for garrison");
            }
        }
        public static string NullHeroDetected 
        {
            get
            {
                return FetchString("str_tt_null_hero_detected",
                    "Detected hero with null hero object in party");
            }
        }
        public static string NullCharacter 
        {
            get
            {
                return FetchString("str_tt_null_character",
                    "Null Character");
            }
        }
        public static string HeroNotInPartyDetected
        {
            get
            {
                return FetchString("str_tt_hero_not_in_party_detected",
                    "Hero doesn't consider itself a member of its party");
            }
        }
        public static string PartyLeaderHeader
        {
            get
            {
                return FetchString("str_tt_party_leader_header",
                    "Party Leader");
            }
        }
        public static string SettlementHeader
        {
            get
            {
                return FetchString("str_tt_settlement_header",
                    "Settlement");
            }
        }
        public static string HeroHeader 
        {
            get
            {
                return FetchString("str_hero_header", "Hero");
            }
        }
    }
}
