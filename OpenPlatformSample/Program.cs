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
using Newtonsoft.Json.Linq;

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


        //用于读取机密信息的接口对象
        private static IConfigurationRoot Secrest = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

        private static string AccessToken = Secrest["AccessToken"];
        private static string OpenId = Secrest["OpenId"];



        public static void Main(string[] args)
        {
            //初始化，必须最先启动，不能删除
            Init();

            //账号授权（包含网页应用唤起，签名，授权，换取token）
            AccountAuthorization();

            //直播能力-获取直播长连消息
            //Live.Live.Start(AccessToken);

            //直播能力-获取直播间基础信息
            //GetRoomInfo();


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
                ////https://open.bilibili.com/doc/4/feb66f99-7d87-c206-00e7-d84164cd701c
                string resp = Signature.SendRequest("https://member.bilibili.com/arcopen/fn/user/account/info", "GET", AccessToken).Result;
                OpenId = JObject.Parse(resp)?["data"]?["openid"]?.ToString();
            }
        }

        /// <summary>
        /// 获取直播间基础信息
        /// </summary>
        /// <returns></returns>
        public static (string open_id, long room_id, string title, bool is_streaming, bool is_banned) GetRoomInfo()
        {
            (string open_id, long room_id, string title, bool is_streaming, bool is_banned) info = new();
            //https://open.bilibili.com/doc/4/67eaa648-3f67-f2bc-0fac-efa5fb922305
            string resp = Signature.SendRequest("https://member.bilibili.com/arcopen/fn/live/room/info", "GET", AccessToken).Result;
            info.open_id = JObject.Parse(resp)?["data"]?["open_id"]?.ToString();//主播的open_id
            long.TryParse(JObject.Parse(resp)?["data"]?["room_id"]?.ToString(), out info.room_id);//房间号
            info.title = JObject.Parse(resp)?["data"]?["title"]?.ToString();//直播间标题
            bool.TryParse(JObject.Parse(resp)?["data"]?["is_streaming"]?.ToString(), out info.is_streaming);//当前是否开播
            bool.TryParse(JObject.Parse(resp)?["data"]?["is_banned"]?.ToString(), out info.is_banned);//当前房间是否被封禁
            return info;
        }


    }
}