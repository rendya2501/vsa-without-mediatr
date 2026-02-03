using Infrastructure.Database;
using Shared.Messaging;

namespace FeatureWithoutMediatR.Feature.VideoGames.GetGameById;

/// <summary>
/// クエリハンドラ（詳細取得処理実行）
/// </summary>
internal sealed class GetGameByIdHandler(VideoGameDbContext dbContext)
    : IQueryHandler<GetGameByIdQuery, GetGameByIdResponse?>
{
    /// <summary>
    /// 指定IDのゲーム情報を取得
    /// </summary>
    /// <param name="query">詳細取得クエリ</param>
    /// <param name="cancellationToken">キャンセルトークン</param>
    /// <returns>ゲーム詳細情報、または存在しない場合はnull</returns>
    /// <remarks>
    /// FindAsync()は主キーによる高速検索を実行。
    /// 存在チェックをハンドラ内で行い、nullを返すことで
    /// Endpointでの404判定を可能にしている。
    /// </remarks>
    public async Task<GetGameByIdResponse?> Handle(GetGameByIdQuery query, CancellationToken cancellationToken)
    {
        var videoGame = await dbContext.VideoGames.FindAsync([query.Id], cancellationToken);

        if (videoGame is null)
        {
            return null;
        }

        return new GetGameByIdResponse(videoGame.Id, videoGame.Title, videoGame.Genre, videoGame.ReleaseYear);
    }
}
