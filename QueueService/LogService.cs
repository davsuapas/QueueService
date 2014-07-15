using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace QueueService {

    public class LogService {

        public const string CONST_EVENTLOG_SOURCENAME = "QueueService";
        public const string CONST_EVENTLOG_LOGNAME = "QueueService";

        private static EventLog GetLogService() {
            var log = new EventLog();
            log.Source = CONST_EVENTLOG_SOURCENAME;
            return log;
        }

        public static void WriteInfo(String message, params object[] args) {
           Util.TryExecute( () => GetLogService().WriteEntry("Information - " + String.Format(message, args)));
        }

        public static void WriteError(String message, params object[] args) {
            Util.TryExecute( () => GetLogService().WriteEntry("Error - " + String.Format(message, args)));
        }

        public static void WriteWarning(String message, params object[] args) {
            Util.TryExecute( () => GetLogService().WriteEntry("Warning - " + String.Format(message, args)));
        }
    }
}
