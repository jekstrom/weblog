using System.Collections.Generic;
using System.Threading.Tasks;
using weblog.Models;

namespace weblog.Services
{
    public interface IPostService
    {
		Task<bool> Insert(PostModel post);
		Task<IReadOnlyCollection<string>> List();
		Task<string> Get(string name);
		Task<string> GetLatestPostName();
		Task<bool> Delete(string name);
	}
}
