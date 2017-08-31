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
		[Route(".well-known/acme-challenge/dIPPI-VdxTwJh91ZxfbfYn927ozJlqbp3Q6BLxM_iP4")]
		public IActionResult Verify()
		{
			string nonce = "dIPPI-VdxTwJh91ZxfbfYn927ozJlqbp3Q6BLxM_iP4.6kqZFPlL1G73MD7rpngpKy3uksSfZQUojbI-XsG2Tp8";
			return new ObjectResult(nonce);
		}
	}
}
