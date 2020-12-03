namespace Func.AspNetCore.Example
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using Func.AspNet;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(config =>
            {
                config.AddResultConversion(resultConfig =>
                    resultConfig
                        .WithExceptionHandler(new TestExceptionResponseConverter())
                        .WithErrorResponseConverter(new TestErrorResponseConverter()));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class TestExceptionResponseConverter : IExceptionResponseConverter
    {
        public ErrorResponse GetExceptionResponse(Exception exception) =>
            new ErrorResponse
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Body = new
                {
                    Message = $"Oops! An error occurred: {exception.GetType().Name}",
                    ExtraDetails = (exception is ValidationException ve) ? ve.Value : null,
                }
            };
    }

    public class TestErrorResponseConverter : IErrorResponseConverter
    {
        public ErrorResponse GetErrorResponse<TError>(TError error, ResponseDetails configuredResponseDetails)
            where TError : ResultError =>
            new ErrorResponse
            {
                StatusCode = HttpStatusCode.NotImplemented,
                Body = new TestErrorResponseData
                {
                    TestField = "This is a test",
                    Message = configuredResponseDetails.Message,
                    ActualStatusCode = (int)configuredResponseDetails.StatusCode
                }
            };

        public class TestErrorResponseData
        {
            public string TestField { get; set; }
            public string Message { get; set; }
            public int ActualStatusCode { get; set; }
        }
    }

}
