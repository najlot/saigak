using Microsoft.AspNetCore.Http;
using Saigak.Processor;
using System.IO;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public sealed class JsRequestHandler : AbstractScriptRequestHandler
	{
		public JsRequestHandler(string contentRootPath) : base(contentRootPath, ".js")
		{
		}

		public override async Task ProcessAsync(string fullPath, HttpContext context)
		{
			var content = await File.ReadAllTextAsync(fullPath);

			context.Response.StatusCode = 200;
			context.Response.ContentType = "text/html; charset=utf-8";

			JsProcessor.Instance.Run(content, new Globals(context), fullPath);
		}
	}
}
