using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public abstract class AbstractRequestHandler : IRequestHandler
	{
		protected readonly string ContentRootPath;
		private IRequestHandler _handler = null;

		protected AbstractRequestHandler(string contentRootPath)
		{
			ContentRootPath = contentRootPath;
		}

		public IRequestHandler SetNext(IRequestHandler handler)
		{
			if (_handler == null)
			{
				_handler = handler;
			}
			else
			{
				_handler.SetNext(handler);
			}

			return this;
		}

		public abstract Task<bool> TryHandle(Globals globals);

		public async Task Handle(Globals globals)
		{
			var result = await TryHandle(globals);
			if (!result && _handler != null)
			{
				await _handler.Handle(globals);
			}
		}
	}
}
