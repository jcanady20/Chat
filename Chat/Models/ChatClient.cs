using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chat.Models
{
	public class ChatClient
	{
		public string ConnectionId { get; set; }
		public string DisplayName { get; set; }
		public string UserName { get; set; }
		public DateTime Connected { get; set; }
		public string Status { get; set; }
		public string StatusMessage { get; set; }
		public string AvatarClass { get; set; }
	}
}