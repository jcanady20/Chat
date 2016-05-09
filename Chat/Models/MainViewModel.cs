using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chat.Models
{
	public class MainViewModel
	{
		public MainViewModel() { }

		public string UserName { get; set; }
		public string AvatarClass { get; set; }

		public static MainViewModel Create()
		{
			var model = new MainViewModel();

			return model;
		}
	}
}