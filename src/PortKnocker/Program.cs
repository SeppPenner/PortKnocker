// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The program class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PortKnocker;

/// <summary>
/// The main class.
/// </summary>
public static class Program
{
    /// <summary>
    /// The main method.
    /// </summary>
    /// <param name="args">The parameters.</param>
    public static void Main(string[] args)
    {
        PrintAsciiArt();

        KnockOptions knockOptions;

        try
        {
            knockOptions = ArgumentParser.Parse(args);
        }
        catch (OptionException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Try `-h' for more information.");
            return;
        }

        if (knockOptions.ShouldShowHelp)
        {
            PrintUsage();
            return;
        }

        Knock(knockOptions);
    }

    /// <summary>
    /// Sends the packet to every given port of every given IP address.
    /// </summary>
    /// <param name="knockOptions">The parsed <see cref="KnockOptions"/>.</param>
    private static void Knock(KnockOptions knockOptions)
    {
        var dataSent = false;

        foreach (var ipAddress in knockOptions.IpAddresses)
        {
            foreach (var udpPort in knockOptions.UdpPorts)
            {
                try
                {
                    using var socketUdp = new Socket(ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                    socketUdp.SendTo(knockOptions.PacketData, new IPEndPoint(ipAddress, udpPort));
                    Console.WriteLine($"Sent UDP packet '{BitConverter.ToString(knockOptions.PacketData).Replace("-", string.Empty)}' to {ipAddress}:{udpPort}.");
                    dataSent = true;
                }
                catch
                {
                    // ignored
                }
            }

            foreach (var tcpPort in knockOptions.TcpPorts)
            {
                try
                {
                    using var socketTcp = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    socketTcp.Connect(new IPEndPoint(ipAddress, tcpPort));
                    socketTcp.Send(knockOptions.PacketData);
                    socketTcp.Shutdown(SocketShutdown.Both);
                    Console.WriteLine($"Sent TCP packet to {ipAddress}:{tcpPort}.");
                    dataSent = true;
                }
                catch
                {
                    // ignored
                }
            }
        }

        if (dataSent)
        {
            Console.WriteLine("I've knocked for you.");
        }
    }

    /// <summary>
    /// Prints out the usage documentation.
    /// </summary>
    private static void PrintUsage()
    {
        Console.WriteLine(@"
PortKnocker                   Commands manual                   PortKnocker

NAME
     PortKnocker — I'm the one who knocks.

SYNOPSIS
     PortKnocker { -i ipAddress { -u udpPort [-p packet | -b binary] | -t tcpPort [-p packet | -b binary]
                 | -u udpPort -t tcpPort [-p packet | -b binary] } | -h }

DESCRIPTION
     PortKnocker is a program to run a simple port knocking client for UDP and TCP connections.

     The options are as follows:

     -i || -ip          Specifies the IP address(es) to knock on.

     -u || -udp         Specifies the UDP port(s).

     -t || -tcp         Specifies the TCP port(s).

     -p || -packet      Specifies the packet.

     -b || -binary      Specifies the packet (in binary form).

     -h || -help        Shows the help.

DEFAULT VALUES
     Packet         0x00 0x01
SEE ALSO
     Port knocking, https://en.wikipedia.org/wiki/Port_knocking.
AUTHORS
     PortKnocker is a free software provided under the MIT license. It is written by SeppPenner.

Hämmer Electronics               Feb 26, 2020              Hämmer Electronics
            ");
    }

    /// <summary>
    /// Prints out some ASCII art.
    /// </summary>
    private static void PrintAsciiArt()
    {
        Console.WriteLine(@"
$$$$$$$\                       $$\     $$\   $$\                               $$\                           
$$  __$$\                      $$ |    $$ | $$  |                              $$ |                          
$$ |  $$ | $$$$$$\   $$$$$$\ $$$$$$\   $$ |$$  / $$$$$$$\   $$$$$$\   $$$$$$$\ $$ |  $$\  $$$$$$\   $$$$$$\  
$$$$$$$  |$$  __$$\ $$  __$$\\_$$  _|  $$$$$  /  $$  __$$\ $$  __$$\ $$  _____|$$ | $$  |$$  __$$\ $$  __$$\ 
$$  ____/ $$ /  $$ |$$ |  \__| $$ |    $$  $$<   $$ |  $$ |$$ /  $$ |$$ /      $$$$$$  / $$$$$$$$ |$$ |  \__|
$$ |      $$ |  $$ |$$ |       $$ |$$\ $$ |\$$\  $$ |  $$ |$$ |  $$ |$$ |      $$  _$$<  $$   ____|$$ |      
$$ |      \$$$$$$  |$$ |       \$$$$  |$$ | \$$\ $$ |  $$ |\$$$$$$  |\$$$$$$$\ $$ | \$$\ \$$$$$$$\ $$ |      
\__|       \______/ \__|        \____/ \__|  \__|\__|  \__| \______/  \_______|\__|  \__| \_______|\__|              

PortKnocker — I'm the one who knocks.");
    }
}
