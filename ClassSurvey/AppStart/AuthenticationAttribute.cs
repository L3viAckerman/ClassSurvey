using System;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Xml;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using ClassSurvey1.Models;
using ClassSurvey1.Modules;
namespace ClassSurvey1
{
    public class AuthenticationFilter : IActionFilter
    {
        private readonly IConfiguration Configuration;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        public TokenValidationParameters Parameters { get; private set; }
        private IJWTHandler JWTHandler;
        public AuthenticationFilter(IConfiguration Configuration, IJWTHandler JWTHandler)
        {
            this.Configuration = Configuration;
            this.JWTHandler = JWTHandler;
        }

        //uy quyen
        public void OnActionExecuting(ActionExecutingContext FilterContext)
        {
            return;
            if (FilterContext.HttpContext.Request.Path.Value.StartsWith("/App/Login"))
                return;
            var Token = FilterContext.HttpContext.Request.Cookies["JWT"];
            var JWTEntity = JWTHandler.Decode(Token);
            if (JWTEntity != null)
            {

                FilterContext.HttpContext.User = new MyPrincipal(JWTEntity.UserEntity);
                string Path = FilterContext.HttpContext.Request.Path.HasValue ? FilterContext.HttpContext.Request.Path.Value : "";
                string Method = FilterContext.HttpContext.Request.Method;
                string[] temp = Path.Split('/');
                for (int i = 0; i < temp.Length; i++)
                {
                    Guid id;
                    bool isGuid = Guid.TryParse(temp[i], out id);
                    if (isGuid) temp[i] = "*";
                }
                Path = string.Join("/", temp);
                ClassSurveyContext Context = new ClassSurveyContext();
                Operation Operation = Context.Operations.Where(o => o.Link.Equals(Path) && o.Method.Equals(Method)).FirstOrDefault();

                string role = string.Join(",", JWTEntity.UserEntity.Roles);
                ROLES roles = (ROLES)Enum.Parse(typeof(ROLES), role);
                var operationRole = (ROLES)Enum.Parse(typeof(ROLES), Operation.Role.ToString());
                if (Operation != null && operationRole != ROLES.NONE)
                {
                    if ((operationRole & roles) == 0)
                        throw new ForbiddenException("Bạn không có quyền truy cập");
                }
                return;
            }
            else
            {
                if (FilterContext.HttpContext.Request.Path.Value.StartsWith("api"))
                    throw new ForbiddenException("Cookie không hợp lệ");
                throw new ForbiddenException("Login First");
                
            }
        }

        public void OnActionExecuted(ActionExecutedContext Context)
        {
            //throw new NotImplementedException();
        }
    }

    public class JWTEntity
    {
        public UserEntity UserEntity { get; set; }
        public string unique_name { get; set; }
        public string iss { get; set; }
        public string iat { get; set; }
        public string exp { get; set; }
        public DateTime ExpTime
        {
            get
            {
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(long.Parse(exp)).ToLocalTime();
                return dtDateTime;
            }
        }

        public JWTEntity()
        {

        }
    }


    public interface IJWTHandler : ITransientService
    {
        JWTEntity Decode(string token, bool verify = true);
        TokenValidationParameters Parameters { get; }
        string CreateToken(UserEntity UserEntity);
    }

    public class JWTHandler : IJWTHandler
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        private SecurityKey _issuerSigningKey;
        private SigningCredentials _signingCredentials;
        private JwtHeader _jwtHeader;
        private RSA RSA;
        public TokenValidationParameters Parameters { get; private set; }
        public JWTHandler()
        {
            InitializeRsa();
            InitializeJwtParameters();
        }

