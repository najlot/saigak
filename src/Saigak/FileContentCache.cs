using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Saigak
{
	public class FileContentCache
	{
		public static FileContentCache Instance { get; } = new FileContentCache();

		private readonly ConcurrentDictionary<(string, DateTime), byte[]> _byteCache = new ConcurrentDictionary<(string, DateTime), byte[]>();
		private readonly ConcurrentDictionary<(string, DateTime), string> _stringCache = new ConcurrentDictionary<(string, DateTime), string>();

		public async Task<(string Key, string Content)> ReadAllTextAsync(string path)
		{
			var time = File.GetLastWriteTime(path);
			var key = (path, time);

			if (!_stringCache.TryGetValue(key, out var val))
			{
				val = await File.ReadAllTextAsync(path);

				var duplicates = _stringCache.Keys.Where(k => k.Item1 == path).ToArray();
				foreach (var duplicate in duplicates)
				{
					_stringCache.TryRemove(duplicate, out _);
				}

				_stringCache[key] = val;
			}

			return (path + time, val);
		}

		public async Task<byte[]> ReadAllBytesAsync(string path)
		{
			var time = File.GetLastWriteTime(path);
			var key = (path, time);

			if (!_byteCache.TryGetValue(key, out var val))
			{
				val = await File.ReadAllBytesAsync(path);

				var duplicates = _byteCache.Keys.Where(k => k.Item1 == path).ToArray();
				foreach (var duplicate in duplicates)
				{
					_byteCache.TryRemove(duplicate, out _);
				}

				_byteCache[key] = val;
			}

			return val;
		}
	}
}
