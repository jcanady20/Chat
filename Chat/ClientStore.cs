using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Chat.Models;

namespace Chat
{
	public class ClientStore
	{
		private readonly Dictionary<string, ChatClient> _clients;
		private static object _objLock = new object();
		private static ClientStore _instance;
		private ClientStore()
		{
			_clients = new Dictionary<string, Models.ChatClient>();
		}

		public static ClientStore Instance
		{
			get
			{
				lock (_objLock)
				{
					if (_instance == null)
					{
						_instance = new ClientStore();
					}
				}
				return _instance;
			}
		}

		public string GetConnectionIdByName(string name)
		{
			var connectionId = String.Empty;
			lock(_objLock)
			{
				var client = _clients.Values.FirstOrDefault(x => String.Compare(x.UserName, name, true) == 0);
				if(client != null)
				{
					connectionId = client.ConnectionId;
				}
			}
			return connectionId;
		}

		public string GetNameByConnectionId(string id)
		{
			var name = String.Empty;

			lock (_objLock)
			{
				if (_clients.ContainsKey(id))
				{
					name = _clients[id].UserName;
				}
			}

			return name;
		}

		public ChatClient GetClientByConnectionId(string id)
		{
			ChatClient client = null;

			lock(_objLock)
			{
				if (_clients.ContainsKey(id))
				{
					client = _clients[id];
				}
			}

			return client;
		}

		public IEnumerable<ChatClient> GetClients()
		{
			lock (_objLock)
			{
				foreach (var kvp in _clients)
				{
					yield return kvp.Value;
				}
			}
		}

		public ChatClient AddClient(string connectionId, string userName, string avatar)
		{
			ChatClient client;
			lock (_objLock)
			{
				var oldid = GetConnectionIdByName(userName);
				if (_clients.ContainsKey(oldid))
				{
					removeClient(oldid);
				}
				if (_clients.ContainsKey(connectionId))
				{
					removeClient(connectionId);
				}
				client = CreateClient(connectionId, userName, avatar);
				_clients.Add(connectionId, client);
			}
			return client;
		}

		private ChatClient CreateClient(string connectionId, string userName, string avatar)
		{
			var client = new Models.ChatClient()
			{
				ConnectionId = connectionId,
				UserName = userName,
				Status = "Online",
				StatusMessage = "",
				Connected = DateTime.Now,
				AvatarClass = avatar
			};

			return client;
		}

		public ChatClient RemoveClient(string connectionId)
		{
			ChatClient client = null;
			lock (_objLock)
			{
				if(_clients.ContainsKey(connectionId))
				{
					client = _clients[connectionId];
					removeClient(connectionId);
				}
			}
			return client;
		}

		private void removeClient(string id)
		{
			_clients.Remove(id);
		}
	}
}