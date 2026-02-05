using DomainKernel;

namespace FeatureShared.Messaging;

/// <summary>
/// コマンドハンドラーインターフェース（値なし）
/// </summary>
/// <typeparam name="TCommand">コマンドの型</typeparam>
/// <remarks>
/// <para>
/// コマンドハンドラーは、コマンドを処理するためのインターフェースです。
/// </para>
/// </remarks>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// コマンドを処理する
    /// </summary>
    /// <param name="command">コマンド</param>
    /// <param name="cancellationToken">キャンセルトークン</param>
    /// <returns>結果</returns>
    Task<Result> Handle(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// コマンドハンドラーインターフェース（値あり）
/// </summary>
/// <typeparam name="TCommand">コマンドの型</typeparam>
/// <typeparam name="TResponse">レスポンスの型</typeparam>
/// <remarks>
/// <para>
/// コマンドハンドラーは、コマンドを処理するためのインターフェースです。
/// </para>
/// </remarks>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    /// <summary>
    /// コマンドを処理する
    /// </summary>
    /// <param name="command">コマンド</param>
    /// <param name="cancellationToken">キャンセルトークン</param>
    /// <returns>結果</returns>
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken = default);
}
