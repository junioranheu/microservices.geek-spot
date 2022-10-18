using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TimeZoneConverter;
using Utils.Entities;

namespace Utils
{
    public static class Biblioteca
    {
        // Pegar informações do appsettings: https://stackoverflow.com/a/58432834 (Necessário instalar o pacote "Microsoft.Extensions.Configuration.Json");
        static readonly string emailApiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SendGridSettings")["EmailApiKey"];
        static readonly string emailPadrao = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SendGridSettings")["EmailPadrao"];
        static readonly string emailNomeRemetente = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SendGridSettings")["NomeRemetente"];
        static readonly string urlFrontDev = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("URLSettings")["FrontDev"];
        static readonly string urlFrontProd = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("URLSettings")["FrontProd"];

        // Pegar informações do appsettings: https://stackoverflow.com/a/58432834
        static readonly string encriptionKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("PasswordEncryptionSettings")["EncryptionKey"];

        // Converter para o horário de Brasilia: https://blog.yowko.com/timezoneinfo-time-zone-id-not-found/;
        public static DateTime HorarioBrasilia()
        {
            TimeZoneInfo timeZone = TZConvert.GetTimeZoneInfo("E. South America Standard Time");
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone);
        }

        // Gerar Lorem Ipsum: https://stackoverflow.com/questions/4286487/is-there-any-lorem-ipsum-generator-in-c;
        public static string LoremIpsum(int minWords, int maxWords, int minSentences, int maxSentences, int numParagraphs, bool isAdicionarTagP)
        {

            var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
        "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
        "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

            var rand = new Random();
            int numSentences = rand.Next(maxSentences - minSentences) + minSentences + 1;
            int numWords = rand.Next(maxWords - minWords) + minWords + 1;

            StringBuilder result = new();
            for (int p = 0; p < numParagraphs; p++)
            {
                if (isAdicionarTagP)
                {
                    result.Append("<p>");
                }

                for (int s = 0; s < numSentences; s++)
                {
                    for (int w = 0; w < numWords; w++)
                    {
                        if (w > 0) { result.Append(" "); }
                        result.Append(words[rand.Next(words.Length)]);
                    }

                    result.Append(". ");
                }

                if (isAdicionarTagP)
                {
                    result.Append("</p>");
                }
            }

            return result.ToString();
        }