        private void InitializeRsa()
        {
            RSA publicRsa = RSA.Create();
            RSA = publicRsa;
            var publicKeyXml = @"<RSAKeyValue>
  <Modulus>gsSlw/Y6kmjNbjb00VrVFc9n8GXVKiKoVVp7ki45xIOWySL/5qfaa7lTVlIJjiOfygYe5mrcFFp08DTLoQgx0/tJVCvotluNwpN6M8GQrsY+1Uw0PmBjXgXcuUPI9jjuOUuUD4gXrjzW0mL61PdJ1yO51YdP72q092w8iGdeWmx4/2ioZKuGViqMef4w2CFpR9KwgQa7DtWd7KTBN2Sup/FGC6FX8pWfsMH8XPP5LsrxASM9Msa4yN8LtYYeA8VOQeM/8WeAyKGABLjJ5hb8uY+stVIaPMgZVbEQY4unjSWENgXHlIlJsu9WkZrpe/rfJO66pr4jh5RuY9Lqi4Xl9Q==</Modulus>
  <Exponent>AQAB</Exponent>
</RSAKeyValue>";
            FromXmlString(publicRsa, publicKeyXml);
            _issuerSigningKey = new RsaSecurityKey(publicRsa);
            RSA privateRsa = RSA.Create();
            var privateKeyXml = @"<RSAKeyValue>
    <Modulus>gsSlw/Y6kmjNbjb00VrVFc9n8GXVKiKoVVp7ki45xIOWySL/5qfaa7lTVlIJjiOfygYe5mrcFFp08DTLoQgx0/tJVCvotluNwpN6M8GQrsY+1Uw0PmBjXgXcuUPI9jjuOUuUD4gXrjzW0mL61PdJ1yO51YdP72q092w8iGdeWmx4/2ioZKuGViqMef4w2CFpR9KwgQa7DtWd7KTBN2Sup/FGC6FX8pWfsMH8XPP5LsrxASM9Msa4yN8LtYYeA8VOQeM/8WeAyKGABLjJ5hb8uY+stVIaPMgZVbEQY4unjSWENgXHlIlJsu9WkZrpe/rfJO66pr4jh5RuY9Lqi4Xl9Q==</Modulus>
    <Exponent>AQAB</Exponent>
    <P>1CmAnmgnRv/e8F8DrgZFf1yTMOowL5kyk9dxGYC5Al4uc6mU8FUla5xKyu9eN87akfkkTnaIu+FI4+1kkXKZHKaDYHXizRHKK3G3fbBi/Ei/4dRz2lLCZErrxdF/jA8PHBukpcgaxP0hbjy0SxM+un5nbhl1YR7ouBEhfGQaG7k=</P>
    <Q>ncm9zVo9vtxSmAJDlahaE7sq4dJPKj1HGqJgdKLkYK+dHGC/rMtXI1Gm6DvexSTrhoIJZmH2ozpWpfEPslm78rG3Rs/pEXPptqF9he9x+IoaZQo38diJGUoyqTM/aVgwuqIilf7U07Irlwz7dKM7Dl4QB2Z1DBLg0iA7Uo/r0h0=</Q>
    <DP>lBfMMcZd9E9SnNKVzPmPtVE3ZSNzMyZqiYwO5FBX0/FN12p+DixBDJZyFqlzSN1Y8B/KWgKfexXMPV1Nn8EwYzFP7xsajy5lwmGERXXEAnn9hnM30yOFkWBCpziPIYK5d1NMYHQHS42tsjcpWmY6mQ72v4GBz6M1qpY6m4t8NfE=</DP>
    <DQ>CuWZ1Aq9ZOb2VUUiwb0kq2Qrq/jIOtSMioxYOPXe68Z1BTaxRg+s7uV+r18jHV5VXa6xU37Ed7VZspAZU2nbDwGy9JL7N5dXtqMkdhF5P37aH8w63NrlbPew0/qUEIRkqR71YIJ+1DfjxsTAwOdc9rsMRFaREspi+F/9eNAzqwk=</DQ>
    <InverseQ>tZ5nzA1Qom7brF9+spdEv5nAUySZLi6ksykdSfAcC495Ii02jzMY2naL0fecJxBpa8+7TYdCn63M338nTAY/3udpMGsicKNd1W8SJcNSQ5pXZkJEG7d6bUsxS0XwwxAXCG2Vr+/PpRUFZu512HD5N0OkVTEsgwwlqC47hGIQiI0=</InverseQ>
    <D>ZM/ta3F8HjslhH5hprR76eCHpNEc0Or/Ey46bautZU59BHszBUMKJFovYTDFpQrZuQmW8NQY7qf91uEcyvxiTaZAFd/z/AIA+6xuXVAwlkzHS/D/pcbxVB741urnFss0/H7DmmW0u+KzSmZ8MYqjJnSoY3F5sn11HfoXgKEQIGy6dipprEDl8yKCgfskQweRdGewaT5lOOnHY6Jmkizmn+PKuJ3KuQYR4d5pTyZetbE6q6tqfw7al7GKC5/7txmVptUPPd0x7p75UKPiS2/EX5wg8qYrMVf7Sy6/sTHldUnnvfeaF9sQmFieiO9GuLiX0S4B5bj5rJJJ8o0TIGzLgQ==</D>
</RSAKeyValue>";
            FromXmlString(privateRsa, privateKeyXml);
            var privateKey = new RsaSecurityKey(privateRsa);
            _signingCredentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);
        }


        private void InitializeJwtParameters()
        {
            _jwtHeader = new JwtHeader(_signingCredentials);
            Parameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidIssuer = "https://portal.fpt.com.vn",
                IssuerSigningKey = _issuerSigningKey
            };
        }

        public static byte[] Base64UrlDecode(string arg)
        {
            var decrypted = ToBase64(arg);

            return Convert.FromBase64String(decrypted);
        }
        public static string ToBase64(string arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException("arg");
            }

            var s = arg
                    .PadRight(arg.Length + (4 - arg.Length % 4) % 4, '=')
                    .Replace("_", "/")
                    .Replace("-", "+");

            return s;
        }
        public JWTEntity Decode(string token, bool verify = true)
        {
            try
            {
                string[] parts = token.Split('.');
                string header = parts[0];
                string payload = parts[1];
                byte[] crypto = Base64UrlDecode(parts[2]);
                string headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
                JObject headerData = JObject.Parse(headerJson);
                string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
                JObject payloadData = JObject.Parse(payloadJson);
                JWTEntity JWTEntity = null;
                if (verify)
                {
                    SHA256 sha256 = SHA256.Create();
                    byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(parts[0] + '.' + parts[1]));
                    RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(RSA);
                    rsaDeformatter.SetHashAlgorithm("SHA256");
                    if (rsaDeformatter.VerifySignature(hash, Base64UrlDecode(parts[2])))
                    {
                        var json = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
                        JWTEntity = JsonConvert.DeserializeObject<JWTEntity>(json);
                        if (JWTEntity.ExpTime < DateTime.Now)
                        {
                            throw new BadRequestException("JWT hết hạn sử dụng!");
                        }
                        return JWTEntity;
                    }
                }
                return JWTEntity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }

        }
        public string CreateToken(UserEntity UserEntity)
        {
            if (_signingCredentials == null) throw new BadRequestException("Server khong tao duoc token");
            var nowUtc = DateTime.UtcNow;
            var expires = nowUtc.AddDays(30);
            var centuryBegin = new DateTime(1970, 1, 1);
            var exp = (long)(new TimeSpan(expires.Ticks - centuryBegin.Ticks).TotalSeconds);
            var now = (long)(new TimeSpan(nowUtc.Ticks - centuryBegin.Ticks).TotalSeconds);
            var issuer = string.Empty;
            var payload = new JwtPayload
            {
                {"UserEntity", UserEntity},
                {"unique_name", UserEntity.Id},
                {"iss", issuer},
                {"iat", now},
                //{"nbf", now},
                {"exp", exp}
            };
            var jwt = new JwtSecurityToken(_jwtHeader, payload);
            var token = _jwtSecurityTokenHandler.WriteToken(jwt);
            return token;
        }

        public static void FromXmlString(RSA rsa, string xmlString)
        {
            var parameters = new RSAParameters();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = Convert.FromBase64String(node.InnerText); break;
                        case "Exponent": parameters.Exponent = Convert.FromBase64String(node.InnerText); break;
                        case "P": parameters.P = Convert.FromBase64String(node.InnerText); break;
                        case "Q": parameters.Q = Convert.FromBase64String(node.InnerText); break;
                        case "DP": parameters.DP = Convert.FromBase64String(node.InnerText); break;
                        case "DQ": parameters.DQ = Convert.FromBase64String(node.InnerText); break;
                        case "InverseQ": parameters.InverseQ = Convert.FromBase64String(node.InnerText); break;
                        case "D": parameters.D = Convert.FromBase64String(node.InnerText); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }
            rsa.ImportParameters(parameters);
        }
    }
}