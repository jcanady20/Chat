using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;

namespace Chat.Logging
{
	public class NLogLogger : ILogger
	{
		private Logger _logger;

		public NLogLogger()
		{
			_logger = LogManager.GetCurrentClassLogger();
		}

		public NLogLogger(string name)
		{
			_logger = LogManager.GetLogger(name);
		}

		public void Trace(string message)
		{
			_logger.Trace(message);
		}

		public void Trace(string message, params object[] args)
		{
			_logger.Trace(message, args);
		}

		public void Info(string message)
		{
			_logger.Info(message);
		}

		public void Info(string message, params object[] args)
		{
			_logger.Info(message, args);
		}

		public void Debug(string message)
		{
			_logger.Debug(message);
		}

		public void Debug(string message, params object[] args)
		{
			_logger.Debug(message, args);
		}

		public void Warn(string message)
		{
			_logger.Warn(message);
		}

		public void Warn(string message, params object[] args)
		{
			_logger.Warn(message, args);
		}

		public void Error(Exception x)
		{
			_logger.ErrorException(x.BuildExceptionMessage(), x);
		}

		public void Fatal(Exception x)
		{
			_logger.FatalException(x.BuildExceptionMessage(), x);
		}

		public void Log(LogLevel logLevel, string message, params object[] args)
		{
			_logger.Log(logLevel, message, args);
		}
	}
}