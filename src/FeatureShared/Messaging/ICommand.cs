namespace FeatureShared.Messaging;

/// <summary>
/// コマンドインターフェース
/// </summary>
/// <remarks>
/// <para>
/// コマンドは、システムの状態を変更するための命令です。
/// </para>
/// </remarks>
public interface ICommand;

/// <summary>
/// コマンドインターフェース（値あり）
/// </summary>
/// <typeparam name="TResponse">レスポンスの型</typeparam>
/// <remarks>
/// <para>
/// コマンドは、システムの状態を変更するための命令です。
/// </para>
/// </remarks>
public interface ICommand<TResponse>;
