using Newtonsoft.Json;
using OpenPlatform_LiveRoomData.Client;
using OpenPlatform_LiveRoomData.Client.Data;
using OpenPlatform_LiveRoomData.Runtime;
using OpenPlatform_LiveRoomData.Runtime.Data;
using OpenPlatform_LiveRoomData.Runtime.Utilities;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Text;

namespace OpenPlatformSample.Live
{
    internal class Live
    {
        public static IBApiClient bApiClient = new BApiClient();
        public static string conn_id = string.Empty;
        public static string AccessToken = string.Empty;
        public static bool IsLive = false;
        

        public static async Task Start(string _AccessToken)
        {
            //是否为测试环境（一般用户可无视，给专业对接测试使用）
            BApi.isTestEnv = false;

            AccessToken = _AccessToken;
            
            var startInfo = new AppStartInfo();


            if (!string.IsNullOrEmpty(AccessToken))
            {
                startInfo = await bApiClient.StartInteractivePlay(AccessToken);
                if (startInfo?.Code != 0)
                {
                    Console.WriteLine(startInfo?.Message);
                    return;
                }

                var connid = startInfo?.Data?.Conn_id;
                if (connid != null)
                {
                    conn_id=connid;
                    IsLive = true;
                    Console.WriteLine("成功开启，开始心跳，场次ID: " + connid);
                    InteractivePlayHeartBeat m_PlayHeartBeat = new InteractivePlayHeartBeat(connid);
                    m_PlayHeartBeat.HeartBeatError += M_PlayHeartBeat_HeartBeatError;
                    m_PlayHeartBeat.HeartBeatSucceed += M_PlayHeartBeat_HeartBeatSucceed;
                    m_PlayHeartBeat.Start();
                    //长链接
                    WebSocketBLiveClient m_WebSocketBLiveClient;
                    m_WebSocketBLiveClient = new WebSocketBLiveClient(startInfo.GetWssLink(), startInfo.GetAuthBody());
                    m_WebSocketBLiveClient.OnDanmaku += WebSocketBLiveClientOnDanmaku;
                    m_WebSocketBLiveClient.OnGift += WebSocketBLiveClientOnGift;
                    m_WebSocketBLiveClient.OnGuardBuy += WebSocketBLiveClientOnGuardBuy;
                    m_WebSocketBLiveClient.OnSuperChat += WebSocketBLiveClientOnSuperChat;
                    m_WebSocketBLiveClient.OnLike += M_WebSocketBLiveClient_OnLike;
                    m_WebSocketBLiveClient.OnEnter += M_WebSocketBLiveClient_OnEnter;
                    m_WebSocketBLiveClient.OnLiveStart += M_WebSocketBLiveClient_OnLiveStart;
                    m_WebSocketBLiveClient.OnLiveEnd += M_WebSocketBLiveClient_OnLiveEnd;
                    //m_WebSocketBLiveClient.Connect();
                    m_WebSocketBLiveClient.Connect(TimeSpan.FromSeconds(30));
                }
                else
                {
                    Console.WriteLine("开启直播间长连错误: " + startInfo.ToString());
                }
                await Task.Run(async () =>
                {
                   
                    return;
                });
            }
        }

        private static void M_WebSocketBLiveClient_OnLiveEnd(LiveEnd liveEnd)
        {
            StringBuilder sb = new StringBuilder($"直播间[{liveEnd.room_id}]直播结束，分区ID：【{liveEnd.area_id}】,标题为【{liveEnd.title}】");
            Logger.Log(sb.ToString());
        }

        private static void M_WebSocketBLiveClient_OnLiveStart(LiveStart liveStart)
        {
            StringBuilder sb = new StringBuilder($"直播间[{liveStart.room_id}]开始直播，分区ID：【{liveStart.area_id}】,标题为【{liveStart.title}】");
            Logger.Log(sb.ToString());
        }

        private static void M_WebSocketBLiveClient_OnEnter(Enter enter)
        {
            StringBuilder sb = new StringBuilder($"用户[{enter.uname}]进入房间");
            Logger.Log(sb.ToString());
        }

        private static void M_WebSocketBLiveClient_OnLike(Like like)
        {
            StringBuilder sb = new StringBuilder($"用户[{like.uname}]点赞了{like.unamelike_count}次");
            Logger.Log(sb.ToString());
        }

        private static void M_PlayHeartBeat_HeartBeatSucceed()
        {
            Logger.Log("心跳成功");
        }

        private static void M_PlayHeartBeat_HeartBeatError(string json)
        {
            JsonConvert.DeserializeObject<EmptyInfo>(json);
            Logger.Log("心跳失败" + json);
        }

        private static void WebSocketBLiveClientOnSuperChat(SuperChat superChat)
        {
            StringBuilder sb = new StringBuilder($"用户[{superChat.userName}]发送了{superChat.rmb}元的醒目留言内容：{superChat.message}");
            Logger.Log(sb.ToString());
        }

        private static void WebSocketBLiveClientOnGuardBuy(Guard guard)
        {
            StringBuilder sb = new StringBuilder($"用户[{guard.userInfo.userName}]充值了{(guard.guardUnit=="月"?(guard.guardNum+"个月"):guard.guardUnit.TrimStart('*'))}[{(guard.guardLevel==1?"总督":guard.guardLevel==2?"提督":"舰长")}]大航海");
            Logger.Log(sb.ToString());
        }

        private static void WebSocketBLiveClientOnGift(SendGift sendGift)
        {
            StringBuilder sb = new StringBuilder($"用户[{sendGift.userName}]赠送了{sendGift.giftNum}个[{sendGift.giftName}]");
            Logger.Log(sb.ToString());
        }

        private static void WebSocketBLiveClientOnDanmaku(Dm dm)
        {
            StringBuilder sb = new StringBuilder($"用户[{dm.userName}]发送弹幕:{dm.msg}");
            Logger.Log(sb.ToString());
        }

    }
}