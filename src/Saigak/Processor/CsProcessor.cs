using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Saigak.Processor
{
	public sealed class CsProcessor
	{
		public static CsProcessor Instance { get; } = new CsProcessor();

		private readonly InteractiveAssemblyLoader _loader;
		private readonly ScriptOptions _options;
		private readonly ConcurrentDictionary<string, ScriptRunner<object>> _cache = new ConcurrentDictionary<string, ScriptRunner<object>>();

		public CsProcessor()
		{
			var types = new[]
			{
				typeof(object),
				typeof(System.IO.FileInfo),
				typeof(System.Linq.IQueryable),
				typeof(System.Threading.Tasks.Task),
				typeof(System.Dynamic.DynamicObject),
				typeof(System.Collections.Generic.List<>),
				typeof(System.Text.RegularExpressions.Regex),
				typeof(ConcurrentDictionary<,>),
				typeof(Newtonsoft.Json.JsonConvert),

				typeof(Microsoft.AspNetCore.Http.HttpResponseWritingExtensions)
			};

			var references = types.Select(type => type.Assembly).ToArray();

			_loader = new InteractiveAssemblyLoader();
			foreach (var reference in references)
			{
				_loader.RegisterDependency(reference);
			}

			_options = ScriptOptions.Default
				.WithReferences(references)
				.AddImports(types.Select(type => type.Namespace).ToArray());
		}

		public async Task Run(string key, string content, Globals globals)
		{
			if (!_cache.TryGetValue(key, out var script))
			{
				script = CSharpScript
				   .Create(content, _options, typeof(Globals), _loader)
				   .CreateDelegate();

				_cache[key] = script;
			}

			await script(globals);
		}
	}
}
