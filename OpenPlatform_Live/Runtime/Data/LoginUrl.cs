using Newtonsoft.Json;

namespace OpenPlatform_LiveRoomData.Runtime.Data
{
    public struct LoginUrl
    {
        [JsonProperty("data")]
        public LoginUrlData data;
        [JsonProperty("status")]
        public bool status;
    }
}