# SCD-WP7

* A decompile of the Sonic CD Windows 7 port & RSDKv3 C# port.
* You NEED the Content folder from the original game to run this decompilation.

# Tools used

I used dnSpy to export the VSProj and VS 2022 to compile.

# Known issues

PC port has no music, only sounds

## How to build

Install the following dependencies:
- [.NET Framework 4.5.2](https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net452-developer-pack-offline-installer)
- [Microsoft XNA Framework Redistributable 4.0](https://www.microsoft.com/en-us/download/details.aspx?id=20914)

Then clone the project, open the Visual Studio solution file, and you should be good to go!

## How to run

The only thing left to do after building is to copy the Content folder from a release of the game to the build folder (bin/x86/(Debug|Release)).