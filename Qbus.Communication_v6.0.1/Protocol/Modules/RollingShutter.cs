// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Modules.RollingShutter
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;

namespace Qbus.Communication.Protocol.Modules
{
  public class RollingShutter : Module
  {
    private byte _status;

    public int? Status
    {
      get
      {
        return new int?((int) this._status);
      }
    }

    public RollingShutter()
    {
      this.Name = "";
      this.Address = (byte) 3;
      this.Subaddress = (byte) 0;
      this._m = Module.MODE.SHUTTER;
    }

    public override void UpdateStatus(Command cmd)
    {
      int index = 0;
      if (cmd.GetType() == typeof (AddressStatus))
        ++index;
      if (cmd.Data.Length >= 4 + index)
      {
        if ((int) this.Subaddress == 0)
        {
          this._status = (byte) 0;
          if ((int) cmd.Data[index] == (int) byte.MaxValue)
            this._status = (byte) 1;
          if ((int) cmd.Data[1 + index] == (int) byte.MaxValue)
            this._status = (byte) 2;
        }
        if ((int) this.Subaddress == 2)
        {
          this._status = (byte) 0;
          if ((int) cmd.Data[2 + index] == (int) byte.MaxValue)
            this._status = (byte) 1;
          if ((int) cmd.Data[3 + index] == (int) byte.MaxValue)
            this._status = (byte) 2;
        }
      }
      this.IsValueChanged();
    }

    public override string ToString()
    {
      if ((int) this._status == 0)
        return "STOP";
      if ((int) this._status == 1)
        return "UP";
      return (int) this._status == 2 ? "DOWN" : "Unknown";
    }

    public void Up()
    {
      this.SetStatus((byte) 1);
    }

    public void Down()
    {
      this.SetStatus((byte) 2);
    }

    public void Stop()
    {
      this.SetStatus((byte) 0);
    }

    public override bool EqualModule(Module m)
    {
      return m.GetType() == typeof (RollingShutter) && (int) m.Address == (int) this.Address && ((int) m.Subaddress == (int) this.Subaddress && m.Controller == this.Controller);
    }

    public override bool EqualState(Module m)
    {
      if (m.GetType() != typeof (RollingShutter))
        return false;
      int? status1 = ((RollingShutter) m).Status;
      int? status2 = this.Status;
      return (status1.GetValueOrDefault() != status2.GetValueOrDefault() ? 1 : (status1.HasValue != status2.HasValue ? 1 : 0)) == 0;
    }
  }
}
