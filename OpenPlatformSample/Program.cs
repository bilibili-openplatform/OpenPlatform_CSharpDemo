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
using System.Net.Http.Headers;
using System;
using System.Xml.Linq;
using System.Collections;

namespace OpenPlatformSample
{


    internal class Program
    {
        /// <summary>
        /// 初始化，启动时必须最先调用，配置应用基础信息，使用前请替换成自己的应用信息
        /// </summary>
        public static void Init()
        {
            OpenPlatform_Signature.Signature.Client_ID = Signature.IsUAT?Secrest["UAT_Client_ID"]:Secrest["PROD_Client_ID"];//入驻开放平台后，通过并且创建应用完成后，应用的Client_ID（https://open.bilibili.com/company-core）
            OpenPlatform_Signature.Signature.App_Secret = Signature.IsUAT?Secrest["UAT_App_Secret"]:Secrest["PROD_App_Secret"];//入驻开放平台后，通过并且创建应用完成后，应用的App_Secret(https://open.bilibili.com/company-core)
            OpenPlatform_Signature.Signature.ReturnUrl = Signature.IsUAT?Secrest["UAT_ReturnUrl"]:Secrest["PROD_ReturnUrl"];//创建应用后，开发者自行设置的'应用回调域'（https://open.bilibili.com/company-core/{Client_ID}/detail）
        }




        /*--------------使用前请按修改以上内容--------------*/
        /*-------请将以上内容修改为对应的应用配置信息--------*/


        //用于读取机密信息的接口对象
        private static IConfigurationRoot Secrest = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        private static string AccessToken = Signature.IsUAT?Secrest["UAT_AccessToken"]:Secrest["PROD_AccessToken"];
        private static string OpenId = Signature.IsUAT?Secrest["UAT_OpenId"]:Secrest["PROD_OpenId"];

     


        public static void Main(string[] args)
        {
            //初始化，必须最先启动，不能删除
            Init();

            while (true)
            {
                Console.WriteLine();

                Console.WriteLine("0.测试签名");
                Console.WriteLine("1.账号授权");
                Console.WriteLine("2.直播能力-获取直播长连消息");
                Console.WriteLine("3.直播能力-获取直播间基础信息");
                Console.WriteLine("4.用户管理-查询用户已授权权限列表");
                Console.WriteLine("5.视频能力-查询单一视频稿件详情");
                Console.WriteLine("6.视频能力-查询当前用户稿件列表");
                Console.WriteLine("7.三方一键开播-获取第三方开播授权链接");
                Console.WriteLine("8.获取投稿分区列表");
                Console.WriteLine("9.视频稿件上传预处理");
                Console.WriteLine("10.单个小文件视频上传");
                Console.WriteLine("11.上传稿件封面");
                Console.WriteLine("12.分片上传稿件视频");
                Console.WriteLine("13.分片上传文件合片");
                Console.WriteLine("14.视频稿件提交");
                Console.WriteLine("15.大会员-二次元通行证信息查询");
                Console.Write("输入编号选择执行的demo功能：");
                string code = Console.ReadLine();
                Console.WriteLine("\r执行结果:");
                selectFunction(code);
            }
        }

