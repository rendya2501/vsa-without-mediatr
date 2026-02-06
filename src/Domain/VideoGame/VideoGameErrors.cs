using DomainKernel;

namespace Domain.VideoGame;

/// <summary>
/// ビデオゲームエラー
/// </summary>
/// <remarks>
/// ビデオゲームに関連するエラーを表します。
/// </remarks>
public static class VideoGameErrors
{
    /// <summary>
    /// ビデオゲームが見つからない
    /// </summary>
    /// <param name="videoGameId">ビデオゲームID</param>
    /// <returns>エラー</returns>
    public static Error NotFound(int videoGameId) => Error.NotFound(
        "VideoGame.NotFound",
        $"VideoGame with Id = '{videoGameId}' was not found");
}
