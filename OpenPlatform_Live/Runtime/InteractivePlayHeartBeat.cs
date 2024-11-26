using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenPlatform_LiveRoomData.Runtime
{
    public delegate void HeartBeatSucceed();

    public delegate void HeartBeatError(string json);

    public class InteractivePlayHeartBeat : IDisposable
    {
        public event HeartBeatSucceed HeartBeatSucceed;
        public event HeartBeatError HeartBeatError;
        private readonly CancellationTokenSource m_Cancellation;
        private readonly string[] m_CoonId;
        private readonly int m_Time;

        public InteractivePlayHeartBeat(string coonId, int time = 20000, CancellationTokenSource cancellation = null)
        {
            m_CoonId = new[] { coonId };
            m_Time = time;
            m_Cancellation = cancellation ?? new CancellationTokenSource();
        }

        public InteractivePlayHeartBeat(string[] coonIds, int time = 20000, CancellationTokenSource cancellation = null)
        {
            m_CoonId = coonIds;
            m_Time = time;
            m_Cancellation = cancellation ?? new CancellationTokenSource();
        }

        private async Task HeartBeatTask()
        {
            while (true)
            {
                try
                {
                    string res = "";
                    if (m_CoonId.Length == 1)
                    {
                        res = await BApi.HeartBeatInteractivePlay(m_CoonId[0]);
                    }
                    else
                    {
                        res = await BApi.BatchHeartBeatInteractivePlay(m_CoonId);
                    }

                    var json = JObject.Parse(res);
                    var code = json["code"]!.ToObject<int>();
                    if (code == 0)
                    {
                        HeartBeatSucceed?.Invoke();
                    }
                    else
                    {
                        HeartBeatError?.Invoke(res);
                        return;
                    }
                }
                catch (Exception e)
                {
                    HeartBeatError?.Invoke(e.Message);
                    return;
                }

                switch (m_Cancellation)
                {
                    case { IsCancellationRequested: true }:
                    case null:
                        return;
                    default:
                        //默认20秒一次心跳
                        await Task.Delay(m_Time);
                        break;
                }
            }
        }

        public void Start()
        {
            var task = HeartBeatTask();
            if (task.Status == TaskStatus.Created)
                task.Start();
        }

        public void Stop()
        {
            m_Cancellation.Cancel();
        }

        public void Dispose()
        {
            m_Cancellation.Dispose();
        }
    }
}