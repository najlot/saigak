using Microsoft.AspNetCore.Http;
using System.Buffers;
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

		public void Write(string s)
		{
			var bytes = _encoding.GetBytes(s);
			Context.Response.BodyWriter.Write(bytes);
		}

		private readonly byte[] _newLineBytes = new byte[] { (byte)'\n' };

		public void WriteLine(string s)
		{
			var bytes = _encoding.GetBytes(s);
			Context.Response.BodyWriter.Write(bytes);
			Context.Response.BodyWriter.Write(_newLineBytes);
		}
	}
}
