using Saigak.Processor;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public sealed class PyRequestHandler : AbstractScriptRequestHandler
	{
		public PyRequestHandler(string contentRootPath) : base(contentRootPath, ".py")
		{
		}

		public override async Task ProcessAsync(string fullPath, Globals globals)
		{
			var (path, time, content) = await FileContentCache.Instance.ReadAllTextAsync(fullPath);
			PyProcessor.Instance.Run((path, time), content, globals);
		}
	}
}
