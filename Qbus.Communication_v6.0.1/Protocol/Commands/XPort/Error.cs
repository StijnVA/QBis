// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.XPort.Error
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;

namespace Qbus.Communication.Protocol.Commands.XPort
{
  public class Error : Command
  {
    public override int Type
    {
      get
      {
        return (int) byte.MaxValue;
      }
    }

    public ERROR_TYPES ErrorNumber { get; set; }

    public override void Parse(byte[] data)
    {
      if (data.Length < 3)
        return;
      this.ErrorNumber = (ERROR_TYPES) data[1];
    }

    public override string ToString()
    {
      return "Error: " + ((object) this.ErrorNumber).ToString();
    }

    public override bool EqualAddress(Command cmd)
    {
      return false;
    }

    public override object Clone()
    {
      return (object) new Error()
      {
        ErrorNumber = this.ErrorNumber
      };
    }
  }
}
