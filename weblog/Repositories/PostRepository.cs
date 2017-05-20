using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

		public async Task<IReadOnlyCollection<string>> List()
		{
			BlobResultSegment segment = await _postContainer.ListBlobsSegmentedAsync(null);

			return segment.Results.Select(blob => ((CloudBlockBlob)blob).Name).ToList();
		}

		public async Task<string> Get(string name)
		{
			CloudBlockBlob blob = _postContainer.GetBlockBlobReference(name);

			return await blob.DownloadTextAsync();
		}

		public async Task<bool> Delete(string name)
		{
			CloudBlockBlob blob = _postContainer.GetBlockBlobReference(name);

			return await blob.DeleteIfExistsAsync();
		}
    }
}
