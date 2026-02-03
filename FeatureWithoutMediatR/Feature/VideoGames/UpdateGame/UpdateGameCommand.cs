using FeatureWithoutMediatR.Constants;
using FluentValidation;
using Shared.Messaging;

namespace FeatureWithoutMediatR.Feature.VideoGames.UpdateGame;

/// <summary>
/// ゲーム更新コマンド（内部処理用）
/// </summary>
/// <param name="Id">更新対象のゲームID</param>
/// <param name="Title">新しいゲームタイトル</param>
/// <param name="Genre">新しいゲームジャンル</param>
/// <param name="ReleaseYear">新しいリリース年</param>
/// <remarks>
/// ルートパラメータとリクエストボディを結合したコマンド。
/// ValidationBehaviorにより自動的にValidatorが適用される。
/// </remarks>
internal sealed record UpdateGameCommand(
    int Id,
    string Title,
    string Genre,
    int ReleaseYear) : ICommand<UpdateGameResponse?>
{
    /// <summary>
    /// コマンド検証ルール
    /// </summary>
    /// <remarks>
    /// CreateGameと同一のバリデーションルールを適用。
    /// 作成と更新で整合性を保つことで、クライアント側の実装を簡素化。
    /// </remarks>
    public class Validator : AbstractValidator<UpdateGameCommand>
    {
        public Validator()
        {
            // タイトルは必須 & 最大文字数
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(VideoGameConstants.Validation.Title.MaxLength);

            // ジャンルは必須 & 最大文字数
            RuleFor(x => x.Genre)
                .NotEmpty()
                .MaximumLength(VideoGameConstants.Validation.Genre.MaxLength);

            // リリース年は現実的な範囲に制限
            RuleFor(x => x.ReleaseYear)
                .InclusiveBetween(VideoGameConstants.Validation.ReleaseYear.MinValue, DateTime.Now.Year);
        }
    }
}
