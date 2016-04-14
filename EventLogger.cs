using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSaver
{
	class EventLogger
	{
		private string LogName = "Application";
		private string SourceName = "Sin Hing Screen Saver";

		static EventLogger()
		{
			Instance = new EventLogger();
		}

		private EventLogger()
		{
			if (!EventLog.SourceExists(SourceName))
				EventLog.CreateEventSource(SourceName, LogName);
		}

		public static EventLogger Instance { get; private set; }

		public void WriteInfo(string eventString)
		{
			EventLog.WriteEntry(SourceName, eventString, EventLogEntryType.Information, 228);
		}

		public void WriteError(string error, params object[] args)
		{
			var e = string.Format(error, args);
			EventLog.WriteEntry(SourceName, e, EventLogEntryType.Error, 229);
		}
	}
}
