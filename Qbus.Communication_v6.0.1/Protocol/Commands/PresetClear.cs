// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.PresetClear
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;

namespace Qbus.Communication.Protocol.Commands
{
  public class PresetClear : Command
  {
    public override int Type
    {
      get
      {
        return 16;
      }
    }

    public override void Parse(byte[] data)
    {
      if ((int) data[1] > 128)
        this.Write = true;
      this.Instruction1 = data[2];
      this.Data = data;
    }

    public override string ToString()
    {
      if ((int) this.Data[5] == 0)
        return "Preset " + (object) this.Instruction1 + " has been cleared";
      else
        return "An error occured while clearing preset " + (object) this.Instruction1;
    }

    public override bool EqualAddress(Command cmd)
    {
      return this.Type == cmd.Type && cmd.GetType() == typeof (PresetClear) && ((int) this.Instruction1 == (int) cmd.Instruction1 && (int) this.Instruction2 == (int) cmd.Instruction2);
    }

    public override object Clone()
    {
      PresetClear presetClear = new PresetClear();
      presetClear.Instruction1 = this.Instruction1;
      presetClear.Instruction2 = this.Instruction2;
      presetClear.Data = this.Data;
      presetClear.Write = this.Write;
      presetClear.Controller = this.Controller;
      return (object) presetClear;
    }

    public override byte[] Serialize()
    {
      return new byte[6]
      {
        (byte) 42,
        (byte) 144,
        this.Instruction1,
        (byte) 0,
        (byte) 0,
        (byte) 35
      };
    }
  }
}
