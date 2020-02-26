PortKnocker
====================================

PortKnocker is a project to run a simple [port knocking](https://en.wikipedia.org/wiki/Port_knocking) client for UDP and TCP connections. The project was written and tested in .NetCore 3.1.

[![Build status](https://ci.appveyor.com/api/projects/status/02q1t1mf09sk95n3?svg=true)](https://ci.appveyor.com/project/SeppPenner/portknocker)
[![GitHub issues](https://img.shields.io/github/issues/SeppPenner/PortKnocker.svg)](https://github.com/SeppPenner/PortKnocker/issues)
[![GitHub forks](https://img.shields.io/github/forks/SeppPenner/PortKnocker.svg)](https://github.com/SeppPenner/PortKnocker/network)
[![GitHub stars](https://img.shields.io/github/stars/SeppPenner/PortKnocker.svg)](https://github.com/SeppPenner/PortKnocker/stargazers)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://raw.githubusercontent.com/SeppPenner/PortKnocker/master/License.txt)
[![Known Vulnerabilities](https://snyk.io/test/github/SeppPenner/PortKnocker/badge.svg)](https://snyk.io/test/github/SeppPenner/PortKnocker)

## Basic usage
```cmd
PortKnocker -i 192.168.0.1 -u 23                    Knocks with UDP on 192.168.0.1:23.
PortKnocker -i 192.168.0.1 -u 25 -u 26              Knocks with UDP on 192.168.0.1:25 and 192.168.0.1:26.
PortKnocker -i 192.168.0.1 -u 38 -p test            Knocks with UDP on 192.168.0.1:38 and sends the string 'test'.
PortKnocker -i 192.168.0.1 -u 40 -b 0103            Knocks with UDP on 192.168.0.1:40 and sends the hexadecimal value '0x01 0x03'.
PortKnocker -i 192.168.0.1 -i 192.168.0.2 -u 23     Knocks with UDP on 192.168.0.1:23 and 192.168.0.2:23.
PortKnocker -i 192.168.0.1 -t 24                    Knocks with TCP on 192.168.0.1:24.
PortKnocker -i 192.168.0.1 -t 32 -t 17              Knocks with TCP on 192.168.0.1:32 and 192.168.0.1:17.
PortKnocker -i 192.168.0.1 -t 11 -p test            Knocks with TCP on 192.168.0.1:11 and sends the string 'test'.
PortKnocker -i 192.168.0.1 -t 42 -b 0103            Knocks with TCP on 192.168.0.1:42 and sends the hexadecimal value '0x01 0x03'.
PortKnocker -i 192.168.0.1 -i 192.168.0.2 -t 25     Knocks with TCP on 192.168.0.1:25 and 192.168.0.2:25.
```

Checkout the examples in the [PortKnockerTest](https://github.com/SeppPenner/PortKnocker/tree/master/PortKnockerTest) project as well.

## Hints
The `-b` option will overwrite the `-p` option if both are specified.

## Commands

```html
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

Haemmer Electronics               Feb 26, 2020              Haemmer Electronics
```

## Further links
* https://github.com/sebastienwarin/Knock
* https://en.wikipedia.org/wiki/Port_knocking
* https://github.com/xamarin/XamarinComponents/tree/master/XPlat/Mono.Options

Change history
--------------

* **Version 1.0.0.0 (2020-02-26)** : 1.0 release.
