using Microsoft.AspNetCore.Http;
using Saigak.Processor;
using System.IO;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public sealed class PyRequestHandler : AbstractScriptRequestHandler
	{
		public PyRequestHandler(string contentRootPath) : base(contentRootPath, ".py")
		{
		}

		public override async Task ProcessAsync(string fullPath, HttpContext context)
		{
			var content = await File.ReadAllTextAsync(fullPath);

			context.Response.StatusCode = 200;
			context.Response.ContentType = "text/html; charset=utf-8";

			PyProcessor.Instance.Run(content, new Globals(context));
		}
	}
}
