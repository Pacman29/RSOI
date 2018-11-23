using Microsoft.IdentityModel.Tokens;
using System.Dynamic;
using System.Text;

namespace AuthOptions
{
    public class AuthOptions
    {
        /// <summary>
        /// Издатель токена
        /// </summary>
        public string ISSUER { get; set; }
        
        /// <summary>
        /// потребитель токена
        /// </summary>
        public string AUDIENCE { get; set; }
        
        /// <summary>
        /// ключ шифрации
        /// </summary>
        const string KEY = "mysupersecret_secretkey!123";
        
        /// <summary>
        /// время жизни токена (мин)
        /// </summary>
        public double Lifetime {get;} = 30 ;
        
        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}