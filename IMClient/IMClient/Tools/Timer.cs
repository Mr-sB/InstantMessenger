using System;
using System.Threading;
using System.Threading.Tasks;

namespace IMClient.Tools
{
    public class Timer
    {
        private CancellationTokenSource mTokenSource;

        private Timer(float delay, Action action)
        {
            mTokenSource = new CancellationTokenSource();
            var token = mTokenSource.Token;
            Task.Run(async () =>
            {
                await Task.Delay((int) Math.Round(delay * 1000), token);
                if (!token.IsCancellationRequested)
                    SafeCall(action);
                mTokenSource.Dispose();
                mTokenSource = null;
            }, token);
        }

        public void Cancel()
        {
            mTokenSource?.Cancel();
        }

        private static void SafeCall(Action action)
        {
            if(action == null) return;
            try
            {
                action();
            }
            catch
            {
                // ignored
            }
        }

        private static bool Check(float delay, Action action)
        {
            if(action == null) return false;
            if (delay > 0) return true;
            SafeCall(action);
            return false;
        }
        
        /// <summary>
        /// 延迟执行Action
        /// </summary>
        /// <param name="action">action</param>
        /// <param name="delay">延迟时间(秒)</param>
        /// <returns>Timer</returns>
        public static Timer DelayAction(float delay, Action action)
        {
            return !Check(delay, action) ? null : new Timer(delay, action);
        }
    }
}