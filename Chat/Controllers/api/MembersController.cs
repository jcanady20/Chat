using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Chat.Models;

namespace Chat.Controllers.api
{
    public class MembersController : ApiController
    {

		public IHttpActionResult Get()
		{
			try
			{
				var clients = ClientStore.Instance.GetClients();
				return Ok(clients);
			}
			catch(Exception e)
			{
				return new ExceptionResult(e, this);
			}
		}

    }
}
