using ESocket.Common;
using ESocket.Common.Tools;
using IMCommon;
using IMCommon.Tools;
using log4net;
using System.Collections.Generic;

namespace IMServer.Handlers
{
    public abstract class HandlerBase
    {
        public abstract OperationCode OperationCode { get; }
        protected abstract ILog mLogger { get; }
        public abstract void OnOperationRequest(IMClientPeer peer, OperationRequest request);
        public virtual void OnOperationResponse(IMClientPeer peer, OperationResponse response) { }

        protected Dictionary<string, object> InitParameters(SubCode? subCode)
        {
            var parameters = ESocketParameterTool.NewParameters.AddOperationCode(OperationCode);
            if(subCode.HasValue)
                parameters.AddSubCode(subCode.Value);
            return parameters;
        }

        protected bool TryInitResponse<T>(SubCode subCode, IMClientPeer peer, OperationBase operation,
            out Dictionary<string, object> parameters, string key, out T model)
        {
            parameters = InitParameters(subCode);
            if (!operation.Parameters.TryGetParameter(key, out model) || model == null)
            {
                mLogger.ErrorFormat("消息错误！客户端{0},OperationCode:{1},SubCode:{2},ParameterKeys:{3}", peer, OperationCode, subCode, key);
                peer.SendResponse(ReturnCode.ParameterException, parameters.AddParameter(ParameterKeys.PARAMETER_KEY, key));
                return false;
            }
            return true;
        }
    }
}
