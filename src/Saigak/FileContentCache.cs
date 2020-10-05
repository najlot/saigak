using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace Saigak
{
	public class FileContentCache
	{
		public static FileContentCache Instance { get; } = new FileContentCache();

		private readonly ConcurrentDictionary<string, byte[]> _byteCache = new ConcurrentDictionary<string, byte[]>();
		private readonly ConcurrentDictionary<string, string> _stringCache = new ConcurrentDictionary<string, string>();

		public async Task<(string Key, string Content)> ReadAllTextAsync(string path)
		{
			var key = path + File.GetLastWriteTime(path);

			if (!_stringCache.TryGetValue(key, out var val))
			{
				val = await File.ReadAllTextAsync(path);
				_stringCache[key] = val;
			}

			return (key, val);
		}

		public async Task<(string Key, byte[] Content)> ReadAllBytesAsync(string path)
		{
			var key = path + File.GetLastWriteTime(path);

			if (!_byteCache.TryGetValue(key, out var val))
			{
				val = await File.ReadAllBytesAsync(path);
				_byteCache[key] = val;
			}

			return (key, val);
		}
	}
}
