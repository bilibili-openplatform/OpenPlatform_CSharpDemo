using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenPlatform_LiveRoomData.Runtime.Data;
using OpenPlatform_LiveRoomData.Runtime.Utilities;
using Logger = OpenPlatform_LiveRoomData.Runtime.Utilities.Logger;
#if NET5_0_OR_GREATER
using System.Net;
#elif UNITY_2020_3_OR_NEWER
using UnityEngine.Networking;
#endif

namespace OpenPlatform_LiveRoomData.Runtime
{
    /// <summary>
    /// 各类b站api
    /// </summary>
    public static class BApi
    {
        /// <summary>
        /// 是否为测试环境的api
        /// </summary>
        public static bool isTestEnv;

        /// <summary>
        /// 开放平台域名
        /// </summary>
        private static string OpenLiveDomain =>
            isTestEnv ? "http://test-member.bilibili.com" : "https://member.bilibili.com";

        /// <summary>
        /// 应用开启
        /// </summary>
        private const string k_InteractivePlayStart = "/arcopen/fn/live/room/ws-start";

        /// <summary>
        /// 应用心跳
        /// </summary>
        private const string k_InteractivePlayHeartBeat = "/arcopen/fn/live/room/ws-heartbeat";

        /// <summary>
        /// 应用批量心跳
        /// </summary>
        private const string k_InteractivePlayBatchHeartBeat = "/arcopen/fn/live/room/ws-batch-heartbeat";


        private const string k_Post = "POST";

 

        public static async Task<string> StartInteractivePlay(string AccessToken)
        {

            var postUrl = OpenLiveDomain + k_InteractivePlayStart;

            var result = await OpenPlatform_Signature.Signature.SendRequest(postUrl, k_Post, AccessToken);

            return result;
        }

        public static async Task<string> HeartBeatInteractivePlay(string connId)
        {
            var postUrl = OpenLiveDomain + k_InteractivePlayHeartBeat + $"?client_id={OpenPlatform_Signature.Signature.Client_ID}";
            string param = "";
            if (connId != null)
            {
                param = $"{{\"conn_id\":\"{connId}\"}}";

            }

            var result = await OpenPlatform_Signature.Signature.SendRequest(postUrl, k_Post,"", param);
            return result;
        }

        public static async Task<string> BatchHeartBeatInteractivePlay(string[] gameIds)
        {
            var postUrl = OpenLiveDomain + k_InteractivePlayBatchHeartBeat + $"?client_id={OpenPlatform_Signature.Signature.Client_ID}";
            GameIds games = new GameIds()
            {
                gameIds = gameIds
            };
            var param = JsonConvert.SerializeObject(games);
            var result = await OpenPlatform_Signature.Signature.SendRequest(postUrl, k_Post,"", param);
            return result;
        }

        private static async Task<string> RequestWebUTF8(string url, string method, string param,
            string cookie = null)
        {
#if NET5_0_OR_GREATER
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = method;

            if (param != null)
            {
                SignUtility.SetReqHeader(req, param, cookie);
            }

            HttpWebResponse httpResponse = (HttpWebResponse)(await req.GetResponseAsync());
            Stream stream = httpResponse.GetResponseStream();

            if (stream != null)
            {
                using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                result = await reader.ReadToEndAsync();
            }

            return result;

#elif UNITY_2020_3_OR_NEWER
            UnityWebRequest webRequest = new UnityWebRequest(url);
            webRequest.method = method;
            if (param != null)
            {
                SignUtility.SetReqHeader(webRequest, param, cookie);
            }

            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.disposeUploadHandlerOnDispose = true;
            webRequest.disposeDownloadHandlerOnDispose = true;
            await webRequest.SendWebRequest();
            var text = webRequest.downloadHandler.text;

            webRequest.Dispose();
            return text;
#endif
        }
#if UNITY_2020_3_OR_NEWER
        private static TaskAwaiter GetAwaiter(this UnityEngine.AsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<object>();
            asyncOp.completed += _ => { tcs.SetResult(null); };
            return ((Task) tcs.Task).GetAwaiter();
        }
#endif
    }
}