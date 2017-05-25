using System.Collections.Generic;

namespace weblog.Models
{
	public class ArchiveModel
	{
		public IReadOnlyCollection<PostModel> Posts { get; set; }
	}
}