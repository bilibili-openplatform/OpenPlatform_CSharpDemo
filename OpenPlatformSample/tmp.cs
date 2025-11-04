using OpenPlatform_Signature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenPlatformSample
{
    public class tmp
    {

        /// <summary>
        /// 测试环境下，读取配置配置文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static void ReadSecurityConfig(string filePath = "./config/sec.json")
        {
            try
            {
                // 检查文件是否存在
                if (!File.Exists(filePath))
                {
                    return;
                }

                // 读取文件内容
                string jsonContent = File.ReadAllText(filePath);

                // 反序列化为对象
                SecurityConfig config = JsonSerializer.Deserialize<SecurityConfig>(jsonContent);

                OpenPlatform_Signature.Signature.Client_ID = Signature.IsUAT ? config.UAT_Client_ID : config.PROD_Client_ID;
                OpenPlatform_Signature.Signature.App_Secret = Signature.IsUAT ? config.UAT_App_Secret : config.PROD_App_Secret;
                OpenPlatform_Signature.Signature.ReturnUrl = Signature.IsUAT ? config.UAT_ReturnUrl : config.PROD_ReturnUrl;
                OpenPlatform_Signature.Signature.Access_Token = Signature.IsUAT ? config.UAT_AccessToken : config.PROD_AccessToken;
                OpenPlatform_Signature.Signature.Open_ID = Signature.IsUAT ? config.UAT_OpenId : config.PROD_OpenId;


                return;
            }
            catch (Exception ex)
            {
                return;
            }
        }
        public class SecurityConfig
        {
            public string PROD_Client_ID { get; set; }
            public string PROD_App_Secret { get; set; }
            public string PROD_ReturnUrl { get; set; }
            public string PROD_AccessToken { get; set; }
            public string PROD_OpenId { get; set; }
            public string UAT_Client_ID { get; set; }
            public string UAT_App_Secret { get; set; }
            public string UAT_ReturnUrl { get; set; }
            public string UAT_AccessToken { get; set; }
            public string UAT_OpenId { get; set; }
        }
    }
}
