// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.Analogica
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;

namespace Qbus.Communication.Protocol.Commands
{
  public class Analogica : Command
  {
    public byte Sector
    {
      get
      {
        return this.Instruction1;
      }
      set
      {
        this.Instruction1 = value;
      }
    }

    public byte Length { get; set; }

    public override int Type
    {
      get
      {
        return 33;
      }
    }

    public override void Parse(byte[] data)
    {
      if ((int) data[1] > 128)
        this.Write = true;
      this.Sector = data[2];
      this.Instruction2 = data[3];
      this.Length = data[6];
      this.Data = new byte[(int) this.Length];
      for (int index = 0; index < (int) this.Length; ++index)
        this.Data[index] = data[7 + index];
    }

    public override string ToString()
    {
      return this.Write ? "Write analogic" : "Read analogic";
    }

    public override bool EqualAddress(Command cmd)
    {
      return this.Type == cmd.Type && cmd.GetType() == typeof (Analogica) && ((int) this.Instruction1 == (int) cmd.Instruction1 && (int) this.Instruction2 == (int) cmd.Instruction2) && (int) this.Sector == (int) ((Analogica) cmd).Sector;
    }

    public override object Clone()
    {
      Analogica analogica = new Analogica();
      analogica.Data = this.Data;
      analogica.Instruction1 = this.Instruction1;
      analogica.Instruction2 = this.Instruction2;
      analogica.Sector = this.Sector;
      analogica.Write = this.Write;
      return (object) analogica;
    }

    public override byte[] Serialize()
    {
      byte[] numArray = !this.Write ? new byte[6] : new byte[261];
      numArray[0] = (byte) 42;
      numArray[1] = (byte) this.Type;
      if (this.Write)
        numArray[1] = (byte) (this.Type + 128);
      numArray[2] = this.Sector;
      numArray[3] = (byte) 0;
      numArray[4] = this.Length;
      if (this.Write)
      {
        for (int index = 0; index < this.Data.Length; ++index)
          numArray[index + 5] = this.Data[index];
      }
      numArray[numArray.Length - 1] = (byte) 35;
      return numArray;
    }
  }
}
