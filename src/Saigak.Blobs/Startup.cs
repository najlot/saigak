using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Dynamic;
using System.IO;
using System.Text.Json;

namespace Saigak.Blobs
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			// Starting with writing to files - "real" database to come
			Repositories.IRepository repository = new Repositories.FileRepository(env.ContentRootPath);
			
			app.Run(async context =>
			{
				var method = context.Request.Method;
				var path = context.Request.Path.Value;

				switch (method)
				{
					case "GET":
						{
							using var rc = await repository.GetAsync(path);
							if (rc == null)
							{
								context.Response.StatusCode = 404;
								return;
							}

							await rc.CopyToAsync(context.Response.Body).ConfigureAwait(false);
						}
						break;

					case "POST":
						{
							await repository.PostAsync(path, context.Request.Body).ConfigureAwait(false);
						}
						break;

					case "PUT":
						{
							await repository.PutAsync(path, context.Request.Body).ConfigureAwait(false);
						}
						break;

					case "DELETE":
						await repository.DeleteAsync(path).ConfigureAwait(false);
						break;
				}
			});
		}
	}
}
