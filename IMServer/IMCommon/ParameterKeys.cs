namespace IMCommon
{
    /// <summary>
    /// 参数Keys
    /// </summary>
    public static class ParameterKeys
    {
        public const string PARAMETER_KEY = "PARAMETER_KEY";
        public const string OPERATION_CODE = "OPERATION_CODE";
        public const string SUB_CODE = "SUB_CODE";

        //Common
        public const string USERNAME = "USERNAME";//string的username
        public const string USER_MODEL = "USER_MODEL";
        public const string USER_MODEL_LIST = "USER_MODEL_LIST";
        //Login
        public const string LOGIN_SIGN_UP_REQUEST = "LOGIN_SIGN_UP_REQUEST";
        public const string LOGIN_SIGN_IN_REQUEST = "LOGIN_SIGN_IN_REQUEST";
        public const string LOGIN_RESET_PASSWORD_REQUEST = "LOGIN_RESET_PASSWORD_REQUEST";
        //Chat
        public const string CHAT_RECORD_REQUEST = "CHAT_RECORD_REQUEST";
        public const string CHAT_RECORD_RESPONSE = "CHAT_RECORD_RESPONSE";
        public const string CHAT_MESSAGE_REQUEST = "CHAT_MESSAGE_REQUEST";
        public const string CHAT_INFO = "CHAT_INFO";
        //Contact
        public const string CONTACT_ADD_CLIENT_RESPONSE = "CONTACT_ADD_CLIENT_RESPONSE";
        public const string CONTACT_ADD_SERVER_RESPONSE = "CONTACT_ADD_SERVER_RESPONSE";
    }
}
