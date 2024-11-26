using Newtonsoft.Json;
namespace OpenPlatform_LiveRoomData.Runtime.Data
{
    public struct LoginUrlData
    {
        [JsonProperty("oauthKey")]
        public string oauthKey;
        [JsonProperty("url")]
        public string url;
    }
}