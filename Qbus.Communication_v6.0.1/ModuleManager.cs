// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.ModuleManager
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;
using Qbus.Communication.Protocol.Downloaders;
using Qbus.Communication.Protocol.Modules;
using System.Collections.Generic;

namespace Qbus.Communication
{
  public class ModuleManager
  {
    private Queue<NextTextCommand> _textqueue;
    private Queue<ParametersAdressen> _paramQueue;
    private Queue<Controller> _controller;
    private Queue<Controller> _PresentController;
    private SortedList<byte, AddressMode> _cachedModes;
    private TextDownloader TxtDwnldr;
    private List<Module> result;
    private byte MaxAddress;
    private Controller _c;
    private bool _inited;

    public event GetModulesAsyncResult GetModules;

    private void Init()
    {
      if (this._inited)
        return;
      ConnectionManager.Instance.CommandReceived += new ControllerCommunication.CommandEventHandler(this.Instance_CommandReceived);
      this._inited = true;
    }

    private void Instance_CommandReceived(object sender, CommandEventArgs e)
    {
      if (e.Command.GetType() != typeof (ReadModeAddress) && e.Command.GetType() != typeof (AddressText) && e.Command.GetType() != typeof (ParametersAdressen))
        return;
      if (e.Command.GetType() == typeof (ReadModeAddress))
      {
        ReadModeAddress readModeAddress = (ReadModeAddress) e.Command;
        this._cachedModes = readModeAddress.Modes;
        this.MaxAddress = readModeAddress.UsedAddresses;
        foreach (AddressMode am in (IEnumerable<AddressMode>) readModeAddress.Modes.Values)
          this.EnqueueTextCommand(am);
        this.EnqueueParamCmd();
        this.GetNextText();
      }
      else if (e.Command.GetType() == typeof (AddressText))
      {
        AddressText addressText = (AddressText) e.Command;
        if (this._cachedModes.ContainsKey(addressText.Address))
          this.CreateModule(this._cachedModes[addressText.Address].Mode, addressText.Texts, addressText.Address, addressText.SubAddress, addressText.Controller);
        this.GetNextText();
      }
      else
      {
        if (e.Command.GetType() != typeof (ParametersAdressen))
          return;
        this.GetNextText();
      }
    }

    private void EnqueueTextCommand(AddressMode am)
    {
      if (this._textqueue == null)
        this._textqueue = new Queue<NextTextCommand>();
      byte num1 = (byte) 4;
      switch (am.Mode)
      {
        case Module.MODE.TOGGLE:
          num1 = (byte) 1;
          break;
        case Module.MODE.MONO:
          num1 = (byte) 1;
          break;
        case Module.MODE.DIMMER1T:
          num1 = (byte) 2;
          break;
        case Module.MODE.DIMMER2T:
          num1 = (byte) 2;
          break;
        case Module.MODE.TIMER1:
          num1 = (byte) 1;
          break;
        case Module.MODE.TIMER2:
          num1 = (byte) 1;
          break;
        case Module.MODE.TIMER3:
          num1 = (byte) 1;
          break;
        case Module.MODE.TIMER4:
          num1 = (byte) 1;
          break;
        case Module.MODE.SHUTTER:
          num1 = (byte) 2;
          break;
        case Module.MODE.STARTSTOP:
          num1 = (byte) 2;
          break;
        case Module.MODE.INTERVAL:
          num1 = (byte) 1;
          break;
        case Module.MODE.THERMO:
          num1 = (byte) 4;
          break;
        case Module.MODE.HVAC:
          num1 = (byte) 4;
          break;
        case Module.MODE.AUDIO:
          num1 = (byte) 4;
          break;
        case Module.MODE.TIMER5:
          num1 = (byte) 1;
          break;
        case Module.MODE.RGB:
          num1 = (byte) 2;
          break;
        case Module.MODE.CLC:
          num1 = (byte) 2;
          break;
        case Module.MODE.ROL02P:
          num1 = (byte) 2;
          break;
        case Module.MODE.PID:
          num1 = (byte) 4;
          break;
        case Module.MODE.RENSON:
          num1 = (byte) 4;
          break;
      }
      int num2 = 4 / (int) num1;
      for (int index = 0; index < num2; ++index)
        this._textqueue.Enqueue(new NextTextCommand(am.Controller, am.Address, (byte) ((uint) index * (uint) num1)));
    }

    private void EnqueueParamCmd()
    {
      byte num1 = (byte) 3;
      byte num2 = (byte) ((uint) this.MaxAddress + 2U);
      if (this._paramQueue == null)
        this._paramQueue = new Queue<ParametersAdressen>();
      for (byte index = num1; (int) index <= (int) num2; ++index)
      {
        ParametersAdressen parametersAdressen = new ParametersAdressen();
        parametersAdressen.Address = index;
        parametersAdressen.Controller = this._c;
        parametersAdressen.Write = false;
        this._paramQueue.Enqueue(parametersAdressen);
      }
    }

