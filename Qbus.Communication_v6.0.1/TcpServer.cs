// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.TcpServer
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;
using Qbus.Communication.Protocol.Commands.XPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Qbus.Communication
{
  public class TcpServer
  {
    private Socket _server;
    private bool _running;
    private Dictionary<Socket, TcpServerController> _clients;
    private byte[] _prefix;
    private Encoding _encoding;

    public List<Controller> ConnectedControllers
    {
      get
      {
        return Enumerable.ToList<Controller>(Enumerable.Select<KeyValuePair<Socket, TcpServerController>, Controller>((IEnumerable<KeyValuePair<Socket, TcpServerController>>) this._clients, (Func<KeyValuePair<Socket, TcpServerController>, Controller>) (o => o.Value.Controller)));
      }
    }

    public event ControllerCommunication.CommandEventHandler CommandReceived;

    public TcpServer()
    {
      this._clients = new Dictionary<Socket, TcpServerController>();
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
      this._encoding = Encoding.GetEncoding(1252);
    }

    public void Start(int port)
    {
      this._server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      this._server.Bind((EndPoint) new IPEndPoint(IPAddress.Any, port));
      this._server.Listen(100);
      this._running = true;
      this._server.BeginAccept(new AsyncCallback(this.OnClientConnect), (object) null);
    }

    public void Stop()
    {
      this._running = false;
      foreach (Socket socket in this._clients.Keys)
        socket.Close();
      this._clients.Clear();
      this._server.Close();
    }

    public void OnClientConnect(IAsyncResult result)
    {
      try
      {
        Socket socket = this._server.EndAccept(result);
        Thread tReceive = new Thread(new ParameterizedThreadStart(this.StartCommunication));
        this._clients.Add(socket, TcpServer.CreateServerController(tReceive, socket));
        tReceive.Start((object) socket);
      }
      catch (ObjectDisposedException ex)
      {
      }
      catch (Exception ex)
      {
      }
      try
      {
        this._server.BeginAccept(new AsyncCallback(this.OnClientConnect), (object) null);
      }
      catch
      {
      }
    }

    private static TcpServerController CreateServerController(Thread tReceive, Socket client)
    {
      return new TcpServerController()
      {
        CommunicationThread = tReceive,
        Socket = client,
        IsLoggedin = false,
        IsPaused = false,
        Controller = new Controller()
        {
          TcpPort = ((IPEndPoint) client.RemoteEndPoint).Port,
          Address = ((IPEndPoint) client.RemoteEndPoint).Address.ToString()
        }
      };
    }

    private void SendLogin(Controller c)
    {
      Login login = new Login();
      login.User = "QBUS";
      login.Password = "";
      login.Controller = c;
      this.Send((Command) login, false);
    }

    public TcpServerController FindConnection(Command cmd)
    {
      Controller c = cmd.Controller;
      if (c == null)
        return (TcpServerController) null;
      IEnumerable<TcpServerController> source = Enumerable.Select<KeyValuePair<Socket, TcpServerController>, TcpServerController>(Enumerable.Where<KeyValuePair<Socket, TcpServerController>>((IEnumerable<KeyValuePair<Socket, TcpServerController>>) this._clients, (Func<KeyValuePair<Socket, TcpServerController>, bool>) (o =>
      {
        if (o.Value.Controller.Address == c.Address)
          return o.Value.Controller.TcpPort == c.TcpPort;
        else
          return false;
      })), (Func<KeyValuePair<Socket, TcpServerController>, TcpServerController>) (o => o.Value));
      if (Enumerable.Any<TcpServerController>(source))
        return Enumerable.First<TcpServerController>(source);
      else
        return (TcpServerController) null;
    }

    public bool Send(Command cmd, bool queue)
    {
      TcpServerController connection = this.FindConnection(cmd);
      if (connection == null)
        return false;
      Socket socket = connection.Socket;
      try
      {
        if (cmd == null || socket == null || !socket.Connected)
          return false;
        byte[] Data = cmd.Serialize();
        if (cmd.GetType() == typeof (Login))
          ProtocolSender.Instance.SendEvent(new ReceivedData(connection.Controller, (byte) 250, byte.MaxValue, Data));
        else
          ProtocolSender.Instance.SendEvent(new ReceivedData(connection.Controller, byte.MaxValue, byte.MaxValue, Data));
        byte[] buffer = new byte[Data.Length + this._prefix.Length + 3];
        this._prefix.CopyTo((Array) buffer, 0);
        Data.CopyTo((Array) buffer, this._prefix.Length + 3);
        buffer[this._prefix.Length] = byte.MaxValue;
        if (cmd.GetType() == typeof (Login))
          buffer[this._prefix.Length] = (byte) 250;
        buffer[this._prefix.Length + 2] = (byte) (Data.Length - 1);
        buffer[this._prefix.Length + 1] = (byte) (Data.Length - 1 >> 8);
        if (socket.Connected)
        {
          socket.Send(buffer);
          return true;
        }
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_MID);
      }
      return false;
    }

    private void StartCommunication(object o)
    {
      Socket key = (Socket) o;
      byte[] buffer1 = new byte[4095];
      TcpServerController serverController = this._clients[key];
      while (key != null && key.Connected)
      {
        int length;
        try
        {
          length = key.Receive(buffer1, 1024, SocketFlags.None);
        }
        catch (Exception ex)
        {
          this._clients.Remove(key);
          break;
        }
        if (!key.Connected)
        {
          this._clients.Remove(key);
          break;
        }
        else if (length == 0)
        {
          try
          {
            key.Disconnect(false);
          }
          catch
          {
          }
          this._clients.Remove(key);
          break;
        }
        else
        {
          byte[] buffer2 = new byte[length];
          for (int index = 0; index < length; ++index)
            buffer2[index] = buffer1[index];
          if (buffer2.Length > 9)
          {
            if ((int) buffer2[9] == 250)
            {
              byte[] numArray = new byte[buffer2.Length - 12];
              for (int index = 0; index < numArray.Length; ++index)
                numArray[index] = buffer2[index + 12];
              ProtocolSender.Instance.SendEvent(new ReceivedData(serverController.Controller, (byte) 250, (byte) 0, buffer2));
            }
            else if ((int) buffer2[9] == (int) byte.MaxValue)
            {
              byte[] numArray = new byte[buffer2.Length - 12];
              for (int index = 0; index < numArray.Length; ++index)
                numArray[index] = buffer2[index + 12];
              ProtocolSender.Instance.SendEvent(new ReceivedData(serverController.Controller, byte.MaxValue, (byte) 0, buffer2));
            }
          }
          List<Command> list1 = ProtocolParser.ParseTCP(ref buffer2, this._prefix, serverController.Controller);
          int num1 = 0;
          while (buffer2.Length != 0 && buffer2.Length != num1)
          {
            for (int index = length - buffer2.Length; index < length; ++index)
              buffer2[index - (length - buffer2.Length)] = buffer1[index];
            num1 = buffer2.Length;
            List<Command> list2 = ProtocolParser.ParseTCP(ref buffer2, this._prefix, serverController.Controller);
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
              cmd.Controller = serverController.Controller;
              if (cmd.GetType() == typeof (Login))
              {
                if (!((Login) cmd).Success)
                  key.Close();
              }
              else if (cmd.GetType() == typeof (Qbus.Communication.Protocol.Commands.XPort.Error))
              {
                if (((Qbus.Communication.Protocol.Commands.XPort.Error) cmd).ErrorNumber == ERROR_TYPES.LOGIN_INCORRECT)
                  key.Close();
                else if (((Qbus.Communication.Protocol.Commands.XPort.Error) cmd).ErrorNumber != ERROR_TYPES.SYSTEM_MANAGER_CONNECTED && ((Qbus.Communication.Protocol.Commands.XPort.Error) cmd).ErrorNumber != ERROR_TYPES.SYSTEM_MANAGER_ACTIVE)
                {
                  int num2 = (int) ((Qbus.Communication.Protocol.Commands.XPort.Error) cmd).ErrorNumber;
                }
              }
              if (!serverController.IsLoggedin)
              {
                this.SendLogin(serverController.Controller);
                serverController.IsLoggedin = true;
              }
              this.Receive(cmd);
            }
            else if (cmd.Data.Length == 8)
            {
              try
              {
                long ticks = DateTime.Now.Ticks;
                BitConverter.ToInt64(cmd.Data, 0);
              }
              catch
              {
              }
            }
            else
            {
              if (!serverController.IsLoggedin && this._encoding.GetString(cmd.Data).ToLower().Contains("welcome to qbus"))
              {
                this.SendLogin(serverController.Controller);
                serverController.IsLoggedin = true;
              }
              this.Receive(cmd);
            }
          }
        }
      }
    }

    public void Receive(Command cmd)
    {
      try
      {
        new Thread(new ParameterizedThreadStart(this.InvokeCommandReceived)).Start((object) cmd);
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_LOW);
      }
    }

    public void InvokeCommandReceived(object o)
    {
      Command cmd = (Command) o;
      if (this.CommandReceived == null)
        return;
      this.CommandReceived((object) this, new CommandEventArgs(cmd));
    }
  }
}
