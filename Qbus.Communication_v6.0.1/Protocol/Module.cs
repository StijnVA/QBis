// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Module
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication;
using Qbus.Communication.Protocol.Commands;
using System;
using System.Text;

namespace Qbus.Communication.Protocol
{
  public abstract class Module
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

    internal byte? Value
    {
      get
      {
        return this._value;
      }
    }

    public Controller Controller { get; set; }

    public Module.MODE _m { get; set; }

    public string Key
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(this.Controller.ToString()).Append("/").Append(this.Address.ToString()).Append("/").Append(this.Subaddress.ToString());
        return ((object) stringBuilder).ToString();
      }
    }

    public event EventHandler ValueChanged;

    internal event EventHandler ValueToSend;

    public virtual void UpdateStatus(Command cmd)
    {
      int num = 0;
      if (cmd.GetType() == typeof (AddressStatus))
        ++num;
      if (cmd.Data.Length < 4 + num)
        return;
      this._value = new byte?(cmd.Data[(int) this.Subaddress + num]);
      if (this.ValueChanged == null)
        return;
      this.ValueChanged((object) this, (EventArgs) null);
    }

    protected void IsValueChanged()
    {
      if (this.ValueChanged == null)
        return;
      this.ValueChanged((object) this, (EventArgs) null);
    }

    public virtual Command GetCommand()
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

    internal void SetStatus()
    {
      if (this.ValueToSend == null)
        return;
      this.ValueToSend((object) this, (EventArgs) null);
    }

    public abstract bool EqualModule(Module m);

    public abstract bool EqualState(Module m);

    public enum MODE
    {
      TOGGLE = 1,
      MONO = 2,
      DIMMER1T = 3,
      DIMMER2T = 4,
      TIMER1 = 5,
      TIMER2 = 6,
      TIMER3 = 7,
      TIMER4 = 8,
      SHUTTER = 9,
      DISPLAY = 10,
      REMOTE = 11,
      RS232 = 12,
      STARTSTOP = 13,
      INTERVAL = 14,
      THERMO = 15,
      MULTI = 16,
      HVAC = 17,
      SEQUENCE = 18,
      ALARM = 19,
      AUDIO = 20,
      TIMER5 = 21,
      RGB = 22,
      CLC = 23,
      ROL02P = 24,
      PID = 25,
      RENSON = 26,
      CO2 = 27,
      CO2TEMP = 28,
      SCENE = 29,
      CLEAR = 30,
    }
  }
}
