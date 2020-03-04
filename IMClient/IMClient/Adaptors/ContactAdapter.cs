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
    public class ContactAdapter : ArrayAdapter<ContactItem>
    {
        private int mResourceId;
        
        public ContactAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public ContactAdapter(Context context, int textViewResourceId) : base(context, textViewResourceId)
        {
            Init(textViewResourceId);
        }

        public ContactAdapter(Context context, int resource, int textViewResourceId) : base(context, resource, textViewResourceId)
        {
            Init(textViewResourceId);
        }

        public ContactAdapter(Context context, int textViewResourceId, ContactItem[] objects) : base(context, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }

        public ContactAdapter(Context context, int resource, int textViewResourceId, ContactItem[] objects) : base(context, resource, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }

        public ContactAdapter(Context context, int textViewResourceId, IList<ContactItem> objects) : base(context, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }

        public ContactAdapter(Context context, int resource, int textViewResourceId, IList<ContactItem> objects) : base(context, resource, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }
        
        private void Init(int resourceId)
        {
            mResourceId = resourceId;
        }
        
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ContactItem contact = GetItem(position);//获取当前项的Item实例
            //LayoutInflater的inflate()方法接收3个参数：需要实例化布局资源的id、ViewGroup类型视图组对象、false
            //false表示只让父布局中声明的layout属性生效，但不会为这个view添加父布局
            View view = LayoutInflater.From(Context).Inflate(mResourceId, parent, false);
            //获取实例
            TextView nameView = view.FindViewById<TextView>(Resource.Id.ContactItemName);
            view.FindViewById<ViewGroup>(Resource.Id.ContactItemRightLayout).Visibility = ViewStates.Gone;
            view.FindViewById<Button>(Resource.Id.ContactItemAddButton).Click += delegate
            {
                SocketEngine.Instance.Peer.SendRequest(ESocketParameterTool.NewParameters
                    .AddOperationCode(OperationCode.Contact)
                    .AddSubCode(SubCode.Contact_Add)
                    .AddParameter(ParameterKeys.USERNAME, contact.Username));
                MainActivity.Instance.AddToConsole("请求添加:" + contact.Username, false);
            };
            
            //设置
            nameView.Text = contact.Nickname + $"({contact.Username})";
            return view;
        }
    }
}