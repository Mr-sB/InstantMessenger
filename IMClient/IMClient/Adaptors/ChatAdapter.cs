using System;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ESocket.Common.Tools;
using IMClient.Controllers;
using IMCommon.DB.Models;

namespace IMClient.Adaptors
{
    public class ChatAdapter : ArrayAdapter<Chat>
    {
        private int mResourceId;

        public ChatAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public ChatAdapter(Context context, int textViewResourceId) : base(context, textViewResourceId)
        {
            Init(textViewResourceId);
        }

        public ChatAdapter(Context context, int resource, int textViewResourceId) : base(context, resource, textViewResourceId)
        {
            Init(textViewResourceId);
        }

        public ChatAdapter(Context context, int textViewResourceId, Chat[] objects) : base(context, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }

        public ChatAdapter(Context context, int resource, int textViewResourceId, Chat[] objects) : base(context, resource, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }

        public ChatAdapter(Context context, int textViewResourceId, IList<Chat> objects) : base(context, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }

        public ChatAdapter(Context context, int resource, int textViewResourceId, IList<Chat> objects) : base(context, resource, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }
        
        private void Init(int resourceId)
        {
            mResourceId = resourceId;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            Chat chat = GetItem(position);//获取当前项的Item实例
            //LayoutInflater的inflate()方法接收3个参数：需要实例化布局资源的id、ViewGroup类型视图组对象、false
            //false表示只让父布局中声明的layout属性生效，但不会为这个view添加父布局
            View view = LayoutInflater.From(Context).Inflate(mResourceId, parent, false);
            string nickname = ChatController.Instance.CurChatUser.Username == chat.SendUsername
                ? ChatController.Instance.CurChatUser.Nickname
                : LoginController.Instance.LoginUser.Nickname;
            //获取实例
            view.FindViewById<TextView>(Resource.Id.ChatItemInfo).Text = $"{nickname} {chat.Time.ParseFromMilliseconds().UtcToLocalTime():G}";
            switch (chat.GetMessageCode())
            {
                case Chat.MessageCode.Word:
                    view.FindViewById<TextView>(Resource.Id.ChatItemMessage).Text = chat.Message;
                    break;
                //TODO
            }
            return view;
        }
    }
}