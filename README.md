# A Guide to Security Game Testing: Finding Exploits in Video Games

## Introduction
In this guide, I’ll walk you through how I create tools to find exploits in video games for bug bounty programs. Specifically, I’ll focus on my research into the game *Sword of Convallaria*. This exploration is purely for educational purposes. As such, I have removed the game URLs and the .protos.

## Game Details
*Sword of Convallaria* is available on both PC and mobile platforms, currently boasting around 2,000 concurrent active users on Steam (source: [SteamDB](https://steamdb.info/app/2526380/charts/)). The game monetizes through pay-to-win microtransactions and features PvP gameplay. Given its size, the developers should be concerned about potential exploits, yet they lack a formal bug bounty program.

The game is built on Unity and uses Lua for much of its game logic and processing. The network protocol employs HTTPS for the authentication/login flow and utilizes UDP packets with protobuf messages for in-game communication.

## High-Level Plan for Reverse Engineering
1. **Extract raw files** to outline the packet structures and translating IDs into English strings.
2. **Analyze the login authentication flow** and lobby server.
3. **Investigate game traffic** and server interactions.

## Acquiring and Dumping Game Data
Since *Sword of Convallaria* is developed in Unity, I used existing tools to extract game data, specifically [AssetsTools.NET](https://github.com/nesrak1/AssetsTools.NET) is helpful for this process.

While extracting the files isn’t the most exciting part and is well-documented, it did reveal some intriguing Lua and protobuf files. These files contain everything needed to create network tools.
Here is how I dump all of those relevant files
https://github.com/shalzuth/SwordOfConvallariaResearch/blob/main/Dumper/Utils/Unity.cs#L24-L41

Let's dig into them. 

## Converting Lua Bytecode to Human-Readable Scripts
The Lua bytecode appears encrypted based on the entropy of the data. To analyze it, I hooked the `slua.dll` function responsible for loading Lua code. This allowed me to examine the loaded bytecode and, crucially, dump a stack trace to identify the encryption method. I discovered it uses a straightforward XOR cipher where the first byte is excluded from the rotation and is XOR'd separately.
https://github.com/shalzuth/SwordOfConvallariaResearch/blob/main/Dumper/Utils/Lua.cs#L10-L13

With the Lua payload decrypted, I used a Lua decompiler, specifically [UnluacNET](https://github.com/Fireboyd78/UnluacNET). Initially, the decompilation failed due to an incorrect magic number. I verified the expected magic number in `slua.dll`.

After addressing the magic check, I encountered further failures. A binary diff between my built `slua.dll` and the game's version revealed differences in the read functions, which led me to identify another layer of encryption.
https://github.com/shalzuth/SwordOfConvallariaResearch/blob/main/Dumper/Utils/Lua.cs#L18-L28

After fixing the read function, I still faced issues with strings. A further diff of `slua.dll` revealed a crucial offset.
https://github.com/shalzuth/SwordOfConvallariaResearch/blob/main/Dumper/libs/Unluac/Parse/LStringType.cs#L27

Now, I was able to read the raw text of all the decompiled Lua scripts, which contain both game logic and data tables.

## Understanding the Network Protocol
Most games that prioritize security will prevent common tools like Fiddler from functioning, but it’s always worth trying, as many developers overlook security. In this case, the developers had implemented some protections, but since the game is built on Unity, it was relatively easy to bypass these restrictions and enable system proxies. There are many others that do il2cpp mod tutorials, and I recommend those, with the key part being hooking **HttpClientHandler.SendAsync**.

With Fiddler active, I could observe the login flow and how tokens are transmitted. In this example, there are some basic client identifiers, such as a ClientId and AppId, with the actual user content being in the post params. For guest accounts, it's a randomly generated string. For Steam accounts and Google accounts, it's the standard token you receive from those OAuth services. The main part of the auth response that is important is the AccessToken and MacKey, as this will be used to identify yourself to the game server.

Game traffic can be easily monitored using Wireshark. Upon analyzing the UDP data, I recognized it as protobuf, which I confirmed using a generic protobuf decoder (I recommend [this one](https://protobuf-decoder.netlify.app/)). The packet header typically includes the packet length, opcode, and occasionally other details like a counter or encryption/compression status. This packet header is as follows:
https://github.com/shalzuth/SwordOfConvallariaResearch/blob/main/Protos/Packet.cs#L74-L80

The final step was to identify the opcodes, which are hardcoded in the Lua scripts as tables.
https://github.com/shalzuth/SwordOfConvallariaResearch/blob/main/Dumper/Utils/OpCodes.cs#L11-L28

## Automating Updates
When the game updates, it is critical to make it easy to update. This is an extremely important step to make sure the tooling doesn't break from week to week. I have included how to download the raw asset files directly from the game servers in [Downloader.cs](https://github.com/shalzuth/SwordOfConvallariaResearch/blob/main/Dumper/Utils/Downloader.cs).

The dumper project glues together all the above steps to make it a one-button push to update to the latest version.

## Putting It All Together
With all these components in place, I can integrate them to conduct security testing. I usually create a simple project that logs in and sends various packets for quick and efficient testing.

Here is a quick test I did to check to basic Gacha functionality.
https://github.com/shalzuth/SwordOfConvallariaResearch/blob/main/SwordOfConvallariaResearch/Program.cs#L9
This is where I'd try negative numbers, funky patterns, etc.

If I had infinite time, I'd also check some lower level vulnerabilities such a protobuf parsing failures.

## Conclusion
This guide on security game testing in *Sword of Convallaria* provides a framework for identifying vulnerabilities in video games. By using the techniques outlined above, you can enhance your skills in finding exploits and contribute to a more secure gaming environment. If you have any questions or insights on security testing, feel free to reach out to me!

---

**Call to Action:** If you found this post helpful, please share it on social media or check out my other articles on game security and testing.
