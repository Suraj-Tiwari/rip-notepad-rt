using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using CommandLine;

namespace RipNotepadRT
{
    class Program
    {

        public class Options
        {
            [Option('i', "inputFile", Required = true, HelpText = "input file to encrypt for notepad rt (ex: input.txt, input.xtt (if decrypting))")]
            public string Input { get; set; }

            [Option('o', "outputFile", Required = true, HelpText = "output file for enxrypted file (ex: Output.xtt, Output.txt (if decrypting))")]
            public string Output { get; set; }

            [Option('d', "decrypt", Default = false, Required = false, HelpText = "output file for enxrypted file (ex: Output.xtt)")]
            public bool Decrypt { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed(run);
        }

        static void run(Options opts)
        {
            string text = File.ReadAllText(opts.Input);
            string encrypted = fuckNotepadRT(text, opts.Decrypt);
            FileStream fileStream = new FileStream(opts.Output, FileMode.OpenOrCreate, FileAccess.Write);
            fileStream.SetLength((long)0);
            byte[] bytes = Encoding.ASCII.GetBytes(encrypted);
            fileStream.Write(bytes, 0, (int)bytes.Length);
            fileStream.Close();
            Console.WriteLine("Success");
        }


        static string fuckNotepadRT(string data, bool decrypt = false)
        {
            string str = "9652AKO98CCR796";
            byte[] actionData = decrypt ? Convert.FromBase64String(data): Encoding.Unicode.GetBytes(data);
            Aes aes = Aes.Create();
            try
            {
                Rfc2898DeriveBytes rfc2898DeriveByte = new Rfc2898DeriveBytes(str, new byte[] { 73, 118, 97, 110, 32, 77, 101, 100, 118, 101, 100, 101, 118 });
                aes.Key = rfc2898DeriveByte.GetBytes(32);
                aes.IV = rfc2898DeriveByte.GetBytes(16);
                MemoryStream memoryStream = new MemoryStream();

                CryptoStream cryptoStream = new CryptoStream(memoryStream, decrypt? aes.CreateDecryptor() :aes.CreateEncryptor(), CryptoStreamMode.Write);
                try
                {
                    cryptoStream.Write(actionData, 0, (int)actionData.Length);
                    cryptoStream.Close();
                }
                finally
                {
                    if (cryptoStream != null)
                    {
                        ((IDisposable)cryptoStream).Dispose();
                    }
                }
                if (decrypt)
                    data = Encoding.Unicode.GetString(memoryStream.ToArray());
                else
                    data = Convert.ToBase64String(memoryStream.ToArray());
            }
            finally
            {
                if (aes != null)
                {
                    ((IDisposable)aes).Dispose();
                }
            }
            return data;
        }
    }
}
