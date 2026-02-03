using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Shared.Messaging;

namespace FeatureWithoutMediatR.Feature.VideoGames.GetAllGames;

/// <summary>
/// クエリハンドラ（一覧取得処理実行）
/// </summary>
internal sealed class GetAllGamesHandler(VideoGameDbContext dbContext)
    : IQueryHandler<GetAllGamesQuery, IEnumerable<GetAllGamesResponse>>
{
    /// <summary>
    /// 全ゲーム情報を取得
    /// </summary>
    /// <param name="_">一覧取得クエリ</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>全ゲーム情報のコレクション</returns>
    /// <remarks>
    /// ToListAsync()により、データベースへの1回のクエリで全データを取得。
    /// その後、メモリ上でEntity→Response DTOへのマッピングを実行。
    /// </remarks>
    public async Task<IEnumerable<GetAllGamesResponse>> Handle(GetAllGamesQuery _, CancellationToken ct)
    {
        var videoGames = await dbContext.VideoGames.ToListAsync(ct);
        return videoGames.Select(vg => new GetAllGamesResponse(vg.Id, vg.Title, vg.Genre, vg.ReleaseYear));
    }
}
