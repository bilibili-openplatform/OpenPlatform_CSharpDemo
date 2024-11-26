using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace OpenPlatform_UserInfo
{
    public class info
    {
        /// <summary>
        /// 获取用户公开信息
        /// 文档:https://member.bilibili.com/arcopen/fn/user/account/info
        /// </summary>
        /// <returns></returns>
        public static (string name, string openid, string face) GetInfo(string AccessToken)
        {
            (string name, string openid, string face) info = ("", "", "");
            string resp = OpenPlatform_Signature.Signature.SendRequest("https://member.bilibili.com/arcopen/fn/user/account/info", "GET", AccessToken).Result;
            info.name = JObject.Parse(resp)?["data"]?["name"]?.ToString();
            info.openid = JObject.Parse(resp)?["data"]?["openid"]?.ToString();
            info.face = JObject.Parse(resp)?["data"]?["face"]?.ToString();
            return info;
        }
    }
}
