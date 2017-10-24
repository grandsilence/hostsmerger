using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HostsUnique
{
    public partial class MainForm : Form
    {
        private readonly HashSet<string> _hosts = new HashSet<string>();

        public MainForm()
        {
            InitializeComponent();
        }

        private async void btnAddHost_Click(object sender, EventArgs e)
        {
            if (openFileDialogHost.ShowDialog() != DialogResult.OK)
                return;

            btnAddHost.Enabled = false;
            btnAddHost.Text = "Appending...";             

            using (var sr = new StreamReader(openFileDialogHost.FileName))
            {
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrEmpty(line) || line[0] == '#') // string.StartsWith is slower
                        continue;

                    string[] host = Regex.Split(line, @"\s+");
                    if (host.Length < 2)
                        continue;

                    _hosts.Add(host[0] + "          " + host[1]);
                }
            }

            tbHosts.Text = string.Join(Environment.NewLine, _hosts);
            btnAddHost.Text = "Add host";
            btnAddHost.Enabled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            const string result = "hosts";
            File.WriteAllText(result, tbHosts.Text, Encoding.UTF8);
            MessageBox.Show("Saved as hosts :)", "Done!", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Reset hosts?", "Warning", MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (result != DialogResult.Yes)
                return;

            _hosts.Clear();
            tbHosts.Clear();
        }
    }
}
