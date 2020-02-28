using System.Reflection;
using ESocket.Common;
using ESocket.Common.Tools;
using IMCommon;
using IMCommon.DB.Models;
using IMCommon.Tools;
using IMCommon.TransferModels;
using IMServer.DB.Managers;
using IMServer.Tools;
using log4net;

namespace IMServer.Handlers
{
    public class LoginHandler : HandlerBase
    {
        public override OperationCode OperationCode => OperationCode.Login;
        protected override ILog mILog
        {
            get
            {
                if (mLog == null) mLog = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                return mLog;
            }
        }
        private ILog mLog;

        public override void OnOperationRequest(IMClientPeer peer, OperationRequest request)
        {
            if(!request.Parameters.TryGetSubCode(out var subCode))
            {
                mLog.ErrorFormat("消息错误！客户端{0},OperationCode:{1},获取SubCode失败", peer, OperationCode);
                peer.SendResponse(ReturnCode.SubCodeException,
                    ESocketParameterTool.NewParameters.AddOperationCode(OperationCode));
                return;
            }
            switch (subCode)
            {
                case SubCode.Login_SignUp:
                    OnSignUp(peer, request);
                    break;
                case SubCode.Login_SignIn:
                    OnSignIn(peer, request);
                    break;
                case SubCode.Login_ResetPassword:
                    OnResetPassword(peer, request);
                    break;
                case SubCode.Login_ForgetPassword:
                    OnForgetPassword(peer, request);
                    break;
                default:
                    mLog.ErrorFormat("消息错误！客户端{0},SubCode未知:{1}", peer, subCode);
                    peer.SendResponse(ReturnCode.SubCodeException,
                        ESocketParameterTool.NewParameters.AddOperationCode(OperationCode));
                    break;
            }
        }

        //注册
        private void OnSignUp(IMClientPeer peer, OperationRequest request)
        {
            if (!TryInitResponse(SubCode.Login_SignUp, peer, request, out var parameters,
                ParameterKeys.LOGIN_SIGN_UP_REQUEST, out SignUpRequestModel model)) return;
            //用户名重复
            if (UserManager.GetUser(model.Username) != null)
            {
                peer.SendResponse(ReturnCode.UsernameRepetition, parameters);
                return;
            }
            //DB注册用户
            UserManager.AddUser(new User
            {
                Username = model.Username,
                Nickname = model.Nickname,
                //密码加盐SHA256加密
                Password = model.Password.SaltEncodeString(DigestUtil.EncodeType.SHA256, out var salt),
                //盐(随机字符串)
                Salt = salt,
                //注册时间
                SignUpTime = TimeUtil.GetCurrentUtcTime().GetTotalMilliseconds()
            });
            //记录事件
            mLog.InfoFormat("用户注册:{0}", model.Username);
            peer.SendResponse(ReturnCode.Success, parameters);
        }

        //登录
        private void OnSignIn(IMClientPeer peer, OperationRequest request)
        {
            if (!TryInitResponse(SubCode.Login_SignIn, peer, request, out var parameters,
                ParameterKeys.LOGIN_SIGN_IN_REQUEST, out SignInRequestModel model)) return;
            //DB获取用户数据
            var dbUser = UserManager.GetUser(model.Username);
            //用户名不存在
            if (dbUser == null)
            {
                peer.SendResponse(ReturnCode.UsernameDoesNotExist, parameters);
                return;
            }
            //对比密码
            if(model.Password.SaltEncodeString(DigestUtil.EncodeType.SHA256, dbUser.Salt) != dbUser.Password)
            {
                peer.SendResponse(ReturnCode.PasswordError, parameters);
                return;
            }
            //记录事件
            mLog.InfoFormat("用户登录:{0}", model.Username);
            //TODO登陆成功发送一些用户基本信息 好友列表之类的
            peer.SendResponse(ReturnCode.Success, parameters);
            //服务器记录下用户登录成功，如果在这之前有别的客户端登陆了这个账号，挤掉
            IMApplication.Instance.AddLoginUser(dbUser, peer);
        }

        //重置密码
        private void OnResetPassword(IMClientPeer peer, OperationRequest request)
        {
            var subCode = SubCode.Login_ResetPassword;
            if (!TryInitResponse(subCode, peer, request, out var parameters,
                ParameterKeys.LOGIN_RESET_PASSWORD_REQUEST, out ResetPasswordRequestModel model)) return;
            //DB获取用户数据
            var dbUser = UserManager.GetUser(peer.LoginUser?.Username);
            //用户名不存在
            if (dbUser == null)
            {
                peer.SendResponse(ReturnCode.UsernameDoesNotExist, parameters);
                return;
            }
            //对比密码
            if (model.OldPassword.SaltEncodeString(DigestUtil.EncodeType.SHA256, dbUser.Salt) != dbUser.Password)
            {
                peer.SendResponse(ReturnCode.PasswordError, parameters);
                return;
            }
            //对比新密码与旧密码
            if(model.NewPassword == model.OldPassword)
            {
                peer.SendResponse(ReturnCode.PasswordSame, parameters);
                return;
            }
            //密码加盐SHA256加密 更新salt
            dbUser.Password = model.NewPassword.SaltEncodeString(DigestUtil.EncodeType.SHA256, out var salt);
            dbUser.Salt = salt;
            UserManager.UpdateUser(dbUser);
            //记录事件
            mLog.InfoFormat("用户更改密码:{0}", peer.LoginUser);
            peer.SendResponse(ReturnCode.Success, parameters);
        }

        //忘记密码
        private void OnForgetPassword(IMClientPeer peer, OperationRequest request)
        {
            //TODO
        }
    }
}
