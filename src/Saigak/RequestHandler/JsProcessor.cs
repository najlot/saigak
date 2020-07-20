using Jurassic;
using System;
using System.Collections.Concurrent;

namespace Saigak.RequestHandler
{
	public sealed class JsProcessor
	{
		public static JsProcessor Instance { get; } = new JsProcessor();

		private readonly ConcurrentDictionary<string, CompiledScript> cache = new ConcurrentDictionary<string, CompiledScript>();

		public void Run(string content, Globals globals, string path = null)
		{
			var engine = new ScriptEngine()
			{
				EnableExposedClrTypes = true
			};

			engine.SetGlobalValue("context", globals.Context);

			engine.SetGlobalFunction("write", (Action<string>)(str =>
			{
				globals.Write(str);
			}));

			engine.SetGlobalFunction("writeline", (Action<string>)(str =>
			{
				globals.Write(str);
			}));

			if (!cache.TryGetValue(content, out var script))
			{
				script = engine.Compile(new StringScriptSource(content, path));
				cache[content] = script;
			}

			script.Execute(engine);
		}
	}
}
