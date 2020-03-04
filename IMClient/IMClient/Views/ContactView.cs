using Android.Widget;

namespace IMClient.Views
{
    public class ContactView : ViewBase
    {
        protected override void OnInit()
        {
            SetContentView(Resource.Layout.Contact);
            Init();
        }

        private void Init()
        {
            FindViewById<Button>(Resource.Id.AddContactButton).Click += delegate { mActivity.ChangeContentView<SearchView>(); };
            var messageTabButton = FindViewById<RadioButton>(Resource.Id.MessageTabButton);
            messageTabButton.Selected = false;
            messageTabButton.Click += delegate { mActivity.ChangeContentView<MessageView>(); };
            FindViewById<RadioButton>(Resource.Id.ContactTabButton).Selected = true;
            
            //TODO 填充List
            
        }
    }
}