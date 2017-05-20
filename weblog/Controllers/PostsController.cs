using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using weblog.Models;
using weblog.Services;
using Microsoft.AspNetCore.Authorization;

namespace weblog.Controllers
{
    public class PostsController : Controller
    {
		private readonly IPostService _postService;

		public PostsController(IPostService postService)
		{
			if (postService == null)
			{
				throw new ArgumentNullException(nameof(postService));
			}

			_postService = postService;
		}

		public async Task<IActionResult> Index()
		{
			IReadOnlyCollection<string> posts = await _postService.List();

			return View(posts);
		}

        public IActionResult Post(string postName)
        {
			return View(new PostModel { Name = postName });
        }

		[Authorize]
		public IActionResult Create()
		{
			return View();
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Create(PostModel post)
		{
			if (ModelState.IsValid)
			{
				await _postService.Insert(post);
			}

			return View(post);
		}
    }
}