# AsyncNamedPipes
 
Quick PoC to send and receive messages over Named Pipes asynchronously.  Start `Server.exe` and then `Client.exe`.

```
C:\> Server.exe
Ping
Ping
Ping
^C
```

```
C:\> Client.exe
Pong
Pong
Pong
^C
```

No error handling anywhere, so YMMV.