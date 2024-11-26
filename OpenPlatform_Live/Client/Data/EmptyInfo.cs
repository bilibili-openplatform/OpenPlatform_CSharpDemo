using Newtonsoft.Json;

namespace OpenPlatform_LiveRoomData.Client.Data
{
    public class EmptyInfo
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

    }
}
