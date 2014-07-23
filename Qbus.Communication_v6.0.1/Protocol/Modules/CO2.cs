// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Modules.CO2
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;
using System;

namespace Qbus.Communication.Protocol.Modules
{
  public class CO2 : Module
  {
    private string[] CO2_Texts_Str = Enum.GetNames(typeof (CO2.CO2_TEXTS));
    private byte _writeSubaddress;
    private byte _value;
    private byte _co2;
    private byte _humid;
    private byte _refresh;
    private byte _co2Regime;

    public int CO2Val
    {
      get
      {
        return (int) this._co2 * 16;
      }
    }

    public int Humidity
    {
      get
      {
        return (int) this._humid;
      }
    }

    public int Refresh
    {
      get
      {
        return (int) this._refresh;
      }
    }

    public string Regime
    {
      get
      {
        return this.Controller.Texts[this.CO2_Texts_Str[(int) this._co2Regime]];
      }
    }

    public string Manual
    {
      get
      {
        return this.Controller.Texts[((object) CO2.CO2_TEXTS.Co_Manual).ToString()];
      }
    }

    public string Night
    {
      get
      {
        return this.Controller.Texts[((object) CO2.CO2_TEXTS.Co_Night).ToString()];
      }
    }

    public string Boost
    {
      get
      {
        return this.Controller.Texts[((object) CO2.CO2_TEXTS.Co_Boost).ToString()];
      }
    }

    public string Off
    {
      get
      {
        return this.Controller.Texts[((object) CO2.CO2_TEXTS.Co_Off).ToString()];
      }
    }

    public string Auto
    {
      get
      {
        return this.Controller.Texts[((object) CO2.CO2_TEXTS.Co_Auto).ToString()];
      }
    }

    public CO2()
    {
      this.Name = "";
      this.Address = (byte) 3;
      this.Subaddress = (byte) 0;
      this._m = Module.MODE.CO2;
    }

    public void ToManual()
    {
      this._value = (byte) 0;
      this._writeSubaddress = (byte) 3;
      this.SetStatus();
    }

    public void ToNight()
    {
      this._value = (byte) 1;
      this._writeSubaddress = (byte) 3;
      this.SetStatus();
    }

    public void ToBoost()
    {
      this._value = (byte) 2;
      this._writeSubaddress = (byte) 3;
      this.SetStatus();
    }

    public void ToOff()
    {
      this._value = (byte) 3;
      this._writeSubaddress = (byte) 3;
      this.SetStatus();
    }

    public void ToAuto()
    {
      this._value = (byte) 4;
      this._writeSubaddress = (byte) 3;
      this.SetStatus();
    }

    public void SetRefresh(byte value)
    {
      if ((int) value < 5 || (int) value > 120)
        return;
      this._value = value;
      this._writeSubaddress = (byte) 2;
      this.SetStatus();
    }

    public override void UpdateStatus(Command cmd)
    {
      if (cmd.Data.Length >= 34)
      {
        this._co2 = cmd.Data[12];
        this._humid = cmd.Data[13];
        this._refresh = cmd.Data[18];
        this._co2Regime = cmd.Data[30];
        this.IsValueChanged();
      }
      else
      {
        if (cmd.Data.Length != 4)
          return;
        this._co2 = cmd.Data[0];
        this._humid = cmd.Data[1];
        this._refresh = cmd.Data[2];
        this._co2Regime = cmd.Data[3];
        this.IsValueChanged();
      }
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
      return m.GetType() == typeof (CO2) && (int) m.Address == (int) this.Address && ((int) m.Subaddress == (int) this.Subaddress && m.Controller == this.Controller);
    }

    public override bool EqualState(Module m)
    {
      if (m.GetType() != typeof (CO2))
        return false;
      CO2 co2 = (CO2) m;
      return (int) co2._co2 == (int) this._co2 && (int) co2._co2Regime == (int) this._co2Regime && ((int) co2._humid == (int) this._humid && (int) co2._refresh == (int) this._refresh);
    }

    private enum CO2_TEXTS
    {
      Co_Manual = 38,
      Co_Night = 39,
      Co_Boost = 40,
      Co_Off = 41,
      Co_Auto = 42,
    }
  }
}
