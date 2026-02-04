using DomainKernel;

namespace Domain.VideoGame;

public static class VideoGameErrors
{
    //public static Error AlreadyCompleted(int videoGameId) => Error.Problem(
    //    "VideoGame.AlreadyCompleted",
    //    $"The VideoGame with Id = '{videoGameId}' is already completed.");

    public static Error NotFound(int videoGameId) => Error.NotFound(
        "VideoGame.NotFound",
        $"The VideoGame with the Id = '{videoGameId}' was not found");
}

//if (deleted.IsFailure)
//{
//    return Results.NotFound($"Video game with id {id} not found.");
//}