// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.PresetObject
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication;
using Qbus.Communication.Protocol.Commands;
using System;
using System.Text;

namespace Qbus.Communication.Protocol
{
  public class PresetObject
  {
    private string _name = "";
    private byte? _value;

    public string Name
    {
      get
      {
        return this._name;
      }
      set
      {
        this._name = value;
      }
    }

    public byte Address { get; set; }

    public byte Subaddress { get; set; }

    public Controller Controller { get; set; }

    public string Key
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(this.Controller.ToString()).Append("/").Append(this.Address.ToString()).Append("/").Append(this.Subaddress.ToString());
        return ((object) stringBuilder).ToString();
      }
    }

    internal event EventHandler ValueToSend;

    internal virtual Command GetCommand()
    {
      AddressStatus addressStatus = new AddressStatus();
      addressStatus.Address = this.Address;
      addressStatus.SubAddress = (AddressStatus.SUBADDRESS) this.Subaddress;
      addressStatus.Data = new byte[2];
      addressStatus.Data[1] = this._value.Value;
      addressStatus.Write = true;
      return (Command) addressStatus;
    }

    internal void SetStatus(byte val)
    {
      this._value = new byte?(val);
      if (this.ValueToSend == null)
        return;
      this.ValueToSend((object) this, (EventArgs) null);
    }
  }
}
