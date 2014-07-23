// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.CopyCtdToSD
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;

namespace Qbus.Communication.Protocol.Commands
{
  public class CopyCtdToSD : Command
  {
    public byte Bank
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

    public override int Type
    {
      get
      {
        return 71;
      }
    }

    public override void Parse(byte[] data)
    {
      if ((int) data[1] <= 128)
        return;
      this.Write = true;
    }

    public override string ToString()
    {
      return "Copy all CTD to SD";
    }

    public override bool EqualAddress(Command cmd)
    {
      return this.Type == cmd.Type && cmd.GetType() == typeof (CopyCtdToSD) && ((int) this.Instruction1 == (int) cmd.Instruction1 && (int) this.Instruction2 == (int) cmd.Instruction2);
    }

    public override object Clone()
    {
      CopyCtdToSD copyCtdToSd = new CopyCtdToSD();
      copyCtdToSd.Instruction1 = this.Instruction1;
      copyCtdToSd.Instruction2 = this.Instruction2;
      copyCtdToSd.Write = this.Write;
      copyCtdToSd.Controller = this.Controller;
      copyCtdToSd.Data = this.Data;
      return (object) copyCtdToSd;
    }

    public override byte[] Serialize()
    {
      return new byte[6]
      {
        (byte) 42,
        (byte) (this.Type + 128),
        this.Bank,
        this.Instruction2,
        (byte) 0,
        (byte) 35
      };
    }
  }
}
