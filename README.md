# Named Pipe Wrapper for .NET

A simple, easy to use, strongly-typed wrapper around .NET named pipes.

## NuGet Package

**NOTE: The NuGet package below links to the [original Named Pipe Wrapper](https://github.com/acdvorak/named-pipe-wrapper).**

Available as a [NuGet package](https://www.nuget.org/packages/NamedPipeWrapper/).

## Features

- Create named pipe servers that can handle multiple client connections simultaneously.
- Send strongly-typed messages between clients and servers: any serializable .NET object can be sent over a pipe and will be automatically serialized/deserialized, including cyclical references and complex object graphs.
- Messages are sent and received asynchronously on a separate background thread and marshalled back to the calling thread (typically the UI).
- Supports large messages - up to 300 MiB.

## Requirements

Requires .NET Framework 4.8, but may work on older .NET.

.NET Core and .NET 5+ have not been tested yet.

## Quick Start

Also refer to the example projects (named ExampleCLI and ExampleGUI).

First, create a message class with a [`[Serializable]` attribute](https://learn.microsoft.com/en-us/dotnet/api/system.serializableattribute?view=netframework-4.8) and add it to both your client and server applications.

You may have as many or as few fields as you want, but for the purposes of this guide, we will only include one `string` field:

```cs
[Serializable]
public class MyMessage
{
    public string Text;

    public MyMessage(string text)
    {
        Text = text;
    }
}
```

Add the following code to your *server* application's startup method (e.g. `Main()` in console applications):

```csharp
NamedPipeServer<MyMessage> server = new NamedPipeServer<MyMessage>("MyServer");

// Set up server events:
server.ClientConnected += delegate(NamedPipeConnection<MyMessage> conn)
{
    Console.WriteLine($"Client {conn.ID} is now connected!");
    conn.PushMessage(new MyMessage("Welcome!"));
};
server.ClientMessage += delegate(NamedPipeConnection<MyMessage> conn, MyMessage message)
{
    Console.WriteLine($"Client {conn.ID} says: {message.Text}");
};

// Start up the server asynchronously and begin listening for connections.
// This method will return immediately while the server runs in a separate background thread.
server.Start();
```

Add the following code to your *client* application:

```cs
NamedPipeClient<MyMessage> client = new NamedPipeClient<MyMessage>("MyServer");

// Set up client events
client.ServerMessage += delegate(NamedPipeConnection<MyMessage> conn, MyMessage message)
{
    Console.WriteLine($"Server says: {message.Text}", );
};

// Start up the client asynchronously and connect to the specified server pipe.
// This method will return immediately while the client runs in a separate background thread.
client.Start();
```

## MIT License

Named Pipe Wrapper for .NET is licensed under the [MIT license](LICENSE.txt).
