// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Modules.Renson
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication;
using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;
using System.Collections.Generic;

namespace Qbus.Communication.Protocol.Modules
{
  public class Renson : Module
  {
    private static Dictionary<Renson.RensonMode, string> RensonTexts;
    private static Dictionary<Renson.RensonCalibrationStatus, string> RensonCalibrationTexts;
    private static Dictionary<int, string> RensonErrors;
    private byte _value;
    private byte _writeSubaddress;
    private byte _regime;
    private byte _voc;
    private byte _humidity;
    private byte _refresh;
    private byte _exhaust;
    private byte _error;
    private byte _calibration;
    private byte _pressure;
    private byte _masterAdr;

    public byte MasterAddress
    {
      get
      {
        return this._masterAdr;
      }
    }

    public int VOC
    {
      get
      {
        return (int) this._voc * 16;
      }
    }

    public int Humidity
    {
      get
      {
        return (int) this._humidity;
      }
    }

    public int Refresh
    {
      get
      {
        return (int) this._refresh;
      }
    }

    public Renson.RensonMode Program
    {
      get
      {
        return (Renson.RensonMode) (byte) ((uint) this._regime & 7U);
      }
    }

    public string Regime
    {
      get
      {
        return Renson.RensonTexts[this.Program];
      }
    }

    public int Exhaust
    {
      get
      {
        if ((int) this._exhaust > 128)
          return ((int) this._exhaust - 128) * 32;
        else
          return (int) this._exhaust * 2;
      }
    }

    public int Error
    {
      get
      {
        int num = (int) this._regime >> 3;
        if (num != 0)
          return num - 1;
        else
          return 0;
      }
    }

    public string ErrorText
    {
      get
      {
        if (Renson.RensonErrors.ContainsKey(this.Error))
          return Renson.RensonErrors[this.Error];
        else
          return "Error " + (object) this.Error;
      }
    }

    public Renson.RensonCalibrationStatus CalibrationProgram
    {
      get
      {
        return (Renson.RensonCalibrationStatus) this._calibration;
      }
    }

    public string CalibrationRegime
    {
      get
      {
        return Renson.RensonCalibrationTexts[this.CalibrationProgram];
      }
    }

    public int Pressure
    {
      get
      {
        return (int) this._pressure;
      }
    }

    public string ECO
    {
      get
      {
        return Renson.RensonTexts[Renson.RensonMode.ECO];
      }
    }

    public string HDC
    {
      get
      {
        return Renson.RensonTexts[Renson.RensonMode.HDC];
      }
    }

    public string Boost
    {
      get
      {
        return Renson.RensonTexts[Renson.RensonMode.Boost];
      }
    }

    public string EmptyHouse
    {
      get
      {
        return Renson.RensonTexts[Renson.RensonMode.EmptyHouse];
      }
    }

    public string Night
    {
      get
      {
        return Renson.RensonTexts[Renson.RensonMode.Night];
      }
    }

    public string Cook
    {
      get
      {
        return Renson.RensonTexts[Renson.RensonMode.Cook];
      }
    }

    public Renson()
    {
      this.Name = "";
      this.Address = (byte) 3;
      this.Subaddress = (byte) 0;
      this._m = Module.MODE.RENSON;
      this._masterAdr = (byte) 0;
      Renson.FillTexts();
    }

