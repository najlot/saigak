using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public sealed class LuaRequestHandler : AbstractRequestHandler
	{
		public LuaRequestHandler(string contentRootPath) : base(contentRootPath)
		{
			Directory.CreateDirectory(Path.Combine(contentRootPath, "wwwroot"));
		}

		public override async Task<bool> TryHandle(HttpContext context)
		{
			string path = null;
			string fullPath = null;

			if (!context.Request.Path.HasValue)
			{
				path = Path.Combine(ContentRootPath, "wwwroot", "index.lua");
			}
			else
			{
				var requestPath = context.Request.Path.Value.TrimStart('/');

				if (string.IsNullOrWhiteSpace(requestPath))
				{
					path = Path.Combine(ContentRootPath, "wwwroot", "index.lua");
				}
				else
				{
					if (Path.GetExtension(requestPath)?.ToLower() != ".lua")
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
				var content = await File.ReadAllTextAsync(fullPath);
				LuaProcessor.Instance.Run(content, new Globals(context));

				return true;
			}

			return false;
		}
	}
}
