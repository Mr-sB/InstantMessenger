using System;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace IMClient.Views
{
    public abstract class ViewBase
    {
        protected MainActivity mActivity;
        protected TextView mConsoleText;

        public abstract void OnInit();
        public abstract void OnViewChanged();

        public void Init(MainActivity activity)
        {
            mActivity = activity;
            OnInit();
        }

        protected virtual void SetContentView(int id)
        {
            mActivity.SetContentView(id);
            InitConsoleText();
        }

        protected virtual void InitConsoleText()
        {
            mConsoleText = mActivity.FindViewById<TextView>(Resource.Id.ConsoleText);
        }
        
        public virtual void AddToConsole(string text, bool append = true)
        {
            RunOnUiThread(() =>
            {
                mConsoleText.Text = append ? mConsoleText.Text + text + "\n" : text;
            });
        }

        protected T FindViewById<T>(int id) where T : View
        {
            return mActivity?.FindViewById<T>(id);
        }

        protected void RunOnUiThread(Action action)
        {
            if(mActivity == null || action == null) return;
            mActivity.RunOnUiThread(action);
        }
    }
}