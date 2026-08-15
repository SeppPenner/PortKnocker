// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentParser.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The class that turns the command line arguments into the knock options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PortKnocker;

/// <summary>
/// The class that turns the command line arguments into the knock options.
/// </summary>
public static class ArgumentParser
{
    /// <summary>
    /// The lowest port number that is accepted.
    /// </summary>
    private const int MinimumPort = 0;

    /// <summary>
    /// The highest port number that is accepted.
    /// </summary>
    private const int MaximumPort = 65535;

    /// <summary>
    /// Parses the given command line arguments.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <exception cref="OptionException">
    /// Thrown if an option misses its value or if the binary packet is no valid hexadecimal value.
    /// </exception>
    /// <returns>The parsed <see cref="KnockOptions"/>.</returns>
    public static KnockOptions Parse(string[] args)
    {
        var knockOptions = new KnockOptions();
        BuildOptionSet(knockOptions).Parse(args);
        return knockOptions;
    }

    /// <summary>
    /// Builds the <see cref="OptionSet"/> that fills the given <see cref="KnockOptions"/>.
    /// </summary>
    /// <param name="knockOptions">The <see cref="KnockOptions"/> to fill.</param>
    /// <returns>The <see cref="OptionSet"/>.</returns>
    private static OptionSet BuildOptionSet(KnockOptions knockOptions)
    {
        return new OptionSet
            {
                {
                    "i|ip=", "Specifies the IP address(es).", ip =>
                    {
                        var trimmedIpAddress = ip.Trim();

                        if (IPAddress.TryParse(trimmedIpAddress, out var ipAddress))
                        {
                            knockOptions.IpAddresses.Add(ipAddress);
                        }
                    }
                },
                {
                    "u|udp=", "Specifies the UDP connection port(s).", port =>
                    {
                        var trimmedPort = port.Trim();
                        var parsed = int.TryParse(trimmedPort, out var udpPort);

                        if (parsed && udpPort >= MinimumPort && udpPort <= MaximumPort)
                        {
                            knockOptions.UdpPorts.Add(udpPort);
                        }
                    }
                },
                {
                    "t|tcp=", "Specifies the TCP connection port(s).", port =>
                    {
                        var trimmedPort = port.Trim();
                        var parsed = int.TryParse(trimmedPort, out var tcpPort);

                        if (parsed && tcpPort >= MinimumPort && tcpPort <= MaximumPort)
                        {
                            knockOptions.TcpPorts.Add(tcpPort);
                        }
                    }
                },
                {
                    "p|packet=", "Specifies the packet.", packet =>
                    {
                        var trimmedPacket = packet.Trim();
                        knockOptions.PacketData = Encoding.UTF8.GetBytes(trimmedPacket);
                    }
                },
                {
                    "b|binary=", "Specifies the packet.", packet =>
                    {
                        var trimmedPacket = packet.Trim();
                        knockOptions.PacketData = ConvertHexStringToByteArray(trimmedPacket);
                    }
                },
                {
                    "h|help", "Shows the help message.", h =>
                    {
                        knockOptions.ShouldShowHelp = h is not null;
                    }
                }
            };
    }

    /// <summary>
    /// Converts a hexadecimal <see cref="string"/> to a <see cref="T:byte[]"/>.
    /// </summary>
    /// <param name="hexString">The hexadecimal <see cref="string"/>.</param>
    /// <exception cref="OptionException">
    /// Thrown if the digit count is odd or if a digit is no hexadecimal digit. Mono.Options lets that exception
    /// pass through its own parse loop, so the caller can report it like any other option error.
    /// </exception>
    /// <returns>The <see cref="T:byte[]"/>.</returns>
    private static byte[] ConvertHexStringToByteArray(string hexString)
    {
        if (hexString.Length % 2 != 0)
        {
            throw new OptionException($"The binary packet cannot have an odd number of digits: {hexString}.", "binary");
        }

        var data = new byte[hexString.Length / 2];

        for (var index = 0; index < data.Length; index++)
        {
            var byteValue = hexString.Substring(index * 2, 2);
            var parsed = byte.TryParse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var parsedByte);

            if (!parsed)
            {
                throw new OptionException($"The binary packet is no valid hexadecimal value: {hexString}.", "binary");
            }

            data[index] = parsedByte;
        }

        return data;
    }
}
