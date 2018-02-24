using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using weblog.Models;
using weblog.Repositories;

namespace weblog.Services
{
    public class PostService : IPostService
    {
		private readonly IPostRepository _repository;

		public PostService(IPostRepository repository)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			_repository = repository;
		}

		public async Task<bool> Insert(PostModel post)
		{
			return await _repository.Insert(post.Name, post.Content);
		}

		public async Task<IReadOnlyCollection<PostModel>> List()
		{
			return await _repository.List();
		}

		public async Task<PostModel> Get(string name)
		{
			return await _repository.Get(name);
		}

		public async Task<PostModel> GetLatestPostName()
		{
			IReadOnlyCollection<PostModel> posts = await List();

			if (posts.Any())
			{ 
				// Just assume the last post in posts is the latest for now. Order is not guaranteed.
				return posts.Last();
			}

			return new PostModel
			{
				Name = String.Empty,
				Content = String.Empty
			};
		}

		public async Task<bool> Delete(string name)
		{
			return await _repository.Delete(name);
		}
	}
}
