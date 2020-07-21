using Microsoft.AspNetCore.Http;
using Peachpie.AspNetCore.Web;
using Saigak.Processor;
using System.IO;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public sealed class PhpRequestHandler : AbstractScriptRequestHandler
	{
		public PhpRequestHandler(string contentRootPath) : base(contentRootPath, ".php")
		{
		}

		public override async Task ProcessAsync(string fullPath, HttpContext context)
		{
			var ctx = context.GetOrCreateContext();
			var content = await File.ReadAllTextAsync(fullPath);
			PhpProcessor.Instance.Run(content, ctx, fullPath, false);
		}
	}
}
