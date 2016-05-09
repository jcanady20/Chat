using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;

namespace Chat.Logging
{
	public class StopWatchLogger : IDisposable
	{
		private const string APPSETTINGNAME = "EnableStopWatchLogger";
		private ILogger m_logger;
		private Stopwatch m_stopwatch;
		private string m_methodName;

		public StopWatchLogger(ILogger logger, /*[CallerMemberName] 4.5 ONLY*/string methodName = "")
		{
			m_logger = logger;
			m_methodName = methodName;

			if (IsDebug() || IsConfigEnabled())
			{
				if (String.IsNullOrEmpty(methodName))
				{
					m_methodName = getCallingMethodName();
				}

				ReportStarting();
				m_stopwatch = new Stopwatch();
				m_stopwatch.Start();
			}
		}

		public StopWatchLogger(string methodName = "")
			: this(null, methodName)
		{ }

		private void ReportStarting()
		{
			string message = String.Format("Started : {0} :", m_methodName);
			LogMessage(message);
		}

		private void ReportElapsedTime()
		{

			m_stopwatch.Stop();
			var elapsed = m_stopwatch.Elapsed;
			string message = String.Format("Completed : {0} : {1}:{2}:{3}", m_methodName, elapsed.Minutes, elapsed.Seconds, elapsed.Milliseconds);
			LogMessage(message);
		}

		private void LogMessage(string message)
		{
			if (m_logger == null)
			{
				Debug.WriteLine(message);
				return;
			}
			m_logger.Debug(message);
		}

		private string getCallingMethodName()
		{
			var methodName = string.Empty;
			var frame = new StackFrame(2);
			var method = frame.GetMethod();

			methodName = method.Name;

			return methodName;
		}

		private bool IsDebug()
		{
			bool isDebug = false;

#if DEBUG
			isDebug = true;
#endif

			return isDebug;
		}

		private bool IsConfigEnabled()
		{
			bool isEnabled = false;

			Boolean.TryParse(ConfigurationManager.AppSettings[APPSETTINGNAME], out isEnabled);

			return isEnabled;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool dispose)
		{
			if (dispose == false || m_stopwatch == null)
			{
				return;
			}

			if (m_stopwatch.IsRunning)
			{
				ReportElapsedTime();
			}
		}
	}
}