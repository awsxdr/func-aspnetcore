namespace Func.AspNet.Example.Controllers
{
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Func;
    using static Func.ResultHelper;

    [RoutePrefix("api/example")]
    public class ExampleController : ApiController
    {
        [HttpGet, Route("a")]
        public Result<int> GetA() => Succeed(123);

        [HttpGet, Route("b")]
        public int GetB() => 123;

        [HttpGet, Route("c")]
        public Result GetC() => Succeed();

        [HttpGet, Route("d")]
        [OnFailure(typeof(NotFoundError), HttpStatusCode.NotFound)]
        public Task<Result<int>> GetD() => ResultHelper<int>.Fail(new NotFoundError()).ToTask();

        [HttpGet, Route("e")]
        public Result GetE() => Fail(new UnauthorizedError());

        [HttpGet, Route("f")]
        [OnFailure(typeof(UnauthorizedError), HttpStatusCode.MethodNotAllowed)]
        public Result GetF() => Fail(new UnauthorizedError());

        [HttpGet, Route("g")]
        [OnSuccess(HttpStatusCode.Created)]
        public Result GetG() => Succeed(123);

        [HttpGet, Route("h")]
        [OnFailure(typeof(PageNotFoundError), HttpStatusCode.NotFound, Message = "Page not found")]
        [OnFailure(typeof(DocumentNotFoundError), HttpStatusCode.NotFound)]
        public Result GetH(string type)
        {
            switch(type)
            {
                case "p": return Fail(new PageNotFoundError());
                case "d": return Fail(new DocumentNotFoundError());
                case "s": return Succeed(123);
                default: return Fail(new NotFoundError());
            }
        }
    }
}
