// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Controller
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Qbus.Communication
{
  public class Controller : IComparable
  {
    private string _login = "QBUS";
    private string _password = "";
    private int _tcpPort = 8446;
    private SortedList<string, string> _texts = new SortedList<string, string>();
    private bool _textsInited;
    private bool _activated;
    private SortedList<string, Module> _modules;
    private SortedList<string, PresetObject> _preset;

    [ScriptIgnore]
    public int LastBank { get; set; }

    [ScriptIgnore]
    public bool AutoConnect { get; set; }

    public string Firmware { get; set; }

    public string Name { get; set; }

    [ScriptIgnore]
    public string dbUID { get; set; }

    [ScriptIgnore]
    public string CtdName
    {
      get
      {
        return this.Texts[((object) TEXTS.Controller).ToString()];
      }
    }

    [ScriptIgnore]
    public string Login
    {
      get
      {
        return this._login;
      }
      set
      {
        this._login = value;
      }
    }

    [ScriptIgnore]
    public string Password
    {
      get
      {
        return this._password;
      }
      set
      {
        this._password = value;
      }
    }

    public byte[] MAC { get; set; }

    public string Address { get; set; }

    public string HostName { get; set; }

    public string ComPort { get; set; }

    public int TcpPort
    {
      get
      {
        return this._tcpPort;
      }
      set
      {
        this._tcpPort = value;
      }
    }

    public bool Connected { get; set; }

    public string SN
    {
      get
      {
        if (this.MAC != null && this.MAC.Length >= 3)
          return this.GetMinChar(this.MAC[0], 2) + this.GetMinChar(this.MAC[1], 2) + this.GetMinChar(this.MAC[2], 2);
        else
          return "";
      }
    }

    [ScriptIgnore]
    public bool TextsInited
    {
      get
      {
        return this._textsInited;
      }
      set
      {
        this._textsInited = value;
      }
    }

    [ScriptIgnore]
    public bool Activated
    {
      get
      {
        return this._activated;
      }
      set
      {
        this._activated = value;
      }
    }

    [ScriptIgnore]
    public SortedList<string, string> Texts
    {
      get
      {
        this.InitTexts();
        return this._texts;
      }
      set
      {
        this.InitTexts();
        this._texts = value;
      }
    }

    public List<Module> Modules
    {
      get
      {
        return Enumerable.ToList<Module>((IEnumerable<Module>) this._modules.Values);
      }
    }

    public List<PresetObject> Presets
    {
      get
      {
        return Enumerable.ToList<PresetObject>((IEnumerable<PresetObject>) this._preset.Values);
      }
    }

    private SortedList<string, Module> ModuleList
    {
      get
      {
        return this._modules;
      }
      set
      {
        this._modules = value;
      }
    }

    private SortedList<string, PresetObject> PresetList
    {
      get
      {
        return this._preset;
      }
      set
      {
        this._preset = value;
      }
    }

    public event EventHandler SendCommand;

    public event EventHandler SendPresetCommand;

    public Controller()
    {
      this.Name = "";
      this.MAC = new byte[0];
      this.Address = "";
      this.ComPort = "";
      this.Firmware = "";
      this.HostName = "";
      this._modules = new SortedList<string, Module>();
      this._preset = new SortedList<string, PresetObject>();
    }

    public override string ToString()
    {
      string str = "";
      if (this.Name != null)
        str = this.Name;
      if (str == "" && this.MAC != null && this.MAC.Length >= 6)
        str = this.SN;
      else if (str == "" && this.MAC != null)
        str = this.SN;
      if (str == "" && this.Address != null)
        str = this.Address + (object) ":" +  this.TcpPort;
      if (this.Firmware != null && this.Firmware != "")
        str = str + " (v " + this.Firmware + ")";
      if (str == "")
        str = "- identifying -";
      return str;
    }

    private string GetMinChar(byte b, int len)
    {
      string str = b.ToString();
      while (len > str.Length)
        str = "0" + str;
      return str;
    }

    public bool IsGreaterFirmware(string fw)
    {
      if (fw == "" || this.Firmware == "" || this.Firmware == null)
        return false;
      int result1 = 0;
      int result2 = 0;
      int result3 = 0;
      int result4 = 0;
      string[] strArray1 = fw.Split('.');
      string[] strArray2 = this.Firmware.Split('.');
      return strArray1.Length == 2 && strArray2.Length == 2 && (int.TryParse(strArray1[0], out result1) && int.TryParse(strArray1[1], out result2)) && (int.TryParse(strArray2[0], out result3) && int.TryParse(strArray2[1], out result4)) && (result3 > result1 || result3 >= result1 && result4 >= result2);
    }

    private void InitTexts()
    {
      try
      {
        foreach (string key in Enum.GetNames(typeof (TEXTS)))
        {
          if (!this._texts.ContainsKey(key))
            this._texts.Add(key, key);
        }
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_MID);
      }
    }

    public void ClearModules()
    {
      foreach (Module module in (IEnumerable<Module>) this._modules.Values)
        module.ValueToSend -= new EventHandler(this.m_ValueToSend);
      this._modules.Clear();
    }

    public void ClearPresets()
    {
      foreach (PresetObject presetObject in (IEnumerable<PresetObject>) this._preset.Values)
        presetObject.ValueToSend -= new EventHandler(this.m_ValueToSend);
      this._preset.Clear();
    }

    public void AddModule(Module m)
    {
      if (this._modules.ContainsKey(m.Key))
        this._modules[m.Key] = m;
      else
        this._modules.Add(m.Key, m);
      m.ValueToSend += new EventHandler(this.m_ValueToSend);
    }

    public void AddPreset(PresetObject p)
    {
      if (this._preset.ContainsKey(p.Key))
        this._preset[p.Key] = p;
      else
        this._preset.Add(p.Key, p);
      p.ValueToSend += new EventHandler(this.m_PresetToSend);
    }

    private void m_ValueToSend(object sender, EventArgs e)
    {
      if (this.SendCommand == null)
        return;
      this.SendCommand(sender, (EventArgs) null);
    }

    private void m_PresetToSend(object sender, EventArgs e)
    {
      if (this.SendPresetCommand == null)
        return;
      this.SendPresetCommand(sender, (EventArgs) null);
    }

    public int CompareTo(object obj)
    {
      if (obj == null || obj.GetType() != this.GetType())
        return -1;
      Controller controller = (Controller) obj;
      if (controller.MAC != null && this.MAC != null)
      {
        bool flag = true;
        for (int index = 0; index < controller.MAC.Length && index < this.MAC.Length; ++index)
        {
          if ((int) controller.MAC[index] != (int) this.MAC[index])
            flag = false;
        }
        if (flag && this.MAC.Length != 0 && controller.MAC.Length != 0)
          return 0;
      }
      return controller.Address == this.Address && (controller.ComPort == this.ComPort || controller.ComPort == null && this.ComPort == "" || this.ComPort == null && controller.ComPort == "") && controller.TcpPort == this.TcpPort ? 0 : -1;
    }
  }
}
