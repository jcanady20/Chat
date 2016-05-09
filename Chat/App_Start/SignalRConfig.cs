using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Chat.App_Start.SignalRConfig))]
namespace Chat.App_Start
{
    public class SignalRConfig
	{
		public void Configuration(IAppBuilder app)
		{
			app.MapSignalR();
		}
	}
}