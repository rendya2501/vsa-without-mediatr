namespace FeatureWithoutMediatR.Feature.VideoGames.GetGameById;

/// <summary>
/// ゲーム詳細レスポンス
/// </summary>
/// <param name="Id">ゲームID</param>
/// <param name="Title">ゲームタイトル</param>
/// <param name="Genre">ゲームジャンル</param>
/// <param name="ReleaseYear">リリース年</param>
/// <remarks>
/// 現状はGetAllGamesResponseと同一構造だが、
/// 将来的に詳細情報（説明文、評価など）を追加する余地を残している。
/// </remarks>
internal sealed record GetGameByIdResponse(
    int Id,
    string Title,
    string Genre,
    int ReleaseYear);
