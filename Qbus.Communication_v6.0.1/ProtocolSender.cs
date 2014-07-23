// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.ProtocolSender
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

namespace Qbus.Communication
{
  public class ProtocolSender
  {
    private static ProtocolSender _instance;

    public static ProtocolSender Instance
    {
      get
      {
        if (ProtocolSender._instance == null)
          ProtocolSender._instance = new ProtocolSender();
        return ProtocolSender._instance;
      }
    }

    public event ReceivedDataHandler DataEvent;

    private ProtocolSender()
    {
    }

    internal void SendEvent(ReceivedData rd)
    {
      if (this.DataEvent == null)
        return;
      this.DataEvent((object) this, rd);
    }
  }
}
