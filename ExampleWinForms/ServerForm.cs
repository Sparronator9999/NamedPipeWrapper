using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ExampleGUI
{
    internal partial class ServerForm : Form
    {
        private readonly NamedPipeServer<string> _server = new NamedPipeServer<string>(Constants.PIPE_NAME);
        private readonly HashSet<string> _clients = new HashSet<string>();

        public ServerForm()
        {
            InitializeComponent();
            _server.ClientConnected += OnClientConnected;
            _server.ClientDisconnected += OnClientDisconnected;
            _server.ClientMessage += OnClientMessage;
            _server.Start();
        }

        private void OnClientConnected(object sender, PipeConnectionEventArgs<string, string> e)
        {
            _clients.Add(e.Connection.Name);
            AddLine($"{e.Connection.Name} connected!");
            UpdateClientList();
            e.Connection.PushMessage("Welcome! You are now connected to the server.");
        }

        private void OnClientDisconnected(object sender, PipeConnectionEventArgs<string, string> e)
        {
            _clients.Remove(e.Connection.Name);
            AddLine($"{e.Connection.Name} disconnected!");
            UpdateClientList();
        }

        private void OnClientMessage(object sender, PipeMessageEventArgs<string, string> e)
        {
            AddLine($"<{e.Connection.Name}> {e.Message}");
        }

        private void AddLine(string text)
        {
            txtMessages.Invoke(new Action(delegate
            {
                txtMessages.Text += text + Environment.NewLine;
            }));
        }

        private void UpdateClientList()
        {
            lstClients.Invoke(new Action(delegate
            {
                lstClients.Items.Clear();
                foreach (string client in _clients)
                {
                    lstClients.Items.Add(client);
                }
            }));
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text))
                return;

            if (lstClients.SelectedItem == null)
            {
                _server.PushMessage(txtMessage.Text);
            }
            else
            {
                string clientName = lstClients.SelectedItem.ToString();
                _server.PushMessage(txtMessage.Text, clientName);
            }

            txtMessage.Text = "";
        }
    }
}
