using ESocket.Common;
using IMClient.Tools;
using IMCommon;
using IMCommon.Tools;

namespace IMClient.Controllers
{
    public class LoginController : ControllerBase
    {
        public override OperationCode OperationCode => OperationCode.Login;

        public override void OnOperationResponse(OperationResponse response)
        {
            if ((ReturnCode)response.ReturnCode != ReturnCode.Success)
            {
                ((ReturnCode)response.ReturnCode).ToString().ToastOnSubThread();
                return;
            }
            response.Parameters.TryGetSubCode(out var subcode);
            switch (subcode)
            {
                case SubCode.Login_SignUp:
                    MainActivity.Instance.OnSignUpSuccess();
                    break;
                case SubCode.Login_SignIn:
                    MainActivity.Instance.OnSignInSuccess();
                    break;
                //TODO
            }
        }
    }
}