using Newtonsoft.Json;

namespace OpenPlatform_LiveRoomData.Runtime.Data
{
    public struct LoginStatusData
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}