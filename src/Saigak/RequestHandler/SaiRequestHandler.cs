﻿using Microsoft.AspNetCore.Http;
using Peachpie.AspNetCore.Web;
using Saigak.Processor;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public sealed class SaiRequestHandler : AbstractScriptRequestHandler
	{
		public SaiRequestHandler(string contentRootPath) : base(contentRootPath, ".sai")
		{
			Directory.CreateDirectory(Path.Combine(contentRootPath, "wwwroot"));
		}

		public override async Task ProcessAsync(string fullPath, HttpContext context)
		{
			context.Response.ContentType = "text/html; charset=utf-8";

			var content = await File.ReadAllTextAsync(fullPath);
			var globals = new Globals(context);
			var sections = GetSections(content);

			var ctx = context.GetOrCreateContext();

			foreach (var (Language, Content) in sections)
			{
				if (Content.Length < 1)
				{
					continue;
				}

				switch (Language.ToLower())
				{
					case "cs":
						await CsProcessor.Instance.Run(Content, globals);
						break;

					case "py":
						PyProcessor.Instance.Run(Content, globals);
						break;

					case "php":
						PhpProcessor.Instance.Run(Content, ctx, fullPath);
						break;

					case "js":
						JsProcessor.Instance.Run(Content, globals);
						break;

					case "lua":
						LuaProcessor.Instance.Run(Content, globals);
						break;

					default:
						await globals.WriteAsync(Content);
						break;
				}
			}
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
	}
}
