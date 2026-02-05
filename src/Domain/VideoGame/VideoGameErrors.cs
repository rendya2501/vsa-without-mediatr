using DomainKernel;

namespace Domain.VideoGame;

/// <summary>
/// ビデオゲームエラー
/// </summary>
/// <remarks>
/// <para>
/// ビデオゲームエラーは、ビデオゲームに関連するエラーを表します。
/// </para>
/// </remarks>
public static class VideoGameErrors
{
    //public static Error AlreadyCompleted(int videoGameId) => Error.Problem(
    //    "VideoGame.AlreadyCompleted",
    //    $"The VideoGame with Id = '{videoGameId}' is already completed.");

    /// <summary>
    /// ビデオゲームが見つからない
    /// </summary>
    /// <param name="videoGameId">ビデオゲームID</param>
    /// <returns>エラー</returns>
    public static Error NotFound(int videoGameId) => Error.NotFound(
        "VideoGame.NotFound",
        $"The VideoGame with the Id = '{videoGameId}' was not found");
}

//if (deleted.IsFailure)
//{
//    return Results.NotFound($"Video game with id {id} not found.");
//}