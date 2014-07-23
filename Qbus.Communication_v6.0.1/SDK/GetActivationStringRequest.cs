// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.SDK.GetActivationStringRequest
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Qbus.Communication.SDK
{
  [DebuggerStepThrough]
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  [MessageContract(IsWrapped = false)]
  [GeneratedCode("System.ServiceModel", "4.0.0.0")]
  public class GetActivationStringRequest
  {
    [MessageBodyMember(Name = "GetActivationString", Namespace = "http://tempuri.org/", Order = 0)]
    public GetActivationStringRequestBody Body;

    public GetActivationStringRequest()
    {
    }

    public GetActivationStringRequest(GetActivationStringRequestBody Body)
    {
      this.Body = Body;
    }
  }
}
