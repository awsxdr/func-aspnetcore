#if NETSTANDARD
namespace Func.AspNet
{
    using System;
    using Microsoft.AspNetCore.Mvc;

    public static class MvcOptionsExtensionMethods
    {
        public static void AddResultConversion(this MvcOptions mvcOptions) =>
            mvcOptions.AddResultConversion(x => x);

        public static void AddResultConversion(this MvcOptions mvcOptions, Func<ResultConversionConfiguration, ResultConversionConfiguration> configure) =>
            mvcOptions.Filters.Add(new ResultFilter(configure(new ResultConversionConfiguration(new DefaultErrorResponseConverter()))));
    }
}
#endif