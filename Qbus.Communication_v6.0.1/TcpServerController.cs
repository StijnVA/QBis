// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.TcpServerController
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using System.Net.Sockets;
using System.Threading;

namespace Qbus.Communication
{
  public class TcpServerController
  {
    public Socket Socket { get; set; }

    public Thread CommunicationThread { get; set; }

    public Controller Controller { get; set; }

    public bool IsLoggedin { get; set; }

    public bool IsPaused { get; set; }
  }
}
