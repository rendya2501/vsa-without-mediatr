namespace FeatureShared.Messaging;

/// <summary>
/// クエリインターフェース
/// </summary>
/// <typeparam name="TResponse">レスポンスの型</typeparam>
/// <remarks>
/// <para>
/// クエリは、システムの状態を取得するためのクエリです。
/// </para>
/// </remarks>
public interface IQuery<TResponse>;
