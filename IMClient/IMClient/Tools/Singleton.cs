namespace IMClient.Tools
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        private static readonly object mLockObj = new object();
        protected static T mInstance;
        public static T Instance
        {
            get
            {
                lock (mLockObj)
                {
                    return mInstance ?? (mInstance = new T());
                }
            }
        }
    }
}