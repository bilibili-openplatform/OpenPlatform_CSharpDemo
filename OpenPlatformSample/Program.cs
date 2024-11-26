using Newtonsoft.Json;
using OpenPlatform_LiveRoomData.Client;
using OpenPlatform_LiveRoomData.Client.Data;
using OpenPlatform_LiveRoomData.Runtime;
using OpenPlatform_LiveRoomData.Runtime.Data;
using OpenPlatform_LiveRoomData.Runtime.Utilities;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Text;
using OpenPlatform_Signature;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace OpenPlatformSample
{


    internal class Program
    {
        /// <summary>
        /// 初始化，启动时必须最先调用，配置应用基础信息，使用前请替换成自己的应用信息
        /// </summary>
        public static void Init()
        {
            OpenPlatform_Signature.Signature.Client_ID = Secrest["Client_ID"];//入驻开放平台后，通过并且创建应用完成后，应用的Client_ID（https://open.bilibili.com/company-core）
            OpenPlatform_Signature.Signature.App_Secret = Secrest["App_Secret"];//入驻开放平台后，通过并且创建应用完成后，应用的App_Secret(https://open.bilibili.com/company-core)
            OpenPlatform_Signature.Signature.ReturnUrl = Secrest["ReturnUrl"];//创建应用后，开发者自行设置的'应用回调域'（https://open.bilibili.com/company-core/{Client_ID}/detail）
        }




        /*--------------使用前请按修改以上内容--------------*/
        /*-------请将以上内容修改为对应的应用配置信息--------*/




        private static string AccessToken = Secrest["AccessToken"];
        private static string OpenId = Secrest["OpenId"];

        //用于读取机密信息的接口对象
        private static IConfigurationRoot Secrest = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

        public static void Main(string[] args)
        {
            //初始化，必须最先启动，不能删除
            Init();

            //账号授权
            AccountAuthorization();

            //直播能力-获取直播长连消息
            Live.Live.Start(AccessToken);


            while (true)
            {
                Console.ReadKey();
            }
        }



        /// <summary>
        /// 账号授权
        /// </summary>
        public static void AccountAuthorization()
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                AccessToken = OpenPlatform_Authorization.Program.GetAccessToken();//授权获取AccessToken
            }
            if (string.IsNullOrEmpty(OpenId))
            {
                var info = OpenPlatform_UserInfo.info.GetInfo(AccessToken);//获取用户公开信息(https://open.bilibili.com/doc/4/feb66f99-7d87-c206-00e7-d84164cd701c)
                OpenId = info.openid;
            }
        }


    }
}