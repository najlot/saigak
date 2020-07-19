using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;

namespace Saigak.RequestHandler
{
	public class CsRequestHandler : AbstractRequestHandler
	{
		public CsRequestHandler(string contentRootPath) : base(contentRootPath)
		{
			Directory.CreateDirectory(Path.Combine(contentRootPath, "wwwroot"));
		}

		private static readonly InteractiveAssemblyLoader loader;
		private static readonly ScriptOptions options;
		private static readonly ConcurrentDictionary<string, ScriptRunner<object>> cache = new ConcurrentDictionary<string, ScriptRunner<object>>();

		static CsRequestHandler()
		{
			var types = new[]
			{
				typeof(object),
				typeof(System.IO.FileInfo),
				typeof(System.Linq.IQueryable),
				typeof(System.Threading.Tasks.Task),
				typeof(System.Dynamic.DynamicObject),
				typeof(System.Text.RegularExpressions.Regex),
				typeof(ConcurrentDictionary<,>),

				typeof(Microsoft.AspNetCore.Http.HttpResponseWritingExtensions)
			};

			var references = types.Select(type => type.Assembly).ToArray();

			loader = new InteractiveAssemblyLoader();
			foreach (var reference in references)
			{
				loader.RegisterDependency(reference);
			}

			options = ScriptOptions.Default
				.WithReferences(references)
				.AddImports(types.Select(type => type.Namespace).ToArray());

			var dummy = CSharpScript
				.Create("if (Context != null) await WriteAsync(\"Something is wrong.\");", options, typeof(Globals), loader)
				.CreateDelegate();

			dummy(new Globals(null));
		}

		public override async Task<bool> TryHandle(HttpContext context)
		{
			string path = null;
			string fullPath = null;

			if (!context.Request.Path.HasValue)
			{
				path = Path.Combine(ContentRootPath, "wwwroot", "index.cs");
			}
			else
			{
				var requestPath = context.Request.Path.Value.TrimStart('/');

				if (string.IsNullOrWhiteSpace(requestPath))
				{
					path = Path.Combine(ContentRootPath, "wwwroot", "index.cs");
				}
				else
				{
					if (Path.GetExtension(requestPath)?.ToLower() != ".cs")
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

				await CsProcessor.Instance.Run(content, new Globals(context));

				return true;
			}

			return false;
		}
	}
}
