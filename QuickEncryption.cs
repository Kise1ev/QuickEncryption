using System;
using System.Windows.Forms;

namespace QuickEncryption
{
    public partial class QuickEncryption : Form
    {

        public QuickEncryption()
            => InitializeComponent();

        private void EncryptButton_Click(object sender, EventArgs e)
        {
            EncryptSecure encryptSecure = new EncryptSecure(this);
            encryptSecure.Show();
            Hide();
        }

        private void DecryptButton_Click(object sender, EventArgs e)
        {
            DecryptSecure decryptSecure = new DecryptSecure(this);
            decryptSecure.Show();
            Hide();
        }
    }
}