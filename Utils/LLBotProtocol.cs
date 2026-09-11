using System;
using System.Text.RegularExpressions;

namespace LuckyLilliaDesktop.Utils;

/// <summary>
/// 无头模式 LLBot 直连协议 (--protocol). 取值用 LLBot getProtocol() 的规范 id;
/// session 文件命名对齐 LuckyLillia.Bot src/main/qqProtocol/direct-lib/session.ts 的 getSessionFilePathForUin.
/// </summary>
public static partial class LLBotProtocol
{
    public const string Linux = "linux";
    public const string MacOS = "macos";
    public const string Default = Linux;

    // LLBot 另支持 windows / watch, Desktop 未开放, 这两种 session 也不列进登录框
    public static readonly string[] Supported = [Linux, MacOS];

    // 兼容手改配置写成的别名 (与 LLBot 一致: mac / darwin / lnx), 其余回落默认
    public static string Normalize(string? protocol) => protocol?.Trim().ToLowerInvariant() switch
    {
        Linux or "lnx" => Linux,
        MacOS or "mac" or "darwin" => MacOS,
        _ => Default
    };

    public static string DisplayName(string protocol) => protocol switch
    {
        Linux => "Linux",
        MacOS => "macOS",
        _ => protocol
    };

    // Linux 不带后缀 (LLBot 兼容老 session), 其余协议带 -<protocol> 后缀
    public static string SessionFileName(string uin, string protocol) =>
        protocol == Linux ? $"qq-session-{uin}.json" : $"qq-session-{uin}-{protocol}.json";

    // 往返校验: 只认 LLBot 真会按 (uin, protocol) 读取的文件名, 如 qq-session-1-linux.json 不算
    public static bool TryParseSessionFileName(string fileName, out string uin, out string protocol)
    {
        var match = SessionFileRegex().Match(fileName);
        uin = match.Groups[1].Value;
        protocol = match.Groups[2].Success ? match.Groups[2].Value : Linux;
        return match.Success
               && Array.IndexOf(Supported, protocol) >= 0
               && SessionFileName(uin, protocol) == fileName;
    }

    [GeneratedRegex(@"^qq-session-([0-9]+)(?:-([a-z]+))?\.json$")]
    private static partial Regex SessionFileRegex();
}
