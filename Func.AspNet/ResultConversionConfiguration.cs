namespace Func.AspNet
{
#if NETSTANDARD2_1
    using static Option;
#else
    using static OptionHelper;
#endif

    public class ResultConversionConfiguration
    {
        public Option<IExceptionResponseConverter> ExceptionResponseConverter { get; }
        public IErrorResponseConverter ErrorResponseConverter { get; }

        public ResultConversionConfiguration(IErrorResponseConverter errorResponseConverter)
            : this(None<IExceptionResponseConverter>(), errorResponseConverter)
        {
        }

        private ResultConversionConfiguration(Option<IExceptionResponseConverter> exceptionResponseConverter, IErrorResponseConverter errorResponseConverter)
        {
            ExceptionResponseConverter = exceptionResponseConverter;
            ErrorResponseConverter = errorResponseConverter;
        }
        public ResultConversionConfiguration WithExceptionHandler(IExceptionResponseConverter exceptionResponseConverter) =>
            new ResultConversionConfiguration(Some(exceptionResponseConverter), ErrorResponseConverter);

        public ResultConversionConfiguration WithErrorResponseConverter(IErrorResponseConverter errorResponseConverter) =>
            new ResultConversionConfiguration(ExceptionResponseConverter, errorResponseConverter);

    }
}