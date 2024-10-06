using System;
using System.IO;
using System.Threading.Tasks;

namespace Saigak.Blobs.Repositories
{
	public class FileRepository : IRepository
	{
		private readonly string _basePath;

		public FileRepository(string basePath)
		{
			_basePath = basePath;
		}

		private string BuildPath(string subPath)
		{
			return Path.Combine(
				_basePath,
				"data",
				subPath
					.Replace('\\', Path.DirectorySeparatorChar)
					.Replace('/', Path.DirectorySeparatorChar)
					.Trim(Path.DirectorySeparatorChar)
					.ToLower());
		}

		public Task<Stream> GetAsync(string subPath)
		{
			var path = BuildPath(subPath);
			Stream stream = null;

			if (File.Exists(path))
			{
				stream = File.OpenRead(path);
			}

			return Task.FromResult(stream);
		}

		public async Task PostAsync(string subPath, Stream stream)
		{
			var path = BuildPath(subPath);
			var directory = Path.GetDirectoryName(path);
			Directory.CreateDirectory(directory);

			if (File.Exists(path))
			{
				throw new Exception("File exists");
			}

			using var file = File.OpenWrite(path);
			await stream.CopyToAsync(file);
		}

		public async Task PutAsync(string subPath, Stream stream)
		{
			var path = BuildPath(subPath);
			File.Delete(path);
			using var file = File.OpenWrite(path);
			await stream.CopyToAsync(file);
		}

		public Task DeleteAsync(string subPath)
		{
			var path = BuildPath(subPath);
			File.Delete(path);
			return Task.CompletedTask;
		}
	}
}
