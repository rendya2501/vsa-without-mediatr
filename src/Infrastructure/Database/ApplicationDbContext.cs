using Domain.VideoGame;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

/// <summary>
/// アプリケーションのデータベースコンテキスト
/// </summary>
/// <remarks>
/// <para>
/// ビデオゲーム管理システムのデータアクセス層を提供します。
/// </para>
/// <para>
/// <strong>サポートするデータベース:</strong><br/>
/// - InMemory データベース（開発・テスト環境）<br/>
/// - SQL Server（本番環境）
/// </para>
/// <para>
/// <strong>注意事項:</strong><br/>
/// InMemory データベースはアプリケーション再起動時にすべてのデータが失われます。
/// </para>
/// </remarks>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    /// <summary>
    /// ビデオゲームのエンティティセット
    /// </summary>
    /// <remarks>
    /// このプロパティを通じて VideoGame エンティティの CRUD 操作を実行します。
    /// </remarks>
    public DbSet<VideoGame> VideoGames => Set<VideoGame>();

    /// <summary>
    /// モデル構築時の設定とシードデータの投入
    /// </summary>
    /// <param name="modelBuilder">モデルビルダー</param>
    /// <remarks>
    /// <para>
    /// HasData メソッドを使用して初期データを設定しています。
    /// これらのデータは以下の目的で使用されます:
    /// </para>
    /// <list type="bullet">
    /// <item>開発環境でのデモンストレーション</item>
    /// <item>統合テストの基礎データ</item>
    /// <item>UI/UX の動作確認</item>
    /// </list>
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VideoGame>().HasData(
            new VideoGame
            {
                Id = 1,
                Title = "The Legend of Zelda: Breath of the Wild",
                Genre = "Action",
                ReleaseYear = 2017
            },
            new VideoGame
            {
                Id = 2,
                Title = "The Witcher 3: Wild Hunt",
                Genre = "RPG",
                ReleaseYear = 2015
            },
            new VideoGame
            {
                Id = 3,
                Title = "DOOM Eternal",
                Genre = "Shooter",
                ReleaseYear = 2020
            },
            new VideoGame
            {
                Id = 4,
                Title = "Red Dead Redemption 2",
                Genre = "Adventure",
                ReleaseYear = 2018
            },
            new VideoGame
            {
                Id = 5,
                Title = "Civilization VI",
                Genre = "Strategy",
                ReleaseYear = 2016
            }
        );
    }
}
