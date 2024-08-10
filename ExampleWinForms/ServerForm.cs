using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NamedPipeWrapper;

namespace ExampleGUI
{
    public partial class ServerForm : Form
    {
        private readonly NamedPipeServer<string> _server = new NamedPipeServer<string>(Constants.PIPE_NAME);
        private readonly ISet<string> _clients = new HashSet<string>();


        public ServerForm()
        {
            InitializeComponent();
            _server.ClientConnected += OnClientConnected;
            _server.ClientDisconnected += OnClientDisconnected;
            _server.ClientMessage += OnClientMessage;
            _server.Start();
        }

        private void OnClientConnected(NamedPipeConnection<string, string> connection)
        {
            _clients.Add(connection.Name);
            AddLine($"{connection.Name} connected!");
            UpdateClientList();
            connection.PushMessage("Welcome! You are now connected to the server.");
        }

        private void OnClientDisconnected(NamedPipeConnection<string, string> connection)
        {
            _clients.Remove(connection.Name);
            AddLine($"{connection.Name} disconnected!");
            UpdateClientList();
        }

        private void OnClientMessage(NamedPipeConnection<string, string> connection, string message)
        {
            AddLine($"<{connection.Name}> {message}");
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
