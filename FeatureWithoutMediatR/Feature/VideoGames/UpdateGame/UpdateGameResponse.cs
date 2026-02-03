namespace FeatureWithoutMediatR.Feature.VideoGames.UpdateGame;

/// <summary>
/// ゲーム更新レスポンス
/// </summary>
/// <param name="Id">更新されたゲームのID</param>
/// <param name="Title">更新後のゲームタイトル</param>
/// <param name="Genre">更新後のゲームジャンル</param>
/// <param name="ReleaseYear">更新後のリリース年</param>
/// <remarks>
/// 更新後の完全な情報を返却することで、クライアント側での再取得を不要にする。
/// </remarks>
internal sealed record UpdateGameResponse(
    int Id,
    string Title,
    string Genre,
    int ReleaseYear);
