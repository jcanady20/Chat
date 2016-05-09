using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Web.Mvc;
using Chat.Logging;

namespace Chat.Controllers
{
    public class MainController : Controller
    {
		private ILogger m_logger = new NLogLogger("MainController");
        
		public ActionResult Index()
        {
			var userName = GetAuthenticatedUserName(HttpContext.User);
			m_logger.Trace("Context User {0}", userName);
			ViewBag.UserName = userName;
            return View();
        }
		
		[HttpPost]
		public ActionResult Chat(string userName, string avatarClass)
		{
			if (String.IsNullOrEmpty(userName))
			{
				m_logger.Warn("Submitted user name is empty, Redirecting to Index");
				return RedirectToAction("Index");
			}
			var model = Models.MainViewModel.Create();
			model.AvatarClass = avatarClass ?? "avatar-01";
			model.UserName = userName;
			return View(model);
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
