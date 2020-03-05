using System;
using System.Collections.Generic;
using Android.Widget;
using ESocket.Common;
using ESocket.Common.Tools;
using IMClient.Adaptors;
using IMClient.Controllers;
using IMClient.Socket;
using IMCommon;
using IMCommon.DB.Models;
using IMCommon.Tools;
using IMCommon.TransferModels;

namespace IMClient.Views
{
    public class ChatView : ViewBase
    {
        private ListView mChatListView;
        private ChatAdapter mChatAdapter;
        
        protected override void OnInit()
        {
            SetContentView(Resource.Layout.Chat);
            Init();
        }

        private void Init()
        {
            AddListeners();
            FindViewById<TextView>(Resource.Id.ChatName).Text = $"{ChatController.Instance.CurChatUser.Nickname}({ChatController.Instance.CurChatUser.Username})";
            FindViewById<Button>(Resource.Id.BackButton).Click += delegate
            {
                mActivity.ChangeContentView<ContactView>();
            };
            var messageText = FindViewById<EditText>(Resource.Id.ChatSendMessageText);
            FindViewById<Button>(Resource.Id.ChatSendButton).Click += delegate
            {
                AddToConsole("发送消息中...", false);
                SocketEngine.Instance.Peer.SendRequest(ESocketParameterTool.NewParameters
                    .AddOperationCode(OperationCode.Chat)
                    .AddSubCode(SubCode.Chat_Message)
                    .AddParameter(ParameterKeys.CHAT_MESSAGE_REQUEST,
                        new ChatMessageRequestModel(ChatController.Instance.CurChatUser.Username, Chat.MessageCode.Word,
                            messageText.Text)));
                messageText.Text = string.Empty;
            };
            mChatListView = FindViewById<ListView>(Resource.Id.ChatListView);
            //请求聊天记录
            SendRecordRequest();
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

        private void SendRecordRequest()
        {
            AddToConsole("请求聊天记录中...", false);
            SocketEngine.Instance.Peer.SendRequest(ESocketParameterTool.NewParameters
                .AddOperationCode(OperationCode.Chat)
                .AddSubCode(SubCode.Chat_Record)
                .AddParameter(ParameterKeys.CHAT_RECORD_REQUEST, new ChatRecordRequestModel(ChatController.Instance.CurChatUser.Username, 0, 7)));
        }
        
        private void OnSuccessOperationRequestEvent(OperationCode operationCode, SubCode subCode, OperationRequest request)
        {
            if (operationCode != OperationCode.Chat) return;
            switch (subCode)
            {
                case SubCode.Chat_Message:
                    if (request.Parameters.TryGetParameter(ParameterKeys.CHAT_INFO, out Chat chat) && chat != null)
                    {
                        mChatAdapter?.Add(chat);
                        RunOnUiThread(() =>
                        {
                            mChatAdapter?.NotifyDataSetChanged();
                        });
                    }
                    break;
                //TODO
            }
        }
        
        private void OnSuccessOperationResponseEvent(OperationCode operationCode, SubCode subCode, OperationResponse response)
        {
            if (operationCode != OperationCode.Chat) return;
            switch (subCode)
            {
                case SubCode.Chat_Record:
                    if (response.Parameters.TryGetParameter(ParameterKeys.CHAT_RECORD_RESPONSE,
                        out ChatRecordResponseModel model) && model != null && model.Records?.Count > 0)
                    {
                        AddToConsole("请求聊天记录成功", false);
                        RunOnUiThread(() =>
                        {
                            mChatAdapter = new ChatAdapter(mActivity, Resource.Layout.ChatItem, model.Records);
                            mChatListView.Adapter = mChatAdapter;
                        });
                    }
                    else
                    {
                        AddToConsole("请求聊天记录为空", false);
                        RunOnUiThread(() =>
                        {
                            mChatAdapter = new ChatAdapter(mActivity, Resource.Layout.ChatItem, new List<Chat>());
                            mChatListView.Adapter = mChatAdapter;
                        });
                    }
                    break;
                case SubCode.Chat_Message:
                    if (response.Parameters.TryGetParameter(ParameterKeys.CHAT_INFO, out Chat chat) && chat != null)
                    {
                        AddToConsole("消息发送成功", false);
                        mChatAdapter?.Add(chat);
                        RunOnUiThread(() =>
                        {
                            mChatAdapter?.NotifyDataSetChanged();
                        });
                    }
                    else
                    {
                        AddToConsole("消息发送失败", false);
                    }
                    break;
            }
        }
    }
}