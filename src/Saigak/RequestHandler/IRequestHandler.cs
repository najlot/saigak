using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public interface IRequestHandler
	{
		IRequestHandler SetNext(IRequestHandler handler);

		Task<bool> TryHandle(Globals globals);

		Task Handle(Globals globals);
	}
}
