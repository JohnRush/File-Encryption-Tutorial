using System;
using System.IO;
using System.Security.Cryptography;

namespace Tutorial
{
    class Tutorial03
    {
        static void Main(string[] args)
        {
            if (args.Length < 3 || args.Length > 4)
            {
                Console.WriteLine("Encrypt or Decrypt a file.");
                Console.WriteLine("\nTutorial03 [-e] [-d] source destination [key]\n");
                Console.WriteLine("{0,-15}Sets the mode to ENCRYPT.", "-e");
                Console.WriteLine("{0,-15}Sets the mode to DECRYPT.", "-d");
                Console.WriteLine("{0,-15}Specifies the source file.", "source");
                Console.WriteLine("{0,-15}Specifies the destination file.", "destination");
                Console.WriteLine("{0,-15}The optional key.", "key");
                return;
            }

            var mode = args[0];
            var sourceFilename = args[1];
            var destinationFilename = args[2];

            byte[] key = null;
            if (args.Length == 4)
            {
                try
                {
                    key = System.Convert.FromBase64String(args[3]);
                }
                catch (System.FormatException)
                {
                    var password = args[3];
                    var salt = System.Convert.FromBase64String("GulO8InaX2CwJw ==");
                    using (var converter = new Rfc2898DeriveBytes(password, salt))
                    {
                        key = converter.GetBytes(32);
                    }
                }
            }

            if (mode == "-e")
            {
                // Encrypt the source file and write it to the destination file.
                using (var sourceStream = File.OpenRead(sourceFilename))
                using (var destinationStream = File.Create(destinationFilename))
                using (var provider = new AesCryptoServiceProvider())
                {
                    if (key != null)
                    {
                        provider.Key = key;
                    }

                    using (var cryptoTransform = provider.CreateEncryptor())
                    using (var cryptoStream = new CryptoStream(destinationStream, cryptoTransform, CryptoStreamMode.Write))
                    {
                        destinationStream.Write(provider.IV, 0, provider.IV.Length);
                        sourceStream.CopyTo(cryptoStream);
                        if (key == null)
                        {
                            Console.WriteLine(System.Convert.ToBase64String(provider.Key));
                        }
                    }
                }
            }
            else if (mode == "-d")
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
            else
            {
                Console.WriteLine("Must select the ecryption/decryption mode using -e or -d.");
            }
        }
    }
}