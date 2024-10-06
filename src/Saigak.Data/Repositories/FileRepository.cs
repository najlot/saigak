using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saigak.Data.Repositories
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

		public async Task<dynamic> GetAsync(string subPath)
		{
			var path = BuildPath(subPath);

			if (Directory.Exists(path))
			{
				var items = new List<dynamic>();

				foreach (var p in Directory.GetFiles(path))
				{
					using var file = File.OpenRead(p);
					dynamic obj = await JsonSerializer.DeserializeAsync(file, typeof(ExpandoObject));
					items.Add(obj);
				}

				return items;
			}
			else if (File.Exists(path))
			{
				using var file = File.OpenRead(path);
				dynamic obj = await JsonSerializer.DeserializeAsync(file, typeof(ExpandoObject));
				return obj;
			}

			return null;
		}

		public async Task CreateAsync(string subPath, dynamic obj)
		{
			var path = BuildPath(subPath);
			var directory = Path.GetDirectoryName(path);
			Directory.CreateDirectory(directory);

			if (File.Exists(path))
			{
				throw new Exception("File exists");
			}

			using var file = File.OpenWrite(path);
			await JsonSerializer.SerializeAsync(file, obj);
		}

		public async Task UpdateAsync(string subPath, dynamic obj)
		{
			var path = BuildPath(subPath);
			File.Delete(path);
			using var file = File.OpenWrite(path);
			await JsonSerializer.SerializeAsync(file, obj);
		}

		public Task DeleteAsync(string subPath)
		{
			var path = BuildPath(subPath);
			File.Delete(path);
			return Task.CompletedTask;
		}
	}
}
