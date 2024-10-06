using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Saigak.Processor
{
	public sealed class CsProcessor
	{
		public static CsProcessor Instance { get; } = new CsProcessor();

		private readonly InteractiveAssemblyLoader _loader = GetLoader();
		private readonly ScriptOptions _options = GetOptions();
		private readonly ConcurrentDictionary<(string Path, DateTime Time), ScriptRunner<object>> _cache = new();
		private readonly Script<object> _initialScript = null;

		private static System.Reflection.Assembly[] GetReferences() => new[]
		{
			typeof(object).Assembly,
			typeof(System.IO.FileInfo).Assembly,
			typeof(System.Linq.IQueryable).Assembly,
			typeof(Newtonsoft.Json.JsonConvert).Assembly,
			typeof(System.Dynamic.DynamicObject).Assembly,
			// typeof(System.Threading.Tasks.Task).Assembly,
			// typeof(System.Collections.Generic.List<>).Assembly,
			typeof(System.Text.RegularExpressions.Regex).Assembly,
			// typeof(System.Collections.Concurrent.ConcurrentDictionary<,>).Assembly,
			typeof(Microsoft.AspNetCore.Http.HttpResponseWritingExtensions).Assembly
		};

		private static InteractiveAssemblyLoader GetLoader()
		{
			var loader = new InteractiveAssemblyLoader();

			foreach (var reference in GetReferences())
			{
				loader.RegisterDependency(reference);
			}

			return loader;
		}

		private static ScriptOptions GetOptions()
		{
			return ScriptOptions.Default
					.WithReferences(GetReferences())
					.AddImports(
						"System",
						"System.IO",
						"System.Linq",
						"System.Text",
						"System.Dynamic",
						"Newtonsoft.Json",
						"System.Collections.Generic",
						"System.Text.RegularExpressions"
						);
		}

		public CsProcessor()
		{
			_initialScript = CSharpScript.Create("", _options, typeof(Globals), _loader);
			_initialScript.Compile();
		}

		public async Task Run((string Path, DateTime Time) key, string content, Globals globals)
		{
			if (!_cache.TryGetValue(key, out var script))
			{
				script = _initialScript
					.ContinueWith(content, _options)
					.CreateDelegate();

				var duplicates = _cache.Keys.Where(k => k.Path == key.Path).ToArray();
				foreach (var duplicate in duplicates)
				{
					_cache.TryRemove(duplicate, out _);
				}

				_cache[key] = script;
			}

			await script(globals);
		}
	}
}
