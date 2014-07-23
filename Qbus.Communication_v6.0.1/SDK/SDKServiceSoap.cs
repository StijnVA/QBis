// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.SDK.SDKServiceSoap
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using System;
using System.CodeDom.Compiler;
using System.ServiceModel;

namespace Qbus.Communication.SDK
{
  [GeneratedCode("System.ServiceModel", "4.0.0.0")]
  [ServiceContract(ConfigurationName = "SDK.SDKServiceSoap")]
  public interface SDKServiceSoap
  {
    [OperationContract(Action = "http://tempuri.org/GetActivationString", ReplyAction = "*")]
    GetActivationStringResponse GetActivationString(GetActivationStringRequest request);

    [OperationContract(Action = "http://tempuri.org/GetActivationString", AsyncPattern = true, ReplyAction = "*")]
    IAsyncResult BeginGetActivationString(GetActivationStringRequest request, AsyncCallback callback, object asyncState);

    GetActivationStringResponse EndGetActivationString(IAsyncResult result);

    [OperationContract(Action = "http://tempuri.org/GetDeactivationString", ReplyAction = "*")]
    GetDeactivationStringResponse GetDeactivationString(GetDeactivationStringRequest request);

    [OperationContract(Action = "http://tempuri.org/GetDeactivationString", AsyncPattern = true, ReplyAction = "*")]
    IAsyncResult BeginGetDeactivationString(GetDeactivationStringRequest request, AsyncCallback callback, object asyncState);

    GetDeactivationStringResponse EndGetDeactivationString(IAsyncResult result);
  }
}
