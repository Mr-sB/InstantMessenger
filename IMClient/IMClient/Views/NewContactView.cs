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
    public class NewContactView : ViewBase
    {
        private ListView mNewContentListView;
        
        protected override void OnInit()
        {
            SetContentView(Resource.Layout.NewContact);
            Init();
        }

        private void Init()
        {
            AddListeners();
            SendAddListRequest();
            FindViewById<Button>(Resource.Id.BackButton).Click += delegate { mActivity.ChangeContentView<ContactView>(); };
            mNewContentListView = FindViewById<ListView>(Resource.Id.NewContactListView);
        }

        public override void OnViewChanged()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            Messenger.OnSuccessOperationRequestEvent += OnSuccessOperationRequestEvent;
            Messenger.OnSuccessOperationResponseEvent += SuccessOperationResponseEvent;
        }

        private void RemoveListeners()
        {
            Messenger.OnSuccessOperationRequestEvent -= OnSuccessOperationRequestEvent;
            Messenger.OnSuccessOperationResponseEvent -= SuccessOperationResponseEvent;
        }

        private void SendAddListRequest()
        {
            //请求添加列表
            SocketEngine.Instance.Peer.SendRequest(ESocketParameterTool.NewParameters
                .AddOperationCode(OperationCode.Contact)
                .AddSubCode(SubCode.Contact_Add_List));
            AddToConsole("请求列表...", false);
        }
        
        private void OnSuccessOperationRequestEvent(OperationCode operationCode, SubCode subCode, OperationRequest request)
        {
            
        }
        
        private void SuccessOperationResponseEvent(OperationCode operationCode, SubCode subCode, OperationResponse response)
        {
            if(operationCode != OperationCode.Contact) return;
            switch (subCode)
            {
                case SubCode.Contact_Add_List:
                    if (response.Parameters.TryGetParameter(ParameterKeys.CONTACT_ADD_SERVER_RESPONSE_LIST,
                        out ContactAddServerResponseModelList model) && model != null)
                    {
                        AddToConsole("请求成功", false);
                        List<NewContactItem> newContacts = new List<NewContactItem>();
                        foreach (var item in model.List)
                            newContacts.Add(new NewContactItem(item));
                        RunOnUiThread(() =>
                        {
                            mNewContentListView.Adapter = new NewContactAdapter(mActivity, Resource.Layout.NewContactItem, newContacts);
                        });
                    }
                    else
                    {
                        AddToConsole("请求列表为空", false);
                    }
                    break;
            }
        }
    }
}