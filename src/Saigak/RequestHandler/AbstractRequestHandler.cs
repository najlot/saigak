using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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

		public abstract Task<bool> TryHandle(HttpContext context);

		public async Task Handle(HttpContext context)
		{
			var result = await TryHandle(context);
			if (!result && _handler != null)
			{
				await _handler.Handle(context);
			}
		}
	}
}
