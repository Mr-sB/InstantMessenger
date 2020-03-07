using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using ESocket.Client;
using ESocket.Common;
using ESocket.Common.Tools;
using IMClient.Activities;
using IMClient.Controllers;
using IMClient.Tools;
using IMCommon;
using IMCommon.Tools;
using IMCommon.TransferModels;
using Timer = IMClient.Tools.Timer;
using TimeUtil = ESocket.Common.Tools.TimeUtil;

namespace IMClient.Socket
{
    public class SocketEngine : Singleton<SocketEngine>
    {
        private ESocketPeer mPeer;
        public ConnectCode ConnectCode =>
            mPeer != null && mPeer.ConnectCode == ConnectCode.Connect
                ? ConnectCode.Connect
                : ConnectCode.Disconnect;

        private bool mConnecting;
        /// <summary>
        /// 是否为主动断开连接
        /// </summary>
        private bool mIsInitiativeDisconnect;
        /// <summary>
        /// 是否正在重新连接（包括重连之前的等待时间）
        /// </summary>
        private bool mIsReconnecting;

        public SocketEngine()
        {
            ClientListener.Instance.OnSocketConnectStateChanged += OnSocketConnectStateChanged;
        }
        
        public void Connect()
        {
            MainActivity.Instance.AddToConsole("等待连接...", false);
            if (mConnecting || ConnectCode == ConnectCode.Connect) return;
            mConnecting = true;
            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        //创建peer
                        if(mPeer == null) mPeer = new ESocketPeer(ClientListener.Instance);
                        //连接
                        mPeer.Connect("47.98.34.239", 5000);
                    }
                    catch (SocketException sex)
                    {
                        mConnecting = false;
                        MainActivity.Instance.AddToConsole("SocketException:" + sex.Message + "\n" + sex);
                    }
                    catch (Exception ex)
                    {
                        mConnecting = false;
                        MainActivity.Instance.AddToConsole("ExceptionType:" + ex.GetType());
                    }
                });
            }
            catch (Exception ex)
            {
                mConnecting = false;
                MainActivity.Instance.AddToConsole("Thread ExceptionType:" + ex.GetType());
            }
        }

        /// <summary>发送请求</summary>
        /// <param name="parameters">参数列表</param>
        public void SendRequest(Dictionary<string, object> parameters)
        {
            if (ConnectCode != ConnectCode.Connect)
            {
                "等待连接...".ToastOnSubThread();
                Connect();
            }
            else
            {
                mPeer.SendRequest(parameters);
            }
        }

        /// <summary>回复响应</summary>
        /// <param name="returnCode">返回码</param>
        /// <param name="parameters">参数列表</param>
        public void SendResponse(ReturnCode returnCode, Dictionary<string, object> parameters)
        {
            if (ConnectCode != ConnectCode.Connect)
            {
                "等待连接...".ToastOnSubThread();
                Connect();
            }
            else
            {
                mPeer.SendResponse((int)returnCode, parameters);
            }
        }

        public void Disconnect()
        {
            if (mConnecting || ConnectCode == ConnectCode.Disconnect) return;
            mIsInitiativeDisconnect = true;
            LoginController.Instance.Disconnect();
            mPeer.Disconnect();
        }

        private void OnSocketConnectStateChanged(ConnectCode connectCode)
        {
            mConnecting = false;
            switch (connectCode)
            {
                case ConnectCode.Connect:
                    //掉线重连之后
                    if (LoginController.Instance.LoginUser != null)
                    {
                        "重新登录中...".ToastOnSubThread();
                        MainActivity.Instance.AddToConsole("重新登录中...", false);
                        mPeer.SendRequest(ESocketParameterTool.NewParameters
                            .AddOperationCode(OperationCode.Login)
                            .AddSubCode(SubCode.Login_SignIn)
                            .AddParameter(ParameterKeys.LOGIN_SIGN_IN_REQUEST,
                                new SignInRequestModel(LoginController.Instance.LoginUser.Username,
                                    LoginController.Instance.Password)));
                    }
                    break;
                case ConnectCode.Disconnect:
                    //正在重连，不需要再次重连
                    if(mIsReconnecting) break;
                    mIsReconnecting = true;
                    //计算心跳超时时间 +3 是加上消息传输时间
                    var seconds = mPeer.LastSendHeartbeatTime.AddSeconds(ESocketConst.HeartbeatTimeout + 3)
                        .GetDifferenceSeconds(TimeUtil.GetCurrentUtcTime(), true);
                    if (seconds < 0)
                        Reconnect();
                    else
                    {
                        $"{seconds}秒后进行重新连接".ToastOnSubThread();
                        //延迟执行,等待服务端检测到客户端掉线，关闭Socket
                        Timer.DelayAction(Reconnect, seconds);
                    }
                    break;
            }
        }

        private void Reconnect()
        {
            mIsReconnecting = false;
            //网络原因掉线，自动重连
            if (mIsInitiativeDisconnect) return;
            "重新连接中...".ToastOnSubThread();
            MainActivity.Instance.AddToConsole("重新连接中...", false);
            Connect();
        }
    }
}