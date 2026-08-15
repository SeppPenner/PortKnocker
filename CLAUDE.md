# Project rules for Claude

## What this is

PortKnocker is a command line [port knocking](https://en.wikipedia.org/wiki/Port_knocking) client.
It sends one packet to every combination of the given IP addresses and the given UDP and TCP ports,
so that a knock daemon on the other side can open a firewall port. The repository is a standalone
tool, it is **not** published as a NuGet package and there is **no** installer: no
`GeneratePackageOnBuild`, no push script, no Inno Setup project. Users build the executable
themselves.

One solution `src/PortKnocker.sln` with exactly two projects:

- `src/PortKnocker/PortKnocker.csproj`, `OutputType` `WinExe`, the tool itself.
- `src/PortKnocker.Tests/PortKnocker.Tests.csproj`, `OutputType` `Exe`, a second console
  application that calls `PortKnocker.Program.Main` with hard coded arguments.

Layout inside `src/PortKnocker`:

- `Program.cs`: the whole tool. `Main` prints the ASCII art, builds a `Mono.Options.OptionSet` with
  the six options `-i`, `-u`, `-t`, `-p`, `-b` and `-h`, parses the arguments and then loops over
  the IP addresses, the UDP ports and the TCP ports and sends the packet. The private helpers
  `ConvertHexStringToByteArray`, `PrintUsage` and `PrintAsciiArt` do one thing each, keep new logic
  in that shape.
- `GlobalUsings.cs`: all usings of the project.

Layout inside `src/PortKnocker.Tests`:

- `Program.cs`: 22 methods named `TestPortKnocker*`, each one a call of `PortKnocker.Program.Main`
  with a different argument list. There is no test framework, no assertion and no result, the
  methods really send packets to `192.168.2.1`. `dotnet test` finds nothing here.
- `GlobalUsings.cs`: empty apart from the pragma block.

Repository root: `README.md` (the only user documentation, with the usage samples and the commands
manual), `Changelog.md`, `License.txt` (MIT), `.gitattributes` and `.gitignore`. The code style
lives in `src/.editorconfig`. There is no `Updating.md`, no `HowToUse.md` and no screenshots.

## Build

```powershell
dotnet build src/PortKnocker.sln
```

- Single target framework `net9.0` in both projects, no multi-targeting, no `RuntimeIdentifiers`.
  Nothing in the code is Windows specific.
- All build properties live directly in the two `.csproj` files and are duplicated there. There is
  **no** `Directory.Build.props` in this repository.
- `TreatWarningsAsErrors` is enabled in both projects, so every warning breaks the build, NuGet
  warnings (`NU****`) from restore included. A clean build reports zero warnings, keep it that way.
- `NU1803` (HTTP source usage during restore) is the one warning suppressed via `NoWarn`. Fix
  warnings instead of extending that list. `NuGetAudit` and `NuGetAuditMode=all` are on, so a
  vulnerable transitive package fails the build too.
- Versions come from GitVersion.MsBuild out of the git tags, for example `1.0.7-1` for the first
  commit after tag `1.0.6`. Never edit a version property or an assembly version by hand.
- Restore needs nuget.org. If a private feed is configured globally on the machine and answers 404
  for public packages, restore fails with `NU1301`. Then build with an explicit source:
  `dotnet build src/PortKnocker.sln --source https://api.nuget.org/v3/index.json`.
- There are no automated tests. A behaviour change is verified by running the tool, for example
  `dotnet run --project src/PortKnocker -- -h` for the manual and
  `dotnet run --project src/PortKnocker -- -i 127.0.0.1 -u 40000` for a knock that reports
  `Sent UDP packet '0001' to 127.0.0.1:40000.`. Never claim a run happened without running it.

## Code conventions

Follow the surrounding code, it is consistent throughout every file:

- File header comment block with `<copyright file="..." company="Hämmer Electronics">` containing
  the line `The project is licensed under the MIT license.` and a `<summary>`, then the file scoped
  namespace.
- XML doc comments on every type and every member, private members included, no exceptions.
- `Nullable`, `ImplicitUsings` and `LangVersion latest` are enabled.
- New `using` directives go into the `GlobalUsings.cs` of the respective project, inside the
  existing `#pragma warning disable IDE0065` block, never at the top of a file. The editorconfig
  requires usings inside the namespace (`csharp_using_directive_placement=inside_namespace:warning`),
  which global usings cannot satisfy, that is what the pragma is for. Do not add other pragmas. The
  comment text in that block is German because Visual Studio generated it, leave it alone.
- Fields, properties, methods and events are always accessed with `this.` qualification
  (`dotnet_style_qualification_for_*` at severity `warning`). The current code is static throughout,
  so there is nothing to qualify yet.
- `src/.editorconfig` also enforces braces everywhere, no multiple blank lines, four spaces, CRLF,
  UTF-8 without BOM, file scoped namespaces, `System` usings sorted first and `IDE0005` as warning.
  Analyzer warnings are fixed, not silenced.

## Known quirks

Do not silently "clean up" these, they are existing behaviour:

- **The leading space in the option values.** The sample calls pass a whole option and its value as
  one argument, `"-i 192.168.2.1"`, so Mono.Options hands the callback `" 192.168.2.1"` with a
  leading space. Every callback therefore starts with `Trim()`. Removing that breaks all the
  documented calls.
- **Invalid input is dropped without a word.** An IP address that does not parse, a port that is
  not a number and a port outside `0` to `65535` are simply not added to their list. The tool then
  knocks on whatever is left, or on nothing at all, and says nothing about it.
- **`-p` and `-b` write the same field.** Both callbacks assign `packetData`, so the option that
  comes **last** on the command line wins. The `Hints` section of the `README.md` claims that `-b`
  overwrites `-p`, which is only true for that order.
- **An invalid `-b` value crashes the tool.** `ConvertHexStringToByteArray` throws
  `ArgumentException` for an odd number of digits and `byte.Parse` throws `FormatException` for a
  non hexadecimal digit. Both are thrown inside the Mono.Options callback, and `Main` only catches
  `OptionException`, so `PortKnocker -i 127.0.0.1 -u 22 -b 112` ends in an unhandled exception with
  a stack trace. The `if (parsedPacketData != null)` check below the call is dead code, the method
  never returns null.
- **The error output glues message and stack trace together.** The `catch (OptionException)` block
  writes `$"{ex.Message}{ex.StackTrace}"` in one line, without a separator.
- **TCP knocking needs a full connection.** The TCP branch calls `Socket.Connect` before `Send`, so
  a knock on a closed port throws and is swallowed by the empty `catch`. Classic TCP knocking only
  needs the SYN packet, which a `Socket` of type `Stream` cannot send on its own. UDP knocking has
  no such problem, `SendTo` succeeds without a listener.
- **The sockets are never disposed.** Neither `socketUdp` nor `socketTcp` is wrapped in a `using`.
  The process is short lived, so the handles die with it.
- **`OutputType` is `WinExe` although the tool only writes to the console.** Started from Explorer
  it therefore shows nothing at all.
- **`Main` never sets an exit code.** Wrong arguments, a failed knock and a successful knock all
  end with `0`.
- **The manual exists twice.** `PrintUsage` in `Program.cs` and the `Commands` block of the
  `README.md` are copies of each other and have already drifted apart: the program prints
  `Hämmer Electronics`, the readme writes `Haemmer Electronics`. Change both or none.
- **The em dash in the program output.** `PortKnocker — I'm the one who knocks.` is part of the
  ASCII art banner and of the manual. It is user facing text of the tool, not documentation, and
  stays as it is.
- **AppVeyor badge without CI in the repository.** `README.md` links an AppVeyor build that is
  configured outside of this repository. There is no `.github` folder and no pipeline file here.
- **`src/PortKnocker.sln.DotSettings`** is tracked and holds nothing but a ReSharper user
  dictionary (`Haemmer`, `H_00E4mmer`, `packetbin`, `Sepp`). Leave it alone.
- **`.gitattributes` sets `* text=auto`** and every rule of the Visual Studio template below it is
  commented out. Any binary file added later needs its own rule.

## Releasing

1. Make the change.
2. Add an entry at the top of `Changelog.md` in the existing format:
   `* **Version 1.0.7.0 (2026-08-15)** : Short description.`
3. Commit that.
4. Tag the commit with the plain version number, no `v` prefix (`1.0.6`, `1.0.5`, ...). The existing
   tags are lightweight tags, create new ones the same way.
5. Push the commits and the tag.

The version in the `Changelog.md` has four parts (`1.0.7.0`), the tag has three (`1.0.7`).
GitVersion turns the tag into the assembly version, so an untagged commit produces something like
`1.0.7-1+Branch.master.Sha...`. There is no installer to build and no package to push, so the
release ends with the push.

## Git

- **Never amend a commit.** No `git commit --amend`, not for a typo in the message, not to add a
  forgotten file, not even when the commit is still local. Write a follow-up commit instead. The
  release versions come from tags on exact commits, an amended commit leaves its tag pointing at a
  commit that no longer exists in the branch.

## Writing style

- Commit messages are written **in English only**: short, precise subject line, explanatory body
  when needed.
- Code comments and comments in project files such as `.csproj` are **always English**, regardless
  of the language used in the conversation.
- **No em dashes or en dashes** (`—`, `–`), neither in prose, commit messages, code comments nor
  documentation. Use a regular hyphen, comma, colon, parentheses or a separate sentence. The one
  exception is the existing banner text of the tool, see the known quirks.
- German texts (documentation, chat replies) always use real umlauts and ß, never ASCII
  transliterations such as `ae`, `oe`, `ue` or `ss`. Identifiers, file names and configuration keys
  stay unchanged where umlauts are technically undesirable.
