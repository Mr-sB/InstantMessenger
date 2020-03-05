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
        private List<NewContactItem> mNewContacts = new List<NewContactItem>();
        private NewContactAdapter mNewContactAdapter;
        
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
            if(operationCode != OperationCode.Contact) return;
            switch (subCode)
            {
                case SubCode.Contact_Add:
                    //更改状态
                    if (request.Parameters.TryGetParameter(ParameterKeys.CONTACT_ADD_SERVER_RESPONSE,
                        out ContactAddServerResponseModel model) && model != null)
                    {
                        for (int i = 0, count = mNewContacts.Count; i < count; i++)
                        {
                            var newContact = mNewContacts[i];
                            if (newContact.IsRequest == model.IsRequest &&
                                newContact.Username == model.ContactUser.Username)
                            {
                                newContact.ResponseCode = model.ResponseCode;
                                RunOnUiThread(() =>
                                {
                                    mNewContactAdapter?.NotifyDataSetChanged();
                                });
                                break;
                            }
                        }
                    }
                    break;
            }
        }
        
        private void SuccessOperationResponseEvent(OperationCode operationCode, SubCode subCode, OperationResponse response)
        {
            if(operationCode != OperationCode.Contact) return;
            switch (subCode)
            {
                case SubCode.Contact_Add_List:
                    if (response.Parameters.TryGetParameter(ParameterKeys.CONTACT_ADD_SERVER_RESPONSE_LIST,
                        out ContactAddServerResponseModelList modelList) && modelList != null && modelList.List?.Count > 0)
                    {
                        AddToConsole("请求成功", false);
                        mNewContacts.Clear();
                        foreach (var item in modelList.List)
                            mNewContacts.Add(new NewContactItem(item));
                        RunOnUiThread(() =>
                        {
                            mNewContactAdapter = new NewContactAdapter(mActivity, Resource.Layout.NewContactItem, mNewContacts);
                            mNewContentListView.Adapter = mNewContactAdapter;
                        });
                    }
                    else
                    {
                        AddToConsole("请求列表为空", false);
                        RunOnUiThread(() =>
                        {
                            mNewContacts.Clear();
                            mNewContactAdapter = new NewContactAdapter(mActivity, Resource.Layout.NewContactItem, mNewContacts);
                            mNewContentListView.Adapter = mNewContactAdapter;
                        });
                    }
                    break;
                case SubCode.Contact_Add:
                    //更改状态
                    if (response.Parameters.TryGetParameter(ParameterKeys.CONTACT_ADD_SERVER_RESPONSE,
                        out ContactAddServerResponseModel model) && model != null)
                    {
                        AddToConsole("响应成功", false);
                        for (int i = 0, count = mNewContacts.Count; i < count; i++)
                        {
                            var newContact = mNewContacts[i];
                            if (newContact.IsRequest == model.IsRequest &&
                                newContact.Username == model.ContactUser.Username)
                            {
                                newContact.ResponseCode = model.ResponseCode;
                                RunOnUiThread(() =>
                                {
                                    mNewContactAdapter?.NotifyDataSetChanged();
                                });
                                break;
                            }
                        }
                    }
                    else
                    {
                        AddToConsole("响应失败", false);
                    }
                    break;
            }
        }
    }
}