using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using weblog.Models;
using Microsoft.AspNetCore.Authorization;
using weblog.Services;

namespace weblog.Controllers
{
	[AllowAnonymous]
    public class HomeController : Controller
    {
		private readonly IPostService _postService;

		public HomeController(IPostService postService)
		{
			if (postService == null)
			{
				throw new ArgumentNullException(nameof(postService));
			}
			_postService = postService;
		}

        public async Task<IActionResult> Index()
        {
			// Get primary post
			
            return View(await _postService.GetLatestPostName());
        }

		public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
