using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace System
{
	public static class ExceptionExtensions
	{
		public static string BuildExceptionMessage(this Exception e)
		{
			var msg = new StringBuilder();
			msg.AppendLine(e.Message);
			if (e.InnerException != null) { msg.Append(e.InnerException.BuildExceptionMessage()); }
			return msg.ToString();
		}
	}
}