// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.Clock
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;

namespace Qbus.Communication.Protocol.Commands
{
  public class Clock : Command
  {
    public byte Length { get; set; }

    public override int Type
    {
      get
      {
        return 24;
      }
    }

    public override void Parse(byte[] data)
    {
      if ((int) data[1] > 128)
        this.Write = true;
      else
        this.Write = false;
      this.Instruction1 = data[2];
      this.Instruction2 = data[3];
      this.Length = data[4];
      this.Data = new byte[(int) this.Length];
      if (!this.Write)
        return;
      for (int index = 0; index < (int) this.Length; ++index)
        this.Data[index] = data[index + 8];
    }

    public override string ToString()
    {
      if (this.Write)
        return "Write Clock nr:" + (object) this.Instruction1;
      else
        return "Read Clock nr:" + (object) this.Instruction1;
    }

    public override bool EqualAddress(Command cmd)
    {
      return this.Type == cmd.Type && cmd.GetType() == typeof (Clock) && ((int) this.Instruction1 == (int) cmd.Instruction1 && (int) this.Instruction2 == (int) cmd.Instruction2);
    }

    public override object Clone()
    {
      Clock clock = new Clock();
      clock.Data = this.Data;
      clock.Instruction1 = this.Instruction1;
      clock.Instruction2 = this.Instruction2;
      clock.Write = this.Write;
      clock.Controller = this.Controller;
      clock.Length = this.Length;
      return (object) clock;
    }

    public override byte[] Serialize()
    {
      byte[] numArray = !this.Write ? new byte[6] : new byte[6 + this.Data.Length];
      numArray[0] = (byte) 42;
      numArray[1] = (byte) this.Type;
      if (this.Write)
        numArray[1] = (byte) (this.Type + 128);
      numArray[2] = this.Instruction1;
      numArray[3] = this.Instruction2;
      numArray[4] = this.Length;
      if (this.Write)
      {
        numArray[4] = (byte) this.Data.Length;
        for (int index = 0; index < this.Data.Length; ++index)
          numArray[index + 5] = this.Data[index];
      }
      numArray[numArray.Length - 1] = (byte) 35;
      return numArray;
    }
  }
}
