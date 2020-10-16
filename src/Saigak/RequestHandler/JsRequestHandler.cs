using Saigak.Processor;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public sealed class JsRequestHandler : AbstractScriptRequestHandler
	{
		public JsRequestHandler(string contentRootPath) : base(contentRootPath, ".js")
		{
		}

		public override async Task ProcessAsync(string fullPath, Globals globals)
		{
			var (path, time, content) = await FileContentCache.Instance.ReadAllTextAsync(fullPath);
			JsProcessor.Instance.Run((path, time), content, globals, fullPath);
		}
	}
}
