#if NETSTANDARD
namespace Func.AspNet
{
    using Microsoft.AspNetCore.Mvc;

    public static class MvcOptionsExtensionMethods
    {
        public static void AddResultConversion(this MvcOptions mvcOptions) =>
            mvcOptions.Filters.Add(new ResultFilter());
    }
}
#endif