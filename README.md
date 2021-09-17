# AsyncNamedPipes
 
Quick PoC to send and receive messages over Named Pipes asynchronously.  Start `Server.exe` and then `Client.exe`.

```
C:\> Server.exe
Hello from client: 17/09/2021 16:44:02
Hello from client: 17/09/2021 16:44:05
Hello from client: 17/09/2021 16:44:08
Hello from client: 17/09/2021 16:44:11
Hello from client: 17/09/2021 16:44:14
```

```
C:\> Client.exe
Hello from server: 17/09/2021 16:44:02
Hello from server: 17/09/2021 16:44:08
Hello from server: 17/09/2021 16:44:14
```

No error handling anywhere, so YMMV.