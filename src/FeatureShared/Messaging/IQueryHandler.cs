using DomainKernel;

namespace FeatureShared.Messaging;

/// <summary>
/// クエリハンドラーインターフェース
/// </summary>
/// <typeparam name="TQuery">クエリの型</typeparam>
/// <typeparam name="TResponse">レスポンスの型</typeparam>
/// <remarks>
/// <para>
/// クエリハンドラーは、クエリを処理するためのインターフェースです。
/// </para>
/// </remarks>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <summary>
    /// クエリを処理する
    /// </summary>
    /// <param name="query">クエリ</param>
    /// <param name="cancellationToken">キャンセルトークン</param>
    /// <returns>結果</returns>
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken = default);
}
