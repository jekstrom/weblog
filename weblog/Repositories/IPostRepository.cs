using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using weblog.Models;

namespace weblog.Repositories
{
    public interface IPostRepository
    {
		Task<bool> Insert(string name, string content);
		Task<IReadOnlyCollection<PostModel>> List();
		Task<PostModel> Get(string name);
		Task<bool> Delete(string name);
	}
}
