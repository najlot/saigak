using Microsoft.AspNetCore.Http;
using Peachpie.AspNetCore.Web;
using Saigak.RequestHandler;
using System.Buffers;
using System.Text;
using System.Threading.Tasks;

namespace Saigak
{
	public class Globals
	{
		private static readonly byte[] _newLineBytes = new byte[] { (byte)'\n' };
		private readonly IRequestHandler handler;

		public HttpContext Context { get; }
		public Pchp.Core.Context PhpContext { get; }

		public Globals(HttpContext context, IRequestHandler handler)
		{
			Context = context;
			this.handler = handler;
			PhpContext = context.GetOrCreateContext();
		}

		private readonly Encoding _encoding = Encoding.UTF8;

		public async Task WriteAsync(string s)
		{
			if (!Context.Response.HasStarted)
			{
				Context.Response.StatusCode = 200;
				Context.Response.ContentType = "text/html; charset=utf-8";
			}

			await Context.Response.WriteAsync(s, _encoding);
		}

		public async Task WriteLineAsync(string s)
		{
			if (!Context.Response.HasStarted)
			{
				Context.Response.StatusCode = 200;
				Context.Response.ContentType = "text/html; charset=utf-8";
			}

			await Context.Response.WriteAsync(s, _encoding);
			await Context.Response.WriteAsync("\n", _encoding);
		}

		public void Write(string s)
		{
			if (!Context.Response.HasStarted)
			{
				Context.Response.StatusCode = 200;
				Context.Response.ContentType = "text/html; charset=utf-8";
			}

			var bytes = _encoding.GetBytes(s);
			Context.Response.BodyWriter.Write(bytes);
		}

		public void WriteLine(string s)
		{
			if (!Context.Response.HasStarted)
			{
				Context.Response.StatusCode = 200;
				Context.Response.ContentType = "text/html; charset=utf-8";
			}

			var bytes = _encoding.GetBytes(s);
			Context.Response.BodyWriter.Write(bytes);
			Context.Response.BodyWriter.Write(_newLineBytes);
		}

		public async Task RunFileAsync(string path)
		{
			if (!path.StartsWith('/'))
			{
				path = '/' + path;
			}

			var originalPath = Context.Request.Path;
			Context.Request.Path = new PathString(path);
			await handler.Handle(this);
			Context.Request.Path = originalPath;
		}
	}
}
