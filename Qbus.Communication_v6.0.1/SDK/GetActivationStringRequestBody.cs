// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.SDK.GetActivationStringRequestBody
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
  [DataContract(Namespace = "http://tempuri.org/")]
  [GeneratedCode("System.ServiceModel", "4.0.0.0")]
  [DebuggerStepThrough]
  public class GetActivationStringRequestBody
  {
    [DataMember(EmitDefaultValue = false, Order = 0)]
    public string CTL;
    [DataMember(EmitDefaultValue = false, Order = 1)]
    public string SN;
    [DataMember(EmitDefaultValue = false, Order = 2)]
    public string name;
    [DataMember(EmitDefaultValue = false, Order = 3)]
    public string email;
    [DataMember(EmitDefaultValue = false, Order = 4)]
    public string installer;

    public GetActivationStringRequestBody()
    {
    }

    public GetActivationStringRequestBody(string CTL, string SN, string name, string email, string installer)
    {
      this.CTL = CTL;
      this.SN = SN;
      this.name = name;
      this.email = email;
      this.installer = installer;
    }
  }
}
