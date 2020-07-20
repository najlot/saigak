using Microsoft.AspNetCore.Http;
using Peachpie.AspNetCore.Web;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public sealed class PhpRequestHandler : AbstractRequestHandler
	{
		public PhpRequestHandler(string contentRootPath) : base(contentRootPath)
		{
			Directory.CreateDirectory(Path.Combine(contentRootPath, "wwwroot"));
		}

		public override async Task<bool> TryHandle(HttpContext context)
		{
			string path = null;
			string fullPath = null;

			if (!context.Request.Path.HasValue)
			{
				path = Path.Combine(ContentRootPath, "wwwroot", "index.php");
			}
			else
			{
				var requestPath = context.Request.Path.Value.TrimStart('/');

				if (string.IsNullOrWhiteSpace(requestPath))
				{
					path = Path.Combine(ContentRootPath, "wwwroot", "index.php");
				}
				else
				{
					if (Path.GetExtension(requestPath)?.ToLower() != ".php")
					{
						return false;
					}

					path = Path.Combine(ContentRootPath, "wwwroot", requestPath
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
				var paths = Directory.GetFiles(Path.Combine(ContentRootPath, "wwwroot"), "*", SearchOption.AllDirectories);
				fullPath = paths.FirstOrDefault(p => p.EndsWith(path, StringComparison.OrdinalIgnoreCase));
			}

			if (fullPath != null && File.Exists(fullPath))
			{
				var ctx = context.GetOrCreateContext();
				var content = await File.ReadAllTextAsync(fullPath);
				PhpProcessor.Instance.Run(content, ctx, fullPath, false);

				return true;
			}

			return false;
		}
	}
}
