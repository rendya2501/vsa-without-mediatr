using FeatureShared.Messaging;

namespace FeatureWithoutMediatR.Feature.VideoGames.GetAllGames;

/// <summary>
/// ゲーム一覧取得クエリ
/// </summary>
/// <remarks>
/// パラメータを持たないシンプルなクエリ。
/// 将来的にフィルタリングやソート機能を追加する場合は、
/// プロパティを追加して拡張可能。
/// </remarks>
internal sealed record GetAllGamesQuery : IQuery<IEnumerable<GetAllGamesResponse>>;
