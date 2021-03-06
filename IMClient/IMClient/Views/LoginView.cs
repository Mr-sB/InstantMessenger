using Android.Views;
using Android.Widget;
using ESocket.Common.Tools;
using IMClient.Controllers;
using IMClient.Socket;
using IMClient.Tools;
using IMCommon;
using IMCommon.Tools;
using IMCommon.TransferModels;

namespace IMClient.Views
{
    public class LoginView : ViewBase
    {
        private EditText mUsernameText;
        private EditText mPasswordText;
        private EditText mNicknameText;
        private LinearLayout mNicknameLayout;
        private Button mSignInButton;
        private Button mSignUpButton;
        private TextView mSignUpClickableText;
        private bool mIsSignIn = true;

        protected override void OnInit()
        {
            SetContentView(Resource.Layout.Login);
            Init();
        }

        private void Init()
        {
            Messenger.OnSuccessNullEvent += OnSuccessNullEvent;
            mUsernameText = FindViewById<EditText>(Resource.Id.UsernameText);
            mPasswordText = FindViewById<EditText>(Resource.Id.PasswordText);
            mNicknameText = FindViewById<EditText>(Resource.Id.NicknameText);
            mNicknameLayout = FindViewById<LinearLayout>(Resource.Id.NicknameLayout);
            mSignInButton = FindViewById<Button>(Resource.Id.SignInButton);
            mSignUpButton = FindViewById<Button>(Resource.Id.SignUpButton);
            mSignUpClickableText = FindViewById<TextView>(Resource.Id.SignUpClickableText);

            mSignInButton.Click += delegate
            {
                if (string.IsNullOrWhiteSpace(mUsernameText.Text))
                {
                    "用户名不能为空!".Toast();
                    return;
                }
                if (string.IsNullOrWhiteSpace(mPasswordText.Text))
                {
                    "密码不能为空!".Toast();
                    return;
                }
                AddToConsole("登录中...", false);
                SocketEngine.Instance.SendRequest(ESocketParameterTool.NewParameters
                    .AddOperationCode(OperationCode.Login)
                    .AddSubCode(SubCode.Login_SignIn)
                    .AddParameter(ParameterKeys.LOGIN_SIGN_IN_REQUEST, new SignInRequestModel(mUsernameText.Text, mPasswordText.Text)));
                LoginController.Instance.Password = mPasswordText.Text;
            };

            mSignUpButton.Click += delegate
            {
                if (string.IsNullOrWhiteSpace(mUsernameText.Text))
                {
                    "用户名不能为空!".Toast();
                    return;
                }
                if (string.IsNullOrWhiteSpace(mPasswordText.Text))
                {
                    "密码不能为空!".Toast();
                    return;
                }
                if (string.IsNullOrWhiteSpace(mNicknameText.Text))
                {
                    "昵称不能为空!".Toast();
                    return;
                }
                AddToConsole("注册中...", false);
                SocketEngine.Instance.SendRequest(ESocketParameterTool.NewParameters
                    .AddOperationCode(OperationCode.Login)
                    .AddSubCode(SubCode.Login_SignUp)
                    .AddParameter(ParameterKeys.LOGIN_SIGN_UP_REQUEST,
                        new SignUpRequestModel(mUsernameText.Text, mPasswordText.Text, mNicknameText.Text)));
            };

            mSignUpClickableText.Click += delegate
            {
                ChangeLoginViewState(!mIsSignIn);
            };
        }
        
        public override void OnViewChanged()
        {
            Messenger.OnSuccessNullEvent -= OnSuccessNullEvent;
        }

        private void ChangeLoginViewState(bool isSignIn)
        {
            mIsSignIn = isSignIn;
            ViewStates signInState = isSignIn ? ViewStates.Visible : ViewStates.Gone;
            ViewStates signUpState = isSignIn ? ViewStates.Gone : ViewStates.Visible;
            mNicknameLayout.Visibility = signUpState;
            mSignInButton.Visibility = signInState;
            mSignUpButton.Visibility = signUpState;
            mSignUpClickableText.Text = isSignIn ? "注册" : "登录";
        }

        private void OnSuccessNullEvent(OperationCode operationCode, SubCode subCode)
        {
            if(operationCode != OperationCode.Login) return;
            switch (subCode)
            {
                case SubCode.Login_SignIn:
                    OnSignInSuccess();
                    break;
                case SubCode.Login_SignUp:
                    OnSignUpSuccess();
                    break;
                //TODO
            }
        }

        private void OnSignInSuccess()
        {
            AddToConsole("登录成功!", false);
            //切换到联系人View
            mActivity.ChangeContentView<ContactView>();
        }

        private void OnSignUpSuccess()
        {
            AddToConsole("注册成功!", false);
            RunOnUiThread(() =>
            {
                ChangeLoginViewState(true);
            });
        }
    }
}