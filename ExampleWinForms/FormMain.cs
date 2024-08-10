using System;
using System.Windows.Forms;

namespace ExampleGUI
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void buttonClient_Click(object sender, EventArgs e)
        {
            Hide();
            new FormClient().ShowDialog(this);
            Close();
        }

        private void buttonServer_Click(object sender, EventArgs e)
        {
            Hide();
            new FormServer().ShowDialog(this);
            Close();
        }
    }
}