    private void GetNextText()
    {
      if (this._textqueue.Count > 0)
      {
        NextTextCommand nextTextCommand = this._textqueue.Dequeue();
        AddressText addressText = new AddressText();
        addressText.Address = nextTextCommand.Address;
        addressText.SubAddress = nextTextCommand.SubAddress;
        addressText.Controller = nextTextCommand.Controller;
        ConnectionManager.Instance.Send((Command) addressText);
      }
      else
      {
        this.Dispose();
        this.UpdateStatus();
        this.GetNextController();
      }
    }

    public void UpdateStatus()
    {
      byte num1 = (byte) 3;
      byte num2 = (byte) ((uint) this.MaxAddress + 2U);
      for (byte index = num1; (int) index <= (int) num2; ++index)
      {
        AddressStatus addressStatus = new AddressStatus();
        addressStatus.Address = index;
        addressStatus.Controller = this._c;
        addressStatus.md = this._cachedModes[index].Mode;
        ConnectionManager.Instance.Send((Command) addressStatus);
      }
    }

    public void GetModulesAsync()
    {
      this.result = new List<Module>();
      this._controller = new Queue<Controller>();
      this._PresentController = new Queue<Controller>();
      foreach (ControllerCommunication controllerCommunication in ConnectionManager.Instance.ActiveConnections)
      {
        this._controller.Enqueue(controllerCommunication.Controller);
        this._PresentController.Enqueue(controllerCommunication.Controller);
      }
      this.GetNextController();
    }

    public void GetModulesAsync(List<Controller> cList)
    {
      this.result = new List<Module>();
      this._controller = new Queue<Controller>();
      this._PresentController = new Queue<Controller>();
      foreach (Controller controller in cList)
      {
        this._controller.Enqueue(controller);
        this._PresentController.Enqueue(controller);
      }
      this.GetNextController();
    }

    private void GetNextController()
    {
      if (this._controller.Count > 0)
      {
        this.GetModulesAsyncExec(this._controller.Dequeue());
      }
      else
      {
        if (this.GetModules == null)
          return;
        this.GetModules(this.GetAllPresentModules());
      }
    }

    private void GetModulesAsyncExec(Controller c)
    {
      this._c = c;
      this.Init();
      c.ClearModules();
      ReadModeAddress readModeAddress = new ReadModeAddress();
      readModeAddress.Controller = c;
      ConnectionManager.Instance.Send((Command) readModeAddress);
      this.TxtDwnldr = new TextDownloader(ConnectionManager.Instance.Find(c));
      this.TxtDwnldr.Start();
    }

    private void Dispose()
    {
      ConnectionManager.Instance.CommandReceived -= new ControllerCommunication.CommandEventHandler(this.Instance_CommandReceived);
      this._inited = false;
    }

    public List<Module> GetAllPresentModules()
    {
      foreach (Controller controller in this._PresentController)
        this.result.AddRange((IEnumerable<Module>) controller.Modules);
      return this.result;
    }

    private void CreateModule(Module.MODE mode, string name, byte address, byte subaddress, Controller c)
    {
      if (name.Length == 0)
        return;
      if (mode == Module.MODE.HVAC)
        mode = Module.MODE.CO2;
      Module module = this.GetModule(mode);
      if (module == null)
        return;
      module.Name = name;
      module.Address = address;
      module.Subaddress = subaddress;
      module.Controller = c;
      c.AddModule(module);
      if (mode != Module.MODE.CO2)
        return;
      mode = Module.MODE.CO2TEMP;
			address += MonoUtils.SByte_MIN;
      this.CreateModule(mode, name + " _Temp", address, subaddress, c);
    }

    private Module GetModule(Module.MODE m)
    {
      switch (m)
      {
        case Module.MODE.TOGGLE:
          return (Module) new Toggle();
        case Module.MODE.MONO:
          return (Module) new Mono();
        case Module.MODE.DIMMER1T:
          return (Module) new Dimmer();
        case Module.MODE.DIMMER2T:
          return (Module) new Dimmer();
        case Module.MODE.TIMER1:
          return (Module) new Timer1();
        case Module.MODE.TIMER2:
          return (Module) new Timer2();
        case Module.MODE.TIMER3:
          return (Module) new Timer3();
        case Module.MODE.TIMER4:
          return (Module) new Timer4();
        case Module.MODE.SHUTTER:
          return (Module) new RollingShutter();
        case Module.MODE.STARTSTOP:
          return (Module) new StartStop();
        case Module.MODE.INTERVAL:
          return (Module) new Interval();
        case Module.MODE.THERMO:
          return (Module) new Thermo();
        case Module.MODE.HVAC:
          return (Module) new CO2();
        case Module.MODE.AUDIO:
          return (Module) new Audio();
        case Module.MODE.TIMER5:
          return (Module) new Timer5();
        case Module.MODE.CLC:
          return (Module) new Clc();
        case Module.MODE.ROL02P:
          return (Module) new Rol02P();
        case Module.MODE.RENSON:
          return (Module) new Renson();
        case Module.MODE.CO2:
          return (Module) new CO2();
        case Module.MODE.CO2TEMP:
          return (Module) new CO2Temperature();
        default:
          return (Module) null;
      }
    }
  }
}
