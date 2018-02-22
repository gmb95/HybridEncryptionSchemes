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
    public partial class OCBForm : Form
    {

        byte[] K = new byte[16];        
        byte[] N = new byte[12];
        StringBuilder K_str = new StringBuilder();        
        StringBuilder N_str = new StringBuilder();

        RSACryptoServiceProvider RSA_B = new RSACryptoServiceProvider();

        public OCBForm()
        {
            InitializeComponent();
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

            rngCsp.GetBytes(K);
            foreach (byte b in K) K_str.AppendFormat("{0:x2}", b);
                    
            rngCsp.GetBytes(N);
            foreach (byte b in N) N_str.AppendFormat("{0:x2}", b);
        }

        private void Run_bt_Click(object sender, EventArgs e)
        {
            RSAParameters bobSecretKeys = RSA_B.ExportParameters(true);
            RSAParameters bobPublicKeys = RSA_B.ExportParameters(false);
            richTextBoxA.AppendText("Открытый ключ Боба: \nE =" + Convert.ToBase64String(bobPublicKeys.Exponent) + "\nМодуль = " + Convert.ToBase64String(bobPublicKeys.Modulus) + "\n");
            //////////////////////////////Alice's side
            string AliceMessage = "Alice`s message";
            byte[] AliceK = K;
            byte[] AliceN = N;
            richTextBoxA.AppendText("\nСообщение Алисы: " + AliceMessage + "\nКлюч К = " + K_str.ToString() + "\nNonce = " + N_str.ToString() + "\n");
            byte[] AliceM0 = EncryptOCB(AliceMessage, AliceK, AliceN);
            StringBuilder AliceM0_str = new StringBuilder();
            foreach (byte b in AliceM0) AliceM0_str.AppendFormat("{0:x2}", b);
            richTextBoxA.AppendText("Алиса Зашифровала симметричным алгоритмом сообщение: " + AliceM0_str.ToString() + "\n");
            byte[] AliceTag = Tagging(AliceMessage, AliceK, AliceN, "Alice");
            StringBuilder AliceTag_str = new StringBuilder();
            foreach (byte b in AliceTag) AliceTag_str.AppendFormat("{0:x2}", b);
            richTextBoxA.AppendText("Алиса посчитала tag: " + AliceTag_str.ToString() + "\n");
            byte[] AliceM1 = ArrayPlus(AliceM0, AliceTag);
            byte[] KN = ArrayPlus(AliceK, AliceN);
            byte[] AliceM2 = RSAEncrypt(KN, bobPublicKeys, false);
            StringBuilder AliceM1_str = new StringBuilder();
            foreach (byte b in AliceM2) AliceM1_str.AppendFormat("{0:x2}", b);
            richTextBoxA.AppendText("\nАлиса Зашифровала асимметричным алгоритмом K+N: " + AliceM1_str.ToString() + "\n");
            byte[] M = ArrayPlus(AliceM1, AliceM2);
            StringBuilder M_str = new StringBuilder();
            foreach (byte b in M) M_str.AppendFormat("{0:x2}", b);
            richTextBoxA.AppendText("\nАлиса передаёт сообщение 'Зашифрованный текс + Tag + Зашифрованный K+N': " + M_str.ToString());

            //////////////////////////////Bob's side
            richTextBoxB.AppendText("Закрытый ключ Боба: \nD =" + Convert.ToBase64String(bobSecretKeys.D) + "\nМодуль = " + Convert.ToBase64String(bobSecretKeys.Modulus) + "\n");
            byte[] BobM0 = new byte[AliceM0.Length];
            byte[] BobTag = new byte[AliceTag.Length];
            byte[] BobM2 = new byte[AliceM2.Length];
            richTextBoxB.AppendText("\nБоб получает сообщение: " + M_str.ToString() + "\n");

            Array.Copy(M, BobM0, BobM0.Length);
            StringBuilder BobM0_str = new StringBuilder();
            foreach (byte b in BobM0) BobM0_str.AppendFormat("{0:x2}", b);            

            for (int i = BobM0.Length, j = 0; i < BobM0.Length + BobTag.Length; i++, j++)
            {
                BobTag[j] = M[i];
            }
            StringBuilder BobTag_str = new StringBuilder();
            foreach (byte b in BobTag) BobTag_str.AppendFormat("{0:x2}", b);
            richTextBoxB.AppendText("\nБоб выделяет из сообщения часть зашифрованную симметричным алгоритмом: " + BobM0_str.ToString() + "\nи Tag: "+ BobTag_str.ToString() + "\n");
            for (int i = BobM0.Length + BobTag.Length, j = 0; i < M.Length; i++, j++)
            {
                BobM2[j] = M[i];
            }
            StringBuilder BobM2_str = new StringBuilder();
            foreach (byte b in BobM2) BobM2_str.AppendFormat("{0:x2}", b);
            richTextBoxB.AppendText("\nБоб выделяет из сообщения часть зашифрованную асимметричным алгоритмом: " + BobM2_str.ToString() + "\n");

            byte[] decBobM2 = RSADecrypt(BobM2, bobSecretKeys, false);
            StringBuilder decBobM2_str = new StringBuilder();
            foreach (byte b in decBobM2) decBobM2_str.AppendFormat("{0:x2}", b);
            richTextBoxB.AppendText("\nБоб расшифровывает вторую часть сообщения с помощью своего сектреного ключа: " + decBobM2_str.ToString() + "\n");

            byte[] BobK = new byte[K.Length];
            Array.Copy(decBobM2, BobK, K.Length);
            StringBuilder BobK_str = new StringBuilder();
            foreach (byte b in BobK) BobK_str.AppendFormat("{0:x2}", b);

            byte[] BobN = new byte[N.Length];
            Array.ConstrainedCopy(decBobM2, K.Length, BobN, 0, N.Length);
            StringBuilder BobN_str = new StringBuilder();
            foreach (byte b in BobN) BobN_str.AppendFormat("{0:x2}", b);
            richTextBoxB.AppendText("\nБоб получает из расшифрованного сообщения \nключ К: " + BobK_str.ToString() + "\nNonce: " + BobN_str.ToString() + "\n");
            string Message = DecryptOCB(BobM0, BobK, BobN);
            richTextBoxB.AppendText("\nС помощью полученных ключа и Nonce Боб расшифровывает первую часть сообщения: " + Message);
            byte[] BobTag1 = Tagging(Message, BobK, BobN, "Alice");
            StringBuilder BobTag1_str = new StringBuilder();
            foreach (byte b in BobTag1) BobTag1_str.AppendFormat("{0:x2}", b);

            if (BobTag_str.ToString() == BobTag1_str.ToString())
            {
                richTextBoxB.AppendText("\nКод аутентификации совпадает\n");
            }
        }

        static byte[] EncryptOCB(string plainText, byte[] K, byte[] N)
        {
            List<byte[]> plainList = Split(Encoding.ASCII.GetBytes(plainText), 16); //разбиваем на блоки по 16 байт
            bool padding = false;
            int lastLength = plainList[plainList.Count - 1].Length;
            if (lastLength < 16)
            {
                padding = true;
            }

            int i = plainList.Count;
            BitArray[] Offsets = new BitArray[i + 3];
            Offsets[0] = init(N, K);
            BitArray[] L = L_table_cteate(N, K, i);
            for (int k = 1; k <= i; k++)
            {
                Offsets[k] = inc(Offsets[k - 1], k, L);
            }
            int Offset_m = Offsets.Length - 2;
            int Offset_s = Offsets.Length - 1;
            Offsets[Offset_m] = inc_m(Offsets[Offsets.Length - 3], L);
            Offsets[Offset_s] = inc_s(Offsets[Offsets.Length - 2], L);
            List<byte[]> encryptedList = new List<byte[]>();

            for (int block = 0; block < plainList.Count - 1; block++) //шифруем все блоки, кроме последнего
            {
                encryptedList.Add(XOR(Encryption(XOR(plainList[block], BitArrayToByteArray(Offsets[block + 1])), K), BitArrayToByteArray(Offsets[block + 1])));
            }
            //шифрование последнего блока
            if (padding == true)
            {
                byte[] buf = Encryption(BitArrayToByteArray(Offsets[Offset_m]), K);
                byte[] cuttedbuf = new byte[lastLength];
                Array.Copy(buf, cuttedbuf, cuttedbuf.Length); // урезаем до размеров последнего блока, чтобы можно было заксорить
                byte[] lastCB = XOR(cuttedbuf, plainList[plainList.Count - 1]);
                encryptedList.Add(lastCB);
            }
            else //если блок был нормального размера, то шифруем также как и остальные
            {
                encryptedList.Add(XOR(Encryption(XOR(plainList[plainList.Count - 1], BitArrayToByteArray(Offsets[plainList.Count])), K), BitArrayToByteArray(Offsets[plainList.Count])));
            }

            byte[] encryptedArray = encryptedList // List<byte[]> to byte[]
                .SelectMany(a => a)
                .ToArray();
            return encryptedArray;
        }

        static string DecryptOCB(byte[] cipherText, byte[] K, byte[] N)
        {
            List<byte[]> cipherList = Split(cipherText, 16); //разбиваем на блоки по 16 байт
            string decrypted = "";
            bool padding = false;
            int lastLength = cipherList[cipherList.Count - 1].Length;
            if (lastLength < 16)
            {
                padding = true;
            }

            int i = cipherList.Count;
            BitArray[] Offsets = new BitArray[i + 3];
            Offsets[0] = init(N, K);
            BitArray[] L = L_table_cteate(N, K, i);
            for (int k = 1; k <= i; k++)
            {
                Offsets[k] = inc(Offsets[k - 1], k, L);
            }
            int Offset_m = Offsets.Length - 2;
            int Offset_s = Offsets.Length - 1;
            Offsets[Offset_m] = inc_m(Offsets[Offsets.Length - 3], L);
            Offsets[Offset_s] = inc_s(Offsets[Offsets.Length - 2], L);
            List<byte[]> decryptedList = new List<byte[]>();

            for (int block = 0; block < cipherList.Count - 1; block++) //расшифруем все блоки, кроме последнего
            {
                decryptedList.Add(XOR(Decryption(XOR(cipherList[block], BitArrayToByteArray(Offsets[block + 1])), K), BitArrayToByteArray(Offsets[block + 1])));
            }
            //расшифрование последнего блока
            if (padding == true)
            {
                byte[] encryptedOffset = Encryption(BitArrayToByteArray(Offsets[Offset_m]), K);
                byte[] encryptedOffset_cut = new byte[lastLength];
                Array.Copy(encryptedOffset, encryptedOffset_cut, encryptedOffset_cut.Length); // урезаем до размеров последнего блока, чтобы можно было заксорить
                byte[] lastCB = XOR(encryptedOffset_cut, cipherList[cipherList.Count - 1]);
                decryptedList.Add(lastCB);
            }
            else //если блок был нормального размера, то шифруем также как и остальные
            {
                decryptedList.Add(XOR(Decryption(XOR(cipherList[cipherList.Count - 1], BitArrayToByteArray(Offsets[cipherList.Count])), K), BitArrayToByteArray(Offsets[cipherList.Count])));
            }

            byte[] decryptedArray = decryptedList  // List<byte[]> to byte[]
                .SelectMany(a => a)
                .ToArray();
            decrypted = Encoding.ASCII.GetString(decryptedArray);
            return decrypted;
        }


        static byte[] Tagging(string text, byte[] K, byte[] N, string A)
        {
            List<byte[]> plainList = Split(Encoding.ASCII.GetBytes(text), 16);

            byte[] CheckSum = new byte[16];
            int lastLength = plainList[plainList.Count - 1].Length;
            bool padding = false;
            int i = plainList.Count;
            BitArray[] Offsets = new BitArray[i + 3];
            Offsets[0] = init(N, K);
            BitArray[] L = L_table_cteate(N, K, i);
            for (int k = 1; k <= i; k++)
            {
                Offsets[k] = inc(Offsets[k - 1], k, L);
            }
            int Offset_m = Offsets.Length - 2;
            int Offset_s = Offsets.Length - 1;
            Offsets[Offset_m] = inc_m(Offsets[Offsets.Length - 3], L);
            Offsets[Offset_s] = inc_s(Offsets[Offsets.Length - 2], L);

            BitArray paddedBlock = new BitArray(128);
            BitArray lastBlock = new BitArray(plainList[plainList.Count - 1]);
            if (lastLength < 16) // дополнение последнего блока
            {
                padding = true;
                for (int q = 0; q < lastBlock.Length; q++)
                {
                    paddedBlock[q] = lastBlock[q];
                }
                paddedBlock[lastBlock.Length] = true; //дополнили блок 10*
            }
            for (int block = 0; block < plainList.Count - 1; block++)
            {
                CheckSum = XOR(plainList[block], CheckSum);//
            }
            if (padding == true)
            {
                CheckSum = XOR(BitArrayToByteArray(paddedBlock), CheckSum);
            }
            else
            {
                CheckSum = XOR(plainList[plainList.Count - 1], CheckSum);//
            }

            byte[] Auth = Auth_create(A, K, N);
            byte[] Tag = XOR(Encryption(XOR(CheckSum, BitArrayToByteArray(Offsets[Offset_s])), K), Auth);
            return Tag;
        }

        static byte[] Auth_create(string A, byte[] K, byte[] N)
        {
            List<byte[]> AList = Split(Encoding.ASCII.GetBytes(A), 16);
            int lastLength = AList[AList.Count - 1].Length;
            bool padding = false;
            BitArray paddedBlock = new BitArray(128);
            BitArray lastBlock = new BitArray(AList[AList.Count - 1]);
            if (lastLength < 16) // дополнение последнего блока
            {
                padding = true;
                for (int q = 0; q < lastBlock.Length; q++)
                {
                    paddedBlock[q] = lastBlock[q];
                }
                paddedBlock[lastBlock.Length] = true; //дополнили блок 10*  
            }
            int i = AList.Count;
            BitArray[] Offsets = new BitArray[i + 2];
            Offsets[0] = new BitArray(128);
            BitArray[] L = L_table_cteate(N, K, i);
            for (int k = 1; k <= i; k++)
            {
                Offsets[k] = inc(Offsets[k - 1], k, L);
            }
            int Offset_m = Offsets.Length - 1;
            Offsets[Offset_m] = inc_m(Offsets[Offsets.Length - 2], L);

            List<byte[]> encryptedList = new List<byte[]>();

            for (int block = 0; block < AList.Count - 1; block++) //считаем все блоки, кроме последнего
            {
                encryptedList.Add(Encryption(XOR(AList[block], BitArrayToByteArray(Offsets[block + 1])), K));
            }
            if (padding == true) //если последний блок был меньше 16 байт
            {
                encryptedList.Add(Encryption(XOR(BitArrayToByteArray(paddedBlock), BitArrayToByteArray(Offsets[Offset_m])), K));
            }
            else //если последний блок был 16 байт
            {
                encryptedList.Add(Encryption(XOR(AList[AList.Count - 1], BitArrayToByteArray(Offsets[AList.Count])), K));
            }
            byte[] Auth = new byte[16];
            for (int q = 0; q < encryptedList.Count; q++)
            {
                Auth = XOR(encryptedList[q], Auth);
            }
            return Auth;
        }

        static BitArray inc(BitArray offset, int i, BitArray[] L)
        {
            return offset.Xor(L[ntz(i)]);
        }

        static BitArray inc_m(BitArray offset, BitArray[] L)
        {
            return offset.Xor(L[L.Length - 2]);
        }

        static BitArray inc_s(BitArray offset, BitArray[] L)
        {
            return offset.Xor(L[L.Length - 1]);
        }

        static BitArray[] L_table_cteate(byte[] N, byte[] K, int i)
        {
            byte[] nullbyte = new byte[16];
            byte[] Lm = Encryption(nullbyte, K);
            BitArray bitL_m = new BitArray(Lm);
            BitArray bitL_s = bitdouble(bitL_m);
            int maxj = 0;
            for (int j = 1; j <= i; j++) //считаем максимальное ntz для данного количества блоков
            {
                int maxj_buf = ntz(j);
                if (maxj_buf > maxj)
                    maxj = maxj_buf;
            }
            BitArray[] L = new BitArray[maxj + 2]; //+2, чтобы в конец записать L_m и L_s
            L[0] = bitdouble(bitL_s);

            for (int j = 1; j < maxj; j++)
            {
                L[j] = bitdouble(L[j - 1]);
            }
            L[maxj] = bitL_m;
            L[maxj + 1] = bitL_s;
            return L;
        }

        static int ntz(int a) //количество нулей в конце двоичного представления числа
        {
            int b = 0;
            BitArray buf = new BitArray(BitConverter.GetBytes(a));
            for (int i = 0; i < buf.Length; i++) //т.к GetBytes возвращает байты в Little Endian, то последние биты будут в начале
            {
                if (buf[i] == false)
                {
                    b++;
                }
                else
                    break;
            }
            return b;
        }

        static BitArray bitdouble(BitArray S)
        {
            BitArray ShiftedS = new BitArray(S.Length);
            if (S[0] == false)
            {
                ShiftedS = leftShift(S, 1);
            }
            else
            {

                ShiftedS = leftShift(S, 1);
                BitArray buf = new BitArray(BitConverter.GetBytes(135));
                BitArray buf2 = new BitArray(128);
                for (int i = 7; i >= 0; i--)
                {
                    buf2[127 - i] = buf[i];
                }
                ShiftedS.Xor(buf2);
            }
            return ShiftedS;
        }

        static BitArray init(byte[] nonce, byte[] K)
        {

            BitArray bitNonce = new BitArray(nonce);
            BitArray Bottom = new BitArray(6);
            for (int i = 90, j = 0; i < 96; i++, j++)
            {
                Bottom[j] = bitNonce[i];
            }

            BitArray Top = new BitArray(128);
            Top[31] = true;
            for (int i = 32, j = 0; i < 122; i++, j++)
            {
                Top[i] = bitNonce[j];
            }
            byte[] byteTop = BitArrayToByteArray(Top);
            byte[] Ktop = Encryption(byteTop, K);
            BitArray bitKtop = new BitArray(Ktop);
            BitArray ShiftedbTop = leftShift(bitKtop, 8);

            BitArray Stratch = new BitArray(128);
            BitArray buf_bitKtop = bitKtop;
            Stratch = bitKtop.Or(buf_bitKtop.Xor(ShiftedbTop));
            BitArray Offset = leftShift(Stratch, Convert.ToInt32(BitArrayToByteArray(Bottom)[0]));
            return Offset;
        }

        static BitArray leftShift(BitArray a, int b) //побитовый сдвиг на b бит
        {
            BitArray c = new BitArray(a.Length);
            for (int i = b, j = 0; i < a.Length; i++, j++)
            {
                c[j] = a[i];
            }
            return c;
        }

        static byte[] Encryption(byte[] plainText, byte[] Key)
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

        static byte[] Decryption(byte[] cipherText, byte[] Key)
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
        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }
        static byte[] XOR(byte[] a1, byte[] a2) // побитовый xor двух байтовых массивов (первый может иметь большую длину)
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
        static List<byte[]> Split(byte[] bt, int chunkSize) // разбиение байтового массива bt на блоки длиной chunkSize
        {
            List<byte[]> list = new List<byte[]>();
            list = bt.Select((by, i) => new { group = i / chunkSize, value = by })
           .GroupBy(item => item.group)
           .Select(group => group.Select(v => v.value).ToArray())
           .ToList();
            return list;
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
