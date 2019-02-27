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
        private const string HELP_TEXT = @"CryptoTests - Perform Key Pair Encrypt Tests

usage: CryptoTests [OPTIONS]

Options:
  --help, -h            Show this help message
  --base <BASE_PATH>    Set a base directory path.
                        > Default is ""$HOME\CryptoTests""
  --message <MESSAGE>   Set a message content to encrypt/decrypt tests
                        > Default is ""Test Message!""
  --pubkey <FILE_PATH>  Set a public key file path
  --privkey <FILE_PATH> Set a private key file path
  --gen, -g             Generate a encrypt key pair files on ""--base"" directory:
                        > key-private.pem  Private key in PEM format
                        > key-public.pem   Public key in PEM format
                        > key-private.xml  Private key in XML format
                        > key-public.xml   Public key in XML format
  --test-xml            Perform encrypt/decrypt tests with generated XML files
  --test-pem            Perform encrypt/decrypt tests with generated PEM files
  --encrypt, -e         Encrypt message
  --decrypt, -d         Decrypt message
  --debug               Waiting for debugger attachment
";

        private string _pathBase;
        private string _message;
        private string _publicKeyPath;
        private string _privateKeyPath;

        public Program()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            _pathBase = Path.Combine(home, nameof(CryptoTests));
        }

        // ///////////////////////////////////////////////////////////////////

        void Main(IList<string> args)
        {
            try
            {
                if (ArgFlagIsPresent(args, "--debug"))
                {
                    while (!System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }

                if (ArgFlagIsPresent(args, "--help|-h"))
                {
                    ShowHelp();

                    return;
                }

                ReadPathBase(args);
                ReadMessage(args);
                ReadPublicKey(args);
                ReadPrivateKey(args);

                if (!Directory.Exists(_pathBase))
                {
                    Directory.CreateDirectory(_pathBase);
                }

                bool executed = false;

                if (ArgFlagIsPresent(args, "--gen|-g"))
                {
                    executed = true;
                    GenerateKeys();
                }

                if (ArgFlagIsPresent(args, "--test-xml"))
                {
                    executed = true;
                    PerformXmlTest();
                }

                if (ArgFlagIsPresent(args, "--test-pem"))
                {
                    executed = true;
                    PerformPEMTest();
                }

                if (ArgFlagIsPresent(args, "--encrypt|-e"))
                {
                    executed = true;
                    EncryptMessage();
                }

                if (ArgFlagIsPresent(args, "--decrypt|-d"))
                {
                    executed = true;
                    DecryptMessage();
                }

                if (!executed)
                {
                    ShowHelp();
                    Environment.ExitCode = 1;
                }
            }
            catch (Exception ex)
            {
                Console.Error.Write("Erro: ");
                Console.Error.WriteLine(ex.Message);

                Environment.ExitCode = 2;
            }
        }

        private void ShowHelp()
        {
            Console.Write(HELP_TEXT);
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
        /// Criptografa uma mensagem
        /// </summary>
        private void EncryptMessage()
        {
            if (string.IsNullOrEmpty(_message))
            {
                throw new Exception("Parameter --message is required!");
            }

            RSACryptoServiceProvider rsa = null;

            if (!string.IsNullOrEmpty(_publicKeyPath) && _publicKeyPath.EndsWith(".pem", StringComparison.OrdinalIgnoreCase))
            {
                rsa = ImportFromPEM(Path.GetFullPath(_publicKeyPath));
            }
            else if (!string.IsNullOrEmpty(_publicKeyPath) && _publicKeyPath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                rsa = ImportFromXml(Path.GetFullPath(_publicKeyPath));
            }
            else
            {
                rsa = ImportFromPEM();
            }

            var messageBytes = Encoding.UTF8.GetBytes(_message);
            var encryptedMessage = rsa.Encrypt(messageBytes, false);
            var encryptedMessageBase64 = Convert.ToBase64String(encryptedMessage);

            Console.WriteLine($"Encrypted message: {encryptedMessageBase64}");
        }

        /// <summary>
        /// Descriptografa uma mensagem
        /// </summary>
        /// <remarks>A mensagem deve estar codificada em base64</remarks>
        private void DecryptMessage()
        {
            if (string.IsNullOrEmpty(_message))
            {
                throw new Exception("Parameter --message is required!");
            }

            RSACryptoServiceProvider rsa = null;

            if (!string.IsNullOrEmpty(_privateKeyPath) && _privateKeyPath.EndsWith(".pem", StringComparison.OrdinalIgnoreCase))
            {
                rsa = ImportFromPEM(Path.GetFullPath(_privateKeyPath));
            }
            else if (!string.IsNullOrEmpty(_privateKeyPath) && _privateKeyPath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                rsa = ImportFromXml(Path.GetFullPath(_privateKeyPath));
            }
            else
            {
                rsa = ImportFromPEM();
            }

            var messageBytes = Convert.FromBase64String(_message);
            var decryptedMessage = rsa.Decrypt(messageBytes, false);
            var decryptedMessageString = Encoding.UTF8.GetString(decryptedMessage);

            Console.WriteLine($"Decrypted message: {decryptedMessageString}");
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
        /// Lê o caminho para arquivo de chave pública
        /// </summary>
        /// <param name="args">Lista de argumentos</param>
        private void ReadPublicKey(IList<string> args)
        {
            _publicKeyPath = GetArgKeyValue(args, "pubkey");
        }

        /// <summary>
        /// Lê o caminho para arquivo de chave privada
        /// </summary>
        /// <param name="args">Lista de argumentos</param>
        private void ReadPrivateKey(IList<string> args)
        {
            _privateKeyPath = GetArgKeyValue(args, "privkey");
        }

        /// <summary>
        /// Lê o caminho base do parâmetro --base [PATH]
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
        /// <param name="filePath">Caminho do arquivo</param>
        /// <returns>Instância de <see cref="RSACryptoServiceProvider"/></returns>
        RSACryptoServiceProvider ImportFromXml(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_pathBase, "key-private.xml");
            }

            var rsa = new RSACryptoServiceProvider();
            var privateXml = File.ReadAllText(filePath);

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
        /// <param name="filePath">Caminho do arquivo</param>
        /// <returns>Instância de <see cref="RSACryptoServiceProvider"/></returns>
        RSACryptoServiceProvider ImportFromPEM(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_pathBase, "key-private.pem");
            }

            var rsa = new RSACryptoServiceProvider();

            using (var stream = File.OpenRead(filePath))
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
            bool found = false;

            flagName.Split('|')
                .ToList()
                .ForEach((flag) =>
                {
                    found = found || args.Any(w => w == flag);
                });

            return found;
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
