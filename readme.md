## Discription

Account server for the TOP server.
This server does

* User login
    - Ensure that only one 1 user can be logged in
* User registration
    - Register the user from the client
* User Start/End billing


## Test status

TOP_Network: [![.NET](https://github.com/BlitzDaWolf/TOP-Network/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/BlitzDaWolf/TOP-Network/actions/workflows/dotnet.yml)


## Folder structure

This file structure is for the current active project.

```
./$(Procjectroot)
    ./TOP-Network (Current project)
        ./Tests
            ./NetworkPacketTests
            ./PacketConnectionTests
        ./TOP-Network
    ./TOP_Utils
```
You can run all of the test in `$(Procjectroot)/TOP-Network/runtests.sh`.

This will run all of the tests with code covrage