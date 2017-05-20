using System.ComponentModel.DataAnnotations;

namespace weblog.Models
{
    public class PostModel
    {
		[Required]
		public string Name { get; set; }
		[Required]
		public string Content { get; set; }
    }
}
