using System.Net.Sockets;
using System.Text;

using var client = new TcpClient();
await client.ConnectAsync("localhost", 5080);
using var stream = client.GetStream();

string request =
    "GET / HTTP/1.1\r\n" +
    "Host: localhost:5080\r\n" +
    "Connection: close\r\n" +
    "\r\n";

await stream.WriteAsync(Encoding.ASCII.GetBytes(request));

using var reader = new StreamReader(stream, Encoding.ASCII);
Console.WriteLine(await reader.ReadToEndAsync());