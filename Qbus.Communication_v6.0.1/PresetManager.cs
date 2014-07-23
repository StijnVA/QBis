// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.PresetManager
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;
using Qbus.Communication.Protocol.Modules;
using System.Collections.Generic;

namespace Qbus.Communication
{
  public class PresetManager
  {
    private Queue<NextTextCommand> _textqueue;
    private Queue<Controller> _controller;
    private Queue<Controller> _PresentController;
    private PresetAddressMode _modes;
    private SortedList<byte, PresetAddressMode> _cachedModes;
    private byte MaxPreset;
    private Controller _c;
    private bool _inited;

    public event GetPresetAsyncResult GetPresets;

    private void Init()
    {
      if (this._inited)
        return;
      ConnectionManager.Instance.CommandReceived += new ControllerCommunication.CommandEventHandler(this.Instance_CommandReceived);
      this._cachedModes = new SortedList<byte, PresetAddressMode>();
      this._inited = true;
    }

    private void Instance_CommandReceived(object sender, CommandEventArgs e)
    {
      if (e.Command.GetType() != typeof (ReadPresetParametersText) && e.Command.GetType() != typeof (ControllerParameters))
        return;
      if (e.Command.GetType() == typeof (ControllerParameters))
      {
        this.MaxPreset = ((ControllerParameters) e.Command).MaxPreset;
        for (byte addr = (byte) 1; (int) addr <= (int) (byte) ((uint) this.MaxPreset - 101U) * 4; ++addr)
          this.EnqueueTextCommand(new PresetAddressMode(addr, this._c));
        this.GetNextText();
      }
      else
      {
        if (e.Command.GetType() != typeof (ReadPresetParametersText))
          return;
        ReadPresetParametersText presetParametersText = (ReadPresetParametersText) e.Command;
        this.CreatePreset(presetParametersText.Name, presetParametersText.Address, presetParametersText.Controller);
        this.GetNextText();
      }
    }

    private void EnqueueTextCommand(PresetAddressMode pam)
    {
      if (this._textqueue == null)
        this._textqueue = new Queue<NextTextCommand>();
      byte num1 = (byte) 4;
      int num2 = 4 / (int) num1;
      for (int index = 0; index < num2; ++index)
        this._textqueue.Enqueue(new NextTextCommand(pam.Controller, pam.Address, (byte) ((uint) index * (uint) num1)));
    }

    private void GetNextText()
    {
      if (this._textqueue == null)
        this._textqueue = new Queue<NextTextCommand>();
      if (this._textqueue.Count > 0)
      {
        NextTextCommand nextTextCommand = this._textqueue.Dequeue();
        ReadPresetParametersText presetParametersText = new ReadPresetParametersText();
        presetParametersText.Address = nextTextCommand.Address;
        presetParametersText.SubAddress = nextTextCommand.SubAddress;
        presetParametersText.Controller = nextTextCommand.Controller;
        ConnectionManager.Instance.Send((Command) presetParametersText);
      }
      else
      {
        this.Dispose();
        this.GetNextController();
      }
    }

    public void UpdateStatus()
    {
      byte num1 = (byte) 102;
      byte num2 = (byte) ((uint) this.MaxPreset + 102U);
      for (byte index = num1; (int) index <= (int) num2; ++index)
      {
        AddressStatus addressStatus = new AddressStatus();
        addressStatus.Address = index;
        addressStatus.Controller = this._c;
        ConnectionManager.Instance.Send((Command) addressStatus);
      }
    }

    public void GetPresetsAsync()
    {
      this._controller = new Queue<Controller>();
      this._PresentController = new Queue<Controller>();
      foreach (ControllerCommunication controllerCommunication in ConnectionManager.Instance.ActiveConnections)
      {
        this._controller.Enqueue(controllerCommunication.Controller);
        this._PresentController.Enqueue(controllerCommunication.Controller);
      }
      this.GetNextController();
    }

    public void GetPresetsAsync(List<Controller> cList)
    {
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
        this.GetPresetsAsyncExec(this._controller.Dequeue());
      }
      else
      {
        if (this.GetPresets == null)
          return;
        this.GetPresets(this.GetAllPresentPresets());
      }
    }

    private void GetPresetsAsyncExec(Controller c)
    {
      this._c = c;
      this.Init();
      c.ClearPresets();
      ControllerParameters controllerParameters = new ControllerParameters();
      controllerParameters.Controller = this._c;
      ConnectionManager.Instance.Send((Command) controllerParameters);
    }

    private void Dispose()
    {
      ConnectionManager.Instance.CommandReceived -= new ControllerCommunication.CommandEventHandler(this.Instance_CommandReceived);
      this._inited = false;
    }

    public List<PresetObject> GetAllPresentPresets()
    {
      List<PresetObject> list = new List<PresetObject>();
      foreach (Controller controller in this._PresentController)
        list.AddRange((IEnumerable<PresetObject>) controller.Presets);
      return list;
    }

    private void CreatePreset(string name, byte PresetNr, Controller c)
    {
      if (name.Length == 0)
        return;
      byte num1 = (byte) (((int) PresetNr - 1) / 4 + 102);
      byte num2 = (byte) ((int) PresetNr - ((int) PresetNr - 1) / 4 * 4 - 1);
      Preset preset = new Preset();
      preset.Name = name;
      preset.Address = num1;
      preset.Subaddress = num2;
      preset.Controller = c;
      c.AddPreset((PresetObject) preset);
    }
  }
}
