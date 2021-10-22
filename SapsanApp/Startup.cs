using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SapsanApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwagger();
            services.AddAuthServices();
            services.AddDbSettings();
            services.AddNewtonSoftJsonServices();
            services.AddTransientServices();
            services.AddApiVersions();
            services.AddCorsPolicy();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // env.EnvironmentName = "Production";
            if (env.IsDevelopment())
            {
                
            }
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sapsan v1"));
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAllOrigins");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}