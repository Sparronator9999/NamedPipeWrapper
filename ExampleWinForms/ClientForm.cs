using NamedPipeWrapper;
using System;
using System.Windows.Forms;

namespace ExampleGUI
{
    internal partial class ClientForm : Form
    {
        private readonly NamedPipeClient<string> _client = new NamedPipeClient<string>(Constants.PIPE_NAME);

        public ClientForm()
        {
            InitializeComponent();
            Load += OnLoad;
        }

        private void OnLoad(object sender, EventArgs eventArgs)
        {
            _client.ServerMessage += OnServerMessage;
            _client.Disconnected += OnDisconnected;
            _client.Start();
        }

        private void OnServerMessage(object sender, PipeMessageEventArgs<string, string> e)
        {
            txtMessages.Invoke(new Action(delegate
            {
                AddLine($"<Server> {e.Message}");
            }));
        }

        private void OnDisconnected(object sender, PipeConnectionEventArgs<string, string> e)
        {
            txtMessages.Invoke(new Action(delegate
            {
                AddLine("Disconnected from server");
            }));
        }

        private void AddLine(string text)
        {
            txtMessages.Invoke(new Action(delegate
            {
                txtMessages.Text += text + Environment.NewLine;
            }));
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text))
                return;

            _client.PushMessage(txtMessage.Text);
            txtMessage.Text = "";
        }
    }
}
