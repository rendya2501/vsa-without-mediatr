using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FeatureWithMediatR.Features.VideoGames;

/// <summary>
/// Video Gamesに関連するすべてのエンドポイントを管理するモジュール
/// </summary>
public sealed class VideoGamesModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/games")
            .WithTags("WithMediatR");

        // GetAll 
        group.MapGet("/", GetAllGames.Endpoint)
            .WithName(VideoGameConstants.RouteNames.GetAll)
            //.WithSummary("Get all video games")
            .WithDescription("Retrieves a list of all video games in the database")
            .Produces<IEnumerable<GetAllGames.GetAllGamesResponse>>(StatusCodes.Status200OK);

        // GetByID
        group.MapGet("/{id:int}", GetGameById.Endpoint)
            .WithName(VideoGameConstants.RouteNames.GetById)
            //.WithSummary("Get a video game by ID")
            .WithDescription("Retrieves a specific video game by its ID")
            .Produces<GetGameById.GetGameByIdResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // Create
        group.MapPost("/", CreateGame.Endpoint)
            .WithName(VideoGameConstants.RouteNames.Create)
            //.WithSummary("Create a new video game")
            .WithDescription("Creates a new video game entry in the database")
            .ProducesValidationProblem()
            .Produces<CreateGame.CreateGameResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        // Update
        group.MapPut("/{id:int}", UpdateGame.Endpoint)
            .WithName(VideoGameConstants.RouteNames.Update)
            //.WithSummary("Update an existing video game")
            .WithDescription("Updates an existing video game by its ID")
            .Produces<UpdateGame.UpdateGameResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        // Delete
        group.MapDelete("/{id:int}", DeleteGame.Endpoint)
            .WithName(VideoGameConstants.RouteNames.Delete)
            //.WithSummary("Delete a video game")
            .WithDescription("Deletes a video game by its ID")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}
