using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Saigak.RequestHandler
{
	public class SaiRequestHandler : AbstractRequestHandler
	{
		public SaiRequestHandler(string contentRootPath) : base(contentRootPath)
		{
			Directory.CreateDirectory(Path.Combine(contentRootPath, "wwwroot"));
		}

		private static IEnumerable<(string Language, string Content)> GetSections(string content)
		{
			while (true)
			{
				var index = content.IndexOf("<?");

				if (index == -1)
				{
					yield return
					(
						"",
						content
					);

					break;
				}
				else
				{
					yield return
					(
						"",
						content.Substring(0, index)
					);

					var language = "";
					var endIndex = content.IndexOf("?>", index);

					if (endIndex == -1)
					{
						break;
					}

					for (index += 2; index < endIndex; index++)
					{
						char c = content[index];

						if (c == '\r')
						{
							continue;
						}

						if (c == ' ' || c == '\n')
						{
							index++;
							break;
						}

						language += c;
					}

					yield return
					(
						language,
						content[index..endIndex]
					);

					content = content.Substring(endIndex + 2);
				}
			}
		}

		public override async Task<bool> TryHandle(HttpContext context)
		{
			string path = null;
			string fullPath = null;

			if (!context.Request.Path.HasValue)
			{
				path = Path.Combine(ContentRootPath, "wwwroot", "index.sai");
			}
			else
			{
				var requestPath = context.Request.Path.Value.TrimStart('/');

				if (string.IsNullOrWhiteSpace(requestPath))
				{
					path = Path.Combine(ContentRootPath, "wwwroot", "index.sai");
				}
				else
				{
					if (Path.GetExtension(requestPath)?.ToLower() != ".sai")
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
				context.Response.StatusCode = 200;
				context.Response.ContentType = "text/html; charset=utf-8";
				
				var content = await File.ReadAllTextAsync(fullPath);
				var globals = new Globals(context);
				var sections = GetSections(content);

				foreach (var (Language, Content) in sections)
				{
					if (Language.Equals("cs", StringComparison.OrdinalIgnoreCase))
					{
						await CsProcessor.Instance.Run(Content, globals);
					}
					else
					{
						await globals.WriteAsync(Content);
					}
				}

				return true;
			}

			return false;
		}
	}
}
