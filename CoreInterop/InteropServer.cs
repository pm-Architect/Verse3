using Core;
using NamedPipeWrapper;

namespace CoreInterop
{
    public class InteropServer
    {
        public InteropServer(string pipeName, string token)
        {
            //TODO: Authenticate with token
            var server = new NamedPipeServer<IDataGoo>(pipeName);
            server.ClientConnected += OnClientConnected;
            server.ClientDisconnected += OnClientDisconnected;
            server.ClientMessage += OnClientMessage;
            server.Error += OnError;
            server.Start();
        }

        private void OnClientConnected(NamedPipeConnection<IDataGoo, IDataGoo> connection)
        {
            //Console.WriteLine("Client {0} is now connected!", connection.Id);
            //connection.PushMessage(new IDataGoo
            //{
            //    Id = new Random().Next(),
            //    Text = "Welcome!"
            //});
        }

        private void OnClientDisconnected(NamedPipeConnection<IDataGoo, IDataGoo> connection)
        {
            //Console.WriteLine("Client {0} disconnected", connection.Id);
        }

        private void OnClientMessage(NamedPipeConnection<IDataGoo, IDataGoo> connection, IDataGoo message)
        {
            //Console.WriteLine("Client {0} says: {1}", connection.Id, message);
        }

        private void OnError(Exception exception)
        {
            //Console.Error.WriteLine("ERROR: {0}", exception);
        }
    }
}