#if NETFRAMEWORK
namespace Func.AspNet
{
    using System.Web.Http;

    public static class HttpConfigurationExtensionMethods
    {
        public static void AddResultConversion(this HttpConfiguration configuration) =>
            configuration.Filters.Add(new ResultFilter());
    }
}
#endif