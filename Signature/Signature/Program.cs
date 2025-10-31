using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenPlatform_Signature
{

    public class Signature
    {

        // 需要填写的内容
        public static string Client_ID = ""; // 入驻开放平台后，通过并且创建应用完成后，应用的Client_ID（https://open.bilibili.com/company-core）
        public static string App_Secret = ""; // 入驻开放平台后，通过并且创建应用完成后，应用的App_Secret（https://open.bilibili.com/company-core）
        public static string ReturnUrl = "";//创建应用后，开发者自行设置的'应用回调域'（https://open.bilibili.com/company-core/{Client_ID}/detail）

        public static bool IsUAT = false; //该参数默认为false，自行打开无用
        public static string Color = ""; //染色参数，自行打开无用
        public static string MainDomain = IsUAT ? "https://uat-member.bilibili.com" : "https://member.bilibili.com";
        public static string VideoDomain = IsUAT ? "https://uat-openupos.bilivideo.com" : "https://openupos.bilivideo.com";
        public static string ApiDomain = IsUAT ? "https://uat-api.bilibili.com" : "https://api.bilibili.com";
        public static string AccountDomain = IsUAT ? "https://uat-account.bilibili.com" : "https://account.bilibili.com";

        //终端是否打印curl详细信息
        public static bool Printf = true;

        // 常量定义
        internal const string AcceptHeader = "Accept";
        internal const string AuthorizationHeader = "Authorization";
        internal const string JsonType = "application/json";
        internal const string BiliVersion = "2.0";
        internal const string HmacSha256 = "HMAC-SHA256";
        internal const string BiliTimestampHeader = "x-bili-timestamp";
        internal const string BiliSignatureMethodHeader = "x-bili-signature-method";
        internal const string BiliSignatureNonceHeader = "x-bili-signature-nonce";
        internal const string BiliAccessKeyIdHeader = "x-bili-accesskeyid";
        internal const string BiliSignVersionHeader = "x-bili-signature-version";
        internal const string BiliContentMD5Header = "x-bili-content-md5";
        internal const string AccessTokenHeader = "access-token";

        // 公共头部类
        public const bool isTestEnv = false;
        internal class CommonHeader
        {
            internal string Accept { get; set; }
            internal string Timestamp { get; set; }
            internal string SignatureMethod { get; set; }
            internal string SignatureVersion { get; set; }
            internal string Authorization { get; set; }
            internal string Nonce { get; set; }
            internal string AccessKeyId { get; set; }
            internal string ContentMD5 { get; set; }
            internal string AccessToken { set; get; }

            // 将所有字段转换为 map
            internal Dictionary<string, string> ToMap()
            {
                return new Dictionary<string, string>
                {
                    {BiliTimestampHeader, Timestamp},
                    {BiliSignatureMethodHeader, SignatureMethod},
                    {BiliSignatureNonceHeader, Nonce},
                    {BiliAccessKeyIdHeader, AccessKeyId},
                    {BiliSignVersionHeader, SignatureVersion},
                    {BiliContentMD5Header, ContentMD5},
                    {AuthorizationHeader, Authorization},
                    {AcceptHeader, Accept},
                    {AccessTokenHeader, AccessToken}
                };
            }

            // 生成需要加密的文本
            internal string ToSortedString()
            {
                var sortedMap = ToMap().Where(kvp => kvp.Key.StartsWith("x-bili-")).OrderBy(kvp => kvp.Key).ToList();
                StringBuilder sb = new StringBuilder();
                foreach (var kvp in sortedMap)
                {
                    sb.Append($"{kvp.Key}:{kvp.Value}\n");
                }
                return sb.ToString().TrimEnd('\n');
            }
        }

        // 基础响应类
        public class BaseResp
        {
            public long code { get; set; }
            public string message { get; set; }
            public string requestId { get; set; }
            public string data { get; set; }
        }

        /// <summary>
        /// 发起请求
        /// </summary>
        /// <param name="InterfaceUrl">接口地址</param>
        /// <param name="RequestType">get或post二选一，用于进行签名</param>
        /// <param name="AccessToken">用户授权后，使用授权code兑换的Token</param>
        /// <param name="reqJson">请求参数的body内容json字符串</param>
        /// <param name="filePath">如果需要上传文件，那么文件路径</param>
        /// <returns></returns>
        public static async Task<string> SendRequest(string InterfaceUrl, string RequestType, string AccessToken, string reqJson = "", string filePath = "", byte[] FileByteArray = null)
        {
            var response = await ApiRequest(reqJson, InterfaceUrl, RequestType, AccessToken, filePath, FileByteArray);
            if (response == null)
            {
                if (Printf)
                    Console.WriteLine("请求失败，网络信息返回为空");
            }
            else
            {
                if (!string.IsNullOrEmpty(response))
                {
                    try
                    {
                        if (JObject.Parse(response)?["code"]?.ToString() != "0")
                            if (Printf)
                                Console.WriteLine($"网络请求出现错误：请求的接口URL为：\n{InterfaceUrl}\ncode：{JObject.Parse(response)?["code"]?.ToString()}\nmessage：{JObject.Parse(response)?["message"]?.ToString()}\nrequest_id：{JObject.Parse(response)?["request_id"]?.ToString()}");
                        if (isTestEnv)
                            if (Printf)
                                Console.WriteLine($"\nResponse内容:\n");
                        // 解析 JSON 字符串到 JsonDocument
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
                                if (isTestEnv)
                                    if (Printf)
                                        Console.WriteLine(finalOutput);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        if (isTestEnv)
                            if (Printf)
                                Console.WriteLine($"\n返回的内容可能不为Json格式，原始内容:\n{response}");
                    }
                }
                else
                {
                    if (isTestEnv)
                        if (Printf)
                            Console.WriteLine($"\nResponse内容为空\n");
                }

            }
            return response;
        }


        // HTTP 请求示例方法
        private static async Task<string> ApiRequest(string reqJson, string InterfaceUrl, string Htpp_Type, string AccessToken = "", string filePath = "", byte[] FileByteArray = null)
        {
            if (Printf)
            {
                Console.WriteLine($"请求地址：{InterfaceUrl}");
                Console.WriteLine($"请求类型：{Htpp_Type}");
                Console.WriteLine($"请求参数：");
                if (!string.IsNullOrEmpty(reqJson))
                    Console.WriteLine($"body:\n{reqJson}");
                else
                    Console.WriteLine($"body为空\n");
            }

            var header = new CommonHeader
            {
                Accept = JsonType,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                SignatureMethod = HmacSha256,
                SignatureVersion = BiliVersion,
                Authorization = "",
                Nonce = Guid.NewGuid().ToString(),
                AccessKeyId = Client_ID,
                ContentMD5 = Md5(reqJson),
                AccessToken = AccessToken
            };
            header.Authorization = CreateSignature(header, App_Secret);

            var sortedMap = header.ToMap().OrderBy(kvp => kvp.Key).ToList();
            if (Printf)
                Console.WriteLine($"Header信息：\n");
            foreach (var item in sortedMap)
            {
                if (Printf)
                    Console.WriteLine($"{item.Key} = {item.Value}");
            }
            if (Printf)
                Console.WriteLine();

            if (isTestEnv)
            {
                if (Printf)
                {
                    Console.WriteLine($"计算生成的签名：{header.Authorization}\n");
                    Console.WriteLine($"Header信息：\n");
                    foreach (var item in sortedMap)
                    {
                        Console.WriteLine($"{item.Key}:{item.Value}");
                    }
                    Console.WriteLine();
                }
            }

            using (var client = new HttpClient())
            {
                if (Htpp_Type.ToLower() == "post")
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, InterfaceUrl);

                    // 添加 Headers
                    foreach (var kvp in header.ToMap())
                    {
                        requestMessage.Headers.Add(kvp.Key, kvp.Value);
                    }
                    // 添加染色参数
                    if (!string.IsNullOrEmpty(Color))
                        requestMessage.Headers.Add("x1-bilispy-color", Color);
                    // 设置文件内容作为请求体
                    if ((!string.IsNullOrEmpty(filePath) && File.Exists(filePath)) || FileByteArray != null)
                    {
                        if (InterfaceUrl.Contains("member.bilibili.com"))
                        {
                            var content = new MultipartFormDataContent();
                            ByteArrayContent fileContent;
                            if (FileByteArray != null)
                            {
                                fileContent = new ByteArrayContent(FileByteArray);
                            }
                            else
                            {
                                fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
                            }
                            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                            {
                                Name = "\"file\"",
                                FileName = "\"" + Path.GetFileName(filePath) + "\""
                            };
                            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                            content.Add(fileContent);
                            if (!string.IsNullOrEmpty(reqJson))
                            {
                                var jsonContent = new StringContent(reqJson, Encoding.UTF8, JsonType);
                                content.Add(jsonContent, "json");
                            }
                            requestMessage.Content = content;
                        }
                        else
                        {
                            byte[] fileBytes = FileByteArray ?? File.ReadAllBytes(filePath);
                            requestMessage.Content = new ByteArrayContent(fileBytes);
                            // 强制设置 Content-Type 为 application/json
                            requestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                        }

                    }
                    else
                    {
                        requestMessage.Content = new StringContent(reqJson, Encoding.UTF8, JsonType);
                    }

                    var response = await client.SendAsync(requestMessage);
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    string curlCommand = GenerateCurlCommand(requestMessage, reqJson, filePath, FileByteArray);
                    if (Printf)
                    {
                        Console.WriteLine("Generated curl command:");
                        Console.WriteLine(curlCommand);
                    }

                    return jsonResponse;
                }
                else if (Htpp_Type.ToLower() == "get")
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, InterfaceUrl);
                    requestMessage.Content = new StringContent("", Encoding.UTF8, "application/json");
                    foreach (var kvp in header.ToMap())
                    {
                        requestMessage.Headers.Add(kvp.Key, kvp.Value);
                    }
                    var response = await client.SendAsync(requestMessage);
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return jsonResponse;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 测试签名
        /// </summary>
        /// <param name="Client_ID">用于计算签名的应用ClientID</param>
        /// <param name="App_Secret">用于计算签名的应用Secret</param>
        /// <param name="Nonce">用于计算签名的全网唯一字符串</param>
        /// <param name="TimeStamp">用于计算签名的秒级时间戳</param>
        /// <param name="ReqJson">用于计算签名的应用body内容或者已计算好的md5值</param>
        /// <returns></returns>
        public static string SignatureTest(string Client_ID, string App_Secret, string Nonce, string TimeStamp, string ReqJson)
        {
            var header = new CommonHeader
            {
                Timestamp = string.IsNullOrEmpty(TimeStamp) ? DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() : TimeStamp,
                SignatureMethod = HmacSha256,
                Nonce = string.IsNullOrEmpty(Nonce) ? Guid.NewGuid().ToString() : Nonce,
                AccessKeyId = Client_ID,
                SignatureVersion = BiliVersion,
                ContentMD5 = Regex.IsMatch(ReqJson, @"^[a-fA-F0-9]{32}$") ? ReqJson : Md5(ReqJson)
            };

            header.Authorization = CreateSignature(header, App_Secret);
            return header.Authorization;
        }

        private static string GenerateCurlCommand(HttpRequestMessage requestMessage, string reqJson, string filePath, byte[] FileByteArray)
        {
            var curlCommand = new StringBuilder();
            curlCommand.Append("curl -X ").Append(requestMessage.Method.Method).Append(" ");

            // 添加 URL
            curlCommand.Append("\"").Append(requestMessage.RequestUri).Append("\" ");

            // 添加请求头
            foreach (var header in requestMessage.Headers)
            {
                curlCommand.Append("\r\n-H \"").Append(header.Key).Append(": ").Append(string.Join(", ", header.Value)).Append("\" ");
            }

            // 添加请求体
            if (requestMessage.Content != null)
            {
                if (requestMessage.Content is MultipartFormDataContent)
                {
                    if (!string.IsNullOrEmpty(filePath) || FileByteArray != null)
                    {
                        curlCommand.Append("\r\n-F \"file=@").Append(filePath).Append("\" ");
                    }
                    if (!string.IsNullOrEmpty(reqJson))
                    {
                        curlCommand.Append("\r\n-F \"json=").Append(reqJson).Append("\" ");
                    }
                }
                else
                {
                    curlCommand.Append("\r\n-d \"").Append(reqJson).Append("\" ");
                }
            }

            return curlCommand.ToString();
        }


        // 生成Authorization加密串
        private static string CreateSignature(CommonHeader header, string accessKeySecret)
        {
            var sStr = header.ToSortedString();
            if (isTestEnv)
                if (Printf)
                    Console.WriteLine($"\n用于计算签名的字符串：\n{sStr}\n");
            return HmacSHA256(accessKeySecret, sStr);
        }

        // MD5加密
        private static string Md5(string str)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        // HMAC-SHA256算法
        private static string HmacSHA256(string key, string data)
        {
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

}
