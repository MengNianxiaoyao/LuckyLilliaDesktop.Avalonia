using LuckyLilliaDesktop.Utils;
using Xunit;

namespace LuckyLilliaDesktop.Tests;

public class LLBotProtocolTests
{
    [Theory]
    [InlineData("12345", LLBotProtocol.Linux, "qq-session-12345.json")]
    [InlineData("12345", LLBotProtocol.MacOS, "qq-session-12345-macos.json")]
    public void SessionFileName_MatchesLLBotNaming(string uin, string protocol, string expected)
    {
        Assert.Equal(expected, LLBotProtocol.SessionFileName(uin, protocol));
    }

    [Theory]
    [InlineData("qq-session-12345.json", "12345", LLBotProtocol.Linux)]
    [InlineData("qq-session-12345-macos.json", "12345", LLBotProtocol.MacOS)]
    public void TryParseSessionFileName_SupportedProtocol_ReturnsUinAndProtocol(
        string fileName,
        string expectedUin,
        string expectedProtocol)
    {
        Assert.True(LLBotProtocol.TryParseSessionFileName(fileName, out var uin, out var protocol));
        Assert.Equal(expectedUin, uin);
        Assert.Equal(expectedProtocol, protocol);
    }

    [Theory]
    // LLBot 支持但 Desktop 未开放的协议
    [InlineData("qq-session-12345-windows.json")]
    [InlineData("qq-session-12345-watch.json")]
    // LLBot 读 linux session 不带后缀, 带 -linux 的文件不会被读到
    [InlineData("qq-session-12345-linux.json")]
    [InlineData("qq-session-abc.json")]
    [InlineData("qq-session-.json")]
    [InlineData("qq-session-12345.json.bak")]
    [InlineData("config_12345.json")]
    public void TryParseSessionFileName_UnsupportedOrMalformed_ReturnsFalse(string fileName)
    {
        Assert.False(LLBotProtocol.TryParseSessionFileName(fileName, out _, out _));
    }

    [Theory]
    [InlineData("linux", LLBotProtocol.Linux)]
    [InlineData("LNX", LLBotProtocol.Linux)]
    [InlineData("macos", LLBotProtocol.MacOS)]
    [InlineData(" Mac ", LLBotProtocol.MacOS)]
    [InlineData("darwin", LLBotProtocol.MacOS)]
    [InlineData(null, LLBotProtocol.MacOS)]
    [InlineData("", LLBotProtocol.MacOS)]
    [InlineData("windows", LLBotProtocol.MacOS)]
    public void Normalize_MapsAliasesAndFallsBackToDefault(string? input, string expected)
    {
        Assert.Equal(expected, LLBotProtocol.Normalize(input));
    }
}
