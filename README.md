# VideoGameApiVsa

ビデオゲーム管理用のRESTful API。  
垂直スライスアーキテクチャとCQRSパターンを採用したASP.NET Core 10.0アプリケーション。  

## 📋 目次

- [概要](#概要)
- [技術スタック](#技術スタック)
- [アーキテクチャ](#アーキテクチャ)
- [機能](#機能)
- [セットアップ](#セットアップ)
- [実行方法](#実行方法)
- [API エンドポイント](#api-エンドポイント)
- [テスト](#テスト)
- [プロジェクト構成](#プロジェクト構成)
- [開発ガイドライン](#開発ガイドライン)

## 概要

VideoGameApiVsaは、ビデオゲーム情報を管理するためのRESTful APIです。以下の特徴があります：

- **垂直スライスアーキテクチャ**: 機能ごとにファイルを集約し、保守性を向上
- **CQRSパターン**: MediatRを使用したCommand/Query分離
- **自動バリデーション**: FluentValidationによる入力検証
- **構造化ログ**: Serilogによる詳細なリクエスト/レスポンス記録
- **構造化されたエラーハンドリング**: RFC 7807準拠のProblemDetails形式
- **包括的なテスト**: xUnitとFluentAssertionsによるテストスイート

## 技術スタック

### フレームワーク・ライブラリ

- **.NET 10.0**: 最新の.NETプラットフォーム
- **ASP.NET Core**: Web APIフレームワーク
- **Carter 10.0.0**: Minimal APIの拡張ライブラリ
- **MediatR 14.0.0**: CQRSパターン実装
- **FluentValidation 12.1.1**: バリデーションライブラリ
- **Entity Framework Core 10.0.1**: ORM
- **Serilog 4.3.0**: 構造化ログライブラリ
- **Scalar.AspNetCore 2.11.10**: API ドキュメントUI

### データベース

- **Entity Framework Core InMemory**: 開発・テスト用インメモリデータベース

### テスト

- **xUnit 2.9.3**: テストフレームワーク
- **FluentAssertions 8.8.0**: アサーションライブラリ
- **Microsoft.AspNetCore.Mvc.Testing**: 統合テスト用

## アーキテクチャ

### 垂直スライスアーキテクチャ

各機能は1つのファイルに集約され、以下の要素を含みます：

```txt
Features/
  VideoGames/
    CreateGame.cs      # Request, Command, Validator, Handler, Endpoint
    GetAllGames.cs     # Query, Handler, Endpoint
    GetGameById.cs     # Query, Handler, Endpoint
    UpdateGame.cs      # Request, Command, Validator, Handler, Endpoint
    DeleteGame.cs      # Command, Handler, Endpoint
    VideoGameModule.cs # ルーティング定義
```

### 処理フロー

```txt
HTTP Request
    ↓
Endpoint (HTTP層)
    ↓
Command/Query (MediatR)
    ↓
LoggingBehavior (構造化ログ記録)
    ↓
ValidationBehavior (FluentValidation)
    ↓
Handler (ビジネスロジック)
    ↓
DbContext (データアクセス)
    ↓
Response (HTTP層)
```

### 設計原則

1. **関心の分離**: Request/Command/Responseを分離し、API契約と内部実装を独立
2. **単一責任**: 各クラスは1つの責任のみを持つ
3. **依存性逆転**: インターフェース経由で依存関係を管理
4. **テスト容易性**: 依存性注入により、テスト可能な設計

### MediatR Pipeline Behaviors

すべてのリクエストに対して自動的に実行される横断的関心事：

#### LoggingBehavior

- リクエスト/レスポンスの構造化ログ記録
- 実行時間の計測
- エラー発生時の詳細ログ
- リクエスト単位でのGUID追跡

```csharp
[14:23:45 INF] Handling CreateGameCommand [a3f2b1c8] {@Request}
[14:23:45 INF] Handled CreateGameCommand [a3f2b1c8] in 45ms {@Response}
```

#### ValidationBehavior

- FluentValidationの自動実行
- バリデーションエラーの統一的な処理
- ProblemDetails形式での返却

## 機能

### VideoGames API

ビデオゲーム情報のCRUD操作を提供：

- **GET /api/games**: 全ゲーム一覧取得
- **GET /api/games/{id}**: 特定ゲームの詳細取得
- **POST /api/games**: 新規ゲーム作成
- **PUT /api/games/{id}**: ゲーム情報更新
- **DELETE /api/games/{id}**: ゲーム削除

### データモデル

```csharp
public class VideoGame
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Genre { get; set; }
    public required int ReleaseYear { get; set; }
}
```

### バリデーションルール

- **Title**: 必須、最大100文字
- **Genre**: 必須、最大50文字
- **ReleaseYear**: 1950年から現在年まで

## セットアップ

### 前提条件

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2026 / Visual Studio Code / JetBrains Rider

### インストール手順

1. リポジトリをクローン

```bash
git clone <repository-url>
cd VideoGameApiVsa
```

2. 依存パッケージを復元

```bash
dotnet restore
```

3. プロジェクトをビルド

```bash
dotnet build
```

## 実行方法

### 開発環境での実行

```bash
cd VideoGameApiVsa
dotnet run
```

アプリケーションは以下のURLで起動します：

- **HTTP**: `http://localhost:5091`
- **HTTPS**: `https://localhost:7212`

### API ドキュメント

開発環境では、以下のURLでAPIドキュメントにアクセスできます：

- **Scalar UI**: `https://localhost:7212/scalar/v1`
- **OpenAPI JSON**: `https://localhost:7212/openapi/v1.json`

### ログ出力

アプリケーション実行時、以下の場所にログが出力されます：

- **コンソール**: 標準出力にリアルタイム表示
- **ファイル**: `logs/` ディレクトリ
  - 本番環境: `logs/app-YYYYMMDD.log`
  - 開発環境: `logs/dev-YYYYMMDD.log`

ログは日次でローテーションされ、開発環境では7日間保持されます。

## API エンドポイント

### VideoGames

#### 全ゲーム一覧取得

```http
GET /api/games
```

**レスポンス例:**

```json
[
  {
    "id": 1,
    "title": "The Legend of Zelda: Breath of the Wild",
    "genre": "Action",
    "releaseYear": 2017
  },
  {
    "id": 2,
    "title": "The Witcher 3: Wild Hunt",
    "genre": "RPG",
    "releaseYear": 2015
  }
]
```

#### ゲーム詳細取得

```http
GET /api/games/{id}
```

**レスポンス例:**

```json
{
  "id": 1,
  "title": "The Legend of Zelda: Breath of the Wild",
  "genre": "Action",
  "releaseYear": 2017
}
```

**エラー:**

- `404 Not Found`: 指定されたIDのゲームが存在しない場合

#### ゲーム作成

```http
POST /api/games
Content-Type: application/json

{
  "title": "New Game",
  "genre": "Action",
  "releaseYear": 2023
}
```

**レスポンス:**

- `201 Created`: 作成成功（Locationヘッダに作成されたリソースのURLを含む）
- `400 Bad Request`: バリデーションエラー

**バリデーションエラーレスポンス例:**

```json
{
  "type": "https://httpstatuses.com/400",
  "title": "Validation failed",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "instance": "/api/games",
  "errors": {
    "Title": ["'Title' must not be empty."],
    "ReleaseYear": ["'Release Year' must be between 1950 and 2026 (inclusive)."]
  }
}
```

#### ゲーム更新

```http
PUT /api/games/{id}
Content-Type: application/json

{
  "title": "Updated Game Title",
  "genre": "RPG",
  "releaseYear": 2024
}
```

**レスポンス:**

- `200 OK`: 更新成功
- `400 Bad Request`: バリデーションエラー
- `404 Not Found`: 指定されたIDのゲームが存在しない場合

#### ゲーム削除

```http
DELETE /api/games/{id}
```

**レスポンス:**

- `204 No Content`: 削除成功
- `404 Not Found`: 指定されたIDのゲームが存在しない場合

### WeatherForecast (サンプル機能)

```http
GET /api/weather-forecast
```

5日間の天気予報データを返します（サンプル機能）。

## テスト

### テストの実行

```bash
dotnet test
```

### テストカバレッジ

プロジェクトには以下のテストが含まれています：

#### 単体テスト（56件）

- **CreateGameTests** (14件)
  - 正常系: ゲーム作成成功
  - バリデーション: Title（空、null、境界値100文字、101文字）
  - バリデーション: Genre（空、null、境界値50文字、51文字）
  - バリデーション: ReleaseYear（1949年、1950年、現在年、未来年）
  - 統合: すべて有効な値
  
- **UpdateGameTests** (14件)
  - 正常系: ゲーム更新成功
  - エラー系: 存在しないゲーム
  - バリデーション: CreateGameと同様の境界値テスト
  
- **DeleteGameTests** (9件)
  - 正常系: ゲーム削除成功
  - エラー系: 存在しないゲーム、同じゲームの二重削除
  - エッジケース: ID=0、負のID、非常に大きなID
  - 複数データ: 特定ゲームのみ削除、データベース整合性確認
  
- **GetAllGamesTests** (3件)
  - 正常系: データ存在時の一覧取得
  - エッジケース: データ0件時の空リスト返却
  - パフォーマンス: 100件のデータ取得
  
- **GetGameByIdTests** (5件)
  - 正常系: ゲーム詳細取得成功
  - エラー系: 存在しないゲーム
  - エッジケース: ID=0、負のID
  - 複数データ: 正しいゲームのみ取得

#### 統合テスト（11件）

- **VideoGamesIntegrationTests** (11件)
  - HTTP統合: 作成（201）、バリデーション失敗（400）
  - HTTP統合: 一覧取得（200）、存在しないゲーム取得（404）
  - E2E: 作成→取得の流れ
  - E2E: 作成→更新→取得→削除の完全フロー
  - エラー処理: 存在しないゲームの更新（404）、無効データでの更新（400）
  - エラー処理: 存在しないゲームの削除（404）
  - 並行処理: 複数ゲームの同時作成

**合計: 62件のテスト**

### テストの特徴

- **InMemory Database**: 各テストで独立したデータベースを使用（`Guid.NewGuid().ToString()`）
- **FluentAssertions**: 読みやすいアサーション（`Should().Be()`, `Should().NotBeNull()`）
- **包括的なカバレッジ**: 正常系・異常系・境界値・エッジケースを網羅
- **統合テスト**: WebApplicationFactoryによる実際のHTTPリクエストシミュレーション

## プロジェクト構成

```txt
VideoGameApiVsa/
├── Behaviors/
│   ├── LoggingBehavior.cs         # Serilogによる構造化ログ記録
│   └── ValidationBehavior.cs      # FluentValidation自動実行
├── Data/
│   └── VideoGameDbContext.cs      # Entity Framework DbContext
├── Entities/
│   └── VideoGame.cs               # エンティティ定義
├── Extensions/                     # 拡張メソッド（DI設定整理）
│   ├── DatabaseExtensions.cs      # データベース初期化・シードデータ
│   ├── MiddlewareExtensions.cs    # ミドルウェアパイプライン設定
│   ├── SerilogExtensions.cs       # Serilog設定
│   └── ServiceExtensions.cs       # サービス登録
├── Features/
│   ├── VideoGames/                # ビデオゲーム機能
│   │   ├── CreateGame.cs
│   │   ├── DeleteGame.cs
│   │   ├── GetAllGames.cs
│   │   ├── GetGameById.cs
│   │   ├── UpdateGame.cs
│   │   ├── VideoGameConstants.cs # 定数定義（バリデーション、ルート名）
│   │   └── VideoGameModule.cs
│   └── WeatherForecast/           # サンプル機能
│       ├── GetWeatherForecast.cs
│       └── WeatherForecastModule.cs
├── Properties/
│   └── launchSettings.json
├── Program.cs                      # アプリケーションエントリーポイント
├── appsettings.json                # Serilog本番設定
├── appsettings.Development.json    # Serilog開発設定
└── VideoGameApiVsa.csproj

VideoGameApiVsa.Tests/
└── Features/
    └── VideoGames/                 # テストクラス
        ├── CreateGameTests.cs
        ├── DeleteGameTests.cs
        ├── GetAllGamesTests.cs
        ├── GetGameByIdTests.cs
        ├── UpdateGameTests.cs
        └── VideoGamesIntegrationTests.cs
```

## 開発ガイドライン

### 新機能の追加

1. **Featuresフォルダに新しいファイルを作成**

```csharp
public static class NewFeature
{
    // Request DTO
    public record NewFeatureRequest(...);
    
    // Command/Query
    public record NewFeatureCommand(...) : IRequest<NewFeatureResponse>;
    
    // Response DTO
    public record NewFeatureResponse(...);
    
    // Validator (必要に応じて)
    public class Validator : AbstractValidator<NewFeatureCommand> { }
    
    // Handler
    public class Handler(...) : IRequestHandler<NewFeatureCommand, NewFeatureResponse> { }
    
    // Endpoint
    public static async Task<IResult> Endpoint(...) { }
}
```

2. **Moduleにルートを追加**

```csharp
group.MapPost("/new-feature", NewFeature.Endpoint)
    .WithName("NewFeature")
    .WithDescription("Description")
    .Produces<NewFeature.NewFeatureResponse>(StatusCodes.Status200OK);
```

3. **テストを追加**

```csharp
public class NewFeatureTests
{
    [Fact]
    public async Task Handle_ShouldReturnExpectedResult_WhenValidInput()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<VideoGameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        // Act & Assert
    }
}
```

### コーディング規約

- **命名規則**: PascalCase（クラス、メソッド）、camelCase（ローカル変数）
- **XMLコメント**: すべての公開メンバーにXMLコメントを追加
- **null安全性**: nullable参照型を有効化し、適切にnullチェック
- **非同期処理**: I/O操作は必ずasync/awaitを使用

### エラーハンドリング

- **バリデーションエラー**: FluentValidationが自動的に検出し、400 Bad Requestを返却
- **未処理例外**: グローバル例外ハンドラでキャッチし、適切なHTTPステータスコードを返却
- **ProblemDetails**: RFC 7807準拠のエラーレスポンス形式

### ログ設定のカスタマイズ

`appsettings.json`または`appsettings.Development.json`でログレベルを調整：

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "VideoGameApiVsa.Behaviors.LoggingBehavior": "Debug"
      }
    }
  }
}
```

- **Debug**: 詳細なリクエスト/レスポンスを記録
- **Information**: 通常の動作ログ
- **Warning**: 警告レベル以上のみ記録

### データベースの変更

現在はInMemoryデータベースを使用していますが、以下のように変更可能：

**PostgreSQLへの変更例:**

```csharp
// VideoGameApiVsa/Extensions/ServiceExtensions.cs
services.AddDbContext<VideoGameDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
```

**SQL Serverへの変更例:**

```csharp
services.AddDbContext<VideoGameDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
```

## ライセンス

このプロジェクトはMITライセンスの下で公開されています。

## 参考資料

- [垂直スライスアーキテクチャ](https://www.jimmybogard.com/vertical-slice-architecture/)
- [MediatR公式ドキュメント](https://github.com/jbogard/MediatR)
- [FluentValidation公式ドキュメント](https://docs.fluentvalidation.net/)
- [Serilog公式ドキュメント](https://serilog.net/)
- [Carter公式ドキュメント](https://github.com/CarterCommunity/Carter)
- [Build a CRUD App with Vertical Slice Architecture in .NET 9](https://youtu.be/dnvi0B76ekg?si=nV0QWSmQTjlQeQ8H)
- [The Cleanest .NET Web API with Vertical Slice Architecture is here!](https://youtu.be/1jYh3j9bGxA?si=U7MWXHkqQ1Kf0b0N)
- [How to Protect Your .NET API with FluentValidation (The Right Way!))](https://youtu.be/u42B4azsNho?si=RGp-uFcBqc7FhqBj)  

- [YouTube_I Removed MediatR – Building Your Own CQRS Handlers in .NET](https://www.youtube.com/watch?v=j1OUToRyVHc)  
- [YouTube_Building Your Own CQRS Pipeline With Decorators (Dropped MediatR!)](https://www.youtube.com/watch?v=gsluG8NdCfw)
