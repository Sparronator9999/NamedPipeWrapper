using System;
using System.Windows.Forms;

namespace ExampleGUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonClient_Click(object sender, EventArgs e)
        {
            Hide();
            new ClientForm().ShowDialog(this);
            Close();
        }

        private void buttonServer_Click(object sender, EventArgs e)
        {
            Hide();
            new ServerForm().ShowDialog(this);
            Close();
        }
    }
}
