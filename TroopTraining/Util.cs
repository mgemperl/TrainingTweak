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
        public static void Warning(string message, Exception exc = null)
        {
            string excString = (exc == null) ? "" : $"\n\n{FlattenException(exc)}";
            MessageBox.Show($"{message}{excString}");
        }

        public static void DebugMessage(string message, Exception exc = null)
        {
            if (Settings.Instance.DebugMode)
            {
                Warning(message, exc);
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
