// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The program class to test the port knocker.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PortKnocker.Tests;

/// <summary>
/// The program class to test the port knocker.
/// </summary>
public static class Program
{
    /// <summary>
    /// The main method.
    /// </summary>
    public static void Main()
    {
        // Test basics:
        TestPortKnockerSimple();
        TestPortKnockerHelp();
        TestPortKnockerHelpWithOtherOption();
        TestPortKnockerSingleIpAddress();
        TestPortKnockerMultipleIpAddresses();

        // Test UDP:
        TestPortKnockerSingleUdpPort();
        TestPortKnockerMultipleUdpPorts();
        TestPortKnockerSingleUdpPortWithSetPackage();
        TestPortKnockerMultipleUdpPortsWithSetPackage();
        TestPortKnockerSingleUdpPortWithSetBinaryPackage();
        TestPortKnockerMultipleUdpPortsWithSetBinaryPackage();
        TestPortKnockerMultipleUdpPortsWithSetBothPackages();

        // Test TCP:
        TestPortKnockerSingleTcpPort();
        TestPortKnockerMultipleTcpPorts();
        TestPortKnockerSingleTcpPortWithSetPackage();
        TestPortKnockerMultipleTcpPortsWithSetPackage();
        TestPortKnockerSingleTcpPortWithSetBinaryPackage();
        TestPortKnockerMultipleTcpPortsWithSetBinaryPackage();
        TestPortKnockerMultipleTcpPortsWithSetBothPackages();

        // Test mixed:
        TestPortKnockerMultiplePortsMixed();
        TestPortKnockerMultiplePortsMixedWithSetPackage();
        TestPortKnockerMultiplePortsMixedWithSetBinaryPackage();
    }

    /// <summary>
    /// A simple test method to test PortKnocker without parameters.
    /// </summary>
    private static void TestPortKnockerSimple()
    {
        PortKnocker.Program.Main(new string[] { });
    }

    /// <summary>
    /// A simple test method to test PortKnocker's help.
    /// </summary>
    private static void TestPortKnockerHelp()
    {
        PortKnocker.Program.Main(new[] { "-h" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker's help with another option set.
    /// </summary>
    private static void TestPortKnockerHelpWithOtherOption()
    {
        PortKnocker.Program.Main(new[] { "-h", "-i 192.168.2.1" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with a single IP address.
    /// </summary>
    private static void TestPortKnockerSingleIpAddress()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with multiple IP addresses.
    /// </summary>
    private static void TestPortKnockerMultipleIpAddresses()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-i 192.168.2.2" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with one UDP port.
    /// </summary>
    private static void TestPortKnockerSingleUdpPort()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-u 22" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with multiple UDP ports.
    /// </summary>
    private static void TestPortKnockerMultipleUdpPorts()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-u 22", "-u 23", "-u 26" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with one UDP port with set package.
    /// </summary>
    private static void TestPortKnockerSingleUdpPortWithSetPackage()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-u 22", "-p 11" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with multiple UDP ports with set package.
    /// </summary>
    private static void TestPortKnockerMultipleUdpPortsWithSetPackage()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-u 22", "-u 23", "-u 26", "-p 11" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with one UDP port with set binary package.
    /// </summary>
    private static void TestPortKnockerSingleUdpPortWithSetBinaryPackage()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-u 22", "-b 1122" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with multiple UDP ports with set binary package.
    /// </summary>
    private static void TestPortKnockerMultipleUdpPortsWithSetBinaryPackage()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-u 22", "-u 23", "-u 26", "-b 1122" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with multiple UDP ports with set package and binary package.
    /// </summary>
    private static void TestPortKnockerMultipleUdpPortsWithSetBothPackages()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-u 22", "-u 23", "-u 26", "-p 11", "-b 0101" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with one TCP port.
    /// </summary>
    private static void TestPortKnockerSingleTcpPort()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-t 22" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with multiple TCP ports.
    /// </summary>
    private static void TestPortKnockerMultipleTcpPorts()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-t 22", "-t 23", "-t 26" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with one TCP port with set package.
    /// </summary>
    private static void TestPortKnockerSingleTcpPortWithSetPackage()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-t 22", "-p 11" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with multiple TCP ports with set package.
    /// </summary>
    private static void TestPortKnockerMultipleTcpPortsWithSetPackage()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-t 22", "-t 23", "-t 26", "-p 11" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with one TCP port with set binary package.
    /// </summary>
    private static void TestPortKnockerSingleTcpPortWithSetBinaryPackage()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-t 22", "-b 1122" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with multiple TCP ports with set binary package.
    /// </summary>
    private static void TestPortKnockerMultipleTcpPortsWithSetBinaryPackage()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-t 22", "-t 23", "-t 26", "-b 1122" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with multiple TCP ports with set package and binary package.
    /// </summary>
    private static void TestPortKnockerMultipleTcpPortsWithSetBothPackages()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-t 22", "-t 23", "-t 26", "-p 11", "-b 0101" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with multiple ports mixed with UDP and TCP.
    /// </summary>
    private static void TestPortKnockerMultiplePortsMixed()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-u 22", "-u 23", "-t 24", "-t 26" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with multiple ports mixed with UDP and TCP with set package.
    /// </summary>
    private static void TestPortKnockerMultiplePortsMixedWithSetPackage()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-u 22", "-u 23", "-t 24", "-t 26", "-p 1122" });
    }

    /// <summary>
    /// A simple test method to test PortKnocker with multiple ports mixed with UDP and TCP with set binary package.
    /// </summary>
    private static void TestPortKnockerMultiplePortsMixedWithSetBinaryPackage()
    {
        PortKnocker.Program.Main(new[] { "-i 192.168.2.1", "-u 22", "-u 23", "-t 24", "-t 26", "-b 0103" });
    }
}
