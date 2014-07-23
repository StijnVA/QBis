// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.NextTextCommand
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

namespace Qbus.Communication
{
  internal class NextTextCommand
  {
    public Controller Controller { get; set; }

    public byte Address { get; set; }

    public byte SubAddress { get; set; }

    public NextTextCommand(Controller c, byte addr, byte sub)
    {
      this.Controller = c;
      this.Address = addr;
      this.SubAddress = sub;
    }
  }
}
