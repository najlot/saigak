using Pchp.Core;
using System.Collections.Concurrent;

namespace Saigak.Processor
{
	public sealed class PhpProcessor
	{
		public static PhpProcessor Instance { get; } = new PhpProcessor();

		private readonly ConcurrentDictionary<string, Context.IScript> _cache = new ConcurrentDictionary<string, Context.IScript>();

		private readonly string[] AdditionalReferences = new[]
		{
			// typeof(Peachpie.Library.Graphics.PhpGd2).Assembly.Location,
			// typeof(Peachpie.Library.PDO.PDO).Assembly.Location,
			// typeof(Peachpie.Library.PDO.Sqlite.PDOSqliteDriver).Assembly.Location,
			typeof(Peachpie.Library.Scripting.Standard).Assembly.Location,
			typeof(Peachpie.Library.XmlDom.XmlDom).Assembly.Location,
			// typeof(Peachpie.Library.Network.CURLFunctions).Assembly.Location,
		};

		public void Run(string content, Context context, string path, bool isSubmission = true)
		{
			var provider = Context.DefaultScriptingProvider;

			var script = provider.CreateScript(new Context.ScriptOptions()
			{
				Context = context,
				IsSubmission = isSubmission,
				EmitDebugInformation = false,
				Location = new Location(path, 0, 0),
				AdditionalReferences = AdditionalReferences,
			}, content);

			script.Evaluate(context, context.Globals);
		}
	}
}
