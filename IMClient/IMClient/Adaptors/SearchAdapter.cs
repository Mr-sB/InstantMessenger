using System;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ESocket.Common.Tools;
using IMClient.Activities;
using IMClient.Socket;
using IMCommon;
using IMCommon.Tools;

namespace IMClient.Adaptors
{
    public class SearchAdapter : ArrayAdapter<SearchItem>
    {
        private int mResourceId;
        
        public SearchAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public SearchAdapter(Context context, int textViewResourceId) : base(context, textViewResourceId)
        {
            Init(textViewResourceId);
        }

        public SearchAdapter(Context context, int resource, int textViewResourceId) : base(context, resource, textViewResourceId)
        {
            Init(textViewResourceId);
        }

        public SearchAdapter(Context context, int textViewResourceId, SearchItem[] objects) : base(context, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }

        public SearchAdapter(Context context, int resource, int textViewResourceId, SearchItem[] objects) : base(context, resource, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }

        public SearchAdapter(Context context, int textViewResourceId, IList<SearchItem> objects) : base(context, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }

        public SearchAdapter(Context context, int resource, int textViewResourceId, IList<SearchItem> objects) : base(context, resource, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }
        
        private void Init(int resourceId)
        {
            mResourceId = resourceId;
        }
        
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            SearchItem search = GetItem(position);//获取当前项的Item实例
            //LayoutInflater的inflate()方法接收3个参数：需要实例化布局资源的id、ViewGroup类型视图组对象、false
            //false表示只让父布局中声明的layout属性生效，但不会为这个view添加父布局
            View view = LayoutInflater.From(Context).Inflate(mResourceId, parent, false);
            //获取实例
            TextView nameView = view.FindViewById<TextView>(Resource.Id.SearchItemName);
            view.FindViewById<ViewGroup>(Resource.Id.SearchItemRightLayout).Visibility = ViewStates.Gone;
            view.FindViewById<Button>(Resource.Id.SearchItemAddButton).Click += delegate
            {
                SocketEngine.Instance.SendRequest(ESocketParameterTool.NewParameters
                    .AddOperationCode(OperationCode.Contact)
                    .AddSubCode(SubCode.Contact_Add_Request)
                    .AddParameter(ParameterKeys.USERNAME, search.Username));
                MainActivity.Instance.AddToConsole("请求添加:" + search.Username, false);
            };
            
            //设置
            nameView.Text = search.Nickname + $"({search.Username})";
            return view;
        }
    }
}