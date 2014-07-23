// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.Echo
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System.Text;

namespace Qbus.Communication.Protocol.Commands
{
  public class Echo : Command
  {
    public override int Type
    {
      get
      {
        return 0;
      }
    }

    public override void Parse(byte[] data)
    {
      this.Data = data;
    }

    public override string ToString()
    {
      return "Echo: " + Encoding.GetEncoding(1252).GetString(this.Data);
    }

    public override bool EqualAddress(Command cmd)
    {
      return false;
    }

    public override object Clone()
    {
      Echo echo = new Echo();
      echo.Data = this.Data;
      return (object) echo;
    }
  }
}
