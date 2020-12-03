#if NETFRAMEWORK
namespace Func.AspNet
{
    using System;
    using System.Web.Http;

    public static class HttpConfigurationExtensionMethods
    {
        public static void AddResultConversion(this HttpConfiguration configuration) =>
            configuration.AddResultConversion(x => x);

        public static void AddResultConversion(this HttpConfiguration mvcOptions, Func<ResultConversionConfiguration, ResultConversionConfiguration> configure) =>
            mvcOptions.Filters.Add(new ResultFilter(configure(new ResultConversionConfiguration(new DefaultErrorResponseConverter()))));
    }
}
#endif