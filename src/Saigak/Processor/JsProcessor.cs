using Jurassic;
using System;
using System.Collections.Concurrent;

namespace Saigak.Processor
{
	public sealed class JsProcessor
	{
		public static JsProcessor Instance { get; } = new JsProcessor();

		private readonly ConcurrentDictionary<string, CompiledScript> _cache = new ConcurrentDictionary<string, CompiledScript>();

		public void Run(string key, string content, Globals globals, string path = null)
		{
			var engine = new ScriptEngine()
			{
				EnableExposedClrTypes = true
			};

			engine.SetGlobalValue("context", globals.Context);

			engine.SetGlobalFunction("write", (Action<string>)(str => globals.Write(str)));
			engine.SetGlobalFunction("writeline", (Action<string>)(str => globals.Write(str)));
			engine.SetGlobalFunction("getrequeststring", (Func<string>)(() => globals.GetRequestString()));
			engine.SetGlobalFunction("getrequestbytes", (Func<byte[]>)(() => globals.GetRequestBytes()));

			if (!_cache.TryGetValue(key, out var script))
			{
				script = engine.Compile(new StringScriptSource(content, path));
				_cache[key] = script;
			}

			script.Execute(engine);
		}
	}
}
