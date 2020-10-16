using Jurassic;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Saigak.Processor
{
	public sealed class JsProcessor
	{
		public static JsProcessor Instance { get; } = new JsProcessor();

		private readonly ConcurrentDictionary<(string Path, DateTime Time), CompiledScript> _cache = new ConcurrentDictionary<(string Path, DateTime Time), CompiledScript>();

		public void Run((string Path, DateTime Time) key, string content, Globals globals, string path = null)
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
				
				var duplicates = _cache.Keys.Where(k => k.Path == key.Path).ToArray();
				foreach (var duplicate in duplicates)
				{
					_cache.TryRemove(duplicate, out _);
				}

				_cache[key] = script;
			}

			script.Execute(engine);
		}
	}
}
