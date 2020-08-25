# Func-AspNet

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/awsxdr/func-aspnetcore/blob/master/LICENSE)
![CI](https://github.com/awsxdr/func-aspnetcore/workflows/CI/badge.svg)

Add-on for [Func](https://github.com/awsxdr/func) with ASP .NET specific features

##  Getting Started

The following sections detail how to get started in Core and Framework versions of ASP. Full example projects are also available in the source.

### ASP .NET Core

**Configure Func.AspNet for use**

Call the `AddResultConversion` extension method when configuring controllers:
```csharp
services.AddControllers(config =>
{
  config.AddResultConversion();
});
```

**Add error handling attributes as appropriate**

Attributes can be added to controllers or to the errors themselves as shown below. Any unhandled error will produce a 500 response. When both attributes are present, the attribute on the controller will take precidence.
```csharp
[HttpGet("")]
[OnFailure(typeof(PageNotFoundError), HttpStatusCode.NotFound, Message = "Page not found")]
[OnFailure(typeof(DocumentNotFoundError), HttpStatusCode.NotFound)]
public Result GetItem([FromQuery] string type)
{
  // Controller logic
}
```
-OR-
```csharp
[ProducesStatusCode(HttpStatusCode.NotFound)]
public class NotFoundError : ResultError { }
```

**Define message sources**

Fixed error messages can be defined on the controller as shown above. If dynamic messages are required then a property or parameterless method should be defined on the error type.
```csharp
[MessageTextSource(nameof(Message))]
public class DocumentNotFoundError : NotFoundError
{
  public Guid Id { get; } = Guid.NewGuid();
  public string Message => $"Could not find document {Id}";
}
```

**Override success status code**

By default, a success is returned with a 200 status code. If another status code is required, this can be achieved by using `OnSuccessAttribute` on the controller action:
```csharp
[HttpPost("/")]
[OnSuccess(HttpStatusCode.Created)]
public Result CreateItem(ItemDetailsModel item)
{
  // Controller logic
}
```

### ASP .NET


**Configure Func.AspNet for use**

Call the `AddResultConversion` extension method when configuring the `HttpConfiguration` object:
```csharp
public static void Register(HttpConfiguration config)
{
    /*
      Other configuration...
    */

    config.AddResultConversion();
}
```

**Add error handling attributes as appropriate**

Attributes can be added to controllers or to the errors themselves as shown below. Any unhandled error will produce a 500 response. When both attributes are present, the attribute on the controller will take precidence.
```csharp
[HttpGet, Route("")]
[OnFailure(typeof(PageNotFoundError), HttpStatusCode.NotFound, Message = "Page not found")]
[OnFailure(typeof(DocumentNotFoundError), HttpStatusCode.NotFound)]
public Result GetItem([FromQuery] string type)
{
  // Controller logic
}
```
-OR-
```csharp
[ProducesStatusCode(HttpStatusCode.NotFound)]
public class NotFoundError : ResultError { }
```

**Define message sources**

Fixed error messages can be defined on the controller as shown above. If dynamic messages are required then a property or parameterless method should be defined on the error type.
```csharp
[MessageTextSource(nameof(Message))]
public class DocumentNotFoundError : NotFoundError
{
  public Guid Id { get; } = Guid.NewGuid();
  public string Message => $"Could not find document {Id}";
}
```

**Override success status code**

By default, a success is returned with a 200 status code. If another status code is required, this can be achieved by using `OnSuccessAttribute` on the controller action:
```csharp
[HttpPost, Route("/")]
[OnSuccess(HttpStatusCode.Created)]
public Result CreateItem(ItemDetailsModel item)
{
  // Controller logic
}
```