    private static void FillTexts()
    {
      if (Renson.RensonTexts == null)
      {
        Renson.RensonTexts = new Dictionary<Renson.RensonMode, string>();
        Renson.RensonTexts.Add(Renson.RensonMode.ECO, "ECO");
        Renson.RensonTexts.Add(Renson.RensonMode.HDC, "HDC");
        Renson.RensonTexts.Add(Renson.RensonMode.BoostClk, "BoostClk");
        Renson.RensonTexts.Add(Renson.RensonMode.EmptyHouse, "Empty House");
        Renson.RensonTexts.Add(Renson.RensonMode.Night, "Night");
        Renson.RensonTexts.Add(Renson.RensonMode.CookClk, "CookClk");
        Renson.RensonTexts.Add(Renson.RensonMode.Boost, "Boost");
        Renson.RensonTexts.Add(Renson.RensonMode.Cook, "Cook");
      }
      if (Renson.RensonCalibrationTexts == null)
      {
        Renson.RensonCalibrationTexts = new Dictionary<Renson.RensonCalibrationStatus, string>();
        Renson.RensonCalibrationTexts.Add(Renson.RensonCalibrationStatus.RENSON_READY, "Ready");
        Renson.RensonCalibrationTexts.Add(Renson.RensonCalibrationStatus.RENSON_CMODE, "Calibration mode");
        Renson.RensonCalibrationTexts.Add(Renson.RensonCalibrationStatus.RENSON_CAL_BUSY, "Calibration busy");
        Renson.RensonCalibrationTexts.Add(Renson.RensonCalibrationStatus.RENSON_CAL_WAIT, "Not calibrated");
      }
      if (Renson.RensonErrors != null)
        return;
      Renson.RensonErrors = new Dictionary<int, string>();
      Renson.RensonErrors.Add(1, "Error 1: Not calibrated");
      Renson.RensonErrors.Add(4, "Error 4: Flow sensor out of range");
      for (int key = 5; key <= 12; ++key)
        Renson.RensonErrors.Add(key, string.Concat(new object[4]
        {
          (object) "Error ",
          (object) key,
          (object) ": No connection with device ",
          (object) (key - 5)
        }));
      Renson.RensonErrors.Add(14, "Error 14: No cook valve responding to CO2");
      Renson.RensonErrors.Add(16, "Error 16: Fan malfunction");
      Renson.RensonErrors.Add(17, "Error 17: No valve responding to NOVY box");
      Renson.RensonErrors.Add(18, "Error 18: No communication with Renson Box. Box not powered?");
      for (int key = 30; key <= 37; ++key)
        Renson.RensonErrors.Add(key, string.Concat(new object[4]
        {
          (object) "Error ",
          (object) key,
          (object) ": RH sensor malfunction on valve ",
          (object) (key - 30)
        }));
      for (int key = 40; key <= 47; ++key)
        Renson.RensonErrors.Add(key, string.Concat(new object[4]
        {
          (object) "Error ",
          (object) key,
          (object) ": RH sensor has the same value during 24h on valve ",
          (object) (key - 40)
        }));
      for (int key = 50; key <= 57; ++key)
        Renson.RensonErrors.Add(key, string.Concat(new object[4]
        {
          (object) "Error ",
          (object) key,
          (object) ": Temp. sensor malfunction on valve ",
          (object) (key - 50)
        }));
      for (int key = 60; key <= 67; ++key)
        Renson.RensonErrors.Add(key, string.Concat(new object[4]
        {
          (object) "Error ",
          (object) key,
          (object) ": Temp. sensor has the same value during 24h on valve ",
          (object) (key - 60)
        }));
      for (int key = 70; key <= 77; ++key)
        Renson.RensonErrors.Add(key, string.Concat(new object[4]
        {
          (object) "Error ",
          (object) key,
          (object) ": VOC/CO2 sensor malfunction on valve ",
          (object) (key - 70)
        }));
      for (int key = 80; key <= 87; ++key)
        Renson.RensonErrors.Add(key, string.Concat(new object[4]
        {
          (object) "Error ",
          (object) key,
          (object) ": VOC/CO2 sensor has the same value on valve ",
          (object) (key - 80)
        }));
      for (int key = 90; key <= 97; ++key)
        Renson.RensonErrors.Add(key, "Error " + (object) key + ": Valve " + (string) (object) (key - 90) + " is broken");
      Renson.RensonErrors.Add(78, "Error 78: VOC/CO2 sensor on control panel is broken");
      Renson.RensonErrors.Add(88, "Error 88: VOC/CO2 sensor has the same value during 24h on the control panel");
      Renson.RensonErrors.Add(39, "Error 39: Flow sensor out of range");
      Renson.RensonErrors.Add(38, "Error 38: Fire protection");
      Renson.RensonErrors.Add(48, "Error 48: Cook enabled but no valve connected");
    }

    public void ToEco()
    {
      this._value = (byte) 0;
      this._writeSubaddress = (byte) 3;
      this.SetStatus();
    }

    public void ToHDC()
    {
      this._value = (byte) 1;
      this._writeSubaddress = (byte) 3;
      this.SetStatus();
    }

    public void ToBoost()
    {
      this._value = !(this.Regime == Renson.RensonTexts[Renson.RensonMode.BoostClk]) ? (byte) 4 : (byte) 6;
      this._writeSubaddress = (byte) 3;
      this.SetStatus();
    }

    public void ToEmptyHouse()
    {
      this._value = (byte) 3;
      this._writeSubaddress = (byte) 3;
      this.SetStatus();
    }

    public void ToNight()
    {
      this._value = (byte) 2;
      this._writeSubaddress = (byte) 3;
      this.SetStatus();
    }

    public void ToCook()
    {
      this._value = !(this.Regime == Renson.RensonTexts[Renson.RensonMode.CookClk]) ? (byte) 5 : (byte) 7;
      this._writeSubaddress = (byte) 3;
      this.SetStatus();
    }

    public void StartCalibration()
    {
      this._value = (byte) 9;
      this._writeSubaddress = (byte) 3;
      this.SetStatus();
    }

    public override void UpdateStatus(Command cmd)
    {
      if (cmd.GetType() == typeof (ParametersAdressen))
      {
        this._masterAdr = cmd.Data[1];
        this.IsValueChanged();
      }
      if ((int) this._masterAdr == 0)
      {
        ParametersAdressen parametersAdressen = new ParametersAdressen();
        parametersAdressen.Address = this.Address;
        parametersAdressen.Controller = cmd.Controller;
        parametersAdressen.Write = false;
        ConnectionManager.Instance.Send((Command) parametersAdressen);
      }
      if (cmd.Data.Length < 4)
        return;
      this._voc = cmd.Data[0];
      this._humidity = cmd.Data[1];
      this._refresh = cmd.Data[2];
      this._regime = cmd.Data[3];
      if (cmd.Data.Length > 4)
      {
        this._exhaust = cmd.Data[4];
        this._error = cmd.Data[5];
        this._calibration = cmd.Data[6];
        this._pressure = cmd.Data[7];
      }
      this.IsValueChanged();
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
      return m.GetType() == typeof (Renson) && (int) m.Address == (int) this.Address && ((int) m.Subaddress == (int) this.Subaddress && m.Controller == this.Controller);
    }

    public override bool EqualState(Module m)
    {
      if (m.GetType() != typeof (Renson))
        return false;
      Renson renson = (Renson) m;
      return (int) renson._voc == (int) this._voc && (int) renson._humidity == (int) this._humidity && ((int) renson._refresh == (int) this._refresh && (int) renson._regime == (int) this._regime);
    }

    public enum RensonMode
    {
      ECO,
      HDC,
      Night,
      EmptyHouse,
      BoostClk,
      CookClk,
      Boost,
      Cook,
      C_mode,
      Start_Calibration,
    }

    public enum RensonCalibrationStatus
    {
      RENSON_READY,
      RENSON_CMODE,
      RENSON_CAL_BUSY,
      RENSON_CAL_WAIT,
    }
  }
}
