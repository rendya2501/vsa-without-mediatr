using Domain.VideoGame;
using DomainKernel;
using FeatureShared.Messaging;
using Infrastructure.Database;

namespace FeatureWithoutMediatR.Feature.VideoGames.GetGameById;

/// <summary>
/// クエリハンドラ（詳細取得処理実行）
/// </summary>
internal sealed class GetGameByIdHandler(ApplicationDbContext dbContext)
    : IQueryHandler<GetGameByIdQuery, GetGameByIdResponse>
{
    /// <summary>
    /// 指定IDのゲーム情報を取得
    /// </summary>
    /// <param name="query">詳細取得クエリ</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>ゲーム詳細情報、または存在しない場合はnull</returns>
    /// <remarks>
    /// FindAsync()は主キーによる高速検索を実行。
    /// 存在チェックをハンドラ内で行い、nullを返すことで
    /// Endpointでの404判定を可能にしている。
    /// </remarks>
    public async Task<Result<GetGameByIdResponse>> Handle(GetGameByIdQuery query, CancellationToken ct)
    {
        var videoGame = await dbContext.VideoGames.FindAsync([query.Id], ct);

        if (videoGame is null)
        {
            return Result.Failure<GetGameByIdResponse>(VideoGameErrors.NotFound(query.Id));
        }

        var resposne = new GetGameByIdResponse(
            videoGame.Id,
            videoGame.Title,
            videoGame.Genre,
            videoGame.ReleaseYear);

        return Result.Success(resposne);
    }
}
