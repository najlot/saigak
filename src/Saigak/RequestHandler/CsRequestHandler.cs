using Saigak.Processor;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public sealed class CsRequestHandler : AbstractScriptRequestHandler
	{
		public CsRequestHandler(string contentRootPath) : base(contentRootPath, ".cs")
		{
		}

		public override async Task ProcessAsync(string fullPath, Globals globals)
		{
			var (path, time, content) = await FileContentCache.Instance.ReadAllTextAsync(fullPath);
			await CsProcessor.Instance.Run((path, time), content, globals);
		}
	}
}
