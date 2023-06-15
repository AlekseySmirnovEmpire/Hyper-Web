using Microsoft.AspNetCore.Mvc;
using Server.Data.Api;
using Server.Data.Api.Interfaces;
using Server.Data.Exceptions;

namespace Server.Controllers;

public class BaseApiController : ControllerBase
{
    public IActionResult JsonApiResponse(Func<IApiResponse> lambda)
    {
        try
        {
            return Ok(lambda());
        }
        catch (Exception ex)
        {
            return JsonApiError(ex);
        }
    }

    private IActionResult JsonApiError(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => StatusCode(
                StatusCodes.Status403Forbidden,
                new ApiError(
                    StatusCodes.Status403Forbidden,
                    "Доступ запрещён")),
            WrongFiltersValueException => StatusCode(
                StatusCodes.Status400BadRequest,
                new ApiError(StatusCodes.Status400BadRequest, exception.Message)),
            WrongDataException => StatusCode(
                StatusCodes.Status400BadRequest,
                new ApiError(StatusCodes.Status400BadRequest, exception.Message)),
            ArgumentNullException => StatusCode(
                StatusCodes.Status400BadRequest,
                new ApiError(StatusCodes.Status400BadRequest, $"Неверно указан параметр {exception.Message}")),
            InvalidDataException => StatusCode(
                StatusCodes.Status409Conflict,
                new ApiError(StatusCodes.Status409Conflict, exception.Message)),
            _ => BadRequest(new ApiError(StatusCodes.Status400BadRequest, "Попробуйте запрос позже"))
        };
    }
}