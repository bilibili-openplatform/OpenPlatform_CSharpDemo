using Newtonsoft.Json;
using OpenPlatform_LiveRoomData.Client.Data;
using OpenPlatform_LiveRoomData.Runtime;

namespace OpenPlatform_LiveRoomData.Client
{
    public class BApiClient : IBApiClient
    {
        /// <summary>
        /// 开启互动玩法
        /// </summary>
        /// <param name="code"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public async Task<AppStartInfo> StartInteractivePlay(string AccessToken)
        {
            var respStr = await BApi.StartInteractivePlay(AccessToken);
            return JsonConvert.DeserializeObject<AppStartInfo>(respStr);
        }

        /// <summary>
        /// 批量应用心跳
        /// </summary>
        /// <param name="gameIds">开启应用 返回的gameId</param>
        /// <returns></returns>
        public async Task<EmptyInfo> HeartBeatInteractivePlay(string gameId)
        {
            var respStr = await BApi.HeartBeatInteractivePlay(gameId);
            return JsonConvert.DeserializeObject<EmptyInfo>(respStr);
        }

        /// <summary>
        /// 批量应用心跳
        /// </summary>
        /// <param name="gameIds">开启应用 返回的gameId</param>
        /// <returns></returns>
        public async Task<EmptyInfo> BatchHeartBeatInteractivePlay(string[] gameIds)
        {
            var respStr = await BApi.BatchHeartBeatInteractivePlay(gameIds);
            return JsonConvert.DeserializeObject<EmptyInfo>(respStr);
        }
    }
}
