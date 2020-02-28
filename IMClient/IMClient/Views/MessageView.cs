using System.Collections.Generic;
using Android.Widget;
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
            Messenger.OnSuccessEvent += OnSuccessEvent;
            List<MessageItem> messages = new List<MessageItem>();
            for (int i = 0; i < 30; i++)
            {
                messages.Add(new MessageItem("Name" + i, "Content" + i));
            }
            FindViewById<ListView>(Resource.Id.MessageListView).Adapter = new MessageAdapter(mActivity, Resource.Layout.MessageItem, messages);
        }

        private void OnSuccessEvent(OperationCode operationCode, SubCode subCode)
        {
            //TODO
            if(operationCode != OperationCode.Chat) return;
        }
        
        public override void OnViewChanged()
        {
            Messenger.OnSuccessEvent -= OnSuccessEvent;
        }
    }
}