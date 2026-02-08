# 🎮 Video Game API - Vertical Slice Architecture

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

**MediatR あり・なし両方の実装を比較できる、Vertical Slice Architecture のリファレンス実装**  

このプロジェクトは、ビデオゲーム管理 API を題材に、Clean Architecture と Vertical Slice Architecture を組み合わせた実装例を提供します。  
特徴的なのは、**MediatR を使用する実装と使用しない実装を同一リポジトリで並行して提供**している点です。  

---

## 📋 目次

- [特徴](#-特徴)
- [アーキテクチャ](#-アーキテクチャ)
- [技術スタック](#-技術スタック)
- [クイックスタート](#-クイックスタート)
- [プロジェクト構成](#-プロジェクト構成)
- [API エンドポイント](#-api-エンドポイント)
- [主要な設計パターン](#-主要な設計パターン)
- [MediatR vs 自作実装の比較](#-mediatr-vs-自作実装の比較)
- [開発ガイド](#-開発ガイド)
- [テスト](#-テスト)
- [デプロイ](#-デプロイ)
- [ライセンス](#-ライセンス)

---

## ✨ 特徴

### 🎯 このプロジェクトで学べること

- ✅ **Vertical Slice Architecture** の実践的な実装
- ✅ **Result パターン** による例外を使わないエラーハンドリング
- ✅ **CQRS パターン** (Command Query Responsibility Segregation)
- ✅ **MediatR あり・なし両方の実装** を比較できる
- ✅ **FluentValidation** による宣言的なバリデーション
- ✅ **Serilog** を使った構造化ログ
- ✅ **Carter** による Minimal API の整理
- ✅ **OpenAPI/Scalar** による自動ドキュメント生成

### 🚀 プロダクションレディな機能

- 🔐 RFC 7807 準拠の ProblemDetails エラーレスポンス
- 📝 構造化ログ（Serilog + ファイル/コンソール出力）
- 🎨 環境別設定（Development / Production）
- 🗄️ InMemory / SQL Server データベースの切り替え
- 🔄 自動バリデーション（Pipeline Behavior / Decorator パターン）
- 📊 OpenAPI ドキュメント自動生成

---

## 🏗️ アーキテクチャ

このプロジェクトは **Vertical Slice Architecture** と **Clean Architecture** を組み合わせた構成になっています。

### レイヤー構成

```txt
┌─────────────────────────────────────────────────────────────┐
│                         Web.Api                              │
│  (Presentation Layer - Minimal API + Carter)                │
│  - Endpoints: HTTP リクエスト/レスポンスの変換              │
│  - Exception Handlers: グローバルエラーハンドリング          │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│              Feature (Application Layer)                     │
│  ┌──────────────────────┐  ┌──────────────────────┐         │
│  │  FeatureWithMediatR  │  │FeatureWithoutMediatR │         │
│  │  - Commands/Queries  │  │  - Commands/Queries  │         │
│  │  - Handlers          │  │  - Handlers          │         │
│  │  - Validators        │  │  - Validators        │         │
│  │  - Pipeline Behavior │  │  - Decorators        │         │
│  └──────────────────────┘  └──────────────────────┘         │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                    Infrastructure                            │
│  - Database: DbContext, Migrations, Seeders                 │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                        Domain                                │
│  - Entities: VideoGame                                      │
│  - Errors: VideoGameErrors                                  │
│  - Validation Rules: VideoGameValidationRules               │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                     DomainKernel                             │
│  - Result Pattern: Result<T>, Error, ValidationError        │
└─────────────────────────────────────────────────────────────┘
```

### Vertical Slice とは？

従来の**レイヤードアーキテクチャ**（Controller → Service → Repository）では、1つの機能が複数のレイヤーに分散します。

**Vertical Slice Architecture** では、1つの機能に関連するすべてのコード（リクエスト・バリデーション・ロジック・レスポンス）を **1つのファイルまたはフォルダにまとめます**。

```txt
FeatureWithMediatR/
└── Features/
    └── VideoGames/
        ├── CreateGame.cs      ← Command, Validator, Handler がすべて含まれる
        ├── GetAllGames.cs
        ├── GetGameById.cs
        ├── UpdateGame.cs
        └── DeleteGame.cs
```

**メリット:**

- 機能追加・変更時に1ファイルを見るだけで完結
- 不要なコードの削除が容易
- チーム開発での競合が減少

---

## 🛠️ 技術スタック

| カテゴリ | 技術 | バージョン | 用途 |
| - | - | - |
| **フレームワーク** | .NET | 10.0 | ランタイム |
| **言語** | C# | 12.0 | プログラミング言語 |
| **Web API** | ASP.NET Core Minimal API | 10.0 | HTTP エンドポイント |
| **API 整理** | Carter | 10.0 | Minimal API のモジュール化 |
| **メディエーター** | MediatR | 14.0 | CQRS パターン実装（オプション） |
| **バリデーション** | FluentValidation | 12.1 | 宣言的バリデーション |
| **ORM** | Entity Framework Core | 10.0 | データアクセス |
| **データベース** | InMemory / SQL Server | - | 開発・本番環境 |
| **ロギング** | Serilog | 4.3 | 構造化ログ |
| **依存性注入支援** | Scrutor | 7.0 | アセンブリスキャン・Decorator登録 |
| **API ドキュメント** | Scalar | 2.12 | OpenAPI UI |

---

## 🚀 クイックスタート

### 前提条件

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) 以上
- （オプション）SQL Server（本番環境用）

### インストールと実行

```bash
# 1. リポジトリをクローン
git clone https://github.com/yourusername/video-game-api-vsa.git
cd video-game-api-vsa

# 2. 依存関係の復元
dotnet restore

# 3. アプリケーションの実行
cd src/Web.Api
dotnet run

# 4. ブラウザで開く
# Scalar UI: https://localhost:7212/scalar/v1
# Swagger UI: https://localhost:7212/openapi/v1.json
```

### 初回起動時の動作

1. InMemory データベースが自動作成される
2. シードデータ（5件のゲーム）が自動投入される
3. Scalar UI で API ドキュメントが閲覧可能

---

## 📁 プロジェクト構成

```txt
src/
├── DomainKernel/                # 共有カーネル
│   ├── Result.cs               # Result パターンの実装
│   ├── Error.cs                # エラー情報の定義
│   ├── ErrorType.cs            # エラー種別（HTTP ステータスへマッピング）
│   └── ValidationError.cs      # バリデーションエラー集約
│
├── Domain/                      # ドメイン層
│   └── VideoGame/
│       ├── VideoGame.cs        # エンティティ
│       ├── VideoGameErrors.cs  # ドメインエラー定義
│       └── VideoGameValidationRules.cs  # バリデーション定数
│
├── Infrastructure/              # インフラ層
│   ├── Database/
│   │   ├── ApplicationDbContext.cs      # EF Core DbContext
│   │   └── Seeding/
│   │       ├── IDbSeeder.cs
│   │       └── ApplicationDbSeeder.cs
│   └── DependencyInjection.cs  # DI 設定
│
├── FeatureShared/               # Feature 層共通コード
│   ├── Messaging/              # CQRS インターフェース
│   │   ├── ICommand.cs
│   │   ├── ICommandHandler.cs
│   │   ├── IQuery.cs
│   │   └── IQueryHandler.cs
│   ├── Extensions/
│   │   ├── ResultExtensions.cs      # Result → Match パターン
│   │   └── ResultHttpExtensions.cs  # Result → HTTP 変換
│   └── Infrastructure/
│       └── CustomResults.cs    # ProblemDetails 生成
│
├── FeatureWithMediatR/          # MediatR 使用版
│   ├── Behaviors/
│   │   ├── LoggingBehavior.cs       # ログ記録
│   │   └── ValidationBehavior.cs    # バリデーション
│   ├── Features/
│   │   └── VideoGames/
│   │       ├── CreateGame.cs   # Command + Validator + Handler
│   │       ├── GetAllGames.cs
│   │       ├── GetGameById.cs
│   │       ├── UpdateGame.cs
│   │       └── DeleteGame.cs
│   └── DependencyInjection.cs
│
├── FeatureWithoutMediatR/       # 自作実装版
│   ├── Behaviors/
│   │   ├── LoggingDecorator.cs      # Decorator パターン
│   │   └── ValidationDecorator.cs
│   ├── Feature/
│   │   └── VideoGames/
│   │       ├── CreateGame/
│   │       │   ├── CreateGameCommand.cs
│   │       │   ├── CreateGameHandler.cs
│   │       │   └── CreateGameResponse.cs
│   │       ├── GetAllGames/
│   │       └── ... (同様の構造)
│   └── DependencyInjection.cs
│
└── Web.Api/                     # プレゼンテーション層
    ├── Endpoints/
    │   ├── WithMediatR/        # MediatR 版エンドポイント
    │   └── WithoutMediatR/     # 自作版エンドポイント
    ├── ExceptionHandlers/
    │   ├── GlobalExceptionHandler.cs
    │   └── ValidationExceptionHandler.cs
    ├── DependencyInjection.cs
    ├── Program.cs
    └── appsettings.json
```

---

## 🌐 API エンドポイント

### MediatR 版

| メソッド | パス | 説明 |
| - | - | - |
| `GET` | `/api/with-mediatr/games` | 全ゲーム一覧取得 |
| `GET` | `/api/with-mediatr/games/{id}` | ID指定ゲーム取得 |
| `POST` | `/api/with-mediatr/games` | 新規ゲーム作成 |
| `PUT` | `/api/with-mediatr/games/{id}` | ゲーム更新 |
| `DELETE` | `/api/with-mediatr/games/{id}` | ゲーム削除 |

### 自作実装版

| メソッド | パス | 説明 |
| - | - | - |
| `GET` | `/api/without-mediatr/games` | 全ゲーム一覧取得 |
| `GET` | `/api/without-mediatr/games/{id}` | ID指定ゲーム取得 |
| `POST` | `/api/without-mediatr/games` | 新規ゲーム作成 |
| `PUT` | `/api/without-mediatr/games/{id}` | ゲーム更新 |
| `DELETE` | `/api/without-mediatr/games/{id}` | ゲーム削除 |

### リクエスト例

```bash
# ゲーム作成
curl -X POST https://localhost:7212/api/with-mediatr/games \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Elden Ring",
    "genre": "Action RPG",
    "releaseYear": 2022
  }'

# 全ゲーム取得
curl https://localhost:7212/api/with-mediatr/games

# ID指定取得
curl https://localhost:7212/api/with-mediatr/games/1
```

### レスポンス例

**成功時 (200 OK):**

```json
{
  "id": 1,
  "title": "The Legend of Zelda: Breath of the Wild",
  "genre": "Action",
  "releaseYear": 2017
}
```

**エラー時 (400 Bad Request):**

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation.General",
  "status": 400,
  "detail": "One or more validation errors occurred",
  "errors": [
    {
      "code": "NotEmptyValidator",
      "description": "'Title' must not be empty."
    }
  ]
}
```

---

## 🎨 主要な設計パターン

### 1. Result パターン

例外を使わずにエラーを表現するパターン。

```csharp
// 成功
Result<VideoGame> result = Result.Success(videoGame);

// 失敗
Result<VideoGame> result = Result.Failure<VideoGame>(
    VideoGameErrors.NotFound(id)
);

// パターンマッチング
return result.Match(
    onSuccess: game => Results.Ok(game),
    onFailure: error => CustomResults.Problem(error)
);
```

**メリット:**

- 例外のパフォーマンスコストを回避
- エラーハンドリングが型安全
- 関数型プログラミングのベストプラクティス

### 2. CQRS (Command Query Responsibility Segregation)

コマンド（書き込み）とクエリ（読み取り）を分離。

```csharp
// Command: データを変更する
public record CreateGameCommand(string Title, string Genre, int ReleaseYear)
    : ICommand<CreateGameResponse>;

// Query: データを取得する
public record GetGameByIdQuery(int Id)
    : IQuery<GetGameByIdResponse>;
```

### 3. Vertical Slice Architecture

1機能 = 1ファイルにすべてを集約。

```csharp
// CreateGame.cs にすべて含まれる
public static class CreateGame
{
    public record CreateGameCommand(...) : IRequest<Result<Response>>;
    public record CreateGameResponse(...);
    public class Validator : AbstractValidator<CreateGameCommand> { }
    public class Handler : IRequestHandler<CreateGameCommand, Result<Response>> { }
}
```

### 4. Decorator パターン (Without MediatR)

MediatR の Pipeline Behavior と同等の機能を Decorator で実現。

```csharp
// Scrutor による自動 Decorator 登録
services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
```

**実行順序:**

```txt
Endpoint
  → LoggingDecorator
    → ValidationDecorator
      → 実際の Handler
```

---

## 🔄 MediatR vs 自作実装の比較

| 観点 | MediatR 版 | 自作実装版 |
| - | - | - |
| **依存関係** | MediatR NuGet 必須 | Scrutor のみ |
| **パイプライン** | `IPipelineBehavior` | Decorator パターン |
| **DI 登録** | `AddMediatR()` で自動 | Scrutor で自動スキャン + Decorator |
| **エラーハンドリング** | `ValidationException` をスロー | `Result.Failure` を返す |
| **コード量** | 少ない（MediatR が抽象化） | やや多い（自前実装） |
| **学習コスト** | MediatR の理解が必要 | CQRS パターンのみ |
| **柔軟性** | MediatR の制約あり | 完全にカスタマイズ可能 |
| **パフォーマンス** | リフレクションのオーバーヘッド | やや高速（直接呼び出し） |

### 推奨する使い分け

- **MediatR を選ぶべき場合:**
  - チームが MediatR に慣れている
  - 迅速な開発を優先
  - 標準的なパターンを採用したい

- **自作実装を選ぶべき場合:**
  - 外部依存を最小化したい
  - カスタマイズの自由度が必要
  - パフォーマンスを重視

---

## 👨‍💻 開発ガイド

### 新機能の追加方法

#### ステップ1: Command/Query の定義

```csharp
// FeatureWithMediatR/Features/VideoGames/ArchiveGame.cs
public static class ArchiveGame
{
    public record ArchiveGameCommand(int Id) : IRequest<Result>;
}
```

#### ステップ2: Validator の実装

```csharp
public class Validator : AbstractValidator<ArchiveGameCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
```

#### ステップ3: Handler の実装

```csharp
public class Handler(ApplicationDbContext dbContext)
    : IRequestHandler<ArchiveGameCommand, Result>
{
    public async Task<Result> Handle(ArchiveGameCommand command, CancellationToken ct)
    {
        var game = await dbContext.VideoGames.FindAsync([command.Id], ct);
        if (game is null) return VideoGameErrors.NotFound(command.Id);
        
        game.IsArchived = true;
        await dbContext.SaveChangesAsync(ct);
        
        return Result.Success();
    }
}
```

#### ステップ4: Endpoint の追加

```csharp
// Web.Api/Endpoints/WithMediatR/VideoGames/ArchiveGameEndpoint.cs
public sealed class ArchiveGameEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapWithMediatRGamesApi()
            .MapPost("/{id:int}/archive", async (ISender sender, int id, CancellationToken ct) =>
            {
                var result = await sender.Send(new ArchiveGameCommand(id), ct);
                return result.ToNoContent();
            })
            .WithName("ArchiveGame")
            .Produces(StatusCodes.Status204NoContent);
    }
}
```

### 環境設定のカスタマイズ

#### appsettings.json (本番環境)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server;Database=GameDB;User Id=sa;Password=***;"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning"
    }
  }
}
```

---

## 🧪 テスト

### 単体テスト例

```csharp
public class CreateGameHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var handler = new CreateGame.Handler(dbContext);
        var command = new CreateGame.CreateGameCommand("Elden Ring", "RPG", 2022);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Elden Ring", result.Value.Title);
    }
}
```

### テスト実行

```bash
dotnet test
```

---

## 🚢 デプロイ

### Docker での実行

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Web.Api.dll"]
```

```bash
docker build -t video-game-api .
docker run -p 8080:80 video-game-api
```

### Azure App Service へのデプロイ

```bash
az webapp up --name my-video-game-api --runtime "DOTNET:10.0"
```

---

## 📚 参考資料

- [Vertical Slice Architecture - Jimmy Bogard](https://www.jimmybogard.com/vertical-slice-architecture/)
- [Result Pattern - Milan Jovanović](https://www.milanjovanovic.tech/blog/functional-error-handling-in-dotnet-with-the-result-pattern)
- [CQRS Pattern - Microsoft](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)

---

## 📄 ライセンス

このプロジェクトは [MIT ライセンス](LICENSE) の下でライセンスされています。

---

## 🙏 謝辞

このプロジェクトは以下の素晴らしいリソースとコミュニティに影響を受けています:

- [Jimmy Bogard](https://www.jimmybogard.com/) - Vertical Slice Architecture の提唱
- [Milan Jovanović](https://www.milanjovanovic.tech/) - Clean Architecture の教育的コンテンツ
- [Jason Taylor](https://github.com/jasontaylordev) - Clean Architecture テンプレート
