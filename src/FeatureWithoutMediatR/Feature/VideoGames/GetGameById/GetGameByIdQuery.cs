using FeatureShared.Messaging;

namespace FeatureWithoutMediatR.Feature.VideoGames.GetGameById;

/// <summary>
/// ゲーム詳細取得クエリ
/// </summary>
/// <param name="Id">取得対象のゲームID</param>
/// <remarks>
/// 戻り値がnullableであり、データ不在時はnullを返す設計。
/// </remarks>
internal sealed record GetGameByIdQuery(int Id) : IQuery<GetGameByIdResponse>;
