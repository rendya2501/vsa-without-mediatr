namespace FeatureWithoutMediatR.Feature.VideoGames.GetAllGames;

/// <summary>
/// ゲーム情報レスポンス（一覧用）
/// </summary>
/// <param name="Id">ゲームID</param>
/// <param name="Title">ゲームタイトル</param>
/// <param name="Genre">ゲームジャンル</param>
/// <param name="ReleaseYear">リリース年</param>
/// <remarks>
/// 一覧表示に必要な最小限のフィールドのみを含む。
/// 詳細情報が必要な場合はGetByIdを使用。
/// </remarks>
public sealed record GetAllGamesResponse(
    int Id,
    string Title,
    string Genre,
    int ReleaseYear);
