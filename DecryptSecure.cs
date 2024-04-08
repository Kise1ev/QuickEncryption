using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace QuickEncryption
{
    public partial class DecryptSecure : Form
    {

        private QuickEncryption encryption { get; set; }

        public DecryptSecure(QuickEncryption encryption)
        {
            this.encryption = encryption;
            InitializeComponent();
        }

        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Text files (*.txt)|*.txt"};
            if (openFileDialog.ShowDialog().Equals(DialogResult.OK))
            {
                string fileName = openFileDialog.FileName;
                string fileData = GetFileData(fileName);
                string decryptedText = Decrypt(fileData);
                string decryptedFileName = $"{Path.GetFileNameWithoutExtension(fileName)}-decrypted.txt";
                string decryptedFilePath = Path.Combine(Path.GetDirectoryName(fileName), decryptedFileName);
                SaveFileData(decryptedFilePath, decryptedText);
                
                MessageBox.Show("Данные файла успешно расшифрованы!", "Успешное дешифрование", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Process.Start("explorer.exe", "/select," + decryptedFilePath);
            }
        }

        private string Decrypt(string data)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(data);
            byte[] iv = cipherTextBytes.Take(16).ToArray();
            byte[] encryptedData = cipherTextBytes.Skip(16).ToArray();

            PasswordDeriveBytes password = new PasswordDeriveBytes("myPassword", null);
            byte[] keyBytes = password.GetBytes(256 / 8);
            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.Mode = CipherMode.CBC;
                using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, iv))
                {
                    using (var memoryStream = new MemoryStream(encryptedData))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            byte[] plainData = new byte[encryptedData.Length];
                            int decryptedByteCount = cryptoStream.Read(plainData, 0, plainData.Length);
                            return Encoding.UTF8.GetString(plainData, 0, decryptedByteCount);
                        }
                    }
                }
            }
        }

        private string GetFileData(string fileName)
        {
            using (StreamReader streamReader = new StreamReader(fileName, Encoding.UTF8))
                return streamReader.ReadToEnd();
        }

        private void SaveFileData(string fileName, string data)
        {
            using (StreamWriter streamWriter = new StreamWriter(fileName, false, Encoding.UTF8))
                streamWriter.Write(data);
        }

        private void DecryptSecure_FormClosed(object sender, FormClosedEventArgs e)
            => encryption.Show();
    }
}