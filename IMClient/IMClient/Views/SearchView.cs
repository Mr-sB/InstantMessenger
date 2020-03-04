using System.Collections.Generic;
using Android.Widget;
using ESocket.Common;
using ESocket.Common.Tools;
using IMClient.Adaptors;
using IMClient.Controllers;
using IMClient.Socket;
using IMClient.Tools;
using IMCommon;
using IMCommon.Tools;
using IMCommon.TransferModels;

namespace IMClient.Views
{
    public class SearchView : ViewBase
    {
        private ListView mContactListView;
        
        protected override void OnInit()
        {
            SetContentView(Resource.Layout.Search);
            Init();
        }

        private void Init()
        {
            Messenger.SuccessOperationResponseEvent += OnSuccessEvent;
            FindViewById<Button>(Resource.Id.BackButton).Click += delegate { mActivity.ChangeContentView<ContactView>(); };
            var usernameText = FindViewById<TextView>(Resource.Id.UsernameText);
            FindViewById<Button>(Resource.Id.SearchButton).Click += delegate
            {
                if (string.IsNullOrWhiteSpace(usernameText.Text))
                {
                    "用户名不能为空!".Toast();
                    return;
                }
                if(SocketEngine.Instance.ConnectCode != ConnectCode.Connect)
                {
                    "等待连接...".Toast();
                    SocketEngine.Instance.Connect();
                    return;
                }
                AddToConsole("查找中...", false);
                SocketEngine.Instance.Peer.SendRequest(ESocketParameterTool.NewParameters
                    .AddOperationCode(OperationCode.Contact)
                    .AddSubCode(SubCode.Contact_Search)
                    .AddParameter(ParameterKeys.USERNAME, usernameText.Text));
            };
            mContactListView = FindViewById<ListView>(Resource.Id.ContactListView);
        }
        
        public override void OnViewChanged()
        {
            Messenger.SuccessOperationResponseEvent -= OnSuccessEvent;
        }

        private void OnSuccessEvent(OperationCode operationCode, SubCode subCode, OperationResponse response)
        {
            if(operationCode != OperationCode.Contact) return;
            switch (subCode)
            {
                case SubCode.Contact_Search:
                    if (response.Parameters.TryGetParameter(ParameterKeys.USER_MODEL_LIST, out UserListModel model) && model != null)
                    {
                        AddToConsole("查找成功", false);
                        List<ContactItem> contacts = new List<ContactItem>(model.Users.Count);
                        foreach (var user in model.Users)
                            contacts.Add(new ContactItem(user));
                        RunOnUiThread(() =>
                        {
                            mContactListView.Adapter = new ContactAdapter(mActivity, Resource.Layout.ContactItem, contacts);
                        });
                    }
                    else
                    {
                        AddToConsole("查找失败", false);
                    }
                    break;
                //TODO
            }
        }
    }
}