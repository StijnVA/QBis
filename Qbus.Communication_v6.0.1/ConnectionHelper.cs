// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.ConnectionHelper
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Qbus.Communication
{
  public class ConnectionHelper
  {
    private List<Controller> _foundControllers = new List<Controller>();
    private List<SerialCommunication> _connections = new List<SerialCommunication>();
    private static ConnectionHelper _instance;
    private Socket _broadcastSocket;
    private bool _listening;
    private System.Threading.Timer _timeout;
    private int _tries;
    private SerialPort _port;
    private List<string> _ports;
    private System.Threading.Timer _usbTimer;

    public static ConnectionHelper Instance
    {
      get
      {
        if (ConnectionHelper._instance == null)
          ConnectionHelper._instance = new ConnectionHelper();
        return ConnectionHelper._instance;
      }
    }

    public bool RouterConnection { get; private set; }

    public string Hostname { get; private set; }

    public List<Controller> FoundControllers
    {
      get
      {
        return this._foundControllers;
      }
      set
      {
        this._foundControllers = value;
      }
    }

    public event EventHandler FindControllersFinished;

    public event EventHandler NewControllerFound;

    private ConnectionHelper()
    {
      this.RouterConnection = false;
      this.Hostname = "";
    }

    public void TestConnection()
    {
      this.Hostname = Dns.GetHostName();
      this.RouterConnection = false;
      int num = 0;
      foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
      {
        if (networkInterface.OperationalStatus == OperationalStatus.Up)
        {
          foreach (GatewayIPAddressInformation addressInformation in networkInterface.GetIPProperties().GatewayAddresses)
          {
            ++num;
            Ping ping = new Ping();
            try
            {
              if (ping.Send(addressInformation.Address, 500).Status == IPStatus.Success)
                this.RouterConnection = true;
            }
            catch
            {
            }
          }
        }
      }
      if (num != 0)
        return;
      this.RouterConnection = false;
    }

    public void FindControllersAsync()
    {
      this.FindControllersAsync(1000);
    }

    public void FindControllersAsync(int timeout)
    {
      this._tries = 3;
      this._foundControllers.Clear();
      try
      {
        this._broadcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        this._broadcastSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        IPEndPoint ipEndPoint1 = new IPEndPoint(IPAddress.Broadcast, 30718);
        Dns.GetHostName();
        Socket socket1 = this._broadcastSocket;
        byte[] numArray1 = new byte[4];
        numArray1[3] = (byte) 246;
        byte[] buffer1 = numArray1;
        IPEndPoint ipEndPoint2 = ipEndPoint1;
        socket1.SendTo(buffer1, (EndPoint) ipEndPoint2);
        Socket socket2 = this._broadcastSocket;
        byte[] numArray2 = new byte[4];
        numArray2[3] = (byte) 244;
        byte[] buffer2 = numArray2;
        IPEndPoint ipEndPoint3 = ipEndPoint1;
        socket2.SendTo(buffer2, (EndPoint) ipEndPoint3);
        Socket socket3 = this._broadcastSocket;
        byte[] numArray3 = new byte[4];
        numArray3[3] = (byte) 224;
        byte[] buffer3 = numArray3;
        IPEndPoint ipEndPoint4 = ipEndPoint1;
        socket3.SendTo(buffer3, (EndPoint) ipEndPoint4);
        this._listening = true;
        new Thread(new ThreadStart(this.Receive)).Start();
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_MID);
      }
      try
      {
        new Thread(new ThreadStart(this.FindControllersUSB)).Start();
      }
      catch (Exception ex)
      {
        ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_MID);
      }
      this._timeout = new System.Threading.Timer(new TimerCallback(this.FindControllerTimeout), (object) null, timeout, timeout);
    }

    private void FindControllersUSB()
    {
      string[] portNames = SerialPort.GetPortNames();
      this._ports = new List<string>();
      this._ports.AddRange((IEnumerable<string>) portNames);
      this.CheckNextPort();
    }

    private void CheckNextPort()
    {
      if (this._ports.Count == 0)
        return;
      string str = this._ports[0];
      this._ports.RemoveAt(0);
      if (this._port != null)
      {
        try
        {
          if (this._port.IsOpen)
            this._port.Close();
          this._port.Dispose();
        }
        catch
        {
        }
      }
      try
      {
        this._port = new SerialPort();
        this._port.BaudRate = 115200;
        this._port.DataBits = 8;
        this._port.Parity = Parity.None;
        this._port.StopBits = StopBits.One;
        this._port.Handshake = Handshake.None;
        this._port.Encoding = Encoding.GetEncoding(1252);
        this._port.DataReceived += new SerialDataReceivedEventHandler(this._port_DataReceived);
        this._usbTimer = new System.Threading.Timer(new TimerCallback(this.usbTimeout), (object) null, 1000, 1000);
        this._port.PortName = str;
        this._port.Open();
        byte[] buffer = new EventStatus().Serialize();
        this._port.Write(buffer, 0, buffer.Length);
      }
      catch (Exception ex)
      {
        string message = ex.Message;
      }
    }

    private void usbTimeout(object o)
    {
      try
      {
        this._port.DiscardInBuffer();
        this._port.DiscardOutBuffer();
        this._port.Close();
        this._port.Dispose();
      }
      catch (Exception ex)
      {
        string message = ex.Message;
      }
      if (this._usbTimer != null)
        this._usbTimer.Dispose();
      this._usbTimer = (System.Threading.Timer) null;
      this.CheckNextPort();
    }

    private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
      string s = this._port.ReadExisting();
      List<Command> list = ProtocolParser.Parse(ref s, Encoding.GetEncoding(1252), (Controller) null);
      if (list.Count <= 0)
        return;
      if (list[0].GetType() == typeof (EventStatus))
      {
        Controller c = new Controller();
        c.ComPort = this._port.PortName;
        this._foundControllers.Add(c);
        if (this.NewControllerFound != null)
          this.NewControllerFound((object) this, new EventArgs());
        this.GetSN(c);
      }
      else
      {
        if (list[0].GetType() != typeof (Qbus.Communication.Protocol.Commands.Version))
          return;
        Controller controller1 = (Controller) null;
        foreach (Controller controller2 in this._foundControllers)
        {
          if (controller2.ComPort == this._port.PortName)
            controller1 = controller2;
        }
        if (controller1 != null)
        {
          controller1.MAC = ((Qbus.Communication.Protocol.Commands.Version) list[0]).Serial;
          if (this.NewControllerFound != null)
            this.NewControllerFound((object) this, new EventArgs());
        }
        this._port.Close();
        this.CheckNextPort();
      }
    }

    private void GetSN(Controller c)
    {
      byte[] buffer = new Qbus.Communication.Protocol.Commands.Version().Serialize();
      this._port.Write(buffer, 0, buffer.Length);
    }

    private void FindControllerTimeout(object target)
    {
      if (this._tries > 0 && !this.IsComplete())
      {
        --this._tries;
      }
      else
      {
        this._listening = false;
        this._broadcastSocket.Close();
        if (this._timeout != null)
        {
          this._timeout.Change(-1, -1);
          this._timeout.Dispose();
          this._timeout = (System.Threading.Timer) null;
        }
        foreach (SerialCommunication serialCommunication in this._connections)
        {
          try
          {
            serialCommunication.Stop();
            serialCommunication.Dispose();
          }
          catch
          {
          }
        }
        this._connections.Clear();
        if (this.FindControllersFinished == null)
          return;
        this.FindControllersFinished((object) this, new EventArgs());
      }
    }

    private void Receive()
    {
      while (this._listening)
      {
        byte[] numArray1 = new byte[1024];
        EndPoint remoteEP = (EndPoint) new IPEndPoint(IPAddress.Any, 0);
        int num;
        try
        {
          num = this._broadcastSocket.ReceiveFrom(numArray1, ref remoteEP);
        }
        catch
        {
          break;
        }
        if (num > 21)
        {
          Controller orAddController = this.FindOrAddController((IPEndPoint) remoteEP);
          if ((int) numArray1[3] == 247)
          {
            byte[] numArray2 = new byte[6];
            for (int index = 0; index < 6; ++index)
              numArray2[index] = numArray1[index + 24];
            orAddController.MAC = numArray2;
          }
          if ((int) numArray1[3] == 245 && (int) numArray1[16] >= 3)
            orAddController.Firmware = Encoding.ASCII.GetString(numArray1, 16, 5);
          if ((int) numArray1[3] == 208)
          {
            orAddController.TcpPort = 8446;
            orAddController.Login = "QBUS";
            orAddController.Password = "";
          }
        }
      }
    }

    private bool IsComplete()
    {
      if (this._foundControllers.Count == 0)
        return false;
      foreach (Controller controller in this._foundControllers)
      {
        if (controller.Address != null && !(controller.Address == ""))
        {
          int tcpPort = controller.TcpPort;
          if (controller.TcpPort != 0)
            continue;
        }
        return false;
      }
      return true;
    }

    private Controller FindOrAddController(IPEndPoint ip)
    {
      foreach (Controller controller in this._foundControllers)
      {
        if (controller.Address.Equals(ip.Address.ToString()))
        {
          int tcpPort = controller.TcpPort;
          if (controller.TcpPort == 0 || controller.TcpPort.Equals(ip.Port))
            return controller;
        }
      }
      Controller controller1 = new Controller();
      controller1.Address = ip.Address.ToString();
      controller1.TcpPort = 0;
      this._foundControllers.Add(controller1);
      if (this.NewControllerFound != null)
        this.NewControllerFound((object) this, new EventArgs());
      return controller1;
    }
  }
}
