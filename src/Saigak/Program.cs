using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Saigak.RequestHandler;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Najlot.Log;
using Najlot.Log.Destinations;
using Najlot.Log.Extensions.Logging;
using Najlot.Log.Middleware;

namespace Saigak
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			LogAdministrator.Instance
#if DEBUG
				.SetLogLevel(Najlot.Log.LogLevel.Debug)
#else
				.SetLogLevel(Najlot.Log.LogLevel.Warn)
#endif
				.SetCollectMiddleware<ConcurrentCollectMiddleware, ConsoleDestination>()
				.AddConsoleDestination(true);

			await Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder
						.ConfigureLogging(builder =>
						{
							builder.ClearProviders();
							builder.AddNajlotLog(LogAdministrator.Instance);
						})
						.UseStartup<Program>();
				})
				.Build()
				.RunAsync();

			LogAdministrator.Instance.Dispose();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			var contentRootPath = env.ContentRootPath;

			var handler = new SaiRequestHandler(contentRootPath)
				.SetNext(new CsRequestHandler(contentRootPath))
				.SetNext(new PhpRequestHandler(contentRootPath))
				.SetNext(new PyRequestHandler(contentRootPath))
				.SetNext(new JsRequestHandler(contentRootPath))
				.SetNext(new LuaRequestHandler(contentRootPath))
				.SetNext(new StaticRequestHandler(contentRootPath))
				.SetNext(new NotFoundRequestHandler(contentRootPath));

			app.Run(async context =>
			{
				var globals = new Globals(context, handler);
				await handler.Handle(globals);
			});
		}
	}
}