        public static void selectFunction(string code)
        {
            if (code != "1" && string.IsNullOrEmpty(AccessToken))
            {
                Console.WriteLine("请先进行账号授权兑换token");
                return;
            }
            switch (code)
            {
                //测试计算签名
                case "0":
                    {
                    Console.WriteLine("请输入计算签名的Client_ID：");
                    string Client_ID = Console.ReadLine();
                    Console.WriteLine("请输入计算签名的App_Secret：");
                    string App_Secret = Console.ReadLine();
                    Console.WriteLine("请输入计算签名的Nonce：");
                    string Nonce = Console.ReadLine();
                    Console.WriteLine("请输入用于计算签名的body ReqJson或者已计算好的md5内容：");
                    string ReqJson = Console.ReadLine();
                    Console.WriteLine("请输入计算签名的TimeStamp：");
                    string TimeStamp = Console.ReadLine();
                    Console.WriteLine("计算签名结果：\n" + OpenPlatform_Signature.Signature.SignatureTest(Client_ID, App_Secret,Nonce,TimeStamp,ReqJson));
                    break;
                }
                //账号授权（包含网页应用唤起，签名，授权，换取token）
                case "1":
                    {
                        AccountAuthorization();
                        break;
                    }
                //直播能力-获取直播长连消息
                case "2":
                    {
                        Live.Live.Start(AccessToken);
                        break;
                    }
                //直播能力-获取直播间基础信息
                case "3":
                    {
                        GetRoomInfo();
                        break;
                    }
                //用户管理-查询用户已授权权限列表
                case "4":
                    {
                        GetScopes();
                        break;
                    }
                //视频能力-查询单一视频稿件详情
                case "5":
                    {
                        GetArchiveView();
                        break;
                    }
                //视频能力-查询当前用户稿件列表
                case "6":
                    {
                        GetArchiveViewList();
                        break;
                    }
                //三方一键开播-获取第三方开播授权链接
                case "7":
                    {
                        ThirdPartyLive_ObtainAuthorizedConnection();
                        break;
                    }
                //获取投稿分区列表
                case "8":
                    {
                        GetPartitionList();
                        break;
                    }
                //视频稿件上传预处理
                case "9":
                    {
                        Console.WriteLine("请输入文件名称：");
                        string name = Console.ReadLine();
                        Console.WriteLine("请输入上传类型：0-多分片，1-单个小文件（不超过100M）：");
                        string utype = Console.ReadLine();
                        VideoManuscriptUploadPreprocess(name, utype);
                        break;
                    }
                //单个小文件视频上传
                case "10":
                    {
                        Console.WriteLine("请输入预处理授权token：");
                        string upload_token = Console.ReadLine();
                        Console.WriteLine("请输入文件路径：");
                        string FilePath = Console.ReadLine();
                        UploadSingleShortVideoFileAsync(upload_token, FilePath);
                        break;
                    }
                //上传稿件封面
                case "11":
                    {
                        Console.WriteLine("请输入封面文件路径：");
                        string coverFilePath = Console.ReadLine();
                        VideoManuscriptCoverUpload(coverFilePath);
                        break;
                    }
                //分片上传稿件视频
                case "12":
                    {
                        Console.WriteLine("请输入视频上传预处理授权token：");
                        string uploadToken = Console.ReadLine();
                        Console.WriteLine("请输入文件路径：");
                        string FilePath = Console.ReadLine();
                        UploadVideoFilesInSegments(uploadToken, FilePath);
                        break;
                    }
                //分片上传文件合片
                case "13":
                    {
                        Console.WriteLine("请输入视频上传预处理授权token：");
                        string uploadToken = Console.ReadLine();
                        SplitUploadFilesMergeThem(uploadToken);
                        break;
                    }
                //视频稿件提交
                case "14":
                    {
                        Console.WriteLine("请输入视频上传预处理授权token：");
                        string uploadToken = Console.ReadLine();
                        Console.WriteLine("请输入标题：");
                        string title = Console.ReadLine();
                        Console.WriteLine("请输入通过接口上传返回的封面url：");
                        string cover = Console.ReadLine();
                        Console.WriteLine("请输入稿件分区id：");
                        int tid = int.Parse(Console.ReadLine());
                        Console.WriteLine("请输入视频标签，多个标签用半角逗号分割：");
                        string tag = Console.ReadLine();
                        Console.WriteLine("请输入版权：1-原创 2-转载：");
                        int copyRight = int.Parse(Console.ReadLine());
                        string source = string.Empty;
                        if (copyRight == 2)
                        {
                            Console.WriteLine("请输入原始稿件来源：");
                            source = Console.ReadLine();
                        }
                        VideoManuscriptSubmission(uploadToken, title, cover, tid, tag, copyRight, source);
                        break;
                    }
                //获取大会员-二次元通行证信息
                case "15":
                    {
                        GetAnimeUserValid();
                        break;
                    }
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
                var url = $"{Signature.MainDomain}/arcopen/fn/user/account/info";
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
            var url = $"{Signature.MainDomain}/arcopen/fn/live/room/info";
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
            var url = $"{Signature.MainDomain}/arcopen/fn/user/account/scopes";

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
            var url = $"{Signature.MainDomain}/arcopen/fn/archive/view?{queryString}";

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
            var url = $"{Signature.MainDomain}/arcopen/fn/archive/viewlist?{queryString}";

            var resp = Signature.SendRequest(url, "GET", AccessToken).Result;
            if (JObject.Parse(resp)?["code"]?.ToString() == "0")
            {
                WriteLog(resp);
            }
        }

        /// <summary>
        /// 获取第三方开播授权链接
        /// </summary>
        public static void ThirdPartyLive_ObtainAuthorizedConnection()
        {
            var requestParameters = new Dictionary<string, string?>
            {
                { "biz_code", "openplatform_demo" },
                { "open_id", OpenId },
                { "live_area_id", "816" },
                { "third_live_uuid",Guid.NewGuid().ToString("N").Substring(0,31)}
            };

            var queryParams = requestParameters
                .Where(kvp => kvp.Value != null)
                .Select(kvp => $"{kvp.Key}={kvp.Value}")
                .ToArray();

            var queryString = string.Join("&", queryParams);
            //https://open.bilibili.com/doc/4/5827c4a4-aab6-235e-624b-a47248d712e3
            var url = $"{Signature.MainDomain}/liveopen/fn/live/thirdPartyLive/grantUrl?{queryString}";

            var resp = Signature.SendRequest(url, "GET", AccessToken).Result;
            if (JObject.Parse(resp)?["code"]?.ToString() == "0")
            {
                WriteLog(resp);
            }
        }

        /// <summary>
        /// 获取大会员-二次元通行证信息
        /// </summary>
        public static void GetAnimeUserValid()
        {
            var requestParameters = new Dictionary<string, string?>
            {
                { "open_id", OpenId }
            };
            var url = $"{Signature.MainDomain}/arcopen/fn/common/vip/anime_user_valid";
            var reqJson = JsonConvert.SerializeObject(requestParameters);
            var resp = Signature.SendRequest(url, "POST", AccessToken, reqJson).Result;
            if (JObject.Parse(resp)?["code"]?.ToString() == "0")
            {
                WriteLog(resp);
            }
        }


        /// <summary>
        /// 获取投稿分区列表
        /// </summary>
        /// <returns></returns>
        public static void GetPartitionList()
        {
            //https://open.bilibili.com/doc/4/4f13299b-5316-142f-df6a-87313eaf85a9
            var url = $"{Signature.MainDomain}/arcopen/fn/archive/type/list";
            var resp = Signature.SendRequest(url, "GET", AccessToken).Result;
            if (JObject.Parse(resp)?["code"]?.ToString() == "0")
            {
                WriteLog(resp);
            }
        }

        /// <summary>
        /// 视频稿件上传预处理
        /// </summary>
        /// <param name="name">文件名称，需要包含拓展名(例如test.mp4)</param>
        /// <param name="utype">上传类型：0，1。0-多分片，1-单个小文件（不超过100M）</param>
        public static void VideoManuscriptUploadPreprocess(string name, string utype)
        {
            var requestParameters = new Dictionary<string, string?>
            {
                { "name", name },
                { "utype", utype }
            };
            //https://open.bilibili.com/doc/4/0c532c6a-e6fb-0aff-8021-905ae2409095
            var url = $"{Signature.MainDomain}/arcopen/fn/archive/video/init";
            var reqJson = JsonConvert.SerializeObject(requestParameters);
            var resp = Signature.SendRequest(url, "POST", AccessToken, reqJson).Result;
            if (JObject.Parse(resp)?["code"]?.ToString() == "0")
            {
                WriteLog(resp);
            }
        }

        /// <summary>
        /// 上传稿件封面
        /// </summary>
        /// <param name="FilePath">封面文件路径</param>
        public static void VideoManuscriptCoverUpload(string FilePath)
        {
            var url = $"{Signature.MainDomain}/arcopen/fn/archive/cover/upload";
            var resp = Signature.SendRequest(url, "POST", AccessToken, "", FilePath).Result;
            if (JObject.Parse(resp)?["code"]?.ToString() == "0")
            {
                WriteLog(resp);
            }
        }

        /// <summary>
        /// 单个小文件视频上传
        /// </summary>
        /// <param name="upload_token">视频稿件上传预处理获得的授权token</param>
        /// <param name="FilePath">要上传的文件路径</param>
        /// <returns></returns>
        public static void UploadSingleShortVideoFileAsync(string upload_token, string FilePath)
        {
            string url = $"{Signature.VideoDomain}/video/v2/upload?upload_token={upload_token}";
            var resp = Signature.SendRequest(url, "POST", AccessToken, "", FilePath).Result;
            if (JObject.Parse(resp)?["code"]?.ToString() == "0")
            {
                WriteLog(resp);
            }
        }

        /// <summary>
        /// 分片上传稿件视频
        /// </summary>
        /// <param name="upload_token">视频稿件上传预处理获得的授权token</param>
        /// <param name="FilePath">要上传的文件路径</param>
        public static void UploadVideoFilesInSegments(string upload_token, string FilePath)
        {
            long SliceSize = 1024 * 1024 * 8;
            byte[] FileByteArray = File.ReadAllBytes(FilePath);
            List<byte[]> chunks = new List<byte[]>();
            for (long i = 0; i < FileByteArray.Length; i += SliceSize)
            {
                long currentChunkSize = Math.Min(SliceSize, FileByteArray.Length - i);
                byte[] chunk = new byte[currentChunkSize];
                Array.Copy(FileByteArray, i, chunk, 0, currentChunkSize);
                chunks.Add(chunk);
            }
            Console.WriteLine($"视频文件长度:{FileByteArray.Length}byte，已切为{chunks.Count()}个切片");

            for (int i = 1; i <= chunks.Count(); i++)
            {
                Console.WriteLine($"({i}/{chunks.Count()})号切片开始上传");
                string url = $"{Signature.VideoDomain}/video/v2/part/upload?upload_token={upload_token}&part_number={i}";
                var resp = Signature.SendRequest(url, "POST", AccessToken, "", FilePath).Result;
                if (JObject.Parse(resp)?["code"]?.ToString() == "0")
                {
                    WriteLog(resp);
                }
                Console.WriteLine($"({i}/{chunks.Count()})号切片上传完成");
            }
        }

        /// <summary>
        /// 分片上传文件合片
        /// </summary>
        /// <param name="upload_token">视频稿件上传预处理获得的授权token</param>
        /// <returns></returns>
        public static void SplitUploadFilesMergeThem(string upload_token)
        {
            string url = $"{Signature.MainDomain}/arcopen/fn/archive/video/complete?upload_token={upload_token}";
            var resp = Signature.SendRequest(url, "POST", AccessToken).Result;
            if (JObject.Parse(resp)?["code"]?.ToString() == "0")
            {
                WriteLog(resp);
            }
        }

        /// <summary>
        /// 视频稿件提交
        /// </summary>
        /// <param name="upload_token">视频上传预处理授权</param>
        /// <param name="title">标题</param>
        /// <param name="cover">封面</param>
        /// <param name="tid">稿件分区id</param>
        /// <param name="tag">视频标签，多个标签用半角逗号分割</param>
        /// <param name="copyright">1-原创 2-转载</param>
        /// <param name="source">转载时必填，原始稿件来源</param>
        public static void VideoManuscriptSubmission(string upload_token, string title, string cover, int tid, string tag, int copyright, string source)
        {
            var requestParameters = new Dictionary<string, object?>
            {
                { "title", title },
                { "cover", cover},
                { "tid", tid },
                { "tag", tag},
                { "copyright", copyright}
            };
            if (copyright == 2)
            {
                requestParameters.Add("source", source);
            }
            //https://open.bilibili.com/doc/4/f7fc57dd-55a1-5cb1-cba4-61fb2994bf0f
            var url = $"{Signature.MainDomain}/arcopen/fn/archive/add-by-utoken?upload_token={upload_token}";
            var reqJson = JsonConvert.SerializeObject(requestParameters);
            var resp = Signature.SendRequest(url, "POST", AccessToken, reqJson).Result;
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