        // Criptografar e decriptografar senha: https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp;
        public static string Criptografar(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new(password: encriptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using MemoryStream ms = new();
                using (CryptoStream cs = new(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }

                clearText = Convert.ToBase64String(ms.ToArray());
            }

            return clearText;
        }

        public static string Descriptografar(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new(password: encriptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using MemoryStream ms = new();
                using (CryptoStream cs = new(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }

                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }

            return cipherText;
        }

        // Pegar a descrição de um enum: https://stackoverflow.com/questions/50433909/get-string-name-from-enum-in-c-sharp;
        public static string GetDescricaoEnum(Enum enumVal)
        {
            MemberInfo[] memInfo = enumVal.GetType().GetMember(enumVal.ToString());
            DescriptionAttribute attribute = CustomAttributeExtensions.GetCustomAttribute<DescriptionAttribute>(memInfo[0]);
            return attribute.Description;
        }

        // Converter Base64 para imagem;
        public static IFormFile Base64ToImage(string base64)
        {
            List<IFormFile> formFiles = new();

            string split = ";base64,";
            string normalizarBase64 = base64.Substring(base64.IndexOf(split) + split.Length);
            byte[] bytes = Convert.FromBase64String(normalizarBase64);
            MemoryStream stream = new(bytes);

            IFormFile file = new FormFile(stream, 0, bytes.Length, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            formFiles.Add(file);

            return formFiles[0];
        }

        // Validar se o e-mail do usuário;
        public static bool ValidarEmail(string email)
        {
            EmailAddressAttribute e = new();
            return e.IsValid(email);
        }

        // Validar se a senha do usuário é forte o suficiente verificando requisitos de senha:
        // #1 - Tem número;
        // #2 - Tem letra maiúscula;
        // #3 - Tem pelo menos X caracteres;
        // #4 - A senha não contém o nome completo, nome de usuário ou e-mail;
        public static Tuple<bool, string> ValidarSenha(string senha, string nomeCompleto, string nomeUsuario, string email)
        {
            bool isValido = true;
            string msgErro = "";

            var temNumero = new Regex(@"[0-9]+");
            if (!temNumero.IsMatch(senha))
            {
                isValido = false;
                msgErro = "A senha deve conter ao menos um número";
                return Tuple.Create(isValido, msgErro);
            }

            var temMaiusculo = new Regex(@"[A-Z]+");
            if (!temMaiusculo.IsMatch(senha))
            {
                isValido = false;
                msgErro = "A senha deve conter ao menos uma letra maiúscula";
                return Tuple.Create(isValido, msgErro);
            }

            int minCaracteres = 6;
            var temXCaracteres = new Regex(@".{" + minCaracteres + ",}");
            if (!temXCaracteres.IsMatch(senha))
            {
                isValido = false;
                msgErro = $"A senha deve conter ao menos {minCaracteres} caracteres";
                return Tuple.Create(isValido, msgErro);
            }

            string nomeCompletoPrimeiraParte = nomeCompleto.Split(' ')[0].ToLowerInvariant();
            bool isRepeteNomeCompleto = senha.ToLowerInvariant().Contains(nomeCompletoPrimeiraParte);
            if (isRepeteNomeCompleto)
            {
                isValido = false;
                msgErro = "A senha não pode conter o seu primeiro nome";
                return Tuple.Create(isValido, msgErro);
            }

            bool isRepeteNomeUsuario = senha.ToLowerInvariant().Contains(nomeUsuario.ToLowerInvariant());
            if (isRepeteNomeUsuario)
            {
                isValido = false;
                msgErro = "A senha não pode conter o seu nome de usuário";
                return Tuple.Create(isValido, msgErro);
            }

            string emailAntesDoArroba = email.Split('@')[0].ToLowerInvariant();
            bool isRepeteEmail = senha.ToLowerInvariant().Contains(emailAntesDoArroba.ToLowerInvariant());
            if (isRepeteEmail)
            {
                isValido = false;
                msgErro = "A senha não pode conter o seu e-mail";
                return Tuple.Create(isValido, msgErro);
            }

            return Tuple.Create(isValido, msgErro);
        }

        // Gerar um número aleatório com base na em um valor mínimo e máximo;
        public static int GerarNumeroAleatorio(int min, int max)
        {
            Random random = new();
            int numeroAleatorio = random.Next(min, max - 1);

            return numeroAleatorio;
        }

        // Gerar uma string aleatória com base na quantidade de caracteres desejados;
        public static string GerarStringAleatoria(int qtdCaracteres, bool isApenasMaiusculas)
        {
            Random random = new();
            string caracteres = (isApenasMaiusculas ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" : "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
            string? stringAleatoria = new(Enumerable.Repeat(caracteres, qtdCaracteres).Select(s => s[random.Next(s.Length)]).ToArray());

            return stringAleatoria;
        }

        // Gerar um código hash para o usuário com base no usuarioId + string aleatória;
        public static string GerarHashUsuario(int usuarioId)
        {
            string palavraAleatoria = $"{usuarioId}_{GerarStringAleatoria(GerarNumeroAleatorio(10, 15), false)}";
            string hash = Criptografar(palavraAleatoria).Replace("/", "");

            return hash;
        }

        // Verificar se a aplicação está sendo executada em localhost ou publicada;
        public static bool IsDebug()
        {
            // https://stackoverflow.com/questions/12135854/best-way-to-tell-if-in-production-or-development-environment-in-net
#if DEBUG
            return true;
#else
        return false;
#endif
        }

        // Verificar se o front-end está sendo executado em localhost ou publicado;
        public static string CaminhoFront()
        {
            string urlApi = urlFrontProd;

            if (IsDebug())
            {
                urlApi = urlFrontDev;
            }

            return urlApi;
        }

        // Método padrão para envio de e-mails: https://app.sendgrid.com/;
        public static async Task<bool> EnviarEmail(string emailTo, string assunto, string nomeArquivo, List<EmailDadosReplace> listaDadosReplace)
        {
            if (String.IsNullOrEmpty(emailTo) || String.IsNullOrEmpty(assunto) || String.IsNullOrEmpty(nomeArquivo))
            {
                return false;
            }

            string conteudoEmailHtml = string.Empty;
            string caminhoFinal = $"{Directory.GetCurrentDirectory()}/Emails/{nomeArquivo}";

            using (var reader = new StreamReader(caminhoFinal))
            {
                // #1 - Ler arquivo;
                string readFile = reader.ReadToEnd();
                string strContent = readFile;

                // #2 - Remover tags desnecessárias;
                strContent = strContent.Replace("\r", string.Empty);
                strContent = strContent.Replace("\n", string.Empty);

                // #3 - Replaces utilizando o parâmetro "listaDadosReplace";
                foreach (var item in listaDadosReplace)
                {
                    strContent = strContent.Replace($"[{item.Key}]", item.Value);
                }

                // #4 - Gerar resultado final;
                conteudoEmailHtml = strContent.ToString();
            }

            // https://app.sendgrid.com/guide/integrate/langs/csharp
            var client = new SendGridClient(emailApiKey);
            var from = new EmailAddress(emailPadrao, emailNomeRemetente);
            var to = new EmailAddress(emailTo);
            var msg = MailHelper.CreateSingleEmail(from, to, assunto, string.Empty, conteudoEmailHtml);
            var resposta = await client.SendEmailAsync(msg);

            return resposta.IsSuccessStatusCode;
        }
    }
}
