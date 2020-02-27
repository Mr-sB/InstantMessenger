using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using IMClient.Tools;
using IMClient.Views;

namespace IMClient
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static MainActivity Instance { private set; get; }
        private ViewBase mCurView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            ToastUtil.SetDefaultContext(this);
            base.OnCreate(savedInstanceState);
            //连接服务器
            SocketEngine.Instance.Connect();
            //创建View
            ChangeContentView<LoginView>();
        }

        public void ChangeContentView<T>() where T : ViewBase, new()
        {
            mCurView?.OnViewChanged();
            mCurView = new T();
            mCurView.Init(this);
        }
        
        public void AddToConsole(string text, bool append = true)
        {
            try
            {
                mCurView.AddToConsole(text, append);
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddToConsole Exception:" + ex);
            }
        }
    }
}