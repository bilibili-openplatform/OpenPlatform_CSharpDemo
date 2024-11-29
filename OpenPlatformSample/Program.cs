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
using System.Text.Json;

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

        private static string Domain = "https://member.bilibili.com";



        public static void Main(string[] args)
        {
            //初始化，必须最先启动，不能删除
            Init();

            //账号授权（包含网页应用唤起，签名，授权，换取token）
            AccountAuthorization();

            ////直播能力-获取直播长连消息
            //Live.Live.Start(AccessToken);

            ////直播能力-获取直播间基础信息
            //GetRoomInfo();

            ////用户管理-查询用户已授权权限列表
            //GetScopes();

            ////视频能力-查询单一视频稿件详情
            //GetViewInfo();

            ////视频能力-查询当前用户稿件列表
            //GetArchiveViewList();

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
                //https://open.bilibili.com/doc/4/feb66f99-7d87-c206-00e7-d84164cd701c
                var url = $"{Domain}/arcopen/fn/user/account/info";
                var resp = Signature.SendRequest(url, "GET", AccessToken).Result;
                if (JObject.Parse(resp)?["code"]?.ToString() == "0")
                {
                    OpenId = JObject.Parse(resp)?["data"]?["openid"]?.ToString();//授权账号的open_id
                    WriteLog(resp);
                }
            }
        }

        /// <summary>
        /// 获取直播间基础信息
        /// </summary>
        /// <returns></returns>
        public static void GetRoomInfo()
        {
            (string open_id, long room_id, string title, bool is_streaming, bool is_banned) info = new();
            //https://open.bilibili.com/doc/4/67eaa648-3f67-f2bc-0fac-efa5fb922305
            var url = $"{Domain}/arcopen/fn/live/room/info";
            var resp = Signature.SendRequest(url, "GET", AccessToken).Result;
            if (JObject.Parse(resp)?["code"]?.ToString() == "0")
            {
                info.open_id = JObject.Parse(resp)?["data"]?["open_id"]?.ToString();//主播的open_id
                long.TryParse(JObject.Parse(resp)?["data"]?["room_id"]?.ToString(), out info.room_id);//房间号
                info.title = JObject.Parse(resp)?["data"]?["title"]?.ToString();//直播间标题
                bool.TryParse(JObject.Parse(resp)?["data"]?["is_streaming"]?.ToString(), out info.is_streaming);//当前是否开播
                bool.TryParse(JObject.Parse(resp)?["data"]?["is_banned"]?.ToString(), out info.is_banned);//当前房间是否被封禁
                WriteLog(resp);
            }
        }

        /// <summary>
        /// 查询用户已授权权限列表
        /// </summary>
        /// <returns></returns>
        public static void GetScopes()
        {
            (string open_id, List<string> scopes) info = new() { scopes = new() };
            //https://open.bilibili.com/doc/4/08f935c5-29f1-e646-85a3-0b11c2830558
            var url = $"{Domain}/arcopen/fn/user/account/scopes";

            var resp = Signature.SendRequest(url, "GET", AccessToken).Result;
            if (JObject.Parse(resp)?["code"]?.ToString() == "0")
            {
                info.open_id = JObject.Parse(resp)?["data"]?["open_id"]?.ToString();//主播的open_id
                var itemesArray = JObject.Parse(resp)?["data"]?["scopes"] as JArray;
                if (itemesArray != null)
                {
                    foreach (var item in itemesArray)
                    {
                        info.scopes.Add(item.ToString());
                    }
                }
                WriteLog(resp);
            }
        }


        /// <summary>
        /// 查询单一视频稿件详情
        /// </summary>
        public static void GetArchiveView()
        {
            var requestParameters = new Dictionary<string, string?>
            {
                { "resource_id", Secrest["resource_id"] }
            };

            var queryParams = requestParameters
                .Where(kvp => kvp.Value != null)
                .Select(kvp => $"{kvp.Key}={kvp.Value}")
                .ToArray();

            var queryString = string.Join("&", queryParams);
            //https://open.bilibili.com/doc/4/d9554788-dcef-f139-6217-b487d41c3826
            var url = $"{Domain}/arcopen/fn/archive/view?{queryString}";

            var resp = Signature.SendRequest(url, "GET", AccessToken).Result;
            if (JObject.Parse(resp)?["code"]?.ToString() == "0")
            {
                WriteLog(resp);
            }
        }

        /// <summary>
        /// 查询当前用户稿件列表
        /// </summary>
        public static void GetArchiveViewList()
        {
            var requestParameters = new Dictionary<string, string?>
            {
                { "pn", "1" },
                { "ps", "20" },
                { "status", "all" }
            };

            var queryParams = requestParameters
                .Where(kvp => kvp.Value != null)
                .Select(kvp => $"{kvp.Key}={kvp.Value}")
                .ToArray();

            var queryString = string.Join("&", queryParams);
            //https://open.bilibili.com/doc/4/a24030b7-6b8f-b36c-32d8-a4aae67fcc35
            var url = $"{Domain}/arcopen/fn/archive/viewlist?{queryString}";

            var resp = Signature.SendRequest(url, "GET", AccessToken).Result;
            if (JObject.Parse(resp)?["code"]?.ToString() == "0")
            {
                WriteLog(resp);
            }
        }



        private static void WriteLog(string response)
        {
            using (JsonDocument doc = JsonDocument.Parse(response))
            {
                // 创建 JsonWriterOptions，设置为格式化输出
                JsonWriterOptions options = new JsonWriterOptions
                {
                    Indented = true
                };

                // 创建一个内存流
                using (var stream = new System.IO.MemoryStream())
                {
                    // 使用 Utf8JsonWriter 来写入内存流
                    using (var writer = new Utf8JsonWriter(stream, options))
                    {
                        doc.WriteTo(writer);
                    }

                    // 将内存流转换为字符串
                    string formattedJson = System.Text.Encoding.UTF8.GetString(stream.ToArray());

                    // 重新序列化以确保汉字不被转义
                    var jsonObject = System.Text.Json.JsonSerializer.Deserialize<object>(formattedJson);
                    string finalOutput = System.Text.Json.JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

                    // 输出格式化后的 JSON 到终端
                    Console.WriteLine(finalOutput);
                }
            }
        }
    }
}