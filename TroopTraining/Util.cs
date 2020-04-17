using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrainingTweak
{
    public static class Util
    {
        public static void Warning(string message, Exception exc = null,
            bool showDisclaimer = true)
        {
            string excString = (exc == null) ? "" : $"\n\n{FlattenException(exc)}";
            string disclaimerString = showDisclaimer
                ? "\n\nNote: This was not necessarily caused by Training Tweak, it just" +
                  "encountered an issue that prevented it from functioning properly."
                : "";
            MessageBox.Show($"{message}{excString}{disclaimerString}");
        }

        public static void DebugMessage(string message, Exception exc = null,
            bool showDisclaimer = true)
        {
            if (Settings.Instance.DebugMode)
            {
                Warning(message, exc, showDisclaimer);
            }

            // TODO: Should probably log something somewhere somehow
        }
         
        public static string FlattenException(Exception exc)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(exc.Message);
            builder.AppendLine(exc.StackTrace);
            while (exc.InnerException != null)
            {
                builder.AppendLine(exc.InnerException.StackTrace);
                exc = exc.InnerException;
            }
            return builder.ToString();
        }
    }
}
