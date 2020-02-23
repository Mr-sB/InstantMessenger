using System.Collections.Generic;
using System.Net.Sockets;
using ESocket.Common;
using ESocket.Common.Tools;
using ESocket.Server;
using IMCommon;
using IMCommon.DB.Models;
using IMCommon.Tools;

namespace IMServer
{
    public class IMClientPeer : ClientPeer
    {
        private static log4net.ILog mLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public User LoginUser = null;

        public IMClientPeer(Socket socket) : base(socket) { }

        public void SendResponse(ReturnCode returnCode, Dictionary<string, object> parameters)
        {
            SendResponse((int)returnCode, parameters);
        }

        protected override void OnOperationRequest(OperationRequest request)
        {
            if(!request.Parameters.TryGetOperationCode(out var operationCode))
            {
                mLogger.Error("Get OperationCode Fail");
                SendResponse(ReturnCode.OperationCodeException,
                    ESocketParameterTool.NewParameters.AddOperationCode(OperationCode.Unknow));
                return;
            }
            if (IMApplication.Instance.TryGetHandler(operationCode, out var handler))
                handler.OnOperationRequest(this, request);
        }

        protected override void OnDisconnect()
        {
            if (LoginUser != null)
            {
                IMApplication.Instance.RemoveLoginUser(this);
            }
            mLogger.InfoFormat("Client disconnect:{0}", ToString());
        }
    }
}
