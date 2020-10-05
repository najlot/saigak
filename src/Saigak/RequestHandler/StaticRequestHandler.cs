using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public sealed class StaticRequestHandler : AbstractRequestHandler
	{
		public StaticRequestHandler(string contentRootPath) : base(contentRootPath)
		{
			Directory.CreateDirectory(Path.Combine(contentRootPath, "wwwroot", "static"));
		}

		public override async Task<bool> TryHandle(Globals globals)
		{
			string path = null;
			string fullPath = null;

			if (!globals.Context.Request.Path.HasValue)
			{
				path = Path.Combine(ContentRootPath, "wwwroot", "static", "index.html");
			}
			else
			{
				var requestPath = globals.Context.Request.Path.Value.TrimStart('/');

				if (string.IsNullOrWhiteSpace(requestPath))
				{
					path = Path.Combine(ContentRootPath, "wwwroot", "static", "index.html");
				}
				else
				{
					path = Path.Combine(ContentRootPath, "wwwroot", "static", requestPath
						.Replace('/', Path.DirectorySeparatorChar)
						.Replace('\\', Path.DirectorySeparatorChar));
				}
			}

			if (File.Exists(path))
			{
				fullPath = path;
			}
			else
			{
				var paths = Directory.GetFiles(Path.Combine(ContentRootPath, "wwwroot", "static"), "*", SearchOption.AllDirectories);
				fullPath = paths.FirstOrDefault(p => p.EndsWith(path, StringComparison.OrdinalIgnoreCase));
			}

			if (fullPath != null && File.Exists(fullPath))
			{
				var (key, content) = await FileContentCache.Instance.ReadAllBytesAsync(fullPath);

				if (!globals.Context.Response.HasStarted)
				{
					globals.Context.Response.StatusCode = 200;

					// https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types
					switch (Path.GetExtension(fullPath).ToLower())
					{
						case ".css":
							globals.Context.Response.ContentType = "text/css";
							break;

						case ".html":
							globals.Context.Response.ContentType = "text/html";
							break;

						case ".js":
							globals.Context.Response.ContentType = "text/javascript";
							break;

						case ".jpg":
							globals.Context.Response.ContentType = "image/jpeg";
							break;

						case ".png":
							globals.Context.Response.ContentType = "image/png";
							break;

						case ".ico":
							globals.Context.Response.ContentType = "image/x-icon";
							break;
					}
				}

				await globals.Context.Response.Body.WriteAsync(content);

				return true;
			}

			return false;
		}
	}
}
