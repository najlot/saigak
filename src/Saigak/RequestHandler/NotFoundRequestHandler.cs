using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public sealed class NotFoundRequestHandler : AbstractRequestHandler
	{
		public NotFoundRequestHandler(string contentRootPath) : base(contentRootPath)
		{
		}

		public override async Task<bool> TryHandle(Globals globals)
		{
			if (!globals.Context.Response.HasStarted)
			{
				globals.Context.Response.StatusCode = 404;
			}

			await globals.Context.Response.WriteAsync($"File '{globals.Context.Request.Path}' not found!");
			return true;
		}
	}
}
