using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public abstract class AbstractScriptRequestHandler : AbstractRequestHandler
	{
		private readonly string _extension;
		private readonly string _rootPath;
		private readonly string _indexPath;

		protected AbstractScriptRequestHandler(string contentRootPath, string extension) : base(contentRootPath)
		{
			_extension = extension.ToLower();
			_rootPath = Path.Combine(ContentRootPath, "wwwroot");
			_indexPath = Path.Combine(ContentRootPath, "wwwroot", "index" + _extension);
		}

		public override async Task<bool> TryHandle(HttpContext context)
		{
			string path = null;
			string fullPath = null;
			bool fileExists = false;

			if (!context.Request.Path.HasValue)
			{
				path = _indexPath;
			}
			else
			{
				var requestPath = context.Request.Path.Value.TrimStart('/');

				if (string.IsNullOrWhiteSpace(requestPath))
				{
					path = _indexPath;
				}
				else
				{
					if (Path.GetExtension(requestPath)?.ToLower() != _extension)
					{
						return false;
					}

					path = Path.Combine(_rootPath, requestPath
						.Replace('/', Path.DirectorySeparatorChar)
						.Replace('\\', Path.DirectorySeparatorChar));
				}
			}

			fileExists = File.Exists(path);

			if (fileExists)
			{
				fullPath = path;
			}
			else
			{
				var paths = Directory.GetFiles(_rootPath, "*", SearchOption.AllDirectories);
				fullPath = paths.FirstOrDefault(p => p.EndsWith(path, StringComparison.OrdinalIgnoreCase));
				fileExists = File.Exists(fullPath);
			}

			if (fileExists)
			{
				await ProcessAsync(fullPath, context);
				return true;
			}

			return false;
		}

		public abstract Task ProcessAsync(string fullPath, HttpContext context);
	}
}
