using IronPython.Hosting;
using System;
using System.Collections.Concurrent;

namespace Saigak.RequestHandler
{
	public sealed class PyProcessor
	{
		public static PyProcessor Instance { get; } = new PyProcessor();

		private readonly Microsoft.Scripting.Hosting.ScriptEngine _engine = Python.CreateEngine();
		private readonly ConcurrentDictionary<string, Microsoft.Scripting.Hosting.ScriptSource> cache = new ConcurrentDictionary<string, Microsoft.Scripting.Hosting.ScriptSource>();

		public void Run(string content, Globals globals)
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

			if (!cache.TryGetValue(content, out var source))
			{
				source = _engine.CreateScriptSourceFromString(content);
				cache[content] = source;
			}

			source.Execute(scope);
		}
	}
}
