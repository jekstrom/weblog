using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using weblog.Models;

namespace weblog.Repositories
{
    public class PostRepository : IPostRepository
    {
		private readonly CloudBlobContainer _postContainer;

		public PostRepository(string connectionString)
		{
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

			CloudBlobClient client = storageAccount.CreateCloudBlobClient();

			_postContainer = client.GetContainerReference("posts");
		}

		public async Task<bool> Insert(string name, string content)
		{
			CloudBlockBlob blob = _postContainer.GetBlockBlobReference(name);

			await blob.UploadTextAsync(content);
			
			return true;
		}

		public async Task<IReadOnlyCollection<PostModel>> List()
		{
			BlobResultSegment segment = await _postContainer.ListBlobsSegmentedAsync(null);

			return segment.Results.Select(
				blob => 
				new PostModel
				{
					Name = ((CloudBlockBlob)blob).Name,
					LastModified = ((CloudBlockBlob)blob).Properties.LastModified?.DateTime.ToUniversalTime() ?? new DateTime()
				}
			).ToList();
		}

		public async Task<PostModel> Get(string name)
		{
			CloudBlockBlob blob = _postContainer.GetBlockBlobReference(name);

			string content = await blob.DownloadTextAsync();

			return new PostModel
			{
				Name = name,
				Content = content,
				LastModified = blob.Properties.LastModified?.DateTime ?? new DateTime()
			};
		}

		public async Task<bool> Delete(string name)
		{
			CloudBlockBlob blob = _postContainer.GetBlockBlobReference(name);

			return await blob.DeleteIfExistsAsync();
		}
    }
}
