using Saigak.Processor;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public sealed class LuaRequestHandler : AbstractScriptRequestHandler
	{
		public LuaRequestHandler(string contentRootPath) : base(contentRootPath, ".lua")
		{
		}

		public override async Task ProcessAsync(string fullPath, Globals globals)
		{
			var (path, time, content) = await FileContentCache.Instance.ReadAllTextAsync(fullPath);

			LuaProcessor.Instance.Run(content, globals);
		}
	}
}
