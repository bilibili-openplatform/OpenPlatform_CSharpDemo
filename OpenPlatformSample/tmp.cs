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
        public static void ReadSecurityConfig(string filePath = "./config/secrets.json")
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

                if (string.IsNullOrEmpty(Signature.Client_ID))
                    OpenPlatform_Signature.Signature.Client_ID = Signature.IsUAT ? config.UAT_Client_ID : config.PROD_Client_ID;
                if (string.IsNullOrEmpty(Signature.App_Secret))
                    OpenPlatform_Signature.Signature.App_Secret = Signature.IsUAT ? config.UAT_App_Secret : config.PROD_App_Secret;
                if (string.IsNullOrEmpty(Signature.ReturnUrl))
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
            public bool UAT { get; set; } = false;
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


        public static async Task<bool> UploadFileAsync(string filePath,string put_url,string content_type = "application/octet-stream")
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // 设置超时时间
                    httpClient.Timeout = TimeSpan.FromMinutes(5);

                    // 读取文件内容
                    byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
                    var fileContent = new ByteArrayContent(fileBytes);

                    // 设置 Content-Type
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(content_type);

                    // 构建请求头集合用于输出
                    var requestHeaders = new Dictionary<string, string>
                    {
                        ["Content-Type"] = content_type,
                        ["Content-Length"] = fileBytes.Length.ToString()
                    };

                    // 输出类似 curl 的命令格式
                    OutputCurlCommand(requestHeaders, fileBytes.Length, put_url);

                    // 使用 PUT 方法上传
                    var response = await httpClient.PutAsync(
                        put_url,
                        fileContent);

                    // 输出响应信息
                    await OutputResponseInfo(response);

                    // 检查响应
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("文件上传成功！");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"上传失败，状态码：{response.StatusCode}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"上传过程中发生错误：{ex.Message}");
                return false;
            }
        }

        private static void OutputCurlCommand(Dictionary<string, string> headers, long contentLength,string put_url)
        {
            Console.WriteLine("Generated curl command:");
            Console.WriteLine($"curl -X PUT \""+ put_url + "\"");

            // 输出请求头
            foreach (var header in headers)
            {
                Console.WriteLine($"-H \"{header.Key}: {header.Value}\"");
            }

            // 如果有额外的固定请求头，也可以在这里添加
            // Console.WriteLine($"-H \"User-Agent: {httpClient.DefaultRequestHeaders.UserAgent}\"");

            Console.WriteLine($"--data-binary \"@your_file_path\"");
            Console.WriteLine();
        }

        private static async Task OutputResponseInfo(HttpResponseMessage response)
        {
            Console.WriteLine("Response:");
            Console.WriteLine($"  Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"  Reason Phrase: {response.ReasonPhrase}");
            Console.WriteLine();

            // 输出响应头
            Console.WriteLine("Response Headers:");
            foreach (var header in response.Headers)
            {
                Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (response.Content.Headers.Any())
            {
                foreach (var header in response.Content.Headers)
                {
                    Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                }
            }
            Console.WriteLine();

            // 输出响应内容
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response Body:");

            try
            {
                // 尝试格式化 JSON 输出
                if (!string.IsNullOrEmpty(responseContent))
                {
                    if (responseContent.TrimStart().StartsWith("{"))
                    {
                        // 简单格式化 JSON（对于复杂 JSON 建议使用 Newtonsoft.Json 或 System.Text.Json）
                        var formattedJson = FormatJson(responseContent);
                        Console.WriteLine(formattedJson);
                    }
                    else
                    {
                        Console.WriteLine(responseContent);
                    }
                }
                else
                {
                    Console.WriteLine("(Empty response body)");
                }
            }
            catch
            {
                Console.WriteLine(responseContent);
            }
            Console.WriteLine();
        }

        private static string FormatJson(string json)
        {
            // 简单的 JSON 格式化方法
            // 对于生产环境，建议使用 Newtonsoft.Json 或 System.Text.Json 进行更好的格式化
            int indentLevel = 0;
            var result = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < json.Length; i++)
            {
                char currentChar = json[i];

                if (currentChar == '"' && (i == 0 || json[i - 1] != '\\'))
                {
                    inQuotes = !inQuotes;
                }

                if (!inQuotes)
                {
                    switch (currentChar)
                    {
                        case '{':
                        case '[':
                            result.Append(currentChar);
                            result.AppendLine();
                            indentLevel++;
                            result.Append(new string(' ', indentLevel * 2));
                            break;
                        case '}':
                        case ']':
                            result.AppendLine();
                            indentLevel--;
                            result.Append(new string(' ', indentLevel * 2));
                            result.Append(currentChar);
                            break;
                        case ',':
                            result.Append(currentChar);
                            result.AppendLine();
                            result.Append(new string(' ', indentLevel * 2));
                            break;
                        case ':':
                            result.Append(": ");
                            break;
                        default:
                            result.Append(currentChar);
                            break;
                    }
                }
                else
                {
                    result.Append(currentChar);
                }
            }

            return result.ToString();
        }

    }
}
