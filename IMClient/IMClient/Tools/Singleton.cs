namespace IMClient.Tools
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        private static readonly object mLockObj = new object();
        private static T mInstance;
        public static T Instance
        {
            get
            {
                if (mInstance != null) return mInstance;
                lock (mLockObj)
                {
                    mInstance = new T();
                    return mInstance;
                }
            }
        }
    }
}