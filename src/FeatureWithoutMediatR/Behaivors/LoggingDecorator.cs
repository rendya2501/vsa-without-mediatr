using DomainKernel;
using FeatureShared.Messaging;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace FeatureWithoutMediatR.Behaivors;

/// <summary>
/// ロギングデコレーター
/// </summary>
/// <remarks>
/// <para>
/// すべてのコマンドとクエリをロギングする。
/// </para>
/// </remarks>
internal static class LoggingDecorator
{
    /// <summary>
    /// レスポンスありコマンドハンドラー
    /// </summary>
    /// <typeparam name="TCommand">コマンド</typeparam>
    /// <typeparam name="TResponse">レスポンス</typeparam>
    /// <param name="innerHandler">内部ハンドラー</param>
    /// <param name="logger">ロガー</param>
    /// <returns>レスポンス</returns>
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
            : ICommandHandler<TCommand, TResponse>
            where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            logger.LogInformation("Processing command {Command}", commandName);

            Result<TResponse> result = await innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed command {Command}", commandName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Completed command {Command} with error", commandName);
                }
            }

            return result;
        }
    }

    /// <summary>
    /// レスポンスなしコマンドハンドラー
    /// </summary>
    /// <typeparam name="TCommand">コマンド</typeparam>
    /// <param name="innerHandler">内部ハンドラー</param>
    /// <param name="logger">ロガー</param>
    /// <returns>レスポンス</returns>
    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        ILogger<CommandBaseHandler<TCommand>> logger)
            : ICommandHandler<TCommand>
            where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            logger.LogInformation("Processing command {Command}", commandName);

            Result result = await innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed command {Command}", commandName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Completed command {Command} with error", commandName);
                }
            }

            return result;
        }
    }

    /// <summary>
    /// クエリハンドラー
    /// </summary>
    /// <typeparam name="TQuery">クエリ</typeparam>
    /// <typeparam name="TResponse">レスポンス</param>
    /// <param name="innerHandler">内部ハンドラー</param>
    /// <param name="logger">ロガー</param>
    /// <returns>レスポンス</returns>
    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
            : IQueryHandler<TQuery, TResponse>
            where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken = default)
        {
            string requestName = typeof(TQuery).Name;

            logger.LogInformation("Processing request {RequestName}", requestName);

            Result<TResponse> result = await innerHandler.Handle(query, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed request {RequestName}", requestName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Processing request {RequestName} with error", requestName);
                }
            }

            return result;
        }
    }
}
