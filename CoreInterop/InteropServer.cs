using Core;
using NamedPipeWrapper;
using System;
using System.Runtime.Serialization;

namespace CoreInterop
{
    public class InteropServer
    {
        private NamedPipeServer<DataStructure> server;
        public InteropServer(string pipeName, string token)
        {
            //TODO: Authenticate with token
            server = new NamedPipeServer<DataStructure>(pipeName);
            server.ClientConnected += OnClientConnected;
            server.ClientDisconnected += OnClientDisconnected;
            server.ClientMessage += OnClientMessage;
            server.Error += OnError;
            server.Start();
        }

        public void Send(DataStructure data)
        {
            server.PushMessage(data);
        }
        public void Send(DataStructure data, string clientName)
        {
            server.PushMessage(data, clientName);
        }

        public event EventHandler ClientConnected;
        private void OnClientConnected(NamedPipeConnection<DataStructure, DataStructure> connection)
        {
            CoreConsole.Log($"Client {connection.Id} is now connected!");
            ClientConnected.Invoke(connection, new EventArgs());
            //connection.PushMessage(new IDataGoo
            //{
            //    Id = new Random().Next(),
            //    Text = "Welcome!"
            //});
        }

        public event EventHandler ClientDisconnected;
        private void OnClientDisconnected(NamedPipeConnection<DataStructure, DataStructure> connection)
        {
            CoreConsole.Log($"Client {connection.Id} is now disconnected!");
            ClientDisconnected.Invoke(this, new EventArgs());
            //Console.WriteLine("Client {0} disconnected", connection.Id);
        }

        public event EventHandler<DataStructure> ClientMessage;
        private void OnClientMessage(NamedPipeConnection<DataStructure, DataStructure> connection, DataStructure message)
        {
            CoreConsole.Log($"Client {connection.Id} sent a message: {message.ToString()}");
            ClientMessage.Invoke(connection, message);
            //Console.WriteLine("Client {0} says: {1}", connection.Id, message);
        }

        public event EventHandler<Exception> Error;
        private void OnError(Exception exception)
        {
            CoreConsole.Log($"Error: {exception}");
            Error.Invoke(this, exception);
            //Console.Error.WriteLine("ERROR: {0}", exception);
        }
    }
}