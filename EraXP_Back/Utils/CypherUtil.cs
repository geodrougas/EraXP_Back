using System.IO.Compression;
using System.Security.Cryptography;

namespace EraXP_Back.Utils;

public class CypherUtil (
    string keyStr
)
{
    private byte[] _key = Convert.FromHexString(keyStr);
    
    public byte[] EncryptStringToBytes_Aes(string plainText)
    {
        // Check arguments.
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (_key == null || _key.Length <= 0)
            throw new ArgumentNullException("Key");

        byte[] iv = new byte[16];

        RandomNumberGenerator.Fill(iv);
        
        byte[] encrypted;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = _key;
            aesAlg.IV = iv;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }
        
        // Return the encrypted bytes from the memory stream.
        return zip(iv, encrypted);
    }

    public byte[] zip(params byte[][] args)
    {
        using (MemoryStream input = new MemoryStream())
        using (MemoryStream output = new MemoryStream())
        {
            foreach(byte[] param in args) {
                input.Write(param);
            }

            using (var compressor = new GZipStream(output, CompressionMode.Compress))
            {
                input.Position = 0;
                input.CopyTo(compressor);
            }
            
            return output.ToArray();
        }
    }

    public byte[] Unzip(byte[] compressedFile)
    {
        using (MemoryStream input = new MemoryStream())
        using (MemoryStream output = new MemoryStream())
        using (var decompressor = new GZipStream(input, CompressionMode.Decompress))
        {
            input.Write(compressedFile);
            input.Position = 0;
            decompressor.CopyTo(output);
            return output.ToArray();
        }
    }

    private ValueTuple<byte[], byte[]> SplitIV(byte[] array)
    {
        byte[] iv = new byte[16];
        byte[] cypher = new byte[array.Length - 16];
        Array.Copy(array, 0, iv, 0, iv.Length);
        Array.Copy(array, iv.Length, cypher, 0, cypher.Length);
        return (iv, cypher);
    }

    public string DecryptStringFromBytes_Aes(byte[] compressedCypherText)
    {
        // Check arguments.
        if (compressedCypherText == null || compressedCypherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (_key == null || _key.Length <= 0)
            throw new ArgumentNullException("Key");

        byte[] cipherText = Unzip(compressedCypherText);
        
        var (iv, cypher) = SplitIV(cipherText);
        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;
        

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = _key;
            aesAlg.IV = iv;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cypher))
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
}
