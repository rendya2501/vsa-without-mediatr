using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.ExceptionHandlers;

/// <summary>
/// ValidationException専用のハンドラー
/// </summary>
/// <remarks>
/// FluentValidationの例外をRFC 7807準拠のProblemDetailsに変換
/// </remarks>
internal sealed class ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        logger.LogWarning(
            validationException,
            "Validation failed for {RequestPath}",
            httpContext.Request.Path);

        // RFC 7807準拠のProblemDetailsレスポンスを作成
        var problemDetails = new ValidationProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest,
            Instance = httpContext.Request.Path
        };

        // FluentValidationのエラーをProblemDetailsのErrors辞書に変換
        // ①
        //foreach (var error in validationException.Errors)
        //{
        //    if (problemDetails.Errors.ContainsKey(error.PropertyName))
        //    {
        //        // 同じプロパティに複数のエラーがある場合
        //        var existingErrors = problemDetails.Errors[error.PropertyName].ToList();
        //        existingErrors.Add(error.ErrorMessage);
        //        problemDetails.Errors[error.PropertyName] = [.. existingErrors]; //existingErrors.ToArray()
        //    }
        //    else
        //    {
        //        problemDetails.Errors.Add(error.PropertyName, [error.ErrorMessage]);
        //    }
        //}

        // FluentValidationのエラーをProblemDetailsのErrors辞書に変換
        foreach (var error in validationException.Errors)
        {
            if (problemDetails.Errors.TryGetValue(error.PropertyName, out var existingErrors))
            {
                var merged = existingErrors.Concat([error.ErrorMessage]).ToArray();
                problemDetails.Errors[error.PropertyName] = merged;
            }
            else
            {
                problemDetails.Errors[error.PropertyName] = [error.ErrorMessage];
            }
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // 例外を処理済み
    }
}
