using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TripHelper.Api.Controllers;

[ApiController]
public class ApiController : ControllerBase
{
    internal IActionResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
            return Problem();

        if (errors.All(error => error.Type == ErrorType.Validation))
            return ValidationProblem(errors);
        
        return Problem(errors[0]);
    }

    internal IActionResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(statusCode: statusCode, detail: error.Description);
    }

    internal IActionResult ValidationProblem(List<Error> errors)
    {
        var modelStateDictionary = new ModelStateDictionary();

        foreach (var error in errors)
            modelStateDictionary.AddModelError(error.Code, error.Description);

        return ValidationProblem(modelStateDictionary);
    }
}