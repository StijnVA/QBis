﻿// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.SDK.GetActivationStringResponse
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Qbus.Communication.SDK
{
  [GeneratedCode("System.ServiceModel", "4.0.0.0")]
  [DebuggerStepThrough]
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  [MessageContract(IsWrapped = false)]
  public class GetActivationStringResponse
  {
    [MessageBodyMember(Name = "GetActivationStringResponse", Namespace = "http://tempuri.org/", Order = 0)]
    public GetActivationStringResponseBody Body;

    public GetActivationStringResponse()
    {
    }

    public GetActivationStringResponse(GetActivationStringResponseBody Body)
    {
      this.Body = Body;
    }
  }
}
