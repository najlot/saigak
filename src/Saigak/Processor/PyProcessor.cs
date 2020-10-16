using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace Saigak.Processor
{
	public sealed class PyProcessor
	{
		public static PyProcessor Instance { get; } = new PyProcessor();

		private readonly ScriptEngine _engine;

		private readonly ConcurrentDictionary<(string Path, DateTime Time), ScriptSource> _cache = new ConcurrentDictionary<(string Path, DateTime Time), ScriptSource>();

		public PyProcessor()
		{
			_engine = Python.CreateEngine();

			string pyLibPath = Path.GetFullPath("py_lib");

			if (Directory.Exists(pyLibPath))
			{
				var sp = _engine.GetSearchPaths();
				sp.Add(pyLibPath);
				_engine.SetSearchPaths(sp);
			}
		}

		public void Run((string Path, DateTime Time) key, string content, Globals globals)
		{
			var scope = _engine.CreateScope();
			
			scope.SetVariable("context", globals.Context);

			scope.SetVariable("write", (Action<string>)(str => globals.Write(str)));
			scope.SetVariable("writeline", (Action<string>)(str => globals.WriteLine(str)));
			scope.SetVariable("getrequeststring", (Func<string>)(() => globals.GetRequestString()));
			scope.SetVariable("getrequestbytes", (Func<byte[]>)(() => globals.GetRequestBytes()));

			if (!_cache.TryGetValue(key, out var source))
			{
				source = _engine.CreateScriptSourceFromString(content);

				var duplicates = _cache.Keys.Where(k => k.Path == key.Path).ToArray();
				foreach (var duplicate in duplicates)
				{
					_cache.TryRemove(duplicate, out _);
				}

				_cache[key] = source;
			}

			source.Execute(scope);
		}
	}
}
