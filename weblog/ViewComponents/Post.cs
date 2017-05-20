using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using weblog.Models;
using weblog.Services;

namespace weblog.ViewComponents
{
	[AllowAnonymous]
    public class Post : ViewComponent
    {
		private readonly IPostService _postService;

		public Post(IPostService postService)
		{
			if (postService == null)
			{
				throw new ArgumentNullException(nameof(postService));
			}

			_postService = postService;
		}

		public async Task<IViewComponentResult> InvokeAsync(string name)
		{
			// Get content from database
			string content = await _postService.Get(name);

			return View(new PostModel { Name = name, Content = content });
		}
    }
}
