using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public class Globals
	{
		public HttpContext Context { get; }

		public Globals(HttpContext context)
		{
			Context = context;
		}

		private readonly Encoding _encoding = Encoding.UTF8;

		public async Task WriteAsync(string s)
		{
			await Context.Response.WriteAsync(s, _encoding);
		}

		public async Task WriteLineAsync(string s)
		{
			await Context.Response.WriteAsync(s, _encoding);
			await Context.Response.WriteAsync("\n", _encoding);
		}
	}
}
