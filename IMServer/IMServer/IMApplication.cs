using ESocket.Server;
using IMCommon;
using IMCommon.DB.Models;
using IMServer.DB.Managers;
using IMServer.Handlers;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Reflection;

namespace IMServer
{
    public class IMApplication : ApplicationBase
    {
        public const string CONFIG_FILE = "Log4Net.config";
        public const string RELATIVE_PATH_HOLDER = "%RelativePath";
        public new static IMApplication Instance => ApplicationBase.Instance as IMApplication;
        private static ILog mLogger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Dictionary<OperationCode, HandlerBase> mHandlers;
        private Dictionary<string, IMClientPeer> mLoginUsers = new Dictionary<string, IMClientPeer>();
        private Config mConfig;

        public IMApplication(Config config) : base(config)
        {
            mConfig = config;
        }

        protected override void Setup()
        {
            Console.WriteLine("Server application start");
            //初始化log4net
            InitLog4Net();
            mLogger.Info("Server application start");
            //初始化处理类
            InitHandlers();
            mLogger.Info("Server application setup complete");
        }

        protected override void OnTearDown()
        {
            mLogger.Info("Server application tear down");
        }

        protected override ClientPeer CreatePeer(Socket clientSocket)
        {
            mLogger.DebugFormat("Client connect:{0}", clientSocket.RemoteEndPoint.ToString());
            return new IMClientPeer(clientSocket);
        }

        public bool TryGetHandler(OperationCode operationCode, out HandlerBase handler)
        {
            if (mHandlers.TryGetValue(operationCode, out handler)) return true;
            mLogger.ErrorFormat("Can't find handler from operation code : " + operationCode);
            return false;
        }

        public bool TryGetPeerByUsername(string username, out IMClientPeer peer)
        {
            return mLoginUsers.TryGetValue(username, out peer);
        }

        public void AddLoginUser(User user, IMClientPeer peer)
        {
            //重复登录
            if (user == peer.LoginUser) return;
            //同一个peer切换用户
            if (peer.LoginUser != null)
            {
                if (mLoginUsers.ContainsKey(peer.LoginUser.Username))
                {
                    mLoginUsers.Remove(peer.LoginUser.Username);
                }
            }
            if(mLoginUsers.TryGetValue(user.Username, out var oldPeer))
            {
                //重复登录
                if (oldPeer == peer) return;
                //旧的挤掉线
                mLogger.InfoFormat("挤掉线。旧客户端:{0},新客户端:{1},用户名:{2}", oldPeer, peer, peer.LoginUser);
                oldPeer.Disconnect();
                mLoginUsers[user.Username] = peer;
            }
            else
            {
                mLoginUsers.Add(user.Username, peer);
            }
            peer.LoginUser = user;
        }

        public void RemoveLoginUser(IMClientPeer peer)
        {
            if (peer.LoginUser == null) return;
            string username = peer.LoginUser.Username;
            if (string.IsNullOrEmpty(username)) return;
            //现在登陆的和掉线的不是同一个客户端
            if (!mLoginUsers.TryGetValue(username, out var nowPeer) || nowPeer != peer) return;
            mLoginUsers.Remove(username);
        }

        /// <summary>
        /// 初始化log4net
        /// </summary>
        private void InitLog4Net()
        {
            try
            {
                string relativePath = "..//" + mConfig.ApplicationConfig.BaseDirectory;
                string configPath = Path.Combine(relativePath, CONFIG_FILE);
                string config = null;
                //初始化Log4Net.config文件内容
                using (StreamReader streamReader = new StreamReader(configPath))
                {
                    config = streamReader.ReadToEnd();
                    if (config.Contains(RELATIVE_PATH_HOLDER))
                        config = config.Replace(RELATIVE_PATH_HOLDER, relativePath);
                    else
                        config = null;
                }
                if (!string.IsNullOrEmpty(config))
                    using (StreamWriter streamWriter = new StreamWriter(configPath))
                        streamWriter.Write(config);
                //初始化log4net
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(configPath));
            }
            catch (Exception e)
            {
                mLogger.Error(e);
            }
        }

        /// <summary>
        /// 初始化处理类
        /// </summary>
        private void InitHandlers()
        {
            mHandlers = new Dictionary<OperationCode, HandlerBase>();
            Type handlerBaseType = typeof(HandlerBase);
            try
            {
                foreach (var type in Assembly.GetAssembly(handlerBaseType).GetTypes())
                {
                    if (!type.IsAbstract && type.IsSubclassOf(handlerBaseType))
                    {
                        if (Activator.CreateInstance(type) is HandlerBase handler)
                            mHandlers.Add(handler.OperationCode, handler);
                        else
                            mLogger.ErrorFormat("Handler type:{0} can not create instance!", type);
                    }
                }
            }
            catch (Exception e)
            {
                mLogger.Error(e);
            }
        }
    }
}
