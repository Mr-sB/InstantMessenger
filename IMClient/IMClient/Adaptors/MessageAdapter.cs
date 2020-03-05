using System;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace IMClient.Adaptors
{
    public class MessageAdapter : ArrayAdapter<MessageItem>
    {
        private int mResourceId;
        public MessageAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public MessageAdapter(Context context, int textViewResourceId) : base(context, textViewResourceId)
        {
            Init(textViewResourceId);
        }

        public MessageAdapter(Context context, int resource, int textViewResourceId) : base(context, resource, textViewResourceId)
        {
            Init(textViewResourceId);
        }

        public MessageAdapter(Context context, int textViewResourceId, MessageItem[] objects) : base(context, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }

        public MessageAdapter(Context context, int resource, int textViewResourceId, MessageItem[] objects) : base(context, resource, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }

        public MessageAdapter(Context context, int textViewResourceId, IList<MessageItem> objects) : base(context, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }

        public MessageAdapter(Context context, int resource, int textViewResourceId, IList<MessageItem> objects) : base(context, resource, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }

        private void Init(int resourceId)
        {
            mResourceId = resourceId;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            MessageItem message = GetItem(position);//获取当前项的MessageItem实例
            //LayoutInflater的inflate()方法接收3个参数：需要实例化布局资源的id、ViewGroup类型视图组对象、false
            //false表示只让父布局中声明的layout属性生效，但不会为这个view添加父布局
            View view = LayoutInflater.From(Context).Inflate(mResourceId, parent, false);
            //获取实例
            TextView nameView = view.FindViewById<TextView>(Resource.Id.MessageItemName);
            TextView contentView = view.FindViewById<TextView>(Resource.Id.MessageItemContent);
            //设置
            nameView.Text = $"{message.UserModel.Nickname}({message.UserModel.Username})";
            contentView.Text = message.Content;
            return view;
        }
    }
}