using System;
using System.Collections;
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
    public partial class HybridMForm : Form
    {
        byte[] K = new byte[16];
        byte[] IV = new byte[16];
        StringBuilder K_str = new StringBuilder();
        StringBuilder IV_str = new StringBuilder();

        RSACryptoServiceProvider RSA_B = new RSACryptoServiceProvider();
        
        public HybridMForm()
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
            richTextBoxA.Clear();
            richTextBoxB.Clear();
            RSAParameters bobSecretKeys = RSA_B.ExportParameters(true);
            RSAParameters bobPublicKeys = RSA_B.ExportParameters(false);
            richTextBoxA.AppendText("Открытый ключ Боба: \nE =" + Convert.ToBase64String(bobPublicKeys.Exponent) + "\nМодуль = " + Convert.ToBase64String(bobPublicKeys.Modulus) + "\n");
            //////////////////////////////Alice's side
            string AliceMessage = "Alice`s message";
            byte[] AliceK = K;
            byte[] AliceIV = IV;
            richTextBoxA.AppendText("\nСообщение Алисы: " + AliceMessage + "\nКлюч К = " + K_str.ToString() + "\nВектор IV = " + IV_str.ToString() + "\n");
            byte[] AliceM0;
            switch (comboBox.SelectedIndex)
            {                
                case 0: // CBC
                    AliceM0 = EncryptCBC(AliceMessage, AliceK, AliceIV);
                    break;
                case 1: // OFB
                    AliceM0 = EncryptOFB(AliceMessage, AliceK, AliceIV);
                    break;
                case 2: // CFB
                    AliceM0 = EncryptCFB(AliceMessage, AliceK, AliceIV);
                    break;
                default:
                    AliceM0 = EncryptCBC(AliceMessage, AliceK, AliceIV);
                    break;
            }
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

            string Message ="";
            switch (comboBox.SelectedIndex)
            {                
                case 0: // CBC
                    Message = DecryptCBC(BobM0, BobK);
                    break;
                case 1: // OFB
                    Message = DecryptOFB(BobM0, BobK);
                    break;
                case 2: // CFB
                    Message = DecryptCFB(BobM0, BobK);
                    break;
            }
            richTextBoxB.AppendText("\nС помощью полученных ключа и вектора Боб расшифровывает первую часть сообщения: " + Message);
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

        byte[] Encryption(byte[] plainText, byte[] Key) // шифрование для остальных режимов (на входе байтовый массив, дополнения нет)
        {
            byte[] encrypted;
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                byte[] IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                aesAlg.Padding = PaddingMode.None;
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (BinaryWriter bwEncrypt = new BinaryWriter(csEncrypt))
                        {
                            bwEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }

        byte[] Decryption(byte[] cipherText, byte[] Key) // расшифрование для остальных режимов (на выходе байтовый массив, дополнения нет)
        {
            byte[] decrypted;
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                byte[] IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                aesAlg.Padding = PaddingMode.None;
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (BinaryReader brDecrypt = new BinaryReader(csDecrypt))
                        {
                            decrypted = brDecrypt.ReadBytes(cipherText.Length);
                        }
                    }
                }
            }
            return decrypted;
        }

        static List<string> Split(string str, int chunkSize) // разбиение строки str на блоки длиной chunkSize
        {
            List<string> list = new List<string>();
            for (int i = 0; i < str.Length; i += chunkSize)
            {
                if ((i + chunkSize) < str.Length)
                    list.Add(str.Substring(i, chunkSize));
                else
                    list.Add(str.Substring(i));
            }
            return list;
        }

        static List<byte[]> Split(byte[] bt, int chunkSize) // разбиение байтового массива bt на блоки длиной chunkSize
        {
            List<byte[]> list = new List<byte[]>();
            list = bt.Select((by, i) => new { group = i / chunkSize, value = by })
           .GroupBy(item => item.group)
           .Select(group => group.Select(v => v.value).ToArray())
           .ToList();
            return list;
        }

        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }

        byte[] XOR(byte[] a1, byte[] a2) // побитовый xor двух байтовых массивов (первый может иметь большую длину)
        {
            BitArray b1 = new BitArray(a1);
            BitArray b2 = new BitArray(a2);
            BitArray buff = new BitArray(b2.Length);
            BitArray bResult = new BitArray(b1.Length);
            for (int i = 0; i < buff.Length; i++)
                buff[i] = b1[i];
            buff = buff.Xor(b2);
            for (int i = 0; i < buff.Length; i++)
                bResult[i] = buff[i];
            for (int i = buff.Length; i < bResult.Length; i++)
                bResult[i] = b1[i];
            byte[] result = BitArrayToByteArray(bResult);
            return result;
        }

        byte[] EncryptCFB(string plainText, byte[] Key, byte[] IV) // шифрование AES CFB
        {
            List<string> plainList = Split(plainText, 16);
            int lastLength = plainList[plainList.Count - 1].Length;
            if (lastLength < 16) // дополнение последнего блока
            {
                for (int i = 0; i < 16 - lastLength; i++)
                    plainList[plainList.Count - 1] = plainList[plainList.Count - 1] + " ";
            }
            List<byte[]> encryptedList = new List<byte[]>();
            encryptedList.Add(IV); // первый блок в шифртексте - IV

            for (int i = 1; i < plainList.Count + 1; i++) // шифрую блоки один за одним
            {
                encryptedList.Add(
                               XOR(
                                Encryption(encryptedList[i - 1], Key), Encoding.ASCII.GetBytes(plainList[i - 1])
                                  )
                                 );
            }
            byte[] encryptedArray = encryptedList // List<byte[]> to byte[]
                .SelectMany(a => a)
                .ToArray();
            return encryptedArray;
        }

        string DecryptCFB(byte[] cipherText, byte[] Key) // расшифрование AES CFB
        {
            List<byte[]> cipherList = Split(cipherText, 16);
            string decrypted = "";
            List<byte[]> decryptedList = new List<byte[]>();
            for (int i = 0; i < cipherList.Count - 1; i++) // расшифрование блоков один за одним
                decryptedList.Add(
                               XOR(
                                Encryption(cipherList[i], Key), cipherList[i + 1]
                                  )
                                 );
            byte[] decryptedArray = decryptedList  // List<byte[]> to byte[]
                .SelectMany(a => a)
                .ToArray();
            decrypted = Encoding.ASCII.GetString(decryptedArray);
            return decrypted;
        }

        byte[] EncryptCBC(string plainText, byte[] Key, byte[] IV) // шифрование AES CBC
        {
            List<string> plainList = Split(plainText, 16);
            int lastLength = plainList[plainList.Count - 1].Length;
            if (lastLength < 16) // дополнение последнего блока
            {
                for (int i = 0; i < 16 - lastLength; i++)
                    plainList[plainList.Count - 1] = plainList[plainList.Count - 1] + " ";
            }
            List<byte[]> encryptedList = new List<byte[]>();
            encryptedList.Add(IV); // первый блок в шифртексте - IV

            for (int i = 1; i < plainList.Count + 1; i++) // шифрую блоки один за одним
            {
                encryptedList.Add(
                                Encryption(
                                  XOR(Encoding.ASCII.GetBytes(plainList[i - 1]), encryptedList[i - 1]),
                                  Key)
                                 );
            }
            byte[] encryptedArray = encryptedList // List<byte[]> to byte[]
                .SelectMany(a => a)
                .ToArray();
            return encryptedArray;
        }

        string DecryptCBC(byte[] cipherText, byte[] Key) // расшифрование AES CBC
        {
            List<byte[]> cipherList = Split(cipherText, 16);
            string decrypted = "";
            List<byte[]> decryptedList = new List<byte[]>();
            for (int i = cipherList.Count - 1; i > 0; i--) // расшифрование блоков один за одним (от большего номера к меньшему)
                decryptedList.Add(
                               XOR(cipherList[i - 1],
                                   Decryption(cipherList[i], Key))
                                 );
            decryptedList.Reverse(); // развернуть список
            byte[] decryptedArray = decryptedList  // List<byte[]> to byte[]
                .SelectMany(a => a)
                .ToArray();
            decrypted = Encoding.ASCII.GetString(decryptedArray);
            return decrypted;
        }

        byte[] EncryptOFB(string plainText, byte[] Key, byte[] IV) // шифрование AES OFB
        {
            List<string> plainList = Split(plainText, 16);
            int lastLength = plainList[plainList.Count - 1].Length;
            if (lastLength < 16) // дополнение последнего блока
            {
                for (int i = 0; i < 16 - lastLength; i++)
                    plainList[plainList.Count - 1] = plainList[plainList.Count - 1] + " ";
            }
            List<byte[]> encryptedList = new List<byte[]>();
            List<byte[]> bufferList = new List<byte[]>();
            bufferList.Add(IV); // первый промежуточный блок - IV
            encryptedList.Add(IV); // первый блок шифртекста - IV
            for (int i = 0; i < plainList.Count; i++) // шифрую блоки один за одним
            {
                bufferList.Add(Encryption(bufferList[i], Key)); // промежуточный блок (bufferList[i+1])
                encryptedList.Add(      // encryptedList[i+1]
                                  XOR(Encoding.ASCII.GetBytes(plainList[i]),
                                      bufferList[i + 1]));
            }
            byte[] encryptedArray = encryptedList // List<byte[]> to byte[]
                .SelectMany(a => a)
                .ToArray();
            return encryptedArray;
        }

        string DecryptOFB(byte[] cipherText, byte[] Key) // расшифрование AES OFB
        {
            List<byte[]> cipherList = Split(cipherText, 16);
            string decrypted = "";
            List<byte[]> decryptedList = new List<byte[]>();
            List<byte[]> bufferList = new List<byte[]>();
            byte[] IV = cipherList[0]; // IV
            bufferList.Add(IV);
            for (int i = 1; i < cipherList.Count; i++) // расшифрование блоков один за одним 
            {
                bufferList.Add(Encryption(bufferList[i - 1], Key)); // промежуточный блок (bufferList[i])
                decryptedList.Add(
                                XOR(cipherList[i],
                                    bufferList[i])
                                 );
            }
            byte[] decryptedArray = decryptedList  // List<byte[]> to byte[]
                .SelectMany(a => a)
                .ToArray();
            decrypted = Encoding.ASCII.GetString(decryptedArray);
            return decrypted;
        }
    }
}
