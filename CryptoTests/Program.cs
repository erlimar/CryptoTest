using PemUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CryptoTests
{
    class Program
    {
        private const string MESSAGE_DEFAULT = "Test Message!";

        private string _pathBase;
        private string _message;

        public Program()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            _pathBase = Path.Combine(home, nameof(CryptoTests));
        }

        // ///////////////////////////////////////////////////////////////////

        void Main(IList<string> args)
        {
            ReadPathBase(args);
            ReadMessage(args);

            if (!Directory.Exists(_pathBase))
            {
                Directory.CreateDirectory(_pathBase);
            }

            if (ArgFlagIsPresent(args, "gen"))
            {
                GenerateKeys();
            }

            if (ArgFlagIsPresent(args, "test-xml"))
            {
                PerformXmlTest();
            }

            if (ArgFlagIsPresent(args, "test-pem"))
            {
                PerformPEMTest();
            }
        }

        /// <summary>
        /// Gera chaves
        /// </summary>
        private void GenerateKeys()
        {
            var rsa = new RSACryptoServiceProvider(2048);

            ExportToXml(rsa);
            ExportToPEM(rsa);
        }

        /// <summary>
        /// Testa criptografar/descriptografar mensagem com chaves Xml
        /// </summary>
        private void PerformXmlTest()
        {
            Console.WriteLine("---------------------- PERFORM XML TEST (BEGIN) ----------------------");

            var message = _message ?? MESSAGE_DEFAULT;
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var rsa = ImportFromXml();
            var encryptedMessage = rsa.Encrypt(messageBytes, false);
            var encryptedMessageBase64 = Convert.ToBase64String(encryptedMessage);
            var decryptedMessage = rsa.Decrypt(encryptedMessage, false);
            var decryptedMessageString = Encoding.UTF8.GetString(decryptedMessage);

            Console.WriteLine($"Original message: {message}");
            Console.WriteLine($"Encrypted message: {encryptedMessageBase64}");
            Console.WriteLine($"Decrypted message: {decryptedMessageString}");

            Console.WriteLine("---------------------- PERFORM XML TEST (END) ------------------------");
        }

        /// <summary>
        /// Testa criptografar/descriptografar mensagem com chaves PEM
        /// </summary>
        private void PerformPEMTest()
        {
            Console.WriteLine("---------------------- PERFORM PEM TEST (BEGIN) ----------------------");

            var message = _message ?? MESSAGE_DEFAULT;
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var rsa = ImportFromPEM();
            var encryptedMessage = rsa.Encrypt(messageBytes, false);
            var encryptedMessageBase64 = Convert.ToBase64String(encryptedMessage);
            var decryptedMessage = rsa.Decrypt(encryptedMessage, false);
            var decryptedMessageString = Encoding.UTF8.GetString(decryptedMessage);

            Console.WriteLine($"Original message: {message}");
            Console.WriteLine($"Encrypted message: {encryptedMessageBase64}");
            Console.WriteLine($"Decrypted message: {decryptedMessageString}");

            Console.WriteLine("---------------------- PERFORM PEM TEST (END) ------------------------");
        }

        /// <summary>
        /// Lê a mensagem dos argumentos --message [MESSAGE]
        /// </summary>
        /// <param name="args">Lista de argumentos</param>
        private void ReadMessage(IList<string> args)
        {
            _message = GetArgKeyValue(args, "message");
        }

        /// <summary>
        /// Lê o caminho báse do parâmetro --base [PATH]
        /// </summary>
        /// <param name="args">Lista de argumentos</param>
        private void ReadPathBase(IList<string> args)
        {
            _pathBase = GetArgKeyValue(args, "base") ?? _pathBase;
        }

        /// <summary>
        /// Exporta chaves para arquivos XML
        /// </summary>
        /// <param name="rsa">Chave</param>
        void ExportToXml(RSACryptoServiceProvider rsa)
        {
            var allParamsXml = rsa.ToXmlString(true);
            var publicParamsXml = rsa.ToXmlString(false);
            var outputPathPrivate = Path.Combine(_pathBase, "key-private.xml");
            var outputPathPublic = Path.Combine(_pathBase, "key-public.xml");

            File.WriteAllText(outputPathPrivate, allParamsXml);
            File.WriteAllText(outputPathPublic, publicParamsXml);
        }

        /// <summary>
        /// Importa chave de arquivos XML
        /// </summary>
        /// <returns>Instância de <see cref="RSACryptoServiceProvider"/></returns>
        RSACryptoServiceProvider ImportFromXml()
        {
            var rsa = new RSACryptoServiceProvider();
            var pathPrivate = Path.Combine(_pathBase, "key-private.xml");
            var privateXml = File.ReadAllText(pathPrivate);

            rsa.FromXmlString(privateXml);

            return rsa;
        }

        /// <summary>
        /// Exporta chaves para arquivos PEM
        /// </summary>
        /// <param name="rsa">Chave</param>
        void ExportToPEM(RSACryptoServiceProvider rsa)
        {
            var allParams = rsa.ExportParameters(true);
            var outputPathPrivate = Path.Combine(_pathBase, "key-private.pem");
            var outputPathPublic = Path.Combine(_pathBase, "key-public.pem");

            using (var stream = File.Create(outputPathPrivate))
            using (var writer = new PemWriter(stream))
            {
                writer.WritePrivateKey(allParams);
            }

            using (var stream = File.Create(outputPathPublic))
            using (var writer = new PemWriter(stream))
            {
                writer.WritePublicKey(allParams);
            }
        }

        /// <summary>
        /// Importa chave de arquivos PEM
        /// </summary>
        /// <returns>Instância de <see cref="RSACryptoServiceProvider"/></returns>
        RSACryptoServiceProvider ImportFromPEM()
        {
            var rsa = new RSACryptoServiceProvider();
            var pathPrivate = Path.Combine(_pathBase, "key-private.pem");

            using (var stream = File.OpenRead(pathPrivate))
            using (var reader = new PemReader(stream))
            {
                var rsaParameters = reader.ReadRsaKey();

                rsa.ImportParameters(rsaParameters);
            }

            return rsa;
        }

        // ///////////////////////////////////////////////////////////////////

        private bool ArgFlagIsPresent(IList<string> args, string flagName)
        {
            return args.Any(w => w == $"--{flagName}");
        }

        private string GetArgKeyValue(IList<string> args, string keyName)
        {
            int idx = args.IndexOf($"--{keyName}");

            if (idx > -1 && args.Count > idx + 1)
            {
                return args[idx + 1];
            }

            return null;
        }

        // ///////////////////////////////////////////////////////////////////

        static void Main(string[] args)
            => new Program().Main(args.ToList());
    }
}
