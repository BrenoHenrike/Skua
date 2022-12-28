using Grpc.Net.Client;
using Skua.gRPC.Server;

namespace Skua.SyncConsole;

internal class Program
{
    static async Task Main(string[] args)
    {
        var channel = GrpcChannel.ForAddress("http://localhost:5074");
        var client  = new Synchronizer.SynchronizerClient(channel);
        bool quit = false;
        while(!quit)
        {
            Console.WriteLine("Write a command:");
            var read = Console.ReadLine();
            if (string.IsNullOrEmpty(read))
                continue;
            if(read == "quit")
                quit = true;
            Console.WriteLine("Sending command...");
            var id = int.Parse(read.Substring(0, 1));
            var split = read.Split(' ');
            var command = split[1];
            var request = new SendCommandRequest()
            {
                Id = id,
                Command = command
            };
            request.Arguments.Add(split[2..]);
            await client.SendCommandAsync(request);
            Console.Clear();
        }
    }
}
