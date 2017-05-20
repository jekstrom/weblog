using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace weblog.Repositories
{
    public interface IPostRepository
    {
		Task<bool> Insert(string name, string content);
		Task<IReadOnlyCollection<string>> List();
		Task<string> Get(string name);
		Task<bool> Delete(string name);
	}
}
