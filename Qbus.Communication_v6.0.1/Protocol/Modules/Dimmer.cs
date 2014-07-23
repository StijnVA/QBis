// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Modules.Dimmer
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;

namespace Qbus.Communication.Protocol.Modules
{
  public class Dimmer : Module
  {
    private int _percentageValue;

    public int? Status
    {
      get
      {
        if (!this.Value.HasValue)
          return new int?();
        double num = Math.Round((double) this.Value.Value / (double) byte.MaxValue * 100.0);
        if (num > 100.0)
          num = 100.0;
        this._percentageValue = (int) (byte) num;
        return new int?((int) num);
      }
    }

    public Dimmer()
    {
      this.Name = "";
      this.Address = (byte) 3;
      this.Subaddress = (byte) 0;
      this._percentageValue = 0;
      this._m = Module.MODE.DIMMER1T;
    }

    public override string ToString()
    {
      if (!this.Value.HasValue)
        return "unknown";
      double num = Math.Round((double) this.Value.Value / (double) byte.MaxValue * 100.0);
      if (num > 100.0)
        num = 100.0;
      this._percentageValue = (int) (byte) num;
      return num.ToString() + "%";
    }

    public void On()
    {
      this.SetStatus(byte.MaxValue);
      this._percentageValue = 100;
    }

    public void Off()
    {
      this._percentageValue = 0;
      this.SetStatus((byte) 0);
    }

    public void Set(int bVal)
    {
      if (bVal <= 0 || bVal >= 100)
        return;
      this.SetStatus((byte) ((double) bVal / 100.0 * (double) byte.MaxValue));
    }

    public void Increment()
    {
      byte? nullable = this.Value;
      if (((int) nullable.GetValueOrDefault() >= (int) byte.MaxValue ? 0 : (nullable.HasValue ? 1 : 0)) == 0)
        return;
      this._percentageValue += 5;
      if (this._percentageValue > (int) byte.MaxValue)
        this._percentageValue = (int) byte.MaxValue;
      this.SetStatus((byte) ((double) this._percentageValue / 100.0 * (double) byte.MaxValue));
    }

    public void Decrement()
    {
      byte? nullable = this.Value;
      if (((int) nullable.GetValueOrDefault() <= 0 ? 0 : (nullable.HasValue ? 1 : 0)) == 0)
        return;
      this._percentageValue -= 5;
      if (this._percentageValue < 0)
        this._percentageValue = 0;
      this.SetStatus((byte) ((double) this._percentageValue / 100.0 * (double) byte.MaxValue));
    }

    public override bool EqualModule(Module m)
    {
      return m.GetType() == typeof (Dimmer) && (int) m.Address == (int) this.Address && ((int) m.Subaddress == (int) this.Subaddress && m.Controller == this.Controller);
    }

    public override bool EqualState(Module m)
    {
      if (m.GetType() != typeof (Dimmer))
        return false;
      int? status1 = ((Dimmer) m).Status;
      int? status2 = this.Status;
      return (status1.GetValueOrDefault() != status2.GetValueOrDefault() ? 1 : (status1.HasValue != status2.HasValue ? 1 : 0)) == 0;
    }
  }
}
