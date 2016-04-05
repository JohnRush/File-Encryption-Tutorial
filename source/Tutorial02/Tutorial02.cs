using System;
using System.IO;
using System.Security.Cryptography;

namespace Tutorial
{
    class Tutorial02
    {
        static void Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                Console.WriteLine("Encrypt or Decrypt a file.");
                Console.WriteLine("\nTutorial02 source destination [key]\n");
                Console.WriteLine("{0,-15}Specifies the source file.", "source");
                Console.WriteLine("{0,-15}Specifies the destination file.", "destination");
                Console.WriteLine("{0,-15}The optional decryption key.", "key");
                Console.WriteLine("{0,-15}If provided we decrypt, otherwise we encrypt.", "");
                return;
            }

            var sourceFilename = args[0];
            var destinationFilename = args[1];

            byte[] key = null;
            if (args.Length == 3)
            {
                key = System.Convert.FromBase64String(args[2]);
            }

            if (key == null)
            {
                // Encrypt the source file and write it to the destination file.
                using (var sourceStream = File.OpenRead(sourceFilename))
                using (var destinationStream = File.Create(destinationFilename))
                using (var provider = new AesCryptoServiceProvider())
                using (var cryptoTransform = provider.CreateEncryptor())
                using (var cryptoStream = new CryptoStream(destinationStream, cryptoTransform, CryptoStreamMode.Write))
                {
                    destinationStream.Write(provider.IV, 0, provider.IV.Length);
                    sourceStream.CopyTo(cryptoStream);
                    Console.WriteLine(System.Convert.ToBase64String(provider.Key));
                }
            }
            else
            {
                // Decrypt the source file and write it to the destination file.
                using (var sourceStream = File.OpenRead(sourceFilename))
                using (var destinationStream = File.Create(destinationFilename))
                using (var provider = new AesCryptoServiceProvider())
                {
                    var IV = new byte[provider.IV.Length];
                    sourceStream.Read(IV, 0, IV.Length);
                    using (var cryptoTransform = provider.CreateDecryptor(key, IV))
                    using (var cryptoStream = new CryptoStream(sourceStream, cryptoTransform, CryptoStreamMode.Read))
                    {
                        cryptoStream.CopyTo(destinationStream);
                    }
                }
            }
        }
    }
}