// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Modules.CO2Temperature
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;
using System;

namespace Qbus.Communication.Protocol.Modules
{
  public class CO2Temperature : Module
  {
    private string[] _thTxt = Enum.GetNames(typeof (CO2Temperature.THERMO_TEXTS));
    internal byte _writeSubaddress;
    internal byte _value;
    internal byte _setTemp;
    internal byte _currentTemp;
    internal byte _tempRegime;

    public float SetpointTemp
    {
      get
      {
        return (float) this._setTemp / 2f;
      }
      set
      {
        this._setTemp = byte.Parse((value * 2f).ToString());
      }
    }

    public float CurrentTemp
    {
      get
      {
        return (float) this._currentTemp / 2f;
      }
    }

    public string Economic
    {
      get
      {
        return this.Controller.Texts[((object) CO2Temperature.THERMO_TEXTS.Economy).ToString()];
      }
    }

    public string Manual
    {
      get
      {
        return this.Controller.Texts[((object) CO2Temperature.THERMO_TEXTS.Manual).ToString()];
      }
    }

    public string Freeze
    {
      get
      {
        return this.Controller.Texts[((object) CO2Temperature.THERMO_TEXTS.Freese).ToString()];
      }
    }

    public string Comfort
    {
      get
      {
        return this.Controller.Texts[((object) CO2Temperature.THERMO_TEXTS.Comfort).ToString()];
      }
    }

    public string Night
    {
      get
      {
        return this.Controller.Texts[((object) CO2Temperature.THERMO_TEXTS.Night).ToString()];
      }
    }

    public string Regime
    {
      get
      {
        return this.Controller.Texts[this._thTxt[(int) this._tempRegime]];
      }
    }

    public CO2Temperature()
    {
      this.Name = "";
      this.Address = (byte) 3;
      this.Subaddress = (byte) 0;
      this._m = Module.MODE.CO2TEMP;
    }

    public void ToComfort()
    {
      this._value = (byte) 3;
      this._writeSubaddress = (byte) 1;
      this.SetStatus();
    }

    public void ToNight()
    {
      this._value = (byte) 4;
      this._writeSubaddress = (byte) 1;
      this.SetStatus();
    }

    public void ToEconomic()
    {
      this._value = (byte) 2;
      this._writeSubaddress = (byte) 1;
      this.SetStatus();
    }

    public void ToFreeze()
    {
      this._value = (byte) 1;
      this._writeSubaddress = (byte) 1;
      this.SetStatus();
    }

    public void ToManual()
    {
      this._value = (byte) 0;
      this._writeSubaddress = (byte) 1;
      this.SetStatus();
    }

    public void IncSetValue()
    {
      this.SetValue(((double) this._setTemp + 1.0) / 2.0);
    }

    public void DecSetValue()
    {
      this.SetValue(((double) this._setTemp - 1.0) / 2.0);
    }

    public void SetValue(double value)
    {
      if (value >= 5.0 && value <= 35.0)
      {
        this._value = (byte) (value * 2.0);
        this._writeSubaddress = (byte) 0;
        this.SetStatus();
      }
      else if (value < 5.0)
      {
        this._value = (byte) 10;
        this._writeSubaddress = (byte) 0;
        this.SetStatus();
      }
      else
      {
        if (value <= 35.0)
          return;
        this._value = (byte) 70;
        this._writeSubaddress = (byte) 0;
        this.SetStatus();
      }
    }

    public override void UpdateStatus(Command cmd)
    {
      if (cmd.Data.Length >= 34)
      {
        this._setTemp = cmd.Data[33];
        this._currentTemp = cmd.Data[14];
        this._tempRegime = cmd.Data[32];
        this.IsValueChanged();
      }
      else
      {
        if (cmd.Data.Length != 4)
          return;
        this._setTemp = cmd.Data[0];
        this._currentTemp = cmd.Data[1];
        this._tempRegime = cmd.Data[3];
        this.IsValueChanged();
      }
    }

    public override Command GetCommand()
    {
      AddressStatus addressStatus = new AddressStatus();
      addressStatus.Address = (byte) ((uint) this.Address - 128U);
      addressStatus.SubAddress = (AddressStatus.SUBADDRESS) this._writeSubaddress;
      addressStatus.Data = new byte[2];
      addressStatus.Data[1] = this._value;
      addressStatus.Write = true;
      return (Command) addressStatus;
    }

    public override bool EqualModule(Module m)
    {
      return m.GetType() == typeof (CO2Temperature) && (int) m.Address == (int) this.Address && ((int) m.Subaddress == (int) this.Subaddress && m.Controller == this.Controller);
    }

    public override bool EqualState(Module m)
    {
      if (m.GetType() != typeof (CO2Temperature))
        return false;
      CO2Temperature co2Temperature = (CO2Temperature) m;
      return (int) co2Temperature._currentTemp == (int) this._currentTemp && (int) co2Temperature._tempRegime == (int) this._tempRegime && (int) co2Temperature._setTemp == (int) this._setTemp;
    }

    private enum THERMO_TEXTS
    {
      Manual = 11,
      Freese = 12,
      Economy = 13,
      Comfort = 14,
      Night = 15,
    }
  }
}
