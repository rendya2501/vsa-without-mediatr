namespace FeatureWithoutMediatR.Feature.VideoGames.CreateGame;

/// <summary>
/// ゲーム作成レスポンス
/// </summary>
/// <param name="Id">作成されたゲームのID</param>
/// <param name="Title">ゲームタイトル</param>
/// <param name="Genre">ゲームジャンル</param>
/// <param name="ReleaseYear">リリース年</param>
/// <remarks>
/// Entityを直接公開せず、API専用のDTOとして定義。
/// 将来的なEntity変更がAPIに影響しないよう分離している。
/// </remarks>
internal sealed record CreateGameResponse(
    int Id,
    string Title,
    string Genre,
    int ReleaseYear);
