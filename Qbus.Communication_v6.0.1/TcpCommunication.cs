// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.TcpCommunication
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;
using Qbus.Communication.Protocol.Commands.XPort;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Qbus.Communication
{
  public class TcpCommunication : ControllerCommunication, IDisposable
  {
    private long _lastHeartbeat = -1L;
    private DateTime _lastLogCheck = DateTime.Now;
    private DateTime _lastHeartbeatDate = DateTime.Now;
    private int _lastControllerState = -1;
    private DateTime _lastRetry = DateTime.Now;
    private byte[] key = new byte[0];
    private byte[] IV = new byte[0];
    private DateTime _lastEventReceived = DateTime.Now;
    private string _receive;
    private string _allComm;
    private Socket _socket;
    private Encoding _encoding;
    private Thread _thread;
    private byte[] _prefix;
    private TcpClient _client;
    private Timer _tim;
    private TimeSpan _delay;
    private TCP_STATUS _status;
    private bool _checking;
    private bool _isLogedIn;
    private bool lastStateConnected;
    private bool _connecting;
    private bool _encrypted;
    private Error _lastError;

    public TCP_STATUS Status
    {
      get
      {
        return this._status;
      }
    }

    public DateTime LastHeartbeat
    {
      get
      {
        return this._lastHeartbeatDate;
      }
    }

    public override bool Connected
    {
      get
      {
        return this._socket != null && this._socket.Connected;
      }
    }

    public Error LastError
    {
      get
      {
        return this._lastError;
      }
    }

    public TimeSpan Delay
    {
      get
      {
        return this._delay;
      }
    }

    public TcpCommunication(Controller controller)
    {
      this._encoding = Encoding.GetEncoding(1252);
      this._tim = new Timer(new TimerCallback(this.CheckConnection), (object) null, 20000, 20000);
      this._prefix = new byte[9]
      {
        (byte) 81,
        (byte) 66,
        (byte) 85,
        (byte) 83,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      };
      this._status = TCP_STATUS.DISCONNECTED;
      this._receive = "";
      this._allComm = "";
      this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      this.Controller = controller;
    }

    private void CheckConnection(object o)
    {
      if (this.Controller == null || this._checking)
        return;
      this._checking = true;
      try
      {
        if (this._lastHeartbeat != -1L && this._lastHeartbeat > 0L && !this.Paused && new DateTime(this._lastHeartbeat).AddMinutes(5.0) < DateTime.Now)
          this.Reconnect();
        if (this._lastHeartbeat == -1L)
        {
          this._lastHeartbeatDate = DateTime.Now;
          this._lastHeartbeat = this._lastHeartbeatDate.Ticks;
        }
        if (this._socket == null || this._socket.Connected)
          this.Start();
        else
          this._lastRetry = DateTime.Now;
        this._socket.Send(BitConverter.GetBytes(this._lastHeartbeat));
        try
        {
          if (this._lastEventReceived.AddMinutes(5.0) < DateTime.Now)
          {
            if (!this.Paused)
              this.Send((Command) new EventStatus(), false);
          }
        }
        catch (Exception ex)
        {
        }
        try
        {
          if (this._lastEventReceived.AddMinutes(20.0) < DateTime.Now)
          {
            if (this._socket.Connected)
            {
              this._lastEventReceived = DateTime.Now;
              this.ResetXport();
              this.Reconnect();
            }
          }
        }
        catch (Exception ex)
        {
        }
        if (this.Manager != null && this._lastControllerState == this.Manager.State && (this._lastControllerState < 3 && !this.Paused))
          this.Reconnect();
        if (this.Manager != null)
          this._lastControllerState = this.Manager.State;
        DateTime now = DateTime.Now;
        try
        {
          if (this._lastLogCheck.AddMinutes(15.0) < now)
          {
            this._lastLogCheck = DateTime.Now;
            if (this.Manager != null)
              this.Manager.ForceLogDownload(false);
          }
        }
        catch (Exception ex)
        {
          ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_MID);
        }
      }
      catch (Exception ex1)
      {
        try
        {
          this._status = TCP_STATUS.FAILED;
          if (this.Controller.AutoConnect)
          {
            this.ResetConnection();
            this.Start();
          }
          else
            this.ResetConnection();
        }
        catch (Exception ex2)
        {
        }
        this.doConnectionChanged();
      }
      this._checking = false;
    }

    private void Reconnect()
    {
      this.ResetConnection();
      this.Start();
      Thread.Sleep(5000);
    }

    public List<Command> GetAllCommands()
    {
      return ProtocolParser.Parse(ref this._allComm, this._encoding, this.Controller);
    }

    public void ResetXport()
    {
      new UdpClient(this.Controller.Address, 8450).Send(new byte[5]
      {
        (byte) 114,
        (byte) 101,
        (byte) 115,
        (byte) 101,
        (byte) 116
      }, 5);
      Thread.Sleep(5000);
    }

    public override void Start()
    {
      if (this.Manager == null && ConnectionManager.Instance.UseControllerManager)
        this.Manager = new ControllerManager((ControllerCommunication) this);
      if (this._socket != null && this._socket.Connected || this._connecting)
        return;
      this._connecting = true;
      try
      {
        if (this._thread != null)
          this.ResetConnection();
        if (this._status != TCP_STATUS.FAILED)
          this._status = TCP_STATUS.DISCONNECTED;
        if (this.Controller.Address != null)
        {
          if (this.Controller.Address != "")
          {
            int tcpPort = this.Controller.TcpPort;
            if (this.Controller.TcpPort != 0)
            {
              if (this._socket != null)
              {
                try
                {
                  this._socket.Disconnect(false);
                }
                catch
                {
                }
                this._socket = (Socket) null;
              }
              this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
              bool flag = false;
              if (this._tim == null)
                flag = true;
              this._socket.Connect(this.Controller.Address, this.Controller.TcpPort);
              if (this._socket.Connected)
              {
                this.lastStateConnected = true;
                this.doConnectionChanged();
                this.StartSendThread();
                this._thread = new Thread(new ThreadStart(this.Receive));
                this._thread.Start();
                if (!flag)
                  ConnectionManager.Instance.ErrorHandle(new Exception("Connection to " + this.Controller.ToString() + " made"), WARNING_TYPES.CONNECTION_GOOD);
              }
              else
              {
                this._status = TCP_STATUS.DISCONNECTED;
                ConnectionManager.Instance.ErrorHandle(new Exception("Connection to " + this.Controller.ToString() + " failed"), WARNING_TYPES.CONNECTION_LOST);
                this.doConnectionChanged();
              }
            }
          }
        }
      }
      catch (Exception ex1)
      {
        try
        {
          if (this._socket == null || this.lastStateConnected != this._socket.Connected || (this.lastStateConnected || this._tim == null))
          {
            this.lastStateConnected = false;
            ConnectionManager.Instance.ErrorHandle(new Exception("Connection to " + this.Controller.ToString() + " failed"), WARNING_TYPES.CONNECTION_LOST);
          }
          this.doConnectionChanged();
        }
        catch (Exception ex2)
        {
          ConnectionManager.Instance.ErrorHandle(new Exception("Connection to controller " + this.Controller.Address + " failed: " + ex1.Message, ex1), WARNING_TYPES.ERROR_HIGH);
        }
      }
      this._connecting = false;
    }

    private void ResetConnection()
    {
      try
      {
        this._socket.Close();
        Thread.Sleep(100);
      }
      catch
      {
      }
      try
      {
        this._thread.Abort();
      }
      catch
      {
      }
      this._lastHeartbeat = -1L;
      this._lastControllerState = -1;
      this.IV = new byte[0];
      this.key = new byte[0];
      this._encrypted = false;
      this.Paused = false;
      this._connecting = false;
      this._isLogedIn = false;
      this._socket = (Socket) null;
      this._status = TCP_STATUS.DISCONNECTED;
      if (this.Manager == null)
        return;
      this.Manager.ResetState();
      this._lastControllerState = this.Manager.State;
    }

    private void SendLogin()
    {
      this.Send((Command) new Login()
      {
        User = this.Controller.Login,
        Password = this.Controller.Password
      }, false);
    }

    private void Receive()
    {
      byte[] buffer1 = new byte[4095];
      while (this._socket != null && this._socket.Connected)
      {
        if (this._status == TCP_STATUS.WRONG_XPORT_VERSION)
          this.Stop();
        int length;
        try
        {
          length = this._socket.Receive(buffer1, 1024, SocketFlags.None);
        }
        catch (Exception ex)
        {
          break;
        }
        if (!this._socket.Connected)
          break;
        if (length == 0)
        {
          try
          {
            this._socket.Disconnect(false);
            break;
          }
          catch
          {
            break;
          }
        }
        else
        {
          byte[] buffer2 = new byte[length];
          for (int index = 0; index < length; ++index)
            buffer2[index] = buffer1[index];
          if (this._encrypted)
            buffer2 = CommunicationEncryption.Instance.Decrypt(buffer2, this.key, this.IV);
          if (buffer2.Length > 9)
          {
            if ((int) buffer2[9] == 250)
            {
              byte[] numArray = new byte[buffer2.Length - 12];
              for (int index = 0; index < numArray.Length; ++index)
                numArray[index] = buffer2[index + 12];
              ProtocolSender.Instance.SendEvent(new ReceivedData(this.Controller, (byte) 250, (byte) 0, buffer2));
            }
            else if ((int) buffer2[9] == (int) byte.MaxValue)
            {
              byte[] numArray = new byte[buffer2.Length - 12];
              for (int index = 0; index < numArray.Length; ++index)
                numArray[index] = buffer2[index + 12];
              ProtocolSender.Instance.SendEvent(new ReceivedData(this.Controller, byte.MaxValue, (byte) 0, buffer2));
            }
          }
          List<Command> list1 = ProtocolParser.ParseTCP(ref buffer2, this._prefix, this.Controller);
          int num = 0;
          while (buffer2.Length != 0 && buffer2.Length != num)
          {
            for (int index = length - buffer2.Length; index < length; ++index)
              buffer2[index - (length - buffer2.Length)] = buffer1[index];
            num = buffer2.Length;
            if (this._encrypted)
              buffer2 = CommunicationEncryption.Instance.Decrypt(buffer2, this.key, this.IV);
            List<Command> list2 = ProtocolParser.ParseTCP(ref buffer2, this._prefix, this.Controller);
            if (list2.Count != 0)
              list1.AddRange((IEnumerable<Command>) list2);
          }
          if (buffer2.Length != 0)
          {
            for (int index = length - buffer2.Length; index < length; ++index)
              buffer2[index - (length - buffer2.Length)] = buffer1[index];
            Echo echo = new Echo();
            echo.Data = buffer2;
            list1.Add((Command) echo);
          }
          foreach (Command cmd in list1)
          {
            if (cmd.GetType() != typeof (Echo))
            {
              cmd.Controller = this.Controller;
              if (cmd.GetType() == typeof (Login))
              {
                if (!((Login) cmd).Success)
                {
                  this._status = TCP_STATUS.LOGIN_FAILED;
                  this._socket.Close();
                  try
                  {
                    this._tim.Dispose();
                  }
                  catch
                  {
                  }
                  this._tim = (Timer) null;
                  this.doConnectionChanged();
                }
              }
              else if (cmd.GetType() == typeof (EventStatus))
                this._lastEventReceived = DateTime.Now;
              else if (cmd.GetType() == typeof (Error))
              {
                this._lastEventReceived = DateTime.Now;
                this._lastHeartbeat = -1L;
                this._lastError = (Error) cmd;
                if (((Error) cmd).ErrorNumber == ERROR_TYPES.LOGIN_INCORRECT)
                {
                  this._status = TCP_STATUS.LOGIN_FAILED;
                  this._socket.Close();
                  try
                  {
                    this._tim.Dispose();
                  }
                  catch
                  {
                  }
                  this._tim = (Timer) null;
                  this.doConnectionChanged();
                }
                else if (((Error) cmd).ErrorNumber == ERROR_TYPES.SYSTEM_MANAGER_CONNECTED || ((Error) cmd).ErrorNumber == ERROR_TYPES.SYSTEM_MANAGER_ACTIVE)
                  this.Paused = true;
                else if (((Error) cmd).ErrorNumber == ERROR_TYPES.SYSTEM_MANAGER_DISCONNECTED)
                  this.Paused = false;
              }
              else if (cmd.GetType() == typeof (EncryptionKey))
              {
                if (cmd.Type == 3)
                  this.IV = ((EncryptionKey) cmd).Key;
                if (cmd.Type == 4)
                  this.key = ((EncryptionKey) cmd).Key;
                this._encrypted = true;
                this.doConnectionChanged();
                if (this.IV.Length != 0 && this.key.Length != 0)
                  this.SendLogin();
              }
              else if (cmd.Data != null && this._encoding.GetString(cmd.Data).ToLower().Contains("welcome to qbus"))
                this.CheckXportVersion(cmd);
              if (!this._isLogedIn && !this._encrypted)
              {
                this.SendLogin();
                this._isLogedIn = true;
              }
              base.Receive(cmd);
            }
            else if (cmd.Data.Length == 8)
            {
              try
              {
                long ticks = DateTime.Now.Ticks;
                if (BitConverter.ToInt64(cmd.Data, 0) == this._lastHeartbeat)
                {
                  this._delay = new TimeSpan(ticks - this._lastHeartbeat);
                  this._lastHeartbeat = -1L;
                }
              }
              catch
              {
              }
            }
            else
            {
              if (!this._isLogedIn && this._encoding.GetString(cmd.Data).ToLower().Contains("welcome to qbus"))
              {
                this.CheckXportVersion(cmd);
                if (this._status == TCP_STATUS.CONNECTED)
                {
                  this.SendLogin();
                  this._isLogedIn = true;
                }
              }
              base.Receive(cmd);
            }
          }
        }
      }
    }

    private void ReconnectInThread()
    {
      this.Paused = false;
      this._socket.Close();
      this._socket = (Socket) null;
      if (this._thread != null && this._thread.IsAlive)
        this._thread.Abort();
      this._thread = (Thread) null;
      this.Start();
    }

    private void CheckXportVersion(Command cmd)
    {
      Match match = new Regex("[v][\\d][.][\\d]*").Match(this._encoding.GetString(cmd.Data).ToLower());
      if (match.Length == 0)
      {
        this._status = TCP_STATUS.WRONG_XPORT_VERSION;
        this.doConnectionChanged();
      }
      string[] strArray = match.ToString().Replace("#", "").Replace("v", "").Split('.');
      int result1 = -1;
      int.TryParse(strArray[0], out result1);
      int result2 = -1;
      int.TryParse(strArray[1], out result2);
      if (result1 > 5 || result1 == 5 && result2 >= 11)
      {
        this._status = TCP_STATUS.CONNECTED;
        this.doConnectionChanged();
      }
      else
      {
        this._status = TCP_STATUS.WRONG_XPORT_VERSION;
        this.doConnectionChanged();
      }
    }

    public override void Stop()
    {
      try
      {
        if (this.Manager != null)
          this.Manager.Dispose();
      }
      catch
      {
      }
      this.Manager = (ControllerManager) null;
      try
      {
        if (this._tim != null)
        {
          this._tim.Dispose();
          this._tim = (Timer) null;
        }
      }
      catch
      {
      }
      this._encrypted = false;
      this.IV = new byte[0];
      this.key = new byte[0];
      try
      {
        if (this._socket != null)
        {
          if (this._socket.Connected)
          {
            this._socket.Disconnect(true);
            this._socket.Close();
          }
        }
      }
      catch
      {
      }
      this._socket = (Socket) null;
      if (this._thread != null)
      {
        try
        {
          this._thread.Abort();
        }
        catch
        {
        }
      }
      this.doConnectionChanged();
    }

    public override bool Send(Command cmd, bool queue)
    {
      try
      {
        if (queue && cmd != null)
        {
          this.Queue(cmd);
          return true;
        }
        else
        {
          if (cmd == null || this._socket == null || !this._socket.Connected)
            return false;
          byte[] Data = cmd.Serialize();
          if (cmd.GetType() == typeof (Login))
            ProtocolSender.Instance.SendEvent(new ReceivedData(this.Controller, (byte) 250, byte.MaxValue, Data));
          else
            ProtocolSender.Instance.SendEvent(new ReceivedData(this.Controller, byte.MaxValue, byte.MaxValue, Data));
          byte[] buffer = new byte[Data.Length + this._prefix.Length + 3];
          this._prefix.CopyTo((Array) buffer, 0);
          Data.CopyTo((Array) buffer, this._prefix.Length + 3);
          buffer[this._prefix.Length] = byte.MaxValue;
          if (cmd.GetType() == typeof (Login))
            buffer[this._prefix.Length] = (byte) 250;
          buffer[this._prefix.Length + 2] = (byte) (Data.Length - 1);
          buffer[this._prefix.Length + 1] = (byte) (Data.Length - 1 >> 8);
          if (this._encrypted)
            buffer = CommunicationEncryption.Instance.Encrypt(buffer, this.key, this.IV);
          if (this._socket != null)
          {
            if (this._socket.Connected)
            {
              this._socket.Send(buffer);
              return true;
            }
          }
        }
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_MID);
      }
      return false;
    }

    public void Dispose()
    {
      this.Stop();
    }
  }
}
