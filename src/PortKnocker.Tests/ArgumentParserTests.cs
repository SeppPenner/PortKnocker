// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentParserTests.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="ArgumentParser" /> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PortKnocker.Tests;

/// <summary>
/// A class to test the <see cref="ArgumentParser"/> class.
/// </summary>
[TestClass]
public class ArgumentParserTests
{
    /// <summary>
    /// Checks whether a call without any argument leaves both port lists and the IP address list empty and keeps
    /// the documented default packet.
    /// </summary>
    [TestMethod]
    public void ParseWithoutArgumentsReturnsTheDefaults()
    {
        var knockOptions = ArgumentParser.Parse(new string[] { });

        Assert.AreEqual(0, knockOptions.IpAddresses.Count);
        Assert.AreEqual(0, knockOptions.UdpPorts.Count);
        Assert.AreEqual(0, knockOptions.TcpPorts.Count);
        Assert.IsFalse(knockOptions.ShouldShowHelp);
        CollectionAssert.AreEqual(new byte[] { 0x00, 0x01 }, knockOptions.PacketData);
    }

    /// <summary>
    /// Checks whether a single IP address is taken over.
    /// </summary>
    [TestMethod]
    public void ParseReadsASingleIpAddress()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-i", "192.168.2.1" });

        Assert.AreEqual(1, knockOptions.IpAddresses.Count);
        Assert.AreEqual(IPAddress.Parse("192.168.2.1"), knockOptions.IpAddresses[0]);
    }

    /// <summary>
    /// Checks whether the option can be repeated and whether the order is kept.
    /// </summary>
    [TestMethod]
    public void ParseReadsMultipleIpAddresses()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-i", "192.168.2.1", "-i", "192.168.2.2" });

        Assert.AreEqual(2, knockOptions.IpAddresses.Count);
        Assert.AreEqual(IPAddress.Parse("192.168.2.1"), knockOptions.IpAddresses[0]);
        Assert.AreEqual(IPAddress.Parse("192.168.2.2"), knockOptions.IpAddresses[1]);
    }

    /// <summary>
    /// Checks whether an option and its value written into one argument still work. Mono.Options hands the value
    /// over with the separating space, which is why every callback trims its value.
    /// </summary>
    [TestMethod]
    public void ParseTrimsTheValueOfACombinedArgument()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-i 192.168.2.1", "-u 22" });

        Assert.AreEqual(1, knockOptions.IpAddresses.Count);
        Assert.AreEqual(IPAddress.Parse("192.168.2.1"), knockOptions.IpAddresses[0]);
        Assert.AreEqual(1, knockOptions.UdpPorts.Count);
        Assert.AreEqual(22, knockOptions.UdpPorts[0]);
    }

    /// <summary>
    /// Checks whether an IP address that does not parse is dropped without an error, while the valid one next to
    /// it survives.
    /// </summary>
    [TestMethod]
    public void ParseSkipsAnInvalidIpAddress()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-i", "300.400.500.600", "-i", "192.168.2.1" });

        Assert.AreEqual(1, knockOptions.IpAddresses.Count);
        Assert.AreEqual(IPAddress.Parse("192.168.2.1"), knockOptions.IpAddresses[0]);
    }

    /// <summary>
    /// Checks whether an IPv6 address is accepted as well.
    /// </summary>
    [TestMethod]
    public void ParseReadsAnIpV6Address()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-i", "::1" });

        Assert.AreEqual(1, knockOptions.IpAddresses.Count);
        Assert.AreEqual(IPAddress.Parse("::1"), knockOptions.IpAddresses[0]);
    }

    /// <summary>
    /// Checks whether the UDP option can be repeated and does not touch the TCP ports.
    /// </summary>
    [TestMethod]
    public void ParseReadsMultipleUdpPorts()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-u", "22", "-u", "23", "-u", "26" });

        CollectionAssert.AreEqual(new[] { 22, 23, 26 }, knockOptions.UdpPorts);
        Assert.AreEqual(0, knockOptions.TcpPorts.Count);
    }

    /// <summary>
    /// Checks whether the TCP option can be repeated and does not touch the UDP ports.
    /// </summary>
    [TestMethod]
    public void ParseReadsMultipleTcpPorts()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-t", "22", "-t", "23", "-t", "26" });

        CollectionAssert.AreEqual(new[] { 22, 23, 26 }, knockOptions.TcpPorts);
        Assert.AreEqual(0, knockOptions.UdpPorts.Count);
    }

    /// <summary>
    /// Checks whether both ends of the allowed port range are accepted.
    /// </summary>
    [TestMethod]
    public void ParseAcceptsTheLowestAndTheHighestPort()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-u", "0", "-t", "65535" });

        CollectionAssert.AreEqual(new[] { 0 }, knockOptions.UdpPorts);
        CollectionAssert.AreEqual(new[] { 65535 }, knockOptions.TcpPorts);
    }

    /// <summary>
    /// Checks whether a port below or above the allowed range is dropped without an error.
    /// </summary>
    [TestMethod]
    public void ParseSkipsAPortOutsideTheAllowedRange()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-u", "-1", "-u", "65536", "-t", "65536" });

        Assert.AreEqual(0, knockOptions.UdpPorts.Count);
        Assert.AreEqual(0, knockOptions.TcpPorts.Count);
    }

    /// <summary>
    /// Checks whether a port that is no number is dropped without an error, while the valid one next to it
    /// survives.
    /// </summary>
    [TestMethod]
    public void ParseSkipsAPortThatIsNoNumber()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-u", "asdf", "-u", "22" });

        CollectionAssert.AreEqual(new[] { 22 }, knockOptions.UdpPorts);
    }

    /// <summary>
    /// Checks whether the packet option is taken over as UTF-8 bytes.
    /// </summary>
    [TestMethod]
    public void ParseConvertsThePacketOptionToUtf8Bytes()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-p", "test" });

        CollectionAssert.AreEqual(Encoding.UTF8.GetBytes("test"), knockOptions.PacketData);
    }

    /// <summary>
    /// Checks whether the binary option is read as a hexadecimal value.
    /// </summary>
    [TestMethod]
    public void ParseConvertsTheBinaryOptionFromHexadecimal()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-b", "0103" });

        CollectionAssert.AreEqual(new byte[] { 0x01, 0x03 }, knockOptions.PacketData);
    }

    /// <summary>
    /// Checks whether lower case hexadecimal digits are accepted.
    /// </summary>
    [TestMethod]
    public void ParseAcceptsLowerCaseHexadecimalDigits()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-b", "ff00" });

        CollectionAssert.AreEqual(new byte[] { 0xFF, 0x00 }, knockOptions.PacketData);
    }

    /// <summary>
    /// Checks whether an empty binary value results in an empty packet instead of an error.
    /// </summary>
    [TestMethod]
    public void ParseAcceptsAnEmptyBinaryPacket()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-b", string.Empty });

        Assert.AreEqual(0, knockOptions.PacketData.Length);
    }

    /// <summary>
    /// Checks whether the binary option overwrites the packet option when it comes after it.
    /// </summary>
    [TestMethod]
    public void ParseLetsTheBinaryOptionWinWhenItComesLast()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-p", "11", "-b", "0101" });

        CollectionAssert.AreEqual(new byte[] { 0x01, 0x01 }, knockOptions.PacketData);
    }

    /// <summary>
    /// Checks whether the packet option overwrites the binary option when it comes after it. The readme claims
    /// that the binary option always wins, but both options write the same property.
    /// </summary>
    [TestMethod]
    public void ParseLetsThePacketOptionWinWhenItComesLast()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-b", "0101", "-p", "11" });

        CollectionAssert.AreEqual(Encoding.UTF8.GetBytes("11"), knockOptions.PacketData);
    }

    /// <summary>
    /// Checks whether an odd number of digits is reported as an option error instead of ending in an unhandled
    /// exception.
    /// </summary>
    [TestMethod]
    public void ParseThrowsForAnOddNumberOfHexadecimalDigits()
    {
        var exception = Assert.ThrowsExactly<OptionException>(() => ArgumentParser.Parse(new[] { "-b", "112" }));

        StringAssert.Contains(exception.Message, "odd number of digits");
    }

    /// <summary>
    /// Checks whether a value that is no hexadecimal value is reported as an option error instead of ending in an
    /// unhandled exception.
    /// </summary>
    [TestMethod]
    public void ParseThrowsForANonHexadecimalPacket()
    {
        var exception = Assert.ThrowsExactly<OptionException>(() => ArgumentParser.Parse(new[] { "-b", "zz" }));

        StringAssert.Contains(exception.Message, "no valid hexadecimal value");
    }

    /// <summary>
    /// Checks whether Mono.Options reports a missing value itself, so that the caller can handle it like the
    /// errors of the hexadecimal conversion.
    /// </summary>
    [TestMethod]
    public void ParseThrowsIfAnOptionMissesItsValue()
    {
        Assert.ThrowsExactly<OptionException>(() => ArgumentParser.Parse(new[] { "-i" }));
    }

    /// <summary>
    /// Checks whether the help option sets the flag that makes the program print the manual.
    /// </summary>
    [TestMethod]
    public void ParseSetsTheHelpFlag()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "-h", "-i", "192.168.2.1" });

        Assert.IsTrue(knockOptions.ShouldShowHelp);
        Assert.AreEqual(1, knockOptions.IpAddresses.Count);
    }

    /// <summary>
    /// Checks whether the long option names work as well.
    /// </summary>
    [TestMethod]
    public void ParseAcceptsTheLongOptionNames()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "--ip=192.168.2.1", "--udp=22", "--tcp=23", "--packet=test" });

        Assert.AreEqual(IPAddress.Parse("192.168.2.1"), knockOptions.IpAddresses[0]);
        CollectionAssert.AreEqual(new[] { 22 }, knockOptions.UdpPorts);
        CollectionAssert.AreEqual(new[] { 23 }, knockOptions.TcpPorts);
        CollectionAssert.AreEqual(Encoding.UTF8.GetBytes("test"), knockOptions.PacketData);
    }

    /// <summary>
    /// Checks whether an option that does not exist is collected as an unprocessed argument by Mono.Options
    /// instead of stopping the run.
    /// </summary>
    [TestMethod]
    public void ParseIgnoresAnUnknownOption()
    {
        var knockOptions = ArgumentParser.Parse(new[] { "--asdf", "-u", "22" });

        CollectionAssert.AreEqual(new[] { 22 }, knockOptions.UdpPorts);
    }
}
