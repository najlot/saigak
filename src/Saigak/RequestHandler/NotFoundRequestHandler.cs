using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public sealed class NotFoundRequestHandler : AbstractRequestHandler
	{
		public NotFoundRequestHandler(string contentRootPath) : base(contentRootPath)
		{
		}

		public override async Task<bool> TryHandle(HttpContext context)
		{
			context.Response.StatusCode = 404;
			await context.Response.WriteAsync($"File '{context.Request.Path}' not found!");
			return true;
		}
	}
}
