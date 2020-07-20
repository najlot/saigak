using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public interface IRequestHandler
	{
		IRequestHandler SetNext(IRequestHandler handler);

		Task<bool> TryHandle(HttpContext context);

		Task Handle(HttpContext context);
	}
}
