
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static OpenPlatform_Signature.Signature;

namespace OpenPlatform_Authorization
{
    public class Program
    {
        public static string GetAccessToken()
        {
            #region 授权
            //https://open.bilibili.com/doc/4/eaf0e2b5-bde9-b9a0-9be1-019bb455701c
            string toAuthorizationUrl = $"{AccountDomain}/pc/account-pc/auth/oauth?client_id={Client_ID}&gourl={ReturnUrl}&state=TestDemo";
            Process.Start(new ProcessStartInfo
            {
                FileName = toAuthorizationUrl,
                UseShellExecute = true
            });
            #endregion

            /*
             * 这中间需要实现授权换取AccsessToken的逻辑，由于这涉及到网页操作，无法在Demo中实现，请参考授权文档中的逻辑实现
             */

            #region 获取AccsessToken       
            Console.WriteLine("请输入授权后获取到的code:");
            string code = Console.ReadLine();//通过授权流程获取到的code
            var response = AccessToken.ExchangeAccessTokenAsync(Client_ID, App_Secret, code).Result;
            string AccsessToken = JObject.Parse(response)?["data"]?["access_token"]?.ToString();
            return AccsessToken;
            #endregion
        }
    }
}
