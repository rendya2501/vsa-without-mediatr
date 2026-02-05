using Domain.VideoGame;
using FeatureShared.Messaging;
using FluentValidation;

namespace FeatureWithoutMediatR.Feature.VideoGames.CreateGame;

/// <summary>
/// ゲーム作成コマンド（内部処理用）
/// </summary>
/// <param name="Title">ゲームタイトル</param>
/// <param name="Genre">ゲームジャンル</param>
/// <param name="ReleaseYear">リリース年</param>
/// <remarks>
/// MediatR経由で処理されるアプリケーション内部のメッセージ。
/// ValidationBehaviorにより自動的にValidatorが適用される。
/// </remarks>
public sealed record CreateGameCommand(
    string Title,
    string Genre,
    int ReleaseYear) : ICommand<CreateGameResponse>
{
    /// <summary>
    /// コマンド検証ルール
    /// </summary>
    /// <remarks>
    /// MediatR Pipelineで自動実行される。
    /// 検証失敗時はValidationExceptionをスローし、
    /// グローバル例外ハンドラでProblemDetails形式に変換される。
    /// </remarks>
    public class Validator : AbstractValidator<CreateGameCommand>
    {
        public Validator()
        {
            // タイトルは必須 & 最大文字数
            RuleFor(x => x.Title)
                .NotEmpty()// .WithMessage("Title is required.")
                .MaximumLength(VideoGameValidationRules.Title.MaxLength);// .WithMessage("Length is Max100.");

            // ジャンルは必須 & 最大文字数
            RuleFor(x => x.Genre)
                .NotEmpty()
                .MaximumLength(VideoGameValidationRules.Genre.MaxLength);

            // リリース年は現実的な範囲に制限
            RuleFor(x => x.ReleaseYear)
                .InclusiveBetween(VideoGameValidationRules.ReleaseYear.MinValue, DateTime.Now.Year);
        }
    }
}
