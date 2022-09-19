using Core;
using NamedPipeWrapper;
using System;
using System.Runtime.Serialization;

namespace CoreInterop
{
    public class InteropClient
    {
        //DEV NOTE: dynamic used here!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public delegate dynamic InteropDelegate(params object[] args);
        private NamedPipeClient<DataStructure> client;
        public InteropClient(string pipeName, string token)
        {
            //TODO: Authenticate with token
            client = new NamedPipeClient<DataStructure>(pipeName);
            client.ServerMessage += OnServerMessage;
            client.Error += OnError;
            client.Disconnected += OnDisconnected;
            client.Start();
            //client.Stop();
        }

        public event EventHandler Disconnected;
        private void OnDisconnected(NamedPipeConnection<DataStructure, DataStructure> connection)
        {
            //CoreConsole.Log($"Server disconnected!");
            Disconnected.Invoke(this, new EventArgs());
            //throw new NotImplementedException();
        }

        public event EventHandler<DataStructure> ServerMessage;
        private void OnServerMessage(NamedPipeConnection<DataStructure, DataStructure> connection, DataStructure message)
        {
            //CoreConsole.Log($"Server sent a message: {message.ToString()}");
            ServerMessage.Invoke(connection, message);
            //Console.WriteLine("Server says: {0}", message);
        }

        public event EventHandler<Exception> Error;
        private void OnError(Exception exception)
        {
            //CoreConsole.Log($"Error: {exception}");
            Error.Invoke(this, exception);
            //Console.Error.WriteLine("ERROR: {0}", exception);
        }

        public void Send(DataStructure data)
        {
            client.PushMessage(data);
        }
    }
}