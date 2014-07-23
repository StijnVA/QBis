// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.SerialCommunication
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace Qbus.Communication
{
  public class SerialCommunication : ControllerCommunication, IDisposable
  {
    private SerialPort _port;
    private string _receive;
    private string _allComm;
    private Encoding _encoding;

    public override bool Connected
    {
      get
      {
        return this._port != null && this._port.IsOpen;
      }
    }

    public Encoding Encoding
    {
      get
      {
        return this._encoding;
      }
    }

    public SerialCommunication(Controller c)
    {
      this.Controller = c;
      this._encoding = Encoding.GetEncoding(1252);
      if (!ConnectionManager.Instance.UseControllerManager)
        return;
      this.Manager = new ControllerManager((ControllerCommunication) this);
    }

    private void Connect()
    {
      try
      {
        if (this._port != null)
        {
          if (this._port.IsOpen)
            this._port.Close();
          this._port.Dispose();
          this._port = (SerialPort) null;
        }
      }
      catch
      {
      }
      this._port = new SerialPort();
      this._port.BaudRate = 115200;
      this._port.DataBits = 8;
      this._port.Parity = Parity.None;
      this._port.StopBits = StopBits.One;
      this._port.Handshake = Handshake.None;
      this._port.PortName = this.Controller.ComPort;
      this._port.Encoding = this._encoding;
      this._port.DataReceived += new SerialDataReceivedEventHandler(this._port_DataReceived);
      this._port.Open();
      this._receive = "";
      this._allComm = "";
      this.Send((Command) new EventStatus(), false);
      this.doConnectionChanged();
    }

    private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
      string str1 = this._port.ReadExisting();
      SerialCommunication serialCommunication = this;
      string str2 = serialCommunication._receive + str1;
      serialCommunication._receive = str2;
      foreach (Command cmd in ProtocolParser.Parse(ref this._receive, this._encoding, this.Controller))
      {
        cmd.Controller = this.Controller;
        this.Receive(cmd);
      }
    }

    public List<Command> GetAllCommands()
    {
      return ProtocolParser.Parse(ref this._allComm, this._encoding, this.Controller);
    }

    public override void Start()
    {
      try
      {
        this.Connect();
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.CONNECTION_LOST);
      }
    }

    public override void Stop()
    {
      try
      {
        if (this._port.IsOpen)
          this._port.Close();
      }
      catch
      {
      }
      this.doConnectionChanged();
    }

    public override bool Send(Command cmd, bool queue)
    {
      if (queue && cmd != null)
      {
        this.Queue(cmd);
        return true;
      }
      else
      {
        if (cmd == null || this._port == null || !this._port.IsOpen)
          return false;
        byte[] buffer = cmd.Serialize();
        try
        {
          this._port.Write(buffer, 0, buffer.Length);
          return true;
        }
        catch (Exception ex1)
        {
          ConnectionManager.Instance.ErrorHandle(ex1, WARNING_TYPES.CONNECTION_LOST);
          try
          {
            this.Connect();
          }
          catch (Exception ex2)
          {
            ConnectionManager.Instance.ErrorHandle(ex1, WARNING_TYPES.CONNECTION_LOST);
          }
        }
        return false;
      }
    }

    public void Dispose()
    {
      this.Stop();
    }
  }
}
