// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Downloaders.EventDownloader
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication;
using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Qbus.Communication.Protocol.Downloaders
{
  public class EventDownloader : IDownloader
  {
    private DateTime LastDate = DateTime.MinValue;
    private ControllerCommunication _comm;
    private long _endPacket;
    private long _currentPacket;
    private Command _lastSend;
    private EventLogs _logSize;
    private DateTime _lastSendDate;
    private byte[] _data;
    private LogHelper _helper;
    private bool _busyProcessing;
    private Timer _timer;
    private int _tries;
    private int _startPacket;
    private int _bank;
    private bool _forced;

    public LogHelper Helper
    {
      get
      {
        return this._helper;
      }
    }

    public byte[] Data
    {
      get
      {
        return this._data;
      }
    }

    public DOWNLOAD_STATUS Status { get; set; }

    public long Current
    {
      get
      {
        return this._currentPacket;
      }
    }

    public long Total
    {
      get
      {
        return this._endPacket;
      }
    }

    public double Progress
    {
      get
      {
        if (this._endPacket == 0L)
          return 0.0;
        else
          return (double) this._currentPacket / (double) this._endPacket;
      }
    }

    public event EventHandler StatusChanged;

    public event EventHandler Progressed;

    public EventDownloader(ControllerCommunication comm)
    {
      this._helper = new LogHelper();
      this._comm = comm;
      this._comm.CommandReceived += new ControllerCommunication.CommandEventHandler(this._comm_CommandReceived);
      this.Status = DOWNLOAD_STATUS.CANCELLED;
      this._timer = new Timer(new TimerCallback(this.TimerCheck), (object) null, 0, 1000);
    }

    public void Dispose()
    {
      this.ResetCurrentDownloader();
      try
      {
        this._timer.Change(-1, -1);
        this._timer.Dispose();
      }
      catch
      {
      }
      this._timer = (Timer) null;
      this._helper = (LogHelper) null;
    }

    private void TimerCheck(object o)
    {
      try
      {
        if (this.Status == DOWNLOAD_STATUS.CANCELLED || this._comm.GetType() == typeof (TcpCommunication) && ((TcpCommunication) this._comm).Status != TCP_STATUS.CONNECTED)
          return;
        if ((this.Status == DOWNLOAD_STATUS.READY || this.Status == DOWNLOAD_STATUS.WAITING) && ConnectionManager.Instance.DownloadEcoLogs)
        {
          if (ConnectionManager.Instance.CurrentDownloader == null)
          {
            ConnectionManager.Instance.CurrentDownloader = this;
            this.ChangeStatus(DOWNLOAD_STATUS.READY);
            this.GetNextPacket();
            return;
          }
          else if (ConnectionManager.Instance.CurrentDownloader != this)
          {
            if (this.Status == DOWNLOAD_STATUS.WAITING)
              return;
            this.ChangeStatus(DOWNLOAD_STATUS.WAITING);
            return;
          }
        }
        if (this._lastSendDate < DateTime.Now.AddSeconds(-2.0) && this._tries < 3)
        {
          this.ResendPacket();
          ++this._tries;
        }
        else
        {
          if (!(this._lastSendDate > DateTime.Now.AddSeconds(-2.0)))
            return;
          this._tries = 0;
        }
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_LOW);
      }
    }

    private void _comm_CommandReceived(object sender, CommandEventArgs e)
    {
      this.NewCommand(e.Command);
    }

    private void NewCommand(Command c)
    {
      try
      {
        if (c.GetType() == typeof (EventLogs))
        {
          this._logSize = (EventLogs) c;
          this._endPacket = (long) this._logSize.EventSectors[this._bank];
          this.GetNextPacket();
        }
        DateTime now1 = DateTime.Now;
        if (c.GetType() != typeof (EventRead))
          return;
        EventRead eventRead = (EventRead) c;
        if (this._lastSend != null && c != null && ((int) c.Instruction1 != (int) this._lastSend.Instruction1 || (int) c.Instruction2 != (int) this._lastSend.Instruction2))
        {
          this.ResendPacket();
        }
        else
        {
          DateTime now2 = DateTime.Now;
          if (((int) eventRead.Instruction1 << 8) + (int) eventRead.Instruction2 == this._startPacket && this.Status == DOWNLOAD_STATUS.READY)
          {
            this.ChangeStatus(DOWNLOAD_STATUS.RECEIVING);
            this._helper = new LogHelper();
            this._helper.Logs.AddRange((IEnumerable<Log>) eventRead.Logs);
            this.GetNextPacket();
            this._lastSendDate = DateTime.MaxValue;
            this._tries = 0;
          }
          else if (this.Status == DOWNLOAD_STATUS.RECEIVING)
          {
            long num = (long) ((eventRead.PointerH << 8) + eventRead.PointerL);
            if (this._endPacket < num)
              this._endPacket = num;
            if (this._helper == null || this._helper.Logs == null)
              this._helper = new LogHelper();
            this._helper.Logs.AddRange((IEnumerable<Log>) eventRead.Logs);
            if (this.Progressed != null)
              this.Progressed((object) this, (EventArgs) null);
            this.GetNextPacket();
          }
          else if (this.Status != DOWNLOAD_STATUS.CANCELLED)
            ConnectionManager.Instance.ErrorHandle(new Exception(string.Concat(new object[4]
            {
              (object) "Unexpected log download status: ",
              (object) ((object) this.Status).ToString(),
              (object) " packet: ",
              (object) this._currentPacket
            })), WARNING_TYPES.ERROR_MID);
          DateTime now3 = DateTime.Now;
          TimeSpan timeSpan1 = now2 - now1;
          TimeSpan timeSpan2 = now3 - now2;
        }
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_MID);
      }
    }

    public void StartDownloadingFile(bool force, int bank)
    {
      this._bank = bank;
      if (ConnectionManager.Instance.BusyProcessingEcoLogs)
        return;
      this.StartDownload(force);
    }

    public void Cancel()
    {
      this.ResetCurrentDownloader();
      this._lastSendDate = DateTime.MaxValue;
      this.ChangeStatus(DOWNLOAD_STATUS.CANCELLED);
    }

    private void SendPacket()
    {
      this._comm.Send(this._lastSend);
      this._lastSendDate = DateTime.Now;
    }

    private void ResendPacket()
    {
      if (this._lastSend == null || this._comm == null)
        return;
      this._comm.Send(this._lastSend);
    }

    public void ChangeStatus(DOWNLOAD_STATUS status)
    {
      this.Status = status;
      if (this.StatusChanged != null)
        this.StatusChanged((object) this, new EventArgs());
      if (status != DOWNLOAD_STATUS.FINISHED)
        return;
      this.ResetCurrentDownloader();
    }

    private void StartDownload(bool force)
    {
      try
      {
        if (this.Status == DOWNLOAD_STATUS.RECEIVING && !force)
          return;
        if (this._helper == null)
          this._helper = new LogHelper();
        this._helper.Logs.Clear();
        this._helper = (LogHelper) null;
        this._logSize = (EventLogs) null;
        this._lastSend = (Command) null;
        this._lastSendDate = DateTime.MaxValue;
        this._forced = force;
        this._startPacket = force ? 0 : ConnectionManager.Instance.GetLastSector(this._comm);
        this._currentPacket = (long) (this._startPacket - 1);
        this._endPacket = this._currentPacket;
        if (ConnectionManager.Instance.CurrentDownloader == null && ConnectionManager.Instance.DownloadEcoLogs)
        {
          ConnectionManager.Instance.CurrentDownloader = this;
          this.ChangeStatus(DOWNLOAD_STATUS.READY);
          this.GetNextPacket();
        }
        else
          this.ChangeStatus(DOWNLOAD_STATUS.READY);
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_MID);
      }
    }

    private void ResetCurrentDownloader()
    {
      if (ConnectionManager.Instance.CurrentDownloader != this)
        return;
      ConnectionManager.Instance.CurrentDownloader = (EventDownloader) null;
    }

    private void GetNextPacket()
    {
      try
      {
        if (this._busyProcessing)
          return;
        if (this._logSize == null)
        {
          EventLogs eventLogs = new EventLogs();
          this._lastSend = (Command) eventLogs;
          this._comm.Send((Command) eventLogs);
          this._lastSendDate = DateTime.Now;
        }
        else
        {
          this._lastSend = (Command) null;
          this._busyProcessing = true;
          if (this.LastDate == DateTime.MinValue && this._helper != null && this._helper.Logs.Count > 0)
            this.LastDate = this._helper.Logs[0].Date;
          if (this.LastDate != DateTime.MinValue && this._helper != null && this._helper.Logs.Count > 0)
          {
            ConnectionManager.Instance.MergeLogs(this._helper, this._comm, (short) this._currentPacket);
            if (this._helper != null && this._helper.Logs != null)
              this._helper.Logs.Clear();
          }
          if (this._endPacket < (long) (this._startPacket - 1))
          {
            this._startPacket = 0;
            this._currentPacket = (long) (this._startPacket - 1);
          }
          ++this._currentPacket;
          if (this._currentPacket > this._endPacket)
          {
            if (this._currentPacket != (long) this._startPacket)
            {
              try
              {
                this._lastSendDate = DateTime.MaxValue;
                this.ChangeStatus(DOWNLOAD_STATUS.RESTORING);
                try
                {
                  ConnectionManager.Instance.ProcessAllGraphs(this._comm, this._forced);
                  this._busyProcessing = false;
                  return;
                }
                catch (Exception ex)
                {
                  ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_MID);
                  this.ResetCurrentDownloader();
                  this.ChangeStatus(DOWNLOAD_STATUS.ERROR);
                  return;
                }
              }
              catch (Exception ex)
              {
                ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_CRITICAL);
                this.ResetCurrentDownloader();
                this.ChangeStatus(DOWNLOAD_STATUS.ERROR);
                return;
              }
            }
          }
          this._busyProcessing = false;
          EventRead eventRead1 = new EventRead();
          eventRead1.Controller = this._comm.Controller;
          EventRead eventRead2 = eventRead1;
          int lastBank = this._comm.Controller.LastBank;
          int num = (int) (byte) this._comm.Controller.LastBank;
          eventRead2.Bank = (byte) num;
          eventRead1.Sector = (int) this._currentPacket;
          this._lastSend = (Command) eventRead1;
          this.SendPacket();
        }
      }
      catch (Exception ex)
      {
        this.ResetCurrentDownloader();
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_MID);
      }
    }
  }
}
