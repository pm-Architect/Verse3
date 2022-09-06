using Core;
using NamedPipeWrapper;

namespace CoreInterop
{
    public class InteropClient
    {
        public InteropClient(string pipeName, string token)
        {
            //TODO: Authenticate with token
            var client = new NamedPipeClient<IDataGoo>(pipeName);
            client.ServerMessage += OnServerMessage;
            client.Error += OnError;
            client.Start();
            //client.Stop();
        }
        private void OnServerMessage(NamedPipeConnection<IDataGoo, IDataGoo> connection, IDataGoo message)
        {
            //Console.WriteLine("Server says: {0}", message);
        }

        private void OnError(Exception exception)
        {
            //Console.Error.WriteLine("ERROR: {0}", exception);
        }
    }
}