﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab5
{
    public partial class HybridForm : Form
    {
        byte[] K = new byte[16];
        byte[] IV = new byte[16];
        StringBuilder K_str = new StringBuilder();
        StringBuilder IV_str = new StringBuilder();        

        RSACryptoServiceProvider RSA_B = new RSACryptoServiceProvider();

        public HybridForm()
        {
            InitializeComponent();
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

            rngCsp.GetBytes(K);
            foreach (byte b in K) K_str.AppendFormat("{0:x2}", b);

            rngCsp.GetBytes(IV);
            foreach (byte b in IV) IV_str.AppendFormat("{0:x2}", b);
        }

        private void Run_bt_Click(object sender, EventArgs e)
        {
            RSAParameters bobSecretKeys = RSA_B.ExportParameters(true);
            RSAParameters bobPublicKeys = RSA_B.ExportParameters(false);
            richTextBoxA.AppendText("Открытый ключ Боба: \nE =" + Convert.ToBase64String(bobPublicKeys.Exponent) + "\nМодуль = " + Convert.ToBase64String(bobPublicKeys.Modulus) + "\n");
            //////////////////////////////Alice's side
            string AliceMessage = "Alice`s message";
            byte[] AliceK = K;
            byte[] AliceIV = IV;
            richTextBoxA.AppendText("\nСообщение Алисы: " + AliceMessage + "\nКлюч К = " + K_str.ToString() + "\nВектор IV = " + IV_str.ToString() + "\n");
            byte[] AliceM0 = Encrypt(AliceMessage, AliceK);
            StringBuilder AliceM0_str = new StringBuilder();
            foreach (byte b in AliceM0) AliceM0_str.AppendFormat("{0:x2}", b);
            richTextBoxA.AppendText("Алиса Зашифровала симметричным алгоритмом сообщение: " + AliceM0_str.ToString() + "\n");
            //byte[] KIV = ArrayPlus(AliceK, AliceIV);
                                    
            byte[] AliceM1 = RSAEncrypt(AliceK, bobPublicKeys, false);
            StringBuilder AliceM1_str = new StringBuilder();
            foreach (byte b in AliceM1) AliceM1_str.AppendFormat("{0:x2}", b);
            richTextBoxA.AppendText("\nАлиса Зашифровала асимметричным алгоритмом K: " + AliceM1_str.ToString() + "\n");
            byte[] M = ArrayPlus(AliceM0, AliceM1);
            StringBuilder M_str = new StringBuilder();
            foreach (byte b in M) M_str.AppendFormat("{0:x2}", b);
            richTextBoxA.AppendText("\nАлиса передаёт сообщение 'Зашифрованный текс + Зашифрованный K': " + M_str.ToString());

            //////////////////////////////Bob's side
            richTextBoxB.AppendText("Закрытый ключ Боба: \nD =" + Convert.ToBase64String(bobSecretKeys.D) + "\nМодуль = " + Convert.ToBase64String(bobSecretKeys.Modulus) + "\n");
            byte[] BobM0 = new byte[AliceM0.Length];
            byte[] BobM1 = new byte[AliceM1.Length];
            richTextBoxB.AppendText("\nБоб получает сообщение: " + M_str.ToString() + "\n");

            Array.Copy(M, BobM0, BobM0.Length);
            StringBuilder BobM0_str = new StringBuilder();
            foreach (byte b in BobM0) BobM0_str.AppendFormat("{0:x2}", b);
            richTextBoxB.AppendText("\nБоб выделяет из сообщения часть зашифрованную симметричным алгоритмом: " + BobM0_str.ToString() + "\n");

            for (int i = BobM0.Length, j = 0; i < M.Length; i++, j++)
            {
                BobM1[j] = M[i];
            }
            StringBuilder BobM1_str = new StringBuilder();
            foreach (byte b in BobM1) BobM1_str.AppendFormat("{0:x2}", b);
            richTextBoxB.AppendText("\nБоб выделяет из сообщения часть зашифрованную асимметричным алгоритмом: " + BobM1_str.ToString() + "\n");

            byte[] decBobM1 = RSADecrypt(BobM1, bobSecretKeys, false);
            StringBuilder decBobM1_str = new StringBuilder();
            foreach (byte b in decBobM1) decBobM1_str.AppendFormat("{0:x2}", b);
            richTextBoxB.AppendText("\nБоб расшифровывает вторую часть сообщения с помощью своего сектреного ключа: " + decBobM1_str.ToString() + "\n");

            byte[] BobK = new byte[K.Length];
            Array.Copy(decBobM1, BobK, K.Length);
            StringBuilder BobK_str = new StringBuilder();
            foreach (byte b in BobK) BobK_str.AppendFormat("{0:x2}", b);

            /*byte[] BobIV = new byte[IV.Length];
            Array.ConstrainedCopy(decBobM1, K.Length, BobIV, 0, IV.Length);
            StringBuilder BobIV_str = new StringBuilder();
            foreach (byte b in BobIV) BobIV_str.AppendFormat("{0:x2}", b);*/
            richTextBoxB.AppendText("\nБоб получает из расшифрованного сообщения \nключ К: " + BobK_str.ToString() + "\n");
            string Message = Decrypt(BobM0, BobK);
            richTextBoxB.AppendText("\nС помощью полученных ключа и вектора Боб расшифровывает первую часть сообщения: " + Message);
        }

        static byte[] Encrypt(string plainText, byte[] Key)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");            
            byte[] encrypted;
            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                byte[] IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                //aesAlg.Padding = PaddingMode.None;
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter bwEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            bwEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        static string Decrypt(byte[] cipherText, byte[] Key)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");            

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                byte[] IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
               // aesAlg.Padding = PaddingMode.None;
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }

        static public byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {

                    //Import the RSA Key information. This only needs
                    //toinclude the public key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Encrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
                }
                return encryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }

        }

        static public byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    //Import the RSA Key information. This needs
                    //to include the private key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Decrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                }
                return decryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }
        }

        byte[] ArrayPlus(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];

            Array.Copy(a, c, a.Length);
            int i = a.Length;
            foreach (byte by in b)
            {
                c[i] = by;
                i++;
            }
            return c;
        }
    }
}
