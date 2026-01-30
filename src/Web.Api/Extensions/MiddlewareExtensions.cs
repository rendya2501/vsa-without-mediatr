using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Web.Api.Extensions;

namespace Web.Api.Extensions;

/// <summary>
/// ミドルウェアパイプラインの設定を整理する拡張メソッド
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Serilogのリクエストロギング設定
    /// </summary>
    public static WebApplication UseSerilogRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
                diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
            };
        });

        return app;
    }

    /// <summary>
    /// グローバル例外ハンドリングの設定
    /// </summary>
    /// <remarks>
    ///  ===== ProblemDetails の JSON レスポンス例 =====
    /// {
    ///   "type": "https://httpstatuses.com/400",
    ///   "title": "Validation failed",
    ///   "status": 400,
    ///   "detail": "One or more validation errors occurred.",
    ///   "instance": "/games",
    ///   "errors": {
    ///     "Name": ["Name must not be empty"],
    ///     "Price": ["Price must be greater than 0"]
    ///   }
    /// }
    /// </remarks>
    public static WebApplication UseGlobalExceptionHandler(this WebApplication app)
    {
        // ここでキャッチされるのは「どこでも未処理で投げられた例外」
        app.UseExceptionHandler(errorApp =>
        {
            // 例外発生時に実行されるパイプラインを定義
            errorApp.Run(async context =>
            {
                // 現在の HTTP コンテキストから例外情報を取得
                // IExceptionHandlerFeature は UseExceptionHandler が内部で設定してくれる
                var exception = context.Features
                    .Get<IExceptionHandlerFeature>()?
                    .Error;

                if (exception == null)
                {
                    return;
                }

                // ProblemDetails の共通設定
                ProblemDetails problemDetails;

                // ValidationException（400 Bad Request）
                if (exception is ValidationException validationException)
                {
                    Log.Warning(
                        "Validation failed for {Path}: {ErrorCount} errors",
                        context.Request.Path,
                        validationException.Errors.Count());

                    // ValidationException が持つ Errors をプロパティ名ごとにグルーピングする
                    var errors = validationException.Errors
                        // PropertyName（例: "Name", "Price"）ごとにまとめる
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,     // プロパティ名
                            g => g.Select(e => e.ErrorMessage).ToArray()    // エラーメッセージ配列
                        );

                    // ProblemDetails を生成
                    problemDetails = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/400",
                        Title = "Validation failed",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "One or more validation errors occurred.",
                        Instance = context.Request.Path,
                    };

                    // 拡張領域に errors を詰める（RFC 準拠）
                    problemDetails.Extensions["errors"] = errors;

                    // HTTP ステータスコードを 400 Bad Request に設定
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
                // DbUpdateException（409 Conflict）
                else if (exception is DbUpdateException dbUpdateException)
                {
                    Log.Error(
                        dbUpdateException,
                        "Database update failed for {Path}",
                        context.Request.Path);

                    problemDetails = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/409",
                        Title = "Database conflict",
                        Status = StatusCodes.Status409Conflict,
                        Detail = app.Environment.IsDevelopment()
                            ? dbUpdateException.Message
                            : "A database conflict occurred. Please try again.",
                        Instance = context.Request.Path,
                    };

                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                }
                // OperationCanceledException（499 Client Closed Request）
                else if (exception is OperationCanceledException)
                {
                    Log.Information(
                        "Request was cancelled by client for {Path}",
                        context.Request.Path);

                    // クライアントがリクエストをキャンセルした場合は何も返さない
                    context.Response.StatusCode = 499; // Nginx の非標準ステータスコード
                    return;
                }
                // UnauthorizedAccessException（403 Forbidden）
                else if (exception is UnauthorizedAccessException)
                {
                    Log.Warning(
                        "Unauthorized access attempt for {Path}",
                        context.Request.Path);

                    problemDetails = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/403",
                        Title = "Forbidden",
                        Status = StatusCodes.Status403Forbidden,
                        Detail = "You do not have permission to access this resource.",
                        Instance = context.Request.Path,
                    };

                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                }
                // その他の例外（500 Internal Server Error）
                else
                {
                    Log.Error(
                        exception,
                        "Unhandled exception occurred for {Path}",
                        context.Request.Path);

                    problemDetails = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/500",
                        Title = "Internal Server Error",
                        Status = StatusCodes.Status500InternalServerError,
                        // 開発環境のみ詳細なエラーメッセージを表示
                        Detail = app.Environment.IsDevelopment()
                            ? $"{exception.Message}\n\nStack Trace:\n{exception.StackTrace}"
                            : "An unexpected error occurred. Please try again later.",
                        Instance = context.Request.Path,
                    };

                    // 開発環境のみ、例外の詳細情報を追加
                    if (app.Environment.IsDevelopment())
                    {
                        problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
                        problemDetails.Extensions["innerException"] = exception.InnerException?.Message;
                    }

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }

                // レスポンスの Content-Type を JSON に設定
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(problemDetails);
            });
        });

        return app;
    }
}
