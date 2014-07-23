// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.CommandEventArgs
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;

namespace Qbus.Communication
{
  public class CommandEventArgs : EventArgs
  {
    public DateTime Time { get; set; }

    public Command Command { get; set; }

    public CommandEventArgs(Command cmd)
    {
      this.Time = DateTime.Now;
      this.Command = cmd;
    }

    public CommandEventArgs()
    {
      this.Time = DateTime.Now;
    }
  }
}
