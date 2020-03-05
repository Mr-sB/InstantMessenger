using System.Collections.Generic;
using Android.Views;
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
        private ListView mSearchListView;
        private ViewGroup mCurSelectedViewGroup;
        
        protected override void OnInit()
        {
            SetContentView(Resource.Layout.Search);
            Init();
        }

        private void Init()
        {
            Messenger.OnSuccessOperationResponseEvent += OnSuccessOperationResponseEvent;
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
            mSearchListView = FindViewById<ListView>(Resource.Id.SearchListView);
            mSearchListView.ItemClick += (sender, args) =>
            {
                if(mCurSelectedViewGroup != null)
                    mCurSelectedViewGroup.Visibility = ViewStates.Gone;
                mCurSelectedViewGroup = args.View.FindViewById<ViewGroup>(Resource.Id.SearchItemRightLayout);
                mCurSelectedViewGroup.Visibility = ViewStates.Visible;
                // AddToConsole(args.View.GetType().ToString() + args.Parent.GetType() + args.Parent.SelectedItemPosition + "  " + args.Id + "  " + args.Position, false);
            };
        }
        
        public override void OnViewChanged()
        {
            Messenger.OnSuccessOperationResponseEvent -= OnSuccessOperationResponseEvent;
        }

        private void OnSuccessOperationResponseEvent(OperationCode operationCode, SubCode subCode, OperationResponse response)
        {
            if(operationCode != OperationCode.Contact) return;
            switch (subCode)
            {
                case SubCode.Contact_Search:
                    AddToConsole("查找成功", false);
                    if (response.Parameters.TryGetParameter(ParameterKeys.USER_MODEL_LIST, out UserListModel model) && model?.Users != null)
                    {
                        List<SearchItem> contacts = new List<SearchItem>(model.Users.Count);
                        foreach (var user in model.Users)
                        {
                            if(user.Username == LoginController.Instance.LoginUser.Username) continue;
                            contacts.Add(new SearchItem(user));
                        }
                        

                        RunOnUiThread(() =>
                        {
                            mSearchListView.Adapter =
                                new SearchAdapter(mActivity, Resource.Layout.SearchItem, contacts);
                        });
                        if(contacts.Count <= 0)
                            AddToConsole("查无此人", false);
                    }
                    else
                    {
                        AddToConsole("查无此人", false);
                        RunOnUiThread(() =>
                        {
                            mSearchListView.Adapter =
                                new SearchAdapter(mActivity, Resource.Layout.SearchItem, new List<SearchItem>());
                        });
                    }

                    break;
                //TODO
            }
        }
    }
}