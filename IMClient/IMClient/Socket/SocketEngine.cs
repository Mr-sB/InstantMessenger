using System;
using System.Net.Sockets;
using System.Threading;
using ESocket.Client;
using ESocket.Common;
using IMClient.Activities;
using IMClient.Tools;

namespace IMClient.Socket
{
    public class SocketEngine : Singleton<SocketEngine>
    {
        private ESocketPeer mPeer;
        public ESocketPeer Peer => mPeer;
        public ConnectCode ConnectCode =>
            mPeer != null && mPeer.ConnectCode == ConnectCode.Connect
                ? ConnectCode.Connect
                : ConnectCode.Disconnect;

        private bool mConnecting;

        public SocketEngine()
        {
            ClientListener.Instance.OnSocketConnectStateChanged += OnSocketConnectStateChanged;
        }
        
        public void Connect()
        {
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

        public void Disconnect()
        {
            if (mConnecting || ConnectCode == ConnectCode.Disconnect) return;
            Peer.Disconnect();
        }

        private void OnSocketConnectStateChanged(ConnectCode connectCode)
        {
            mConnecting = false;
        }
    }
}