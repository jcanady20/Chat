using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Security.Principal;
using Microsoft.AspNet.SignalR;

using Chat.Logging;
using Chat.Models;

namespace Chat.Controllers.signalR
{
	public class MainHub : Hub
	{
		private ILogger _logger = new NLogLogger("MainHub");

		private const string CHANNELNAME = "general";
		private ClientStore m_clientStore = ClientStore.Instance;

		public Task JoinChat(string userName, string avatar)
		{
			_logger.Trace("{0} Joined the Channel", userName);
			var id = Context.ConnectionId;
			var client = m_clientStore.AddClient(id, userName, avatar);
			Clients.Group(CHANNELNAME).addClient(client);
			return Groups.Add(id, CHANNELNAME);
		}

		public void SendMessage(string userName, string message)
		{
			_logger.Trace("{0} is sending a message", userName);
			var id = Context.ConnectionId;
			ChatClient client = m_clientStore.GetClientByConnectionId(id);
			//	Create a new Client object and added it to the pool
			if(String.IsNullOrEmpty(userName) || client == null)
			{
				var identityUserName = GetAuthenticatedUserName(Context.User);
				client = m_clientStore.AddClient(id, identityUserName, "avatar-01");
			}
			if (String.IsNullOrEmpty(message) == false)
			{
				parseMessage(client, message);
			}
		}

		private void parseMessage(ChatClient client, string message)
		{
			_logger.Trace("Parsing Chat Message");
			var cm = new ChatMessage();
			cm.FromUser = client.UserName;
			cm.Sent = DateTime.Now;
			cm.FromAvatar = client.AvatarClass;

			var parts = message.Split(new char[] {' '});
			var cmd = parts[0];
			if (cmd.StartsWith("/"))
			{
				if(cmd == "/w")
				{
					cm.ToUser = parts[1];
					var toUsrId = m_clientStore.GetConnectionIdByName(cm.ToUser);
					var msg = String.Join(" ", parts, 2, parts.Count() - 2);

					cm.Message = msg;
					Clients.Clients(new string[] {toUsrId, Context.ConnectionId}).addChatMessage(cm);
				}
			}
			else
			{
				cm.Message = message;
				Clients.Group(CHANNELNAME).addChatMessage(cm);
			}
		}

		public override Task OnConnected()
		{
			return base.OnConnected();
		}

		public override Task OnDisconnected()
		{
			var id = Context.ConnectionId;
			var client = m_clientStore.RemoveClient(id);
			Clients.Group(CHANNELNAME).removeClient(client);
			Groups.Remove(id, CHANNELNAME);
			return base.OnDisconnected();
		}

		public override Task OnReconnected()
		{
			return base.OnReconnected();
		}
		
		private string GetAuthenticatedUserName(IPrincipal user)
		{
			string name = string.Empty;
			if (user.Identity.IsAuthenticated)
			{
				name = user.Identity.Name;
				if (name.IndexOf("\\") > 0)
				{
					var tmp = name.Split("\\".ToCharArray());
					name = tmp[1];
				}
			}
			else
			{
				name = "Unknown";
			}
			return name;
		}
	}
}