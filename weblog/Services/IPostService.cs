using System.Collections.Generic;
using System.Threading.Tasks;
using weblog.Models;

namespace weblog.Services
{
    public interface IPostService
    {
		Task<bool> Insert(PostModel post);
		Task<IReadOnlyCollection<PostModel>> List();
		Task<PostModel> Get(string name);
		Task<PostModel> GetLatestPostName();
		Task<bool> Delete(string name);
	}
}
