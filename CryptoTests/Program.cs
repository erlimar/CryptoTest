using PemUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace CryptoTests
{
    class Program
    {
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
                var rsa = new RSACryptoServiceProvider(2048);

                ExportaXml(rsa);
                ExportaPEM(rsa);
            }
        }

        /// <summary>
        /// Lê a mensagem dos argumentos
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
        /// Cria um par de chaves pública e privada e salva em arquivos XML
        /// </summary>
        /// <param name="rsa">Chave</param>
        void ExportaXml(RSACryptoServiceProvider rsa)
        {
            var allParamsXml = rsa.ToXmlString(true);
            var publicParamsXml = rsa.ToXmlString(false);
            var outputPathPrivate = Path.Combine(_pathBase, "key-private.xml");
            var outputPathPublic = Path.Combine(_pathBase, "key-public.xml");

            File.WriteAllText(outputPathPrivate, allParamsXml);
            File.WriteAllText(outputPathPublic, publicParamsXml);
        }

        /// <summary>
        /// Cria um par de chaves pública e privada e salva em arquivos PEM
        /// </summary>
        /// <param name="rsa">Chave</param>
        void ExportaPEM(RSACryptoServiceProvider rsa)
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
