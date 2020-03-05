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
using IMCommon.DB.Models;
using IMCommon.Tools;
using IMCommon.TransferModels;

namespace IMClient.Adaptors
{
    public class NewContactAdapter : ArrayAdapter<NewContactItem>
    {
        private int mResourceId;

        public NewContactAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public NewContactAdapter(Context context, int textViewResourceId) : base(context, textViewResourceId)
        {
            Init(textViewResourceId);
        }

        public NewContactAdapter(Context context, int resource, int textViewResourceId) : base(context, resource, textViewResourceId)
        {
            Init(textViewResourceId);
        }

        public NewContactAdapter(Context context, int textViewResourceId, NewContactItem[] objects) : base(context, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }

        public NewContactAdapter(Context context, int resource, int textViewResourceId, NewContactItem[] objects) : base(context, resource, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }

        public NewContactAdapter(Context context, int textViewResourceId, IList<NewContactItem> objects) : base(context, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }

        public NewContactAdapter(Context context, int resource, int textViewResourceId, IList<NewContactItem> objects) : base(context, resource, textViewResourceId, objects)
        {
            Init(textViewResourceId);
        }
        
        private void Init(int resourceId)
        {
            mResourceId = resourceId;
        }
        
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            NewContactItem contact = GetItem(position);//获取当前项的Item实例
            //LayoutInflater的inflate()方法接收3个参数：需要实例化布局资源的id、ViewGroup类型视图组对象、false
            //false表示只让父布局中声明的layout属性生效，但不会为这个view添加父布局
            View view = LayoutInflater.From(Context).Inflate(mResourceId, parent, false);
            //获取实例
            TextView nameView = view.FindViewById<TextView>(Resource.Id.NewContactItemName);
            //设置
            nameView.Text = contact.Nickname + $"({contact.Username})";

            if (contact.IsRequest)
            {
                view.FindViewById<ViewGroup>(Resource.Id.NewContactItemButtonsLayout).Visibility = ViewStates.Gone;
                view.FindViewById<ViewGroup>(Resource.Id.NewContactItemTextLayout).Visibility = ViewStates.Visible;
                var textView = view.FindViewById<TextView>(Resource.Id.NewContactResponseText);
                switch (contact.ResponseCode)
                {
                    case ContactAddRequest.ContactAddResponseCode.Waite:
                        textView.Text = "等待添加...";
                        break;
                    case ContactAddRequest.ContactAddResponseCode.Accept:
                        textView.Text = "被添加";
                        break;
                    case ContactAddRequest.ContactAddResponseCode.Refuse:
                        textView.Text = "被拒绝";
                        break;
                }
            }
            else
            {
                view.FindViewById<ViewGroup>(Resource.Id.NewContactItemButtonsLayout).Visibility =
                    contact.ResponseCode == ContactAddRequest.ContactAddResponseCode.Waite
                        ? ViewStates.Visible
                        : ViewStates.Gone;
                view.FindViewById<ViewGroup>(Resource.Id.NewContactItemTextLayout).Visibility =
                    contact.ResponseCode == ContactAddRequest.ContactAddResponseCode.Waite
                        ? ViewStates.Gone
                        : ViewStates.Visible;
                if (contact.ResponseCode == ContactAddRequest.ContactAddResponseCode.Waite)
                {
                    view.FindViewById<Button>(Resource.Id.NewContactItemAcceptButton).Click += delegate
                    {
                        OnResponseClick(new ContactAddClientResponseModel(contact.Username, true));
                    };
                
                    view.FindViewById<Button>(Resource.Id.NewContactItemRefuseButton).Click += delegate
                    {
                        OnResponseClick(new ContactAddClientResponseModel(contact.Username, false));
                    };
                }
                else
                {
                    view.FindViewById<TextView>(Resource.Id.NewContactResponseText).Text =
                        contact.ResponseCode == ContactAddRequest.ContactAddResponseCode.Accept ? "已添加" : "已拒绝";
                }
            }
            return view;
        }

        private static void OnResponseClick(ContactAddClientResponseModel responseModel)
        {
            SocketEngine.Instance.SendResponse(ReturnCode.Success, ESocketParameterTool.NewParameters
                .AddOperationCode(OperationCode.Contact)
                .AddSubCode(SubCode.Contact_Add_Response)
                .AddParameter(ParameterKeys.CONTACT_ADD_CLIENT_RESPONSE, responseModel));
            MainActivity.Instance.AddToConsole("回应添加请求:" + responseModel.RequestUsername + "," + responseModel.Accept, false);
        }
    }
}