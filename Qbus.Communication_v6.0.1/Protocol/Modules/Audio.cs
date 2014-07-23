// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Modules.Audio
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;
using System;

namespace Qbus.Communication.Protocol.Modules
{
  public class Audio : Module
  {
    private byte _writeSubaddress;
    private byte _value;

    public Audio()
    {
      this.Name = "";
      this.Address = (byte) 3;
      this.Subaddress = (byte) 0;
      this._m = Module.MODE.AUDIO;
    }

    private void SetIrCode(byte code)
    {
      this._value = code;
      this._writeSubaddress = (byte) 3;
      this.SetStatus();
    }

    public void SetIrCode1()
    {
      this.SetIrCode((byte) 0);
    }

    public void SetIrCode2()
    {
      this.SetIrCode((byte) 1);
    }

    public void VolumeUp()
    {
      this._value = (byte) 1;
      this._writeSubaddress = (byte) 1;
      this.SetStatus();
    }

    public void VolumeDn()
    {
      this._value = (byte) 2;
      this._writeSubaddress = (byte) 1;
      this.SetStatus();
    }

    public void VolumeStop()
    {
      this._value = (byte) 0;
      this._writeSubaddress = (byte) 1;
      this.SetStatus();
    }

    public void SetVolume(byte Volume)
    {
      if ((int) Volume >= 64)
        return;
      byte? nullable = this.Value;
      int num = 192 + (int) Volume;
      this._value = (byte) (nullable.HasValue ? new int?((int) nullable.GetValueOrDefault() & num) : new int?()).Value;
      this._writeSubaddress = (byte) 0;
      this.SetStatus();
    }

    private void SetSource(byte Source)
    {
      if ((int) Source > 3)
        return;
      int num = (int) Source * 64;
      byte? nullable1 = this.Value;
      int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault() % 64) : new int?();
      this._value = (byte) (nullable2.HasValue ? new int?(num + nullable2.GetValueOrDefault()) : new int?()).Value;
      this._writeSubaddress = (byte) 0;
      this.SetStatus();
    }

    public void SetSource1()
    {
      this.SetSource((byte) 0);
    }

    public void SetSource2()
    {
      this.SetSource((byte) 1);
    }

    public void SetSource3()
    {
      this.SetSource((byte) 2);
    }

    public void SetSource4()
    {
      this.SetSource((byte) 3);
    }

    public override Command GetCommand()
    {
      AddressStatus addressStatus = new AddressStatus();
      addressStatus.Address = this.Address;
      addressStatus.SubAddress = (AddressStatus.SUBADDRESS) this._writeSubaddress;
      addressStatus.Data = new byte[2];
      addressStatus.Data[1] = this._value;
      addressStatus.Write = true;
      return (Command) addressStatus;
    }

    public override bool EqualModule(Module m)
    {
      throw new NotImplementedException();
    }

    public override bool EqualState(Module m)
    {
      throw new NotImplementedException();
    }

    private enum SOURCE_TEXTS
    {
      SOURCE1,
      SOURCE2,
      SOURCE3,
      SOURCE4,
    }

    private enum AUDIO_TEXTS
    {
      POWER_ON,
      POWER_OFF,
    }
  }
}
