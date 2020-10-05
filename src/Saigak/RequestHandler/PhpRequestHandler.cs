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

		public override async Task ProcessAsync(string fullPath, Globals globals)
		{
			var content = await File.ReadAllTextAsync(fullPath);
			PhpProcessor.Instance.Run(content, globals.PhpContext, fullPath, false);
		}
	}
}
