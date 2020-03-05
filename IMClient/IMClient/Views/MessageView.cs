using System.Collections.Generic;
using Android.Widget;
using ESocket.Common;
using IMClient.Adaptors;
using IMClient.Controllers;
using IMCommon;

namespace IMClient.Views
{
    public class MessageView : ViewBase
    {
        protected override void OnInit()
        {
            SetContentView(Resource.Layout.Message);
            Init();
        }

        private void Init()
        {
            Messenger.OnSuccessOperationRequestEvent += OnSuccessOperationRequestEvent;
            // List<MessageItem> messages = new List<MessageItem>();
            // for (int i = 0; i < 30; i++)
            // {
            //     messages.Add(new MessageItem("Name" + i, "Content" + i));
            // }
            // FindViewById<ListView>(Resource.Id.MessageListView).Adapter = new MessageAdapter(mActivity, Resource.Layout.MessageItem, messages);

            FindViewById<RadioButton>(Resource.Id.MessageTabButton).Selected = true;
            var contactTabButton = FindViewById<RadioButton>(Resource.Id.ContactTabButton);
            contactTabButton.Selected = false;
            contactTabButton.Click += delegate { mActivity.ChangeContentView<ContactView>(); };
        }

        private void OnSuccessOperationRequestEvent(OperationCode operationCode, SubCode subCode, OperationRequest request)
        {
            //TODO
            if(operationCode != OperationCode.Chat) return;
        }
        
        public override void OnViewChanged()
        {
            Messenger.OnSuccessOperationRequestEvent -= OnSuccessOperationRequestEvent;
        }
    }
}