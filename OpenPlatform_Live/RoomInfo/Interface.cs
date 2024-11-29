using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPlatform_LiveRoomData.RoomInfo
{
    public class Interface
    {
        public static string GetRoomInfo(string AccessToken)
        {
            string resp = OpenPlatform_Signature.Signature.SendRequest("https://member.bilibili.com/arcopen/fn/live/room/info", "GET", AccessToken).Result;
            //info.name = JObject.Parse(resp)?["data"]?["name"]?.ToString();
            //info.openid = JObject.Parse(resp)?["data"]?["openid"]?.ToString();
            //info.face = JObject.Parse(resp)?["data"]?["face"]?.ToString();
            return resp;
        }
    }
}
