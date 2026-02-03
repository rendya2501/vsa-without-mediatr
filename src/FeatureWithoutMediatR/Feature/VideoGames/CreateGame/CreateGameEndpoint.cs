using Carter;
using FeatureWithoutMediatR.Constants;
using FeatureWithoutMediatR.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Messaging;

namespace FeatureWithoutMediatR.Feature.VideoGames.CreateGame;

public sealed class CreateGameEndpoint2 : ICarterModule
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
        int ReleaseYear = VideoGameConstants.Validation.ReleaseYear.DefaultValue);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapApiGroup("/api/games2", "WithoutMediatR")
            .MapPost("/", async (
                CreateGameRequest request,
                ICommandHandler<CreateGameCommand, CreateGameResponse> handler,
                CancellationToken cancellationToken) =>
            {
                // 外部入力 DTO → 内部 Command へ変換
                var command = new CreateGameCommand(
                    request.Title,
                    request.Genre,
                    request.ReleaseYear
                );

                // MediatR 経由で処理を実行
                var result = await handler.Handle(command, cancellationToken);

                // 201 Created + Location ヘッダ付きレスポンス
                return Results.CreatedAtRoute(
                    routeName: VideoGameConstants.RouteNames.GetById,
                    routeValues: new { id = result.Id },
                    value: result);
            })
            .WithName(VideoGameConstants.RouteNames.Create)
            //.WithSummary("Create a new video game")
            .WithDescription("Creates a new video game entry in the database")
            .ProducesValidationProblem()
            .Produces<CreateGameResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }
}
