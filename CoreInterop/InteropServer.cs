using Core;
using NamedPipeWrapper;
using System;
using System.Runtime.Serialization;

namespace CoreInterop
{
    public class InteropServer
    {
        public static InteropServer _LocalInteropServer { get; private set; }
        internal static InteropServer _NetworkInteropServer { get; private set; }
        private NamedPipeServer<DataStructure> server;
        public InteropServer(string pipeName, string token)
        {
            //TODO: Authenticate with token
            server = new NamedPipeServer<DataStructure>(pipeName);
            if (pipeName == "Verse3")
            {
                _LocalInteropServer = this;
            }
            else if (pipeName == "Verse3Network")
            {
                _NetworkInteropServer = this;
            }
            server.ClientConnected += OnClientConnected;
            server.ClientDisconnected += OnClientDisconnected;
            server.ClientMessage += OnClientMessage;
            server.Error += OnError;
            server.Start();
        }

        public void Send(DataStructure data)
        {
            try
            {
                server.PushMessage(data);
            }
            catch (Exception ex)
            {
                //throw ex;
                CoreConsole.Log(ex);
            }
        }
        public void Send(DataStructure data, string clientName)
        {
            try
            {
                server.PushMessage(data, clientName);
            }
            catch (Exception ex)
            {
                //throw ex;
                CoreConsole.Log(ex);
            }
        }

        public event EventHandler ClientConnected;
        private void OnClientConnected(NamedPipeConnection<DataStructure, DataStructure> connection)
        {
            //CoreConsole.Log($"Client {connection.Id} is now connected!");
            if (ClientConnected != null && ClientConnected.GetInvocationList().Length > 0)
                ClientConnected.Invoke(connection, new EventArgs());
            connection.PushMessage(new DataStructure<string>("Connected to Verse3"));
        }

        public event EventHandler ClientDisconnected;
        private void OnClientDisconnected(NamedPipeConnection<DataStructure, DataStructure> connection)
        {
            CoreConsole.Log($"Client {connection.Id} is now disconnected!");
            if (ClientDisconnected != null && ClientDisconnected.GetInvocationList().Length > 0)
                ClientDisconnected.Invoke(this, new EventArgs());
            //CoreConsole.Log("Client {0} disconnected", connection.Id);
        }

        public event EventHandler<DataStructure> ClientMessage;
        private void OnClientMessage(NamedPipeConnection<DataStructure, DataStructure> connection, DataStructure message)
        {
            CoreConsole.Log($"Client {connection.Id} sent a message: {message.ToString()}");
            if (ClientMessage != null && ClientMessage.GetInvocationList().Length > 0)
                ClientMessage.Invoke(connection, message);
            //CoreConsole.Log("Client {0} says: {1}", connection.Id, message);
        }

        public event EventHandler<Exception> Error;
        private void OnError(Exception exception)
        {
            CoreConsole.Log($"Error: {exception}");
            if (Error != null && Error.GetInvocationList().Length > 0)
                Error.Invoke(this, exception);
            //Console.Error.WriteLine("ERROR: {0}", exception);
        }
    }
}