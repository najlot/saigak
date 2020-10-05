using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Concurrent;

namespace Saigak.Processor
{
	public sealed class PyProcessor
	{
		public static PyProcessor Instance { get; } = new PyProcessor();

		private readonly ScriptEngine _engine = Python.CreateEngine();
		private readonly ConcurrentDictionary<string, ScriptSource> _cache = new ConcurrentDictionary<string, ScriptSource>();

		public void Run(string key, string content, Globals globals)
		{
			var scope = _engine.CreateScope();

			scope.SetVariable("context", globals.Context);

			scope.SetVariable("write", (Action<string>)(str =>
			{
				globals.Write(str);
			}));

			scope.SetVariable("writeline", (Action<string>)(str =>
			{
				globals.WriteLine(str);
			}));

			if (!_cache.TryGetValue(key, out var source))
			{
				source = _engine.CreateScriptSourceFromString(content);
				_cache[key] = source;
			}

			source.Execute(scope);
		}
	}
}
