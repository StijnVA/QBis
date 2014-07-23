// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.ConnectionManager
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;
using Qbus.Communication.Protocol.Downloaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Qbus.Communication
{
  public class ConnectionManager
  {
    private bool _useControllerManager = true;
    private Queue<Controller> _toConnect = new Queue<Controller>();
    private static ConnectionManager _instance;
    private bool _downloadEcoLogs;
    private bool _downloadQDB;
    private List<ControllerCommunication> _activeConnections;
    private Thread _connectionThread;
    private ModuleManager _mm;
    private PresetManager _pm;

    public static ConnectionManager Instance
    {
      get
      {
        if (ConnectionManager._instance == null)
          ConnectionManager._instance = new ConnectionManager();
        return ConnectionManager._instance;
      }
    }

    public bool DisableSecurity
    {
      get
      {
        return false;
      }
    }

    public bool DisableEcoPart
    {
      get
      {
        return false;
      }
    }

    public bool DownloadEcoLogs
    {
      get
      {
        return this._downloadEcoLogs;
      }
      set
      {
        this._downloadEcoLogs = value;
      }
    }

    internal bool DownloadQDB
    {
      get
      {
        return this._downloadQDB;
      }
    }

    internal bool UseControllerManager
    {
      get
      {
        return this._useControllerManager;
      }
    }

    public bool BusyProcessingEcoLogs { get; set; }

    public string TempFolder
    {
      get
      {
        if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "HomeCenter\\Temp\\"))
          Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "HomeCenter\\Temp\\");
        return AppDomain.CurrentDomain.BaseDirectory + "HomeCenter\\Temp\\";
      }
    }

    internal EventDownloader CurrentDownloader { get; set; }

    public List<ControllerCommunication> ActiveConnections
    {
      get
      {
        return this._activeConnections;
      }
      set
      {
        this._activeConnections = value;
      }
    }

    public List<Controller> FoundControllers
    {
      get
      {
        return ConnectionHelper.Instance.FoundControllers;
      }
    }

    public List<Controller> AllControllers
    {
      get
      {
        List<Controller> list = new List<Controller>();
        foreach (Controller controller1 in ConnectionHelper.Instance.FoundControllers)
        {
          bool flag = false;
          foreach (Controller controller2 in list)
          {
            if (controller2.CompareTo((object) controller1) == 0)
              flag = true;
          }
          if (!flag)
            list.Add(controller1);
        }
        return list;
      }
    }

    public ModuleManager Modules
    {
      get
      {
        return this._mm;
      }
    }

    public PresetManager Presets
    {
      get
      {
        return this._pm;
      }
    }

    public event ConnectionManager.ErrorEventHandler onError;

    public event ConnectionManager.SendRecEventHandler onSendRec;

    public event ControllerCommunication.CommandEventHandler CommandReceived;

    public event ConnectionManager.ConnectionChangedHandler ConnectionChanged;

    public event ConnectionManager.MergeLogHandler onMergeLogs;

    public event ConnectionManager.ProcessingGraphHandler StartProcessingGraphs;

    public event ConnectionManager.GetLastEventSectorHandler GetLastEventSector;

    private ConnectionManager()
    {
      this._activeConnections = new List<ControllerCommunication>();
      this._mm = new ModuleManager();
      this._pm = new PresetManager();
    }

    public void ErrorHandle(Exception ex, WARNING_TYPES level)
    {
      try
      {
        if (this.onError == null)
          return;
        this.onError((object) this, new ConnectionManager.ErrorEventArgs(ex, level));
      }
      catch (Exception ex1)
      {
        string message = ex1.Message;
      }
    }

    public void SendRecHandle(Exception ex, WARNING_TYPES level)
    {
      try
      {
        if (this.onSendRec == null)
          return;
        this.onSendRec((object) this, new ConnectionManager.SendRecEventArgs(ex, level));
      }
      catch (Exception ex1)
      {
        string message = ex1.Message;
      }
    }

    internal int GetLastSector(ControllerCommunication comm)
    {
      int num = 0;
      if (this.GetLastEventSector != null)
        num = this.GetLastEventSector(comm);
      return num;
    }

    internal void MergeLogs(LogHelper logs, ControllerCommunication comm, short packet)
    {
      if (this.onMergeLogs == null)
        return;
      this.onMergeLogs((object) this, new ConnectionManager.MergeLogArgs(logs, comm, packet));
    }

    internal void ProcessAllGraphs(ControllerCommunication cc, bool forced)
    {
      if (this.StartProcessingGraphs == null)
        return;
      this.StartProcessingGraphs(cc, forced);
    }

    public void Disconnect(Controller c)
    {
      ControllerCommunication controllerCommunication1 = (ControllerCommunication) null;
      foreach (ControllerCommunication controllerCommunication2 in this._activeConnections)
      {
        if (controllerCommunication2.Controller.CompareTo((object) c) == 0)
          controllerCommunication1 = controllerCommunication2;
      }
      if (controllerCommunication1 == null)
        return;
      this._activeConnections.Remove(controllerCommunication1);
      controllerCommunication1.Stop();
      controllerCommunication1.Controller.Connected = false;
    }

    public void DisconnectAll()
    {
      foreach (ControllerCommunication controllerCommunication in this._activeConnections)
      {
        controllerCommunication.Stop();
        controllerCommunication.Controller.Connected = false;
      }
      this._activeConnections.Clear();
    }

    public void Connect(Controller c)
    {
      this._toConnect.Enqueue(c);
      if (this._connectionThread != null)
        return;
      this._connectionThread = new Thread(new ThreadStart(this.ConnectThreaded));
      this._connectionThread.Start();
    }

    private void ConnectThreaded()
    {
      while (this._toConnect.Count > 0)
      {
        Controller c = this._toConnect.Dequeue();
        try
        {
          int tcpPort1 = c.TcpPort;
          if ((c.TcpPort == 0 || c.Address == null || c.Address == "") && (c.ComPort != null && c.ComPort != ""))
          {
            this.ConnectUSB(c);
          }
          else
          {
            int tcpPort2 = c.TcpPort;
            if (c.Address != null)
            {
              if (!(c.ComPort == ""))
              {
                if (c.ComPort != null)
                  goto label_10;
              }
              if (c.Address != "")
                this.ConnectTCP(c);
            }
          }
        }
        catch (Exception ex)
        {
          ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_MID);
        }
label_10:
        if (this.ConnectionChanged != null)
          this.ConnectionChanged((ControllerCommunication) null);
      }
      this._connectionThread = (Thread) null;
    }

    public SerialCommunication ConnectUSB(Controller c)
    {
      this.Find(c);
      SerialCommunication serialCommunication = new SerialCommunication(c);
      serialCommunication.WaitTime = 50;
      serialCommunication.CommandReceived += new ControllerCommunication.CommandEventHandler(this.comm_CommandReceived);
      serialCommunication.ConnectionChanged += new ConnectionManager.ConnectionChangedHandler(this.comm_ConnectionChanged);
      this._activeConnections.Add((ControllerCommunication) serialCommunication);
      serialCommunication.Start();
      EventStatus eventStatus = new EventStatus();
      ((ControllerCommunication) serialCommunication).Send((Command) eventStatus);
      c.Connected = true;
      if (c.Connected)
        c.AutoConnect = true;
      return serialCommunication;
    }

    private void comm_ConnectionChanged(ControllerCommunication sender)
    {
      if (this.ConnectionChanged == null)
        return;
      this.ConnectionChanged(sender);
    }

    public TcpCommunication ConnectTCP(Controller c)
    {
      ControllerCommunication controllerCommunication = this.Find(c);
      if (controllerCommunication == null || controllerCommunication.GetType() != typeof (TcpCommunication))
      {
        controllerCommunication = (ControllerCommunication) new TcpCommunication(c);
        controllerCommunication.CommandReceived += new ControllerCommunication.CommandEventHandler(this.comm_CommandReceived);
        controllerCommunication.ConnectionChanged += new ConnectionManager.ConnectionChangedHandler(this.comm_ConnectionChanged);
        this._activeConnections.Add(controllerCommunication);
      }
      else
        controllerCommunication.Controller = c;
      controllerCommunication.Start();
      c.Connected = true;
      return (TcpCommunication) controllerCommunication;
    }

    public ControllerCommunication Find(Controller c)
    {
      ControllerCommunication controllerCommunication1 = (ControllerCommunication) null;
      if (c == null)
        return (ControllerCommunication) null;
      if (this._activeConnections == null)
      {
        this._activeConnections = new List<ControllerCommunication>();
        return (ControllerCommunication) null;
      }
      else
      {
        foreach (ControllerCommunication controllerCommunication2 in this._activeConnections)
        {
          if (controllerCommunication2.Controller.CompareTo((object) c) == 0)
            controllerCommunication1 = controllerCommunication2;
        }
        return controllerCommunication1;
      }
    }

    public void Send(Command cmd)
    {
      if (cmd == null)
        return;
      foreach (ControllerCommunication controllerCommunication in this._activeConnections)
      {
        if (controllerCommunication.Controller == cmd.Controller || cmd.Controller == null)
          controllerCommunication.Send(cmd);
      }
    }

    public void Stop()
    {
      foreach (ControllerCommunication controllerCommunication in this._activeConnections)
        controllerCommunication.Stop();
      this._activeConnections.Clear();
    }

    private void comm_CommandReceived(object sender, CommandEventArgs e)
    {
      if (this.CommandReceived == null)
        return;
      this.CommandReceived(sender, e);
    }

    public delegate void ErrorEventHandler(object sender, ConnectionManager.ErrorEventArgs args);

    public class ErrorEventArgs
    {
      public Exception Exception { get; set; }

      public WARNING_TYPES Level { get; set; }

      public ErrorEventArgs(Exception ex, WARNING_TYPES level)
      {
        this.Exception = ex;
        this.Level = level;
      }
    }

    public delegate void SendRecEventHandler(object sender, ConnectionManager.SendRecEventArgs args);

    public class SendRecEventArgs
    {
      public Exception Exception { get; set; }

      public WARNING_TYPES Level { get; set; }

      public SendRecEventArgs(Exception ex, WARNING_TYPES level)
      {
        this.Exception = ex;
        this.Level = level;
      }
    }

    public delegate void ConnectionChangedHandler(ControllerCommunication cc);

    public delegate void MergeLogHandler(object sender, ConnectionManager.MergeLogArgs args);

    public class MergeLogArgs
    {
      public LogHelper Logs;
      public ControllerCommunication Communication;
      public short Packet;

      public MergeLogArgs(LogHelper logs, ControllerCommunication comm, short packet)
      {
        this.Logs = logs;
        this.Communication = comm;
        this.Packet = packet;
      }
    }

    public delegate void ProcessingGraphHandler(ControllerCommunication cc, bool forced);

    public delegate int GetLastEventSectorHandler(ControllerCommunication cc);
  }
}
