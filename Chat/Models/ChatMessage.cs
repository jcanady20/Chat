using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chat.Models
{
	public class ChatMessage
	{
		public string Message { get; set; }
		public string FromUser { get; set; }
		public string FromAvatar { get; set; }
		public string ToUser { get; set; }
		public DateTime Sent { get; set; }
	}
}