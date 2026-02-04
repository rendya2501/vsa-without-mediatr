using FeatureShared.Messaging;

namespace FeatureWithoutMediatR.Feature.VideoGames.DeleteGame;

/// <summary>
/// ゲーム削除コマンド
/// </summary>
/// <param name="Id">削除対象のゲームID</param>
/// <remarks>
/// 削除成功時はtrue、対象が存在しない場合はfalseを返す。
/// </remarks>
internal sealed record DeleteGameCommand(int Id) : ICommand;
