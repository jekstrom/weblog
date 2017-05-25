using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using weblog.Models;
using weblog.Services;

namespace weblog.ViewComponents
{
	[AllowAnonymous]
    public class Archive : ViewComponent
    {
		private readonly IPostService _postService;

		public Archive(IPostService postService)
		{
			if (postService == null)
			{
				throw new ArgumentNullException(nameof(postService));
			}

			_postService = postService;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			IReadOnlyCollection<PostModel> posts = await _postService.List();

			return View(new ArchiveModel { Posts = posts });
		}
    }
}
