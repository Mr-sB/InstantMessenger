using Android.Content;
using Android.OS;
using Android.Widget;
using System;

namespace IMClient.Tools
{
    public static class ToastUtil
    {
        private static Context mContext;

        /// <summary>
        /// 设置默认的上下文
        /// </summary>
        public static void SetDefaultContext(Context context)
        {
            mContext = context;
        }

        /// <summary>
        /// 在子线程调用Toast
        /// </summary>
        public static void ToastOnSubThread(this Context context, string text, ToastLength duration = ToastLength.Short)
        {
            if (context == null) return;
            try
            {
                if (Looper.MyLooper() == null)
                {
                    Looper.Prepare();
                }
                Android.Widget.Toast.MakeText(context, text, duration).Show();
                Looper.Loop();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void ToastOnSubThread(this string text, ToastLength duration = ToastLength.Short)
        {
            ToastOnSubThread(mContext, text, duration);
        }

        public static void Toast(this Context context, string text, ToastLength duration = ToastLength.Short)
        {
            if (mContext == null) return;
            Android.Widget.Toast.MakeText(context, text, duration).Show();
        }

        public static void Toast(this string text, ToastLength duration = ToastLength.Short)
        {
            Toast(mContext, text, duration);
        }
    }
}