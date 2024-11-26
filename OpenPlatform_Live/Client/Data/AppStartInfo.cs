using Newtonsoft.Json;

namespace OpenPlatform_LiveRoomData.Client.Data
{
    public class AppStartInfo
    {
        /// <summary>
        /// 请求相应 非0为异常case 业务处理
        /// </summary>
        [JsonProperty("code")]
        public int Code;
        /// <summary>
        /// 异常case提示文案
        /// </summary>
        [JsonProperty("message")]
        public string Message;
        /// <summary>
        ///响应体
        /// </summary>
        [JsonProperty("data")]
        public AppStartData Data { get; set; }


        /// <summary>
        /// 获取场次ID
        /// </summary>
        /// <returns></returns>
        public string GetGameId() => Data?.Conn_id;
        /// <summary>
        /// 获取长链地址
        /// </summary>
        /// <returns></returns>
        public IList<string> GetWssLink() => Data?.WebsocketInfo?.WssLink;
        /// <summary>
        /// 获取长链地址
        /// </summary>
        /// <returns></returns>
        public string GetAuthBody() => Data?.WebsocketInfo?.AuthBody;
    }



    public class AppStartData
    {
        /// <summary>
        /// 场次信息
        /// </summary>
        [JsonProperty("conn_id")]
        public string Conn_id;
        /// <summary>
        /// 长连信息
        /// </summary>
        [JsonProperty("websocket_info")]
        public AppStartWebsocketInfo WebsocketInfo;
    }
    public class AppStartWebsocketInfo
    {
        /// <summary>
        /// 长连使用的请求json体 第三方无需关注内容,建立长连时使用即可
        /// </summary>
        [JsonProperty("auth_body")]
        public string AuthBody;
        /// <summary>
        ///  wss 长连地址
        /// </summary>
        [JsonProperty("wss_link")]
        public List<string> WssLink;
    }


    public class AppStartAnchorInfo
    {
        /// <summary>
        /// 主播房间号
        /// </summary>
        [JsonProperty("room_id")]
        public long RoomId;
        /// <summary>
        /// 主播昵称
        /// </summary>
        [JsonProperty("uname")]
        public string UName;
        /// <summary>
        /// 主播头像
        /// </summary>
        [JsonProperty("uface")]
        public string UFace;
        /// <summary>
        /// 主播Uid
        /// </summary>
        [JsonProperty("uid")]
        public string Uid;
    }
}
