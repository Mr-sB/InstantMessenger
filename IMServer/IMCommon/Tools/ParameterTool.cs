using ESocket.Common.Tools;
using System.Collections.Generic;

namespace IMCommon.Tools
{
    public static class ParameterTool
    {
        public static Dictionary<string, object> AddOperationCode(this Dictionary<string, object> parameters, OperationCode operationCode)
        {
            if (parameters == null || parameters.ContainsKey(ParameterKeys.OPERATION_CODE)) return parameters;
            return parameters.AddParameter(ParameterKeys.OPERATION_CODE, operationCode);
        }

        public static Dictionary<string, object> AddSubCode(this Dictionary<string, object> parameters, SubCode subCode)
        {
            if (parameters == null || parameters.ContainsKey(ParameterKeys.SUB_CODE)) return parameters;
            return parameters.AddParameter(ParameterKeys.SUB_CODE, subCode);
        }

        public static bool TryGetOperationCode(this Dictionary<string, object> parameters, out OperationCode operationCode)
        {
            return parameters.TryGetParameter(ParameterKeys.OPERATION_CODE, out operationCode);
        }

        public static bool TryGetSubCode(this Dictionary<string, object> parameters, out SubCode subCode)
        {
            return parameters.TryGetParameter(ParameterKeys.SUB_CODE, out subCode);
        }
    }
}
