﻿// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.SDK.GetActivationStringResponseBody
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Qbus.Communication.SDK
{
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  [GeneratedCode("System.ServiceModel", "4.0.0.0")]
  [DataContract(Namespace = "http://tempuri.org/")]
  [DebuggerStepThrough]
  public class GetActivationStringResponseBody
  {
    [DataMember(EmitDefaultValue = false, Order = 0)]
    public string GetActivationStringResult;

    public GetActivationStringResponseBody()
    {
    }

    public GetActivationStringResponseBody(string GetActivationStringResult)
    {
      this.GetActivationStringResult = GetActivationStringResult;
    }
  }
}
