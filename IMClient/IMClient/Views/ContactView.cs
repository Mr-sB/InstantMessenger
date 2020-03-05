using System.Collections.Generic;
using Android.Widget;
using ESocket.Common;
using ESocket.Common.Tools;
using IMClient.Adaptors;
using IMClient.Controllers;
using IMClient.Socket;
using IMCommon;
using IMCommon.Tools;
using IMCommon.TransferModels;

namespace IMClient.Views
{
    public class ContactView : ViewBase
    {
        private ListView mContactListView;
        private List<UserModel> mContacts;
        private ContactAdapter mContactAdapter;
        
        protected override void OnInit()
        {
            SetContentView(Resource.Layout.Contact);
            Init();
        }

        private void Init()
        {
            AddListeners();
            FindViewById<Button>(Resource.Id.AddContactButton).Click += delegate { mActivity.ChangeContentView<SearchView>(); };
            FindViewById<Button>(Resource.Id.NewContactButton).Click += delegate { mActivity.ChangeContentView<NewContactView>(); };
            var messageTabButton = FindViewById<RadioButton>(Resource.Id.MessageTabButton);
            messageTabButton.Selected = false;
            messageTabButton.Click += delegate { mActivity.ChangeContentView<MessageView>(); };
            FindViewById<RadioButton>(Resource.Id.ContactTabButton).Selected = true;
            
            mContactListView = FindViewById<ListView>(Resource.Id.ContactListView);
            mContactListView.ItemClick += (sender, args) =>
            {
                ChatController.Instance.CurChatUser = mContacts[args.Position];
                mActivity.ChangeContentView<ChatView>();
            };
            //请求联系人列表
            SendContactListRequest();
        }

        public override void OnViewChanged()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            Messenger.OnSuccessOperationRequestEvent += OnSuccessOperationRequestEvent;
            Messenger.OnSuccessOperationResponseEvent += OnSuccessOperationResponseEvent;
        }

        private void RemoveListeners()
        {
            Messenger.OnSuccessOperationRequestEvent -= OnSuccessOperationRequestEvent;
            Messenger.OnSuccessOperationResponseEvent -= OnSuccessOperationResponseEvent;
        }

        private void SendContactListRequest()
        {
            SocketEngine.Instance.SendRequest(ESocketParameterTool.NewParameters
                .AddOperationCode(OperationCode.Contact)
                .AddSubCode(SubCode.Contact_List));
            AddToConsole("请求联系人列表中...", false);
        }
        
        private void OnSuccessOperationRequestEvent(OperationCode operationCode, SubCode subCode, OperationRequest request)
        {
            if(operationCode != OperationCode.Contact) return;
            switch (subCode)
            {
                case SubCode.Contact_Add_Response:
                    
                    break;
            }
        }
        
        private void OnSuccessOperationResponseEvent(OperationCode operationCode, SubCode subCode, OperationResponse response)
        {
            if(operationCode != OperationCode.Contact) return;
            switch (subCode)
            {
                case SubCode.Contact_List:
                    if (response.Parameters.TryGetParameter(ParameterKeys.USER_MODEL_LIST, out UserListModel model) && model != null && model.Users?.Count > 0)
                    {
                        AddToConsole("请求联系人列表成功", false);
                        mContacts = model.Users;
                        RunOnUiThread(() =>
                        {
                            mContactAdapter = new ContactAdapter(mActivity, Resource.Layout.ContactItem, mContacts);
                            mContactListView.Adapter = mContactAdapter;
                        });
                    }
                    else
                    {
                        AddToConsole("联系人列表为空", false);
                        mContacts = new List<UserModel>();
                        RunOnUiThread(() =>
                        {
                            mContactAdapter = new ContactAdapter(mActivity, Resource.Layout.ContactItem, mContacts);
                            mContactListView.Adapter = mContactAdapter;
                        });
                    }
                    break;
                case SubCode.Contact_Add_Response:
                    
                    break;
            }
        }
    }
}