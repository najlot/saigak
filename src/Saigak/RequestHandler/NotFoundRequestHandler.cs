using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Saigak.RequestHandler
{
	public class NotFoundRequestHandler : AbstractRequestHandler
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
