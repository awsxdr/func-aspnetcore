namespace Func.AspNetCore.Example.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using static Func.Result;

    [ApiController]
    public class ExampleController : ControllerBase
    {
        [HttpGet("/a")]
        public Result<int> GetA() => Succeed(123);

        [HttpGet("/b")]
        public int GetB() => 123;

        [HttpGet("/c")]
        public Result GetC() => Succeed();

        [HttpGet("/d")]
        [OnFailure(typeof(NotFoundError), 404)]
        public Result<int> GetD() => Result<int>.Fail(new NotFoundError());

        [HttpGet("/e")]
        public Result GetE() => Fail(new UnauthorizedError());

        [HttpGet("/f")]
        [OnFailure(typeof(UnauthorizedError), 405)]
        public Result GetF() => Fail(new UnauthorizedError());

        [HttpGet("/g")]
        [OnSuccess(201)]
        public Result GetG() => Succeed(123);

        [HttpGet("/h")]
        [OnFailure(typeof(PageNotFoundError), 404, Message = "Page not found")]
        [OnFailure(typeof(DocumentNotFoundError), 404, Message = "Document not found")]
        public Result GetH([FromQuery] string type) =>
            type switch
            {
                "p" => Fail(new PageNotFoundError()),
                "d" => Fail(new DocumentNotFoundError()),
                "s" => Succeed(123),
                _ => Fail(new NotFoundError())
            };
    }

    public class TestError : ResultError
    {

    }
}
