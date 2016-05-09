using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace Chat.Logging
{
	public interface ILogger
	{
		void Trace(string message);
		void Trace(string message, params object[] args);
		void Info(string message);
		void Info(string message, params object[] args);
		void Debug(string message);
		void Debug(string message, params object[] args);
		void Warn(string message);
		void Warn(string message, params object[] args);
		void Error(Exception x);
		void Fatal(Exception x);
		void Log(LogLevel logLevel, string message, params object[] items);
	}
}
