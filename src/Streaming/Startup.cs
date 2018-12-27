using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rtsp.Streaming.Video;

namespace Rtsp.Streaming
{
	public class Startup
	{
		readonly IConfiguration config;
		readonly ApiOptions apiOpts;

		public Startup(IConfiguration config)
		{
			this.config = config;
			apiOpts = config.GetSection("api").Get<ApiOptions>();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<StreamingOptions>(config);

			services.AddSingleton<VideoConverter>();

			services.AddHostedService<StreamCleaningHost>();

			services.AddHttpClient<IRtspService, HttpRtspService>(c =>
			{
				c.BaseAddress = new Uri(apiOpts.Url);
				c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiOpts.Key);
			});

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();

			app.Use((context, next) =>
			{
				context.Response.Headers["Access-Control-Allow-Origin"] = "*";
				context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type";
				context.Response.Headers["Cache-Control"] = "no-cache";
				return next.Invoke();
			});

			app.UseMvc();
		}
	}
}
