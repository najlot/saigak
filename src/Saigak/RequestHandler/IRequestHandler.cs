using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Saigak.RequestHandler
{
	public interface IRequestHandler
	{
		IRequestHandler SetNext(IRequestHandler handler);
		Task<bool> TryHandle(HttpContext context);
		Task Handle(HttpContext context);
	}
}
