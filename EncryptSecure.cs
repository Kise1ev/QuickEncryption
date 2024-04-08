using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace QuickEncryption
{
    public partial class EncryptSecure : Form
    {

        private QuickEncryption encryption { get; set; }

        public EncryptSecure(QuickEncryption encryption)
        {
            this.encryption = encryption;
            InitializeComponent();
        }

        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Text files (*.txt)|*.txt" };
            if (openFileDialog.ShowDialog().Equals(DialogResult.OK))
            {
                string fileName = openFileDialog.FileName;
                string fileData = GetFileData(fileName);
                string encryptedText = Encrypt(fileData);
                string encryptedFileName = $"{Path.GetFileNameWithoutExtension(fileName)}-encrypted.txt";
                string encryptedFilePath = Path.Combine(Path.GetDirectoryName(fileName), encryptedFileName);
                SaveFileData(encryptedFilePath, encryptedText);
                
                MessageBox.Show("Данные файла успешно зашифрованы!", "Успешное шифрование", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Process.Start("explorer.exe", "/select," + encryptedFilePath);
            }
        }

        private string Encrypt(string data)
        {
            byte[] plainData = Encoding.UTF8.GetBytes(data);
            PasswordDeriveBytes password = new PasswordDeriveBytes("myPassword", null);
            byte[] keyBytes = password.GetBytes(256 / 8);
            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.Mode = CipherMode.CBC;
                byte[] iv = symmetricKey.IV;
                using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, iv))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainData, 0, plainData.Length);
                            cryptoStream.FlushFinalBlock();
                            byte[] cipherTextBytes = iv.Concat(memoryStream.ToArray()).ToArray();
                            return Convert.ToBase64String(cipherTextBytes);
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

        private void EncryptSecure_FormClosed(object sender, FormClosedEventArgs e)
            => encryption.Show();
    }
}