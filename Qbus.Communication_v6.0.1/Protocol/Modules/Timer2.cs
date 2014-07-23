// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Modules.Timer2
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;

namespace Qbus.Communication.Protocol.Modules
{
  public class Timer2 : Module
  {
    public bool? Status
    {
      get
      {
        byte? nullable1 = this.Value;
        if (((int) nullable1.GetValueOrDefault() != 0 ? 0 : (nullable1.HasValue ? 1 : 0)) != 0)
          return new bool?(false);
        byte? nullable2 = this.Value;
        if (((int) nullable2.GetValueOrDefault() != (int) byte.MaxValue ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
          return new bool?(true);
        else
          return new bool?();
      }
    }

    public Timer2()
    {
      this.Name = "";
      this.Address = (byte) 3;
      this.Subaddress = (byte) 0;
      this._m = Module.MODE.TIMER2;
    }

    public override string ToString()
    {
      byte? nullable1 = this.Value;
      if (((int) nullable1.GetValueOrDefault() != 0 ? 0 : (nullable1.HasValue ? 1 : 0)) != 0)
        return "OFF";
      byte? nullable2 = this.Value;
      return ((int) nullable2.GetValueOrDefault() != 0 ? 1 : (!nullable2.HasValue ? 1 : 0)) != 0 ? "ON" : "";
    }

    public void Activate()
    {
      this.SetStatus(byte.MaxValue);
    }

    public override bool EqualModule(Module m)
    {
      return m.GetType() == typeof (Timer2) && (int) m.Address == (int) this.Address && ((int) m.Subaddress == (int) this.Subaddress && m.Controller == this.Controller);
    }

    public override bool EqualState(Module m)
    {
      if (m.GetType() != typeof (Timer2))
        return false;
      bool? status1 = ((Timer2) m).Status;
      bool? status2 = this.Status;
      return (status1.GetValueOrDefault() != status2.GetValueOrDefault() ? 1 : (status1.HasValue != status2.HasValue ? 1 : 0)) == 0;
    }
  }
}
