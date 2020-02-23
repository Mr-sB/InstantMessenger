using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Threading;
using System;
using System.Net.Sockets;
using IMCommon.Tools;
using ESocket.Common;
using ESocket.Client;
using ESocket.Common.Tools;
using IMCommon;
using IMCommon.TransferModels;
using IMClient.Tools;
using Android.Views;

namespace IMClient
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static MainActivity Instance { private set; get; }
        private EditText mUsernameText;
        private EditText mPasswordText;
        private EditText mNicknameText;
        private LinearLayout mNicknameLayout;
        private Button mSignInButton;
        private Button mSignUpButton;
        private TextView mSignUpClickableText;
        private TextView mConsoleText;
        private ESocketPeer mPeer = null;
        private bool mIsSignIn = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            ToastUtil.SetDefaultContext(this);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);
            InitLoginLayout();

            ConnectToServer();
        }

        private void InitLoginLayout()
        {
            // Get our UI controls from the loaded layout
            mUsernameText = FindViewById<EditText>(Resource.Id.UsernameText);
            mPasswordText = FindViewById<EditText>(Resource.Id.PasswordText);
            mNicknameText = FindViewById<EditText>(Resource.Id.NicknameText);
            mNicknameLayout = FindViewById<LinearLayout>(Resource.Id.NicknameLayout);
            mSignInButton = FindViewById<Button>(Resource.Id.SignInButton);
            mSignUpButton = FindViewById<Button>(Resource.Id.SignUpButton);
            mSignUpClickableText = FindViewById<TextView>(Resource.Id.SignUpClickableText);
            mConsoleText = FindViewById<TextView>(Resource.Id.LoginConsoleText);

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
                if(mPeer == null)
                {
                    "等待连接...".Toast();
                    return;
                }
                if (mPeer.ConnectCode == ConnectCode.Disconnect)
                {
                    mPeer.Connect("47.98.34.239", 5000);
                    "等待连接...".Toast();
                    return;
                }
                mPeer.SendRequest(ESocketParameterTool.NewParameters
                    .AddOperationCode(OperationCode.Login)
                    .AddSubCode(SubCode.Login_SignIn)
                    .AddParameter(ParameterKeys.LOGIN_SIGN_IN_REQUEST, new SignInRequestModel(mUsernameText.Text, mPasswordText.Text)));
                mConsoleText.Text = "登录中...";
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
                if (mPeer == null)
                {
                    "等待连接...".Toast();
                    return;
                }
                if (mPeer.ConnectCode == ConnectCode.Disconnect)
                {
                    mPeer.Connect("47.98.34.239", 5000);
                    "等待连接...".Toast();
                    return;
                }
                mPeer.SendRequest(ESocketParameterTool.NewParameters
                    .AddOperationCode(OperationCode.Login)
                    .AddSubCode(SubCode.Login_SignUp)
                    .AddParameter(ParameterKeys.LOGIN_SIGN_UP_REQUEST,
                        new SignUpRequestModel(mUsernameText.Text, mPasswordText.Text, mNicknameText.Text)));
                mConsoleText.Text = "注册中...";
            };

            mSignUpClickableText.Click += delegate
            {
                ChangeViewState(!mIsSignIn);
            };
        }

        private void ConnectToServer()
        {
            try
            {
                try
                {
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        try
                        {
                            //创建peer
                            mPeer = new ESocketPeer(ClientListener.Instance);
                            //连接
                            mPeer.Connect("47.98.34.239", 5000);
                        }
                        catch (SocketException sex)
                        {
                            RunOnUiThread(() =>
                            {
                                mConsoleText.Text = "SocketException:" + sex.Message + "\n" + sex.ToString() + "\n";
                            });
                        }
                        catch (Exception ex)
                        {
                            RunOnUiThread(() =>
                            {
                                mConsoleText.Text += "111ExceptionType:" + ex.GetType().ToString() + "\n";
                                mConsoleText.Text += ex.ToString() + "\n";
                            });
                        }
                    });
                }
                catch (Exception ex)
                {
                    RunOnUiThread(() =>
                    {
                        mConsoleText.Text += "Thread ExceptionType:" + ex.GetType().ToString() + "\n";
                        mConsoleText.Text += ex.ToString() + "\n";
                    });
                }
            }
            catch (Exception ex)
            {
                RunOnUiThread(() =>
                {
                    mConsoleText.Text += "ExceptionType:" + ex.GetType().ToString() + "\n";
                    mConsoleText.Text += ex.ToString() + "\n";
                });
            }
        }

        private void ChangeViewState(bool isSignIn)
        {
            mIsSignIn = isSignIn;
            ViewStates signInState = isSignIn ? ViewStates.Visible : ViewStates.Gone;
            ViewStates signUpState = isSignIn ? ViewStates.Gone : ViewStates.Visible;
            mNicknameLayout.Visibility = signUpState;
            mSignInButton.Visibility = signInState;
            mSignUpButton.Visibility = signUpState;
            mSignUpClickableText.Text = isSignIn ? "注册" : "登录";
        }

        public void OnSignInSuccess()
        {
            RunOnUiThread(() =>
            {
                mConsoleText.Text = "登录成功!";
            });
        }

        public void OnSignUpSuccess()
        {
            RunOnUiThread(() =>
            {
                ChangeViewState(true);
                mConsoleText.Text = "注册成功!";
            });
        }

        public void AddToConsole(string text)
        {
            RunOnUiThread(() =>
            {
                mConsoleText.Text += text + "\n";
            });
        }
    }
}