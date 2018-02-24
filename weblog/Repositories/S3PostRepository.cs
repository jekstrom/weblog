using Amazon.S3;
using Amazon.S3.Model;
using Polly;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using weblog.Models;

namespace weblog.Repositories
{
	public class S3PostRepository : IPostRepository
    {
		private readonly IAmazonS3 _client;
		private readonly ILogger _logger;
		private readonly IAsyncPolicy _policy;
		private readonly string _bucketName;
		private readonly string _keyPrefix;

		public S3PostRepository(IAmazonS3 client, string bucketName, string keyPrefix, ILogger logger, IAsyncPolicy policy)
		{
			_client = client;
			_logger = logger;
			_policy = policy;
			_bucketName = bucketName;
			_keyPrefix = keyPrefix;
		}

		public async Task<bool> Insert(string inputName, string inputContent)
		{
			return await _policy.ExecuteAsync(async (context) =>
			{
				string name = (string)context["name"];
				string content = (string)context["content"];

				PutObjectResponse response = await _client.PutObjectAsync(
					new PutObjectRequest
					{
						BucketName = _bucketName,
						Key = $"{_keyPrefix}/{name}",
						CannedACL = S3CannedACL.PublicRead,
						ContentBody = content
					}
				).ConfigureAwait(false);

				return (int)response.HttpStatusCode < 400;
			}, new Dictionary<string, object> { { "name", inputName }, { "content", inputContent } });
		}

		public async Task<IReadOnlyCollection<PostModel>> List()
		{
			return await _policy.ExecuteAsync(async () =>
			{
				IList<string> keys = await _client.GetAllObjectKeysAsync(_bucketName, _keyPrefix, new Dictionary<string, object>()).ConfigureAwait(false);

				List<PostModel> posts = new List<PostModel>();
				foreach (string key in keys.Where(k => !k.Equals($"{_keyPrefix}/")))
				{
					try
					{
						GetObjectMetadataResponse metaData = await _client.GetObjectMetadataAsync(new GetObjectMetadataRequest { BucketName = _bucketName, Key = key }).ConfigureAwait(false);
						posts.Add(new PostModel
						{
							Name = key.Replace($"{_keyPrefix}/", String.Empty),
							LastModified = metaData.LastModified
						});
					}
					catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
					{
						_logger.Error("MetaData forbidden.", ex);
					}
				}

				return posts;
			});
		}

		public async Task<PostModel> Get(string inputName)
		{
			return await _policy.ExecuteAsync(async (context) =>
			{
				string name = (string)context["name"];

				var response = await _client.GetObjectAsync(new GetObjectRequest { BucketName = _bucketName, Key = $"{_keyPrefix}/{name}" }).ConfigureAwait(false);

				using (response.ResponseStream)
				{
					byte[] contentBuffer = new byte[response.ContentLength];
					await response.ResponseStream.ReadAsync(contentBuffer, 0, (int)response.ContentLength);
					string content = Encoding.UTF8.GetString(contentBuffer);

					return new PostModel
					{
						Name = name,
						Content = content,
						LastModified = response.LastModified
					};
				}
			}, new Dictionary<string, object> { { "name", inputName } });
		}

		public async Task<bool> Delete(string inputName)
		{
			return await _policy.ExecuteAsync(async (context) =>
			{
				string name = (string)context["name"];

				await _client.DeleteAsync(_bucketName, $"{_keyPrefix}/{name}", new Dictionary<string, object>()).ConfigureAwait(false);

				return true;
			}, new Dictionary<string, object> { { "name", inputName } });
		}
    }
}
