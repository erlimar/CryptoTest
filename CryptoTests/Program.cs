using PemUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTests
{
    class Program
    {
        private string _pathBase = @"C:\Users\Erlimar\Desktop\CryptoTests";

        void Main(IList<string> args)
        {
            ObtemPathBase(args);

            if (args.Any(w => w == "--gerar"))
            {
                var rsa = new RSACryptoServiceProvider(2048);

                ExportaXml(rsa);
                ExportaPEM(rsa);
            }
        }

        /// <summary>
        /// Lê o caminho báse do parâmetro --base [PATH]
        /// </summary>
        /// <param name="args">Lista de argumentos</param>
        private void ObtemPathBase(IList<string> args)
        {
            int idx = args.IndexOf("--base");

            if (idx > -1 && args.Count > idx)
            {
                _pathBase = Path.GetFullPath(args[idx + 1]);
            }
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

        static void Main(string[] args)
            => new Program().Main(args.ToList());
    }
}
