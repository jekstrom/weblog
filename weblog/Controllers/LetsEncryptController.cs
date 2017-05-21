using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace weblog.Controllers
{
	public class LetsEncryptController : Controller
	{
		[HttpGet]
		[Route(".well-known/acme-challenge/UFC70B-R9iV1vDOMG2SJJ94Bn18KAitU-zgsnot853A")]
		public IActionResult Verify()
		{
			string nonce = "UFC70B-R9iV1vDOMG2SJJ94Bn18KAitU-zgsnot853A.5zRqNqFhk-Ia6CwtsCtB9_TxvElwheqNigRuXxqyEO8";
			return new ObjectResult(nonce);
		}
	}
}
