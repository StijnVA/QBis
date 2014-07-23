// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.ControllerManager
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;
using Qbus.Communication.Protocol.Downloaders;
using Qbus.Communication.SDK;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Qbus.Communication
{
  public class ControllerManager
  {
    private BasicHttpBinding binding = new BasicHttpBinding();
    private EndpointAddress url = new EndpointAddress("http://qbus.aaltra.eu/sdkservice.asmx");
    private ControllerCommunication _comm;
    private bool _connected;
    private EventDownloader _event;
    private TextDownloader _text;
    private bool _initing;
    private int _state;
    private int _bank;

    public DOWNLOAD_STATUS ControllerStatus { get; set; }

    public int State
    {
      get
      {
        return this._state;
      }
    }

    public IDownloader Events
    {
      get
      {
        return (IDownloader) this._event;
      }
    }

    public event EventHandler StatusChanged;

    public event EventHandler LoginFailed;

    public event EventHandler NotActivated;

    public event ControllerManager.ActivationHandler ActivationFinished;

    public event ControllerManager.BankChangedHandler BankChanged;

    public ControllerManager(ControllerCommunication comm)
    {
      this._comm = comm;
      this._comm.CommandReceived += new ControllerCommunication.CommandEventHandler(this._comm_CommandReceived);
      this._comm.ConnectionChanged += new ConnectionManager.ConnectionChangedHandler(this._comm_ConnectionChanged);
      this._text = new TextDownloader(comm);
      this._text.StatusChanged += new EventHandler(this._text_StatusChanged);
      this._event = new EventDownloader(comm);
      this._event.Progressed += new EventHandler(this._event_Progressed);
      this._event.StatusChanged += new EventHandler(this._event_StatusChanged);
    }

    public void _database_DatabaseFinished(ControllerCommunication cc, string filepath, bool newversion)
    {
    }

    private void _comm_CommandReceived(object sender, CommandEventArgs e)
    {
      try
      {
        this.CheckActivation(e);
        this.CheckVersion(e);
        this.CheckFat(e);
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_CRITICAL);
      }
    }

    private void _comm_ConnectionChanged(ControllerCommunication cc)
    {
      try
      {
        ConnectionManager.Instance.ErrorHandle(new Exception(string.Concat(new object[4]
        {
          (object) "Connection state changed to:  ",
		  (object) (bool) (this._comm.Connected),
          (object) " initing: ",
          (object) (bool) (this._initing)
        })), WARNING_TYPES.INFO);
        if (this._comm.Connected && !this._initing && (this._comm.GetType() == typeof (TcpCommunication) && ((TcpCommunication) this._comm).Status == TCP_STATUS.CONNECTED || this._comm.GetType() != typeof (TcpCommunication)))
        {
          ConnectionManager.Instance.ErrorHandle(new Exception("Check controller"), WARNING_TYPES.INFO);
          this._initing = true;
          this.CheckController();
        }
        if (this._comm.GetType() == typeof (TcpCommunication) && ((TcpCommunication) this._comm).Status == TCP_STATUS.LOGIN_FAILED && this.LoginFailed != null)
          this.LoginFailed((object) this, (EventArgs) null);
        if (!this._comm.Connected)
        {
          this._initing = false;
          if (this._event == ConnectionManager.Instance.CurrentDownloader)
          {
            ConnectionManager.Instance.CurrentDownloader = (EventDownloader) null;
            this._event.Cancel();
          }
        }
        this._connected = this._comm.Connected;
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_CRITICAL);
      }
    }

    public void ResetState()
    {
      this._state = 0;
      this._initing = false;
      if (this._text == null)
        return;
      this._text.ResetTextDownloader();
    }

    public void CheckControllerStatus()
    {
    }

    public void CheckController()
    {
      try
      {
        this._state = 0;
        this.GetVersion();
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_CRITICAL);
      }
    }

    private void GetVersion()
    {
      try
      {
        this._state = 0;
        this._comm.Send((Command) new Qbus.Communication.Protocol.Commands.Version());
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_CRITICAL);
      }
    }

    private void CheckFat()
    {
      try
      {
        this._state = 1;
        this._comm.Send((Command) new FatData());
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_CRITICAL);
      }
    }

    private void CheckActivation()
    {
      try
      {
        this._state = 1;
        if (!ConnectionManager.Instance.DisableSecurity)
          this._comm.Send((Command) new ControllerOptions());
        else
          this.CheckFat();
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_CRITICAL);
      }
    }

    private void CheckActivation(CommandEventArgs e)
    {
      try
      {
        if (e.Command.GetType() != typeof (ControllerOptions))
          return;
        ControllerOptions controllerOptions = (ControllerOptions) e.Command;
        if (!controllerOptions.Write)
        {
          e.Command.Controller.Activated = controllerOptions.EQOmmand || controllerOptions.SDK;
          if (e.Command.Controller.Activated)
          {
            this.ControllerStatus = DOWNLOAD_STATUS.FINISHED;
            if (this.StatusChanged != null)
              this.StatusChanged((object) this, (EventArgs) null);
            this.CheckFat();
          }
          else
          {
            if (this.NotActivated != null)
              this.NotActivated((object) this, (EventArgs) null);
            this.ControllerStatus = DOWNLOAD_STATUS.ERROR;
            if (this.StatusChanged == null)
              return;
            this.StatusChanged((object) this, (EventArgs) null);
          }
        }
        else
          this._comm.Send((Command) new ControllerOptions());
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_CRITICAL);
      }
    }

    private void CheckFat(CommandEventArgs e)
    {
      try
      {
        Command command = e.Command;
        if (command.GetType() != typeof (FatData) || this._comm.Controller == null)
          return;
        int newBank = (int) ((FatData) command).CurrentBank;
        this._bank = newBank;
        if (newBank != this._comm.Controller.LastBank)
        {
          int lastBank = this._comm.Controller.LastBank;
          if (this.BankChanged != null)
            this.BankChanged((object) this, new ControllerManager.BankChangedArgs(this._comm.Controller.LastBank, newBank, this._comm.Controller));
        }
        this._comm.Controller.LastBank = newBank;
        this.GetTexts();
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_CRITICAL);
      }
    }

    private void CheckVersion(CommandEventArgs e)
    {
      try
      {
        Command command = e.Command;
        if (command.GetType() != typeof (Qbus.Communication.Protocol.Commands.Version) || this._comm.Controller == null)
          return;
        if (this._comm.Controller.Firmware != ((Qbus.Communication.Protocol.Commands.Version) command).VersionNumber)
          this._comm.Controller.Firmware = ((Qbus.Communication.Protocol.Commands.Version) command).VersionNumber;
        if (this._comm.Controller.MAC == null || !ByteArrayUtils.Compare(this._comm.Controller.MAC, ((Qbus.Communication.Protocol.Commands.Version) command).Serial, 0))
          this._comm.Controller.MAC = ((Qbus.Communication.Protocol.Commands.Version) command).Serial;
        this._comm.UpdateConnectionState();
        this.CheckActivation();
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_CRITICAL);
      }
    }

    private void GetTexts()
    {
      try
      {
        if (this._text != null)
        {
          this._state = 2;
          this._text.Start();
        }
        else
          this.ContinueAfterTexts();
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_CRITICAL);
      }
    }

    public void ForceTextDownload()
    {
      try
      {
        if (this._text != null)
          this._text.Start();
        else
          this.ContinueAfterTexts();
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_CRITICAL);
      }
    }

    private void SendActivationString(string ac)
    {
      ControllerOptions controllerOptions = new ControllerOptions();
      controllerOptions.ActivateString = ac;
      controllerOptions.Write = true;
      this._comm.Send((Command) controllerOptions);
    }

    public void ActivateController(string SN, string name, string email, string installer)
    {
      try
      {
        SDKServiceSoapClient serviceSoapClient = new SDKServiceSoapClient((Binding) this.binding, this.url);
        serviceSoapClient.GetActivationStringCompleted += new EventHandler<GetActivationStringCompletedEventArgs>(this.client_GetActivationStringCompleted);
        serviceSoapClient.GetActivationStringAsync(this._comm.Controller.SN, SN, name, email, installer);
      }
      catch (Exception ex)
      {
        if (this.ActivationFinished == null)
          return;
        this.ActivationFinished(this, ControllerManager.ACTIVATION_RESULT.UNKNOWN_ERROR);
      }
    }

    private void client_GetActivationStringCompleted(object sender, GetActivationStringCompletedEventArgs e)
    {
      if (e.Error != null)
      {
        if (this.ActivationFinished == null)
          return;
        this.ActivationFinished(this, ControllerManager.ACTIVATION_RESULT.CONNECTION_ERROR);
      }
      else if (e.Cancelled)
      {
        if (this.ActivationFinished == null)
          return;
        this.ActivationFinished(this, ControllerManager.ACTIVATION_RESULT.CONNECTION_ERROR);
      }
      else
      {
        if (e.Result == "")
        {
          if (this.ActivationFinished != null)
            this.ActivationFinished(this, ControllerManager.ACTIVATION_RESULT.BAD_SN);
        }
        try
        {
          this.SendActivationString(e.Result);
          if (this.ActivationFinished == null)
            return;
          this.ActivationFinished(this, ControllerManager.ACTIVATION_RESULT.SUCCESS);
        }
        catch
        {
          if (this.ActivationFinished == null)
            return;
          this.ActivationFinished(this, ControllerManager.ACTIVATION_RESULT.UNKNOWN_ERROR);
        }
      }
    }

    private void _text_StatusChanged(object sender, EventArgs e)
    {
      if (this._text.Status != DOWNLOAD_STATUS.FINISHED && this._text.Status != DOWNLOAD_STATUS.ERROR)
        return;
      this.ContinueAfterTexts();
    }

    private void ContinueAfterTexts()
    {
      this._state = 3;
      this.CheckLogs();
    }

    public void ForceLogDownload(bool force)
    {
      try
      {
        if (!ConnectionManager.Instance.DisableEcoPart)
          this._event.StartDownloadingFile(force, this._bank);
        else
          this._initing = false;
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_CRITICAL);
      }
    }

    private void CheckLogs()
    {
      try
      {
        if (this._state != 3)
          return;
        this._state = 4;
        if (!ConnectionManager.Instance.DisableEcoPart)
          this._event.StartDownloadingFile(false, this._bank);
        else
          this._initing = false;
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_CRITICAL);
      }
    }

    private void _event_StatusChanged(object sender, EventArgs e)
    {
      if (this.StatusChanged == null)
        return;
      this.StatusChanged((object) this, new EventArgs());
    }

    private void _event_Progressed(object sender, EventArgs e)
    {
      if (this.StatusChanged == null)
        return;
      this.StatusChanged((object) this, new EventArgs());
    }

    public void Dispose()
    {
      try
      {
        this._comm.CommandReceived -= new ControllerCommunication.CommandEventHandler(this._comm_CommandReceived);
        this._comm.ConnectionChanged -= new ConnectionManager.ConnectionChangedHandler(this._comm_ConnectionChanged);
        this._text.StatusChanged -= new EventHandler(this._text_StatusChanged);
        this._event.Progressed -= new EventHandler(this._event_Progressed);
        this._event.StatusChanged -= new EventHandler(this._event_StatusChanged);
      }
      catch
      {
      }
      this._initing = false;
      this._connected = false;
      try
      {
        if (this._event != null)
          this._event.Dispose();
      }
      catch
      {
      }
      this._text = (TextDownloader) null;
      this._event = (EventDownloader) null;
    }

    public enum ACTIVATION_RESULT
    {
      SUCCESS,
      CONNECTION_ERROR,
      BAD_SN,
      UNKNOWN_ERROR,
    }

    public delegate void ActivationHandler(ControllerManager manager, ControllerManager.ACTIVATION_RESULT result);

    public delegate void BankChangedHandler(object sender, ControllerManager.BankChangedArgs args);

    public class BankChangedArgs
    {
      public int OldBank { get; set; }

      public int NewBank { get; set; }

      public Controller Controller { get; set; }

      public BankChangedArgs(int oldBank, int newBank, Controller ctl)
      {
        this.OldBank = oldBank;
        this.NewBank = newBank;
        this.Controller = ctl;
      }
    }
  }
}
