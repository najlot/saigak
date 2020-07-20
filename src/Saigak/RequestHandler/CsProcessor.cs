using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Saigak.RequestHandler
{
	public sealed class CsProcessor
	{
		public static CsProcessor Instance { get; } = new CsProcessor();

		private readonly InteractiveAssemblyLoader loader;
		private readonly ScriptOptions options;
		private readonly ConcurrentDictionary<string, ScriptRunner<object>> cache = new ConcurrentDictionary<string, ScriptRunner<object>>();

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

			loader = new InteractiveAssemblyLoader();
			foreach (var reference in references)
			{
				loader.RegisterDependency(reference);
			}

			options = ScriptOptions.Default
				.WithReferences(references)
				.AddImports(types.Select(type => type.Namespace).ToArray());

			var dummy = CSharpScript
				.Create("if (Context != null) await WriteAsync(\"Something is wrong.\");", options, typeof(Globals), loader)
				.CreateDelegate();

			dummy(new Globals(null));
		}

		public async Task Run(string content, Globals globals)
		{
			if (!cache.TryGetValue(content, out var script))
			{
				script = CSharpScript
				   .Create(content, options, typeof(Globals), loader)
				   .CreateDelegate();

				cache[content] = script;
			}

			await script(globals);
		}
	}
}
