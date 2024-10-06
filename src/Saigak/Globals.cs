using Microsoft.AspNetCore.Http;
using Peachpie.AspNetCore.Web;
using Saigak.RequestHandler;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Saigak
{
	public sealed class Globals : IDisposable
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

		private static readonly Encoding _encoding = Encoding.UTF8;
		private static readonly byte[] _endOfLine = Encoding.UTF8.GetBytes("\n");

		private readonly MemoryStream _stream = new();
		private long _streamOffset = 0;

		public async Task WriteAsync(string s)
		{
			var bytes = _encoding.GetBytes(s);
			await _stream.WriteAsync(bytes);
		}

		public async Task WriteLineAsync(string s)
		{
			var bytes = _encoding.GetBytes(s);
			await _stream.WriteAsync(bytes);
			await _stream.WriteAsync(_endOfLine);
		}

		public void Write(string s)
		{
			var bytes = _encoding.GetBytes(s);
			_stream.Write(bytes);
		}

		public void WriteLine(string s)
		{
			var bytes = _encoding.GetBytes(s);
			_stream.Write(bytes);
			_stream.Write(_endOfLine);
		}

		public string GetRequestString()
		{
			using (var streamReader = new StreamReader(Context.Request.Body, Encoding.UTF8))
			{
				return streamReader.ReadToEnd();
			}
		}

		public byte[] GetRequestBytes()
		{
			using (var ms = new MemoryStream())
			{
				Context.Request.Body.CopyTo(ms);
				return ms.ToArray();
			}
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

		public async Task FlushAsync()
		{
			if (_stream.Position == 0)
			{
				return;
			}

			if (!Context.Response.HasStarted)
			{
				Context.Response.StatusCode = 200;
				Context.Response.ContentType = "text/html; charset=utf-8";
			}

			_stream.Seek(_streamOffset, SeekOrigin.Begin);
			await _stream.CopyToAsync(Context.Response.Body);
			_streamOffset = _stream.Position;

			await Context.Response.Body.FlushAsync();
		}

		public void Dispose()
		{
			_stream.Dispose();
		}
	}
}
