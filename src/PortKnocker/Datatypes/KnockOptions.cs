// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnockOptions.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The class that carries the parsed command line options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PortKnocker.Datatypes;

/// <summary>
/// The class that carries the parsed command line options.
/// </summary>
public sealed class KnockOptions
{
    /// <summary>
    /// Gets the IP addresses to knock on. Values that are no valid IP address are not added.
    /// </summary>
    public List<IPAddress> IpAddresses { get; } = new();

    /// <summary>
    /// Gets the UDP ports to knock on. Values that are no number or outside 0 to 65535 are not added.
    /// </summary>
    public List<int> UdpPorts { get; } = new();

    /// <summary>
    /// Gets the TCP ports to knock on. Values that are no number or outside 0 to 65535 are not added.
    /// </summary>
    public List<int> TcpPorts { get; } = new();

    /// <summary>
    /// Gets or sets the packet that is sent. Both the packet option and the binary packet option write this
    /// property, so the one that comes last on the command line wins.
    /// </summary>
    public byte[] PacketData { get; set; } = new byte[] { 0x00, 0x01 };

    /// <summary>
    /// Gets or sets a value indicating whether the manual is printed instead of knocking.
    /// </summary>
    public bool ShouldShowHelp { get; set; }
}
