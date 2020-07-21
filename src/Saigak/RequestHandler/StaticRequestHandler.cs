using Microsoft.AspNetCore.Http;
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

		public override async Task<bool> TryHandle(HttpContext context)
		{
			string path = null;
			string fullPath = null;

			if (!context.Request.Path.HasValue)
			{
				path = Path.Combine(ContentRootPath, "wwwroot", "static", "index.html");
			}
			else
			{
				var requestPath = context.Request.Path.Value.TrimStart('/');

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
				using var file = File.OpenRead(fullPath);
				context.Response.StatusCode = 200;

				// https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types
				switch (Path.GetExtension(fullPath).ToLower())
				{
					case ".css":
						context.Response.ContentType = "text/css";
						break;

					case ".html":
						context.Response.ContentType = "text/html";
						break;

					case ".js":
						context.Response.ContentType = "text/javascript";
						break;

					case ".jpg":
						context.Response.ContentType = "image/jpeg";
						break;

					case ".png":
						context.Response.ContentType = "image/png";
						break;

					case ".ico":
						context.Response.ContentType = "image/x-icon";
						break;
				}

				await file.CopyToAsync(context.Response.Body);

				return true;
			}

			return false;
		}
	}
}
