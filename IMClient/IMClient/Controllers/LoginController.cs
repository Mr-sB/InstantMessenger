using ESocket.Common;
using ESocket.Common.Tools;
using IMClient.Activities;
using IMClient.Tools;
using IMCommon;
using IMCommon.Tools;
using IMCommon.TransferModels;

namespace IMClient.Controllers
{
    public class LoginController : ControllerBase
    {
        public override OperationCode OperationCode => OperationCode.Login;
        public static LoginController Instance { private set; get; }
        public UserModel LoginUser { private set; get; }
        public string Password;

        public LoginController()
        {
            Instance = this;
        }
        public override void OnOperationResponse(OperationResponse response)
        {
            if ((ReturnCode)response.ReturnCode != ReturnCode.Success)
            {
                ((ReturnCode)response.ReturnCode).ToString().ToastOnSubThread();
                return;
            }

            if (response.Parameters.TryGetSubCode(out var subCode))
            {
                if (subCode == SubCode.Login_SignIn)
                {
                    //第一次登陆
                    if (LoginUser == null)
                    {
                        if (response.Parameters.TryGetParameter(ParameterKeys.USER_MODEL, out UserModel userModel))
                            LoginUser = userModel;
                        Messenger.Broadcast(OperationCode, subCode);
                    }
                    else
                    {
                        //断线重连
                        "重新登录成功".ToastOnSubThread();
                        MainActivity.Instance.AddToConsole("重新登录成功", false);
                    }
                }
                else
                {
                    Messenger.Broadcast(OperationCode, subCode);
                }
            }
        }

        //主动断开连接
        public void Disconnect()
        {
            LoginUser = null;
        }
    }
}