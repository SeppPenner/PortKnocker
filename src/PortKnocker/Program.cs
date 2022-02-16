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

        var ipAddresses = new List<IPAddress>();
        var udpPorts = new List<int>();
        var tcpPorts = new List<int>();
        var shouldShowHelp = false;
        var packetData = new byte[] { 0x00, 0x01 };

        var options = new OptionSet
            {
                {
                    "i|ip=", "Specifies the IP address(es).", ip =>
                    {
                        var trimmedIpAddress = ip.Trim();
                        var parsed = IPAddress.TryParse(trimmedIpAddress, out var ipAddress);

                        if (parsed && ipAddress is not null && !ipAddress.Equals(default))
                        {
                            ipAddresses.Add(ipAddress);
                        }
                    }
                },
                {
                    "u|udp=", "Specifies the UDP connection port(s).", port =>
                    {
                        var trimmedPort = port.Trim();
                        var parsed = int.TryParse(trimmedPort, out var udpPort);

                        if (parsed && udpPort >= 0 && udpPort <= 65535)
                        {
                            udpPorts.Add(udpPort);
                        }
                    }
                },
                {
                    "t|tcp=", "Specifies the TCP connection port(s).", port =>
                    {
                        var trimmedPort = port.Trim();
                        var parsed = int.TryParse(trimmedPort, out var tcpPort);

                        if (parsed && tcpPort >= 0 && tcpPort <= 65535)
                        {
                            tcpPorts.Add(tcpPort);
                        }
                    }
                },
                {
                    "p|packet=", "Specifies the packet.", packet =>
                    {
                        var trimmedPacket = packet.Trim();
                        packetData = Encoding.UTF8.GetBytes(trimmedPacket);
                    }
                },
                {
                    "b|binary=", "Specifies the packet.", packet =>
                    {
                        var trimmedPacket = packet.Trim();
                        var parsedPacketData = ConvertHexStringToByteArray(trimmedPacket);

                        if (parsedPacketData != null)
                        {
                            packetData = parsedPacketData;
                        }
                    }
                },
                {
                    "h|help", "Shows the help message.", h =>
                    {
                        shouldShowHelp = h != null;
                    }
                }
            };

        try
        {
            options.Parse(args);
        }
        catch (OptionException ex)
        {
            Console.WriteLine($"{ex.Message}{ex.StackTrace}");
            Console.WriteLine("Try `-h' for more information.");
            return;
        }

        if (shouldShowHelp)
        {
            PrintUsage();
            return;
        }

        var dataSent = false;

        foreach (var ipAddress in ipAddresses)
        {
            foreach (var udpPort in udpPorts)
            {
                try
                {
                    var socketUdp = new Socket(ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                    socketUdp.SendTo(packetData, new IPEndPoint(ipAddress, udpPort));
                    Console.WriteLine($"Sent UDP packet '{BitConverter.ToString(packetData).Replace("-", string.Empty)}' to {ipAddress}:{udpPort}.");
                    dataSent = true;
                }
                catch
                {
                    // ignored
                }
            }

            foreach (var tcpPort in tcpPorts)
            {
                try
                {
                    var socketTcp = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    socketTcp.Connect(new IPEndPoint(ipAddress, tcpPort));
                    socketTcp.Send(packetData);
                    socketTcp.Shutdown(SocketShutdown.Both);
                    socketTcp.Close();
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
    /// Converts a hexadecimal <see cref="string"/> to a <see cref="T:byte[]"/>.
    /// </summary>
    /// <param name="hexString">The hexadecimal <see cref="string"/>.</param>
    /// <returns>The <see cref="T:byte[]"/>.</returns>
    private static byte[] ConvertHexStringToByteArray(string hexString)
    {
        if (hexString.Length % 2 != 0)
        {
            throw new ArgumentException($"The binary string cannot have an odd number of digits: {hexString}.");
        }

        var data = new byte[hexString.Length / 2];
        for (var index = 0; index < data.Length; index++)
        {
            var byteValue = hexString.Substring(index * 2, 2);
            data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        return data;
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
