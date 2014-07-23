// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Modules.Thermo
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;
using System;

namespace Qbus.Communication.Protocol.Modules
{
  public class Thermo : Module
  {
    private string[] _thTxt = Enum.GetNames(typeof (Thermo.THERMO_TEXTS));
    private float _setTemp;
    private float _writeSubaddress;
    private float _value;
    private byte _currentTemp;
    private byte _regime;

    internal float? Offset { get; set; }

    internal bool? Negative { get; set; }

    internal bool? byDegree { get; set; }

    public float SetpointTemp
    {
      get
      {
        bool? byDegree = this.byDegree;
        if ((!byDegree.GetValueOrDefault() ? 0 : (byDegree.HasValue ? 1 : 0)) != 0)
          return this._setTemp + this.Offset.Value;
        else
          return (float) (((double) this._setTemp + (double) this.Offset.Value) / 2.0);
      }
      set
      {
        bool? byDegree = this.byDegree;
        if ((!byDegree.GetValueOrDefault() ? 0 : (byDegree.HasValue ? 1 : 0)) != 0)
          this._setTemp = value - this.Offset.Value;
        else if (this.byDegree.HasValue)
          this._setTemp = (float) (((double) value - (double) this.Offset.Value) * 2.0);
        else
          this._setTemp = 0.0f;
      }
    }

    public float CurrentTemp
    {
      get
      {
        bool? byDegree = this.byDegree;
        if ((!byDegree.GetValueOrDefault() ? 0 : (byDegree.HasValue ? 1 : 0)) != 0)
          return (float) this._currentTemp + this.Offset.Value;
        if (this.byDegree.HasValue)
          return (float) (((double) this._currentTemp + (double) this.Offset.Value) / 2.0);
        else
          return 0.0f;
      }
    }

    public string Economic
    {
      get
      {
        return this.Controller.Texts[((object) Thermo.THERMO_TEXTS.Economy).ToString()];
      }
    }

    public string Manual
    {
      get
      {
        return this.Controller.Texts[((object) Thermo.THERMO_TEXTS.Manual).ToString()];
      }
    }

    public string Freeze
    {
      get
      {
        return this.Controller.Texts[((object) Thermo.THERMO_TEXTS.Freese).ToString()];
      }
    }

    public string Comfort
    {
      get
      {
        return this.Controller.Texts[((object) Thermo.THERMO_TEXTS.Comfort).ToString()];
      }
    }

    public string Night
    {
      get
      {
        return this.Controller.Texts[((object) Thermo.THERMO_TEXTS.Night).ToString()];
      }
    }

    public string Regime
    {
      get
      {
        return this.Controller.Texts[this._thTxt[(int) this._regime]];
      }
    }

    public Thermo()
    {
      this.Name = "";
      this.Address = (byte) 3;
      this.Subaddress = (byte) 0;
      this._m = Module.MODE.THERMO;
    }

    public override void UpdateStatus(Command cmd)
    {
      if (cmd.GetType() == typeof (ParametersAdressen))
      {
        byte num = cmd.Data[15];
        this.Negative = new bool?(((int) num & 64) == 64);
        this.byDegree = new bool?(((int) num & 128) == 128);
        this.Offset = new float?((float) ((int) num & 63));
        bool? negative = this.Negative;
        if ((!negative.GetValueOrDefault() ? 0 : (negative.HasValue ? 1 : 0)) != 0)
        {
          float? offset = this.Offset;
          this.Offset = offset.HasValue ? new float?(0.0f - offset.GetValueOrDefault()) : new float?();
        }
        this.IsValueChanged();
      }
      if (this.Offset.HasValue && this.Negative.HasValue)
      {
        int num1 = this.byDegree.HasValue ? 1 : 0;
      }
      if (cmd.Data.Length < 4 || cmd.Data.Length >= 6)
        return;
      this._setTemp = (float) cmd.Data[1];
      this._currentTemp = cmd.Data[2];
      this._regime = cmd.Data[3];
      if (!this.Offset.HasValue && !this.Negative.HasValue && !this.byDegree.HasValue)
        return;
      this.IsValueChanged();
    }

    public void ToComfort()
    {
      this._value = 3f;
      this._writeSubaddress = 3f;
      this.SetStatus();
    }

    public void ToNight()
    {
      this._value = 4f;
      this._writeSubaddress = 3f;
      this.SetStatus();
    }

    public void ToEconomic()
    {
      this._value = 2f;
      this._writeSubaddress = 3f;
      this.SetStatus();
    }

    public void ToFreeze()
    {
      this._value = 1f;
      this._writeSubaddress = 3f;
      this.SetStatus();
    }

    public void ToManual()
    {
      this._value = 0.0f;
      this._writeSubaddress = 3f;
      this.SetStatus();
    }

    public void IncSetValue()
    {
      if ((double) this._setTemp < 70.0)
      {
        this._value = (float) (byte) ((uint) (int) this._setTemp + 1U);
        this._writeSubaddress = 1f;
        this.SetStatus();
      }
      else
      {
        this._value = 70f;
        this._writeSubaddress = 1f;
        this.SetStatus();
      }
    }

    public void DecSetValue()
    {
      if ((double) this._setTemp > 10.0)
      {
        this._value = (float) (byte) ((uint) (int) this._setTemp - 1U);
        this._writeSubaddress = 1f;
        this.SetStatus();
      }
      else
      {
        this._value = 10f;
        this._writeSubaddress = 1f;
        this.SetStatus();
      }
    }

    public void SetValue(float value)
    {
      float num = value - this.Offset.Value;
      if ((double) num >= 5.0 && (double) num <= 35.0)
      {
        bool? byDegree = this.byDegree;
        this._value = (!byDegree.GetValueOrDefault() ? 0 : (byDegree.HasValue ? 1 : 0)) == 0 ? (float) (byte) ((double) num * 2.0) : (float) (byte) num;
        this._writeSubaddress = 1f;
        this.SetStatus();
      }
      else if ((double) num < 5.0)
      {
        this._value = 10f;
        this._writeSubaddress = 1f;
        this.SetStatus();
      }
      else
      {
        if ((double) num <= 35.0)
          return;
        this._value = 70f;
        this._writeSubaddress = 1f;
        this.SetStatus();
      }
    }

    public override Command GetCommand()
    {
      AddressStatus addressStatus = new AddressStatus();
      addressStatus.Address = this.Address;
      addressStatus.SubAddress = (AddressStatus.SUBADDRESS) this._writeSubaddress;
      addressStatus.Data = new byte[2];
      addressStatus.Data[1] = (byte) this._value;
      addressStatus.Write = true;
      return (Command) addressStatus;
    }

    public override bool EqualModule(Module m)
    {
      return m.GetType() == typeof (Thermo) && (int) m.Address == (int) this.Address && ((int) m.Subaddress == (int) this.Subaddress && m.Controller == this.Controller);
    }

    public override bool EqualState(Module m)
    {
      if (m.GetType() != typeof (Thermo))
        return false;
      Thermo thermo = (Thermo) m;
      return (int) thermo._currentTemp == (int) this._currentTemp && (int) thermo._regime == (int) this._regime && (double) thermo._setTemp == (double) this._setTemp;
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
