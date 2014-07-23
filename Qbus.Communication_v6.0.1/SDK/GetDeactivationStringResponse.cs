// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.SDK.GetDeactivationStringResponse
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Qbus.Communication.SDK
{
  [MessageContract(IsWrapped = false)]
  [GeneratedCode("System.ServiceModel", "4.0.0.0")]
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  [DebuggerStepThrough]
  public class GetDeactivationStringResponse
  {
    [MessageBodyMember(Name = "GetDeactivationStringResponse", Namespace = "http://tempuri.org/", Order = 0)]
    public GetDeactivationStringResponseBody Body;

    public GetDeactivationStringResponse()
    {
    }

    public GetDeactivationStringResponse(GetDeactivationStringResponseBody Body)
    {
      this.Body = Body;
    }
  }
}
