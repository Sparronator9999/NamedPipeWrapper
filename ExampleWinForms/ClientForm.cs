using System;
using System.Windows.Forms;
using NamedPipeWrapper;

namespace ExampleGUI
{
    public partial class ClientForm : Form
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

        private void OnServerMessage(NamedPipeConnection<string, string> connection, string message)
        {
            txtMessages.Invoke(new Action(delegate
            {
                AddLine($"<Server> {message}");
            }));
        }

        private void OnDisconnected(NamedPipeConnection<string, string> connection)
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
