// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.ControllerCommunication
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;
using Qbus.Communication.Protocol.Commands.XPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Qbus.Communication
{
  public abstract class ControllerCommunication
  {
    private List<Command> _errorCommands = new List<Command>();
    private Queue<Command> _queue;
    private Thread _sendThread;
    private Command _lastSent;
    private int _tries;
    private Controller _controller;
    private ControllerManager _manager;
    private bool _paused;
    private bool _queueing;
    private Thread _staThread;

    public object Tag { get; set; }

    public bool Paused
    {
      get
      {
        return this._paused;
      }
      set
      {
        this._paused = value;
      }
    }

    public Controller Controller
    {
      get
      {
        return this._controller;
      }
      set
      {
        if (this._controller != null)
        {
          this._controller.SendCommand -= new EventHandler(this._controller_SendCommand);
          this._controller.SendPresetCommand += new EventHandler(this._controller_SendPresetCommand);
        }
        this._controller = value;
        if (this._controller == null)
          return;
        this._controller.SendCommand += new EventHandler(this._controller_SendCommand);
        this._controller.SendPresetCommand += new EventHandler(this._controller_SendPresetCommand);
      }
    }

    public ControllerManager Manager
    {
      get
      {
        return this._manager;
      }
      set
      {
        this._manager = value;
      }
    }

    public int WaitTime { get; set; }

    public int SendTries { get; set; }

    public virtual bool Connected
    {
      get
      {
        if (this._controller == null)
          return false;
        int num = this._controller.Connected ? 1 : 0;
        return this._controller.Connected;
      }
    }

    public event ControllerCommunication.CommandEventHandler CommandReceived;

    public event ControllerCommunication.CommandEventHandler CommandFailed;

    public event ConnectionManager.ConnectionChangedHandler ConnectionChanged;

    public void UpdateConnectionState()
    {
      if (this.ConnectionChanged == null)
        return;
      this.ConnectionChanged(this);
    }

    private void _controller_SendPresetCommand(object sender, EventArgs e)
    {
      Command command = ((PresetObject) sender).GetCommand();
      command.Controller = this._controller;
      this.Send(command);
    }

    private void _controller_SendCommand(object sender, EventArgs e)
    {
      Command command = ((Module) sender).GetCommand();
      command.Controller = this._controller;
      this.Send(command);
    }

    public void Queue(Command cmd)
    {
      if (!this._controller.Activated)
      {
        Type type = cmd.GetType();
        bool flag = true;
        if (type == typeof (Login) || type == typeof (EventStatus) || (type == typeof (Qbus.Communication.Protocol.Commands.Version) || type == typeof (ControllerOptions)) || type == typeof (ControllerParameters))
          flag = true;
        if (!flag)
          return;
      }
      this._queueing = true;
      int num1 = 0;
      try
      {
        ++num1;
        if (this._queue == null)
          this._queue = new Queue<Command>();
        ++num1;
        if (cmd == null)
          return;
        ++num1;
        List<Command> list = new List<Command>();
        ++num1;
        lock (this._queue)
        {
          ++num1;
          foreach (Command item_0 in Enumerable.AsEnumerable<Command>((IEnumerable<Command>) this._queue))
          {
            if (item_0 != null && item_0.EqualAddress(cmd))
              list.Add(item_0);
          }
          ++num1;
          if (list.Count > 0)
          {
            Queue<Command> local_5 = new Queue<Command>();
            while (this._queue.Count > 0)
            {
              Command local_6 = this._queue.Dequeue();
              if (!list.Contains(local_6))
                local_5.Enqueue(local_6);
            }
            this._queue = local_5;
          }
        }
        ++num1;
        if (cmd == null)
          throw new Exception("Command is null");
        ++num1;
        if (this._queue == null)
          this._queue = new Queue<Command>();
        this._queue.Enqueue(cmd);
        ++num1;
        this.StartSendThread();
        int num2 = num1 + 1;
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(new Exception(string.Concat(new object[4]
        {
          (object) "Error at line: ",
          (object) num1,
          (object) ": ",
          (object) ex.Message
        }), ex), WARNING_TYPES.ERROR_MID);
      }
      this._queueing = false;
    }

    internal void StartSendThread()
    {
      if (this._queue == null || this._queue.Count <= 0)
        return;
      this._staThread = Thread.CurrentThread;
      if (this._sendThread == null)
      {
        this._sendThread = new Thread(new ThreadStart(this.ThreadSend));
        this._sendThread.Priority = ThreadPriority.Lowest;
      }
      if (this._sendThread == null || this._sendThread.ThreadState != ThreadState.Unstarted)
        return;
      this._sendThread.Start();
    }

    private void ThreadSend()
    {
      try
      {
        if (this.WaitTime <= 0)
          this.WaitTime = 10;
        if (this.SendTries <= 0)
          this.SendTries = 3;
        DateTime dateTime = DateTime.MinValue;
        if (!this.Connected)
          return;
        while (true)
        {
          do
          {
            if (this._queue.Count <= 0)
              goto label_32;
label_5:
            if (!this.Paused)
            {
              DateTime now = DateTime.Now;
              if (this._tries == 0 || this._lastSent == null)
              {
                Command cmd = (Command) null;
                lock (this._queue)
                {
                  if (this._queue.Count > 0)
                    cmd = this._queue.Dequeue();
                }
                if (cmd != null)
                {
                  this._lastSent = cmd;
                  dateTime = now;
                  if (!this.Send(cmd, false))
                  {
                    if (!this._errorCommands.Contains(cmd))
                    {
                      this._lastSent = (Command) null;
                      this._queue.Enqueue(cmd);
                      this._errorCommands.Add(cmd);
                    }
                    else
                    {
                      this._errorCommands.Remove(cmd);
                      this._lastSent = (Command) null;
                    }
                  }
                  this._tries = 1;
                }
              }
              else if (dateTime.AddSeconds(0.7) < now)
              {
                if (this._tries > this.SendTries)
                {
                  this._tries = 0;
                  Command cmd = this._lastSent;
                  this._lastSent = (Command) null;
                  if (this.CommandFailed != null)
                    this.CommandFailed((object) this, new CommandEventArgs(cmd));
                }
                else
                {
                  if (this._lastSent != null)
                  {
                    dateTime = now;
                    this.Send(this._lastSent, false);
                  }
                  if (this._tries != 0)
                    ++this._tries;
                }
              }
              else if (!this.Connected)
                Thread.Sleep(500);
              else
                Thread.Sleep(10);
              continue;
            }
            else
              goto label_6;
label_32:
            if (this._lastSent != null)
              goto label_5;
            else
              goto label_35;
          }
          while (this._queue.Count != 0);
          goto label_30;
label_6:
          Thread.Sleep(500);
          continue;
label_30:
          Thread.Sleep(this.WaitTime);
        }
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_LOW);
      }
label_35:
      this._sendThread = (Thread) null;
      if (this._queue == null || this._queue.Count <= 0)
        return;
      this.StartSendThread();
    }

    public void Send(Command cmd)
    {
      this.Queue(cmd);
    }

    public abstract bool Send(Command cmd, bool queue);

    public abstract void Start();

    public abstract void Stop();

    internal void doConnectionChanged()
    {
      if (this.ConnectionChanged == null)
        return;
      this.ConnectionChanged(this);
    }

    public void Receive(Command cmd)
    {
      try
      {
        try
        {
          if (this._lastSent != null)
          {
            Command command = this._lastSent;
            if (cmd != null)
            {
              if (command != null)
              {
                if (cmd.Type == command.Type)
                {
                  if ((int) cmd.Instruction1 == (int) command.Instruction1)
                  {
                    this._tries = 0;
                    this._lastSent = (Command) null;
                  }
                }
              }
            }
          }
        }
        catch
        {
        }
        try
        {
          this.UpdateModules(cmd);
        }
        catch
        {
        }
        new Thread(new ParameterizedThreadStart(this.InvokeCommandReceived)).Start((object) cmd);
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_LOW);
      }
    }

    private void UpdateModules(Command cmd)
    {
      if (cmd == null)
        return;
      int num1 = -1;
      bool flag = true;
      if (cmd.GetType() == typeof (AddressStatus))
      {
        AddressStatus addressStatus = (AddressStatus) cmd;
        num1 = (int) addressStatus.Address;
        int num2 = (int) addressStatus.SubAddress;
      }
      else if (cmd.GetType() == typeof (ParametersAdressen))
        num1 = (int) ((ParametersAdressen) cmd).Address;
      else if (cmd.GetType() == typeof (EventStatus))
        num1 = (int) ((EventStatus) cmd).Address;
      else
        flag = false;
      if (!flag)
        return;
      if (cmd.Type == 56)
      {
        if (cmd.Write)
          return;
      }
      try
      {
        List<Module> modules = this._controller.Modules;
        for (int index = 0; index < modules.Count; ++index)
        {
          Module module = modules[index];
          if ((int) module.Address == num1 || cmd.Type == 56 && (int) module.Address - 128 == num1)
            module.UpdateStatus(cmd);
        }
      }
      catch (InvalidOperationException ex)
      {
        ConnectionManager.Instance.ErrorHandle((Exception) ex, WARNING_TYPES.ERROR_LOW);
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_MID);
      }
    }

    public void InvokeCommandReceived(object o)
    {
      Command cmd = (Command) o;
      if (!this._controller.Activated)
      {
        Type type = cmd.GetType();
        bool flag = false;
        if (type == typeof (Login) || type == typeof (EventStatus) || (type == typeof (Qbus.Communication.Protocol.Commands.Version) || type == typeof (ControllerOptions)) || type == typeof (Qbus.Communication.Protocol.Commands.XPort.Error))
          flag = true;
        if (!flag)
          return;
      }
      if (this.CommandReceived == null)
        return;
      this.CommandReceived((object) this, new CommandEventArgs(cmd));
    }

    public delegate void CommandEventHandler(object sender, CommandEventArgs e);

    private delegate void CommandReceivedDelegate(Command cmd);
  }
}
