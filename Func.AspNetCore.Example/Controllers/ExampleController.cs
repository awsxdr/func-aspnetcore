namespace Func.AspNetCore.Example.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using Func.AspNet;
    using Microsoft.AspNetCore.Mvc;
    using System.Net;
    using System.Threading.Tasks;
    using static Func.Result;

    [ApiController]
    public class ExampleController : ControllerBase
    {
        [HttpGet("/a")]
        public Result<int> GetA() => Succeed(123);

        [HttpGet("/b")]
        public int GetB() => 123;

        [HttpGet("/c")]
        [OnSuccess(HttpStatusCode.NoContent)]
        public Result GetC() => Succeed();

        [HttpGet("/d")]
        [OnFailure(typeof(NotFoundError), HttpStatusCode.NotFound)]
        public Task<Result<int>> GetD() => Result<int>.Fail(new NotFoundError()).ToTask();

        [HttpGet("/e")]
        public Result GetE() => Fail(new UnauthorizedError());

        [HttpGet("/f")]
        [OnFailure(typeof(UnauthorizedError), HttpStatusCode.MethodNotAllowed)]
        public Result GetF() => Fail(new UnauthorizedError());

        [HttpGet("/g")]
        [OnSuccess(HttpStatusCode.Created)]
        public Result GetG() => Succeed(123);

        [HttpGet("/h")]
        [OnFailure(typeof(PageNotFoundError), HttpStatusCode.NotFound, Message = "Page not found")]
        [OnFailure(typeof(DocumentNotFoundError), HttpStatusCode.NotFound)]
        public Result GetH([FromQuery] string type) =>
            type switch
            {
                "p" => Fail(new PageNotFoundError()),
                "d" => Fail(new DocumentNotFoundError()),
                "s" => Succeed(123),
                "x" => throw new ValidationException("Something terrible happened!", null, new { MoreDetails = "Some more details would go here" }),
                _ => Fail(new NotFoundError())
            };
    }
}
