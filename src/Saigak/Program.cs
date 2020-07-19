using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Saigak.RequestHandler;

namespace Saigak
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			await Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Program>();
				})
				.Build()
				.RunAsync();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			var handler = new StaticRequestHandler(env.ContentRootPath)
				.SetNext(new SaiRequestHandler(env.ContentRootPath))
				.SetNext(new CsRequestHandler(env.ContentRootPath))
				.SetNext(new NotFoundRequestHandler(env.ContentRootPath));

			app.Run(async context =>
			{
				await handler.Handle(context);
			});
		}
	}
}
