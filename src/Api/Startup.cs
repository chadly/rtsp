using System.Data;
using System.Data.SqlClient;
using Archon.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rtsp
{
	public class Startup
	{
		readonly IConfiguration config;

		public Startup(IConfiguration config)
		{
			this.config = config;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			ConfigureOverridableServices(services);

			services.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
				// https://github.com/aspnet/Mvc/issues/8238
				.AddApplicationPart(typeof(Program).Assembly);
		}

		protected virtual void ConfigureOverridableServices(IServiceCollection services)
		{
			services.AddScoped<IDbConnection>(s => new DataContext(new SqlConnection(config["db"])));
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();

			app.UseMvc();
		}
	}
}
