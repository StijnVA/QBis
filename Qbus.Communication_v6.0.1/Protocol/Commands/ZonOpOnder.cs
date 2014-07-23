// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.ZonOpOnder
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;

namespace Qbus.Communication.Protocol.Commands
{
  public class ZonOpOnder : Command
  {
    public byte Sector { get; set; }

    public override int Type
    {
      get
      {
        return 25;
      }
    }

    public override void Parse(byte[] data)
    {
      this.Sector = data[2];
    }

    public override string ToString()
    {
      return "ZonOpOnder: " + this.Sector.ToString();
    }

    public override bool EqualAddress(Command cmd)
    {
      return this.Type == cmd.Type && cmd.GetType() == typeof (ZonOpOnder) && ((int) this.Instruction1 == (int) cmd.Instruction1 && (int) this.Instruction2 == (int) cmd.Instruction2) && (int) this.Sector == (int) ((ZonOpOnder) cmd).Sector;
    }

    public override object Clone()
    {
      ZonOpOnder zonOpOnder = new ZonOpOnder();
      zonOpOnder.Instruction1 = this.Instruction1;
      zonOpOnder.Instruction2 = this.Instruction2;
      zonOpOnder.Data = this.Data;
      return (object) zonOpOnder;
    }

    public override byte[] Serialize()
    {
      byte[] numArray = new byte[262];
      numArray[0] = (byte) 42;
      numArray[1] = (byte) 153;
      numArray[2] = this.Sector;
      numArray[4] = byte.MaxValue;
      for (int index = 0; index < 256; ++index)
        numArray[5 + index] = this.Data[index];
      numArray[numArray.Length - 1] = (byte) 35;
      return numArray;
    }
  }
}
