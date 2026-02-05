using Carter;
using Domain.VideoGame;
using FeatureShared.Extensions;
using MediatR;
using static FeatureWithMediatR.Features.VideoGames.CreateGame;

namespace Web.Api.Endpoints.WithMediatR.VideoGames;

public sealed class CreateGameEndpoint : ICarterModule
{
    /// <summary>
    /// ゲーム作成リクエスト（外部APIインターフェース）
    /// </summary>
    /// <param name="Title">ゲームタイトル（最大100文字）</param>
    /// <param name="Genre">ゲームジャンル（最大50文字）</param>
    /// <param name="ReleaseYear">リリース年（1950年以降）</param>
    /// <remarks>
    /// OpenAPI/Scalarでドキュメント化される公開API契約。
    /// 内部のCommandとは意図的に分離し、API仕様の独立性を保つ。
    /// </remarks>
    private record CreateGameRequest(
        string Title,
        string Genre,
        int ReleaseYear = VideoGameValidationRules.ReleaseYear.DefaultValue);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapWithMediatRGamesApi()
            .MapPost("/", async (
                ISender sender,
                CreateGameRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateGameCommand(
                    request.Title,
                    request.Genre,
                    request.ReleaseYear
                );

                var result = await sender.Send(command, cancellationToken);

                // 201 Created + Location ヘッダ付きレスポンス
                return result.ToCreatedAtRoute(
                    routeName: VideoGameRouteNames.GetById,
                    routeValuesSelector: response => new { id = response.Id });
            })
            .WithName(VideoGameRouteNames.Create)
            //.WithSummary("Create a new video game")
            .WithDescription("Creates a new video game entry in the database")
            .ProducesValidationProblem()
            .Produces<CreateGameResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }
}
