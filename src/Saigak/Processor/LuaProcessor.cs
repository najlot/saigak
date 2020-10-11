using MoonSharp.Interpreter;
using System;

namespace Saigak.Processor
{
	public sealed class LuaProcessor
	{
		public static LuaProcessor Instance { get; } = new LuaProcessor();

		public void Run(string content, Globals globals)
		{
			var script = new Script();

			script.Globals["write"] = (Action<string>)(str => globals.Write(str));
			script.Globals["writeline"] = (Action<string>)(str => globals.Write(str));
			script.Globals["getrequeststring"] = (Func<string>)(() => globals.GetRequestString());
			script.Globals["getrequestbytes"] = (Func<byte[]>)(() => globals.GetRequestBytes());

			script.DoString(content);
		}
	}
}
