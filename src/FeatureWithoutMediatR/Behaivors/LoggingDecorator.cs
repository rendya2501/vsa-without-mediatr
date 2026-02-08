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
/// Decorator パターンを使用して、すべての Command/Query Handler にロギング機能を追加します。
/// MediatR を使用しない軽量なアーキテクチャ向けの実装です。
/// </para>
/// <para>
/// <strong>設計の特徴:</strong><br/>
/// - MediatR の Pipeline Behavior と同等の機能を Decorator で実現<br/>
/// - 各 Handler 型（Command with Response / Command without Response / Query）ごとに専用クラスを提供<br/>
/// - DI コンテナで自動的に Handler をラップして登録
/// </para>
/// <para>
/// <strong>構造化ログ（Serilog）:</strong><br/>
/// LogContext.PushProperty でエラー情報を構造化してログに記録します。
/// これにより、ログ分析ツール（Seq、Elasticsearch等）で効率的に検索できます。
/// </para>
/// <para>
/// <strong>ログ出力例:</strong>
/// <code>
/// [14:23:45 INF] Command started: CreateGameCommand
/// [14:23:45 INF] Command completed: CreateGameCommand
/// [14:23:46 ERR] Command failed: UpdateGameCommand
/// </code>
/// </para>
/// </remarks>
internal static class LoggingDecorator
{
    /// <summary>
    /// レスポンスありコマンドハンドラー用ロギングデコレーター
    /// </summary>
    /// <typeparam name="TCommand">コマンド型（ICommand&lt;TResponse&gt; を実装）</typeparam>
    /// <typeparam name="TResponse">レスポンス型</typeparam>
    /// <param name="innerHandler">デコレート対象の実際のハンドラー</param>
    /// <param name="logger">ロガー</param>
    /// <remarks>
    /// Result&lt;TResponse&gt; を返す Command 専用のデコレーターです。
    /// 成功/失敗に応じて適切なログレベルで記録します。
    /// </remarks>
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
            : ICommandHandler<TCommand, TResponse>
            where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            logger.LogInformation("Command started: {CommandName}", commandName);

            Result<TResponse> result = await innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Command completed: {CommandName}", commandName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Command failed: {CommandName}", commandName);
                }
            }

            return result;
        }
    }

    /// <summary>
    /// レスポンスなしコマンドハンドラー用ロギングデコレーター
    /// </summary>
    /// <typeparam name="TCommand">コマンド型（ICommand を実装）</typeparam>
    /// <param name="innerHandler">デコレート対象の実際のハンドラー</param>
    /// <param name="logger">ロガー</param>
    /// <remarks>
    /// Result（レスポンスなし）を返す Command 専用のデコレーターです。
    /// 副作用のみを実行する Command（削除、更新等）で使用されます。
    /// </remarks>
    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        ILogger<CommandBaseHandler<TCommand>> logger)
            : ICommandHandler<TCommand>
            where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            logger.LogInformation("Command started: {CommandName}", commandName);

            Result result = await innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Command completed: {CommandName}", commandName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Command failed: {CommandName}", commandName);
                }
            }

            return result;
        }
    }

    /// <summary>
    /// クエリハンドラー用ロギングデコレーター
    /// </summary>
    /// <typeparam name="TQuery">クエリ型（IQuery&lt;TResponse&gt; を実装）</typeparam>
    /// <typeparam name="TResponse">レスポンス型</typeparam>
    /// <param name="innerHandler">デコレート対象の実際のハンドラー</param>
    /// <param name="logger">ロガー</param>
    /// <remarks>
    /// Result&lt;TResponse&gt; を返す Query 専用のデコレーターです。
    /// Command と異なり、データの読み取りのみを行います（副作用なし）。
    /// </remarks>
    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
            : IQueryHandler<TQuery, TResponse>
            where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken = default)
        {
            string queryName = typeof(TQuery).Name;

            logger.LogInformation("Query started: {QueryName}", queryName);

            Result<TResponse> result = await innerHandler.Handle(query, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Query completed: {QueryName}", queryName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Query failed: {QueryName}", queryName);
                }
            }

            return result;
        }
    }
}
