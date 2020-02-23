﻿using ESocket.Client;
using ESocket.Common;
using ESocket.Common.Tools;
using IMClient.Controllers;
using IMCommon;
using IMCommon.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IMClient
{
    public class ClientListener : IPeerListener
    {
        private Dictionary<OperationCode, ControllerBase> mControllers;
        private static ClientListener mInstance;
        public static ClientListener Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new ClientListener();
                return mInstance;
            }
        }

        private ClientListener()
        {
            InitControllers();
        }

        private void InitControllers()
        {
            mControllers = new Dictionary<OperationCode, ControllerBase>();
            Type controllerBaseType = typeof(ControllerBase);
            try
            {
                foreach (var type in Assembly.GetAssembly(controllerBaseType).GetTypes())
                {
                    if (!type.IsAbstract && type.IsSubclassOf(controllerBaseType))
                    {
                        try
                        {
                            var controller = Activator.CreateInstance(type) as ControllerBase;
                            mControllers.Add(controller.OperationCode, controller);
                        }
                        catch(Exception e)
                        {
                            MainActivity.Instance.AddToConsole(e.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MainActivity.Instance.AddToConsole(e.ToString());
            }
        }

        public void OnConnectStateChanged(ConnectCode connectCode)
        {
            MainActivity.Instance.AddToConsole("OnConnectStateChanged:" + connectCode);
        }

        public void OnOperationRequest(OperationRequest request)
        {
            if (!request.Parameters.TryGetOperationCode(out var operationCode))
            {
                operationCode = OperationCode.Unknow;
                MainActivity.Instance.AddToConsole("OperationCode error");
            }
            if (!mControllers.TryGetValue(operationCode, out var controller))
            {
                Console.WriteLine("OnOperationRequest Can not find operationCode:{0}", operationCode);
                return;
            }
            controller.OnOperationRequest(request);
        }

        public void OnOperationResponse(OperationResponse response)
        {
            if(!response.Parameters.TryGetOperationCode(out var operationCode))
            {
                operationCode = OperationCode.Unknow;
                MainActivity.Instance.AddToConsole("OperationCode error");
            }
            if (!mControllers.TryGetValue(operationCode, out var controller))
            {
                Console.WriteLine("OnOperationResponse Can not find operationCode:{0}", operationCode);
                return;
            }
            controller.OnOperationResponse(response);
        }
    }
}