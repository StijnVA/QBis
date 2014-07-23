// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.ControllerOptions
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;

namespace Qbus.Communication.Protocol.Commands
{
  public class ControllerOptions : Command
  {
    public override int Type
    {
      get
      {
        return 13;
      }
    }

    public string ActivateString { get; set; }

    public bool UART { get; set; }

    public bool Events { get; set; }

    public bool Module { get; set; }

    public bool SunRiseSet { get; set; }

    public bool Analogic { get; set; }

    public bool Simulation { get; set; }

    public bool EQOmmand { get; set; }

    public bool SDK { get; set; }

    public ControllerOptions()
    {
      this.Instruction1 = (byte) 13;
      this.Instruction2 = (byte) 0;
      this.Data = new byte[1];
      this.Data[0] = (byte) 7;
    }

    public override void Parse(byte[] data)
    {
      if ((int) data[0] != (int) (byte) this.Type && (int) data[0] - 128 != (int) (byte) this.Type)
        throw new Exception("Tried to parse a Controller options command while it is not");
      if (data.Length < 3)
        throw new Exception("Data length not valid for a Controller options command");
      if ((int) data[0] > 128)
        this.Write = true;
      this.Instruction1 = data[1];
      this.Instruction2 = data[2];
      if (data.Length < 13)
        return;
      this.UART = (int) data[6] == (int) byte.MaxValue;
      this.Events = (int) data[7] == (int) byte.MaxValue;
      this.Module = (int) data[8] == (int) byte.MaxValue;
      this.SunRiseSet = (int) data[9] == (int) byte.MaxValue;
      this.Analogic = (int) data[10] == (int) byte.MaxValue;
      this.Simulation = (int) data[11] == (int) byte.MaxValue;
      this.EQOmmand = (int) data[12] == (int) byte.MaxValue;
      this.SDK = (int) data[13] == (int) byte.MaxValue;
    }

    public override byte[] Serialize()
    {
      if (this.ActivateString == null || this.ActivateString == "")
      {
        byte[] numArray = new byte[this.Data.Length + 5];
        numArray[0] = (byte) 42;
        byte num = (byte) this.Type;
        if (this.Write)
					num += MonoUtils.SByte_MIN;
        numArray[1] = num;
        numArray[2] = this.Instruction1;
        numArray[3] = this.Instruction2;
        if (this.Data.Length > 0)
          this.Data.CopyTo((Array) numArray, 4);
        numArray[numArray.Length - 1] = (byte) 35;
        return numArray;
      }
      else
      {
        byte[] numArray = new byte[this.ActivateString.Length / 3 + 5];
        numArray[0] = (byte) 42;
        byte num = (byte) this.Type;
        if (this.Write)
					num += MonoUtils.SByte_MIN;
        numArray[1] = num;
        numArray[2] = this.Instruction1;
        numArray[3] = (byte) 0;
        numArray[4] = (byte) (this.ActivateString.Length / 3 - 2);
        for (int index = 0; index < this.ActivateString.Length / 3; ++index)
        {
          string s = this.ActivateString.Substring(index * 3, 3);
          if (index == 0)
            numArray[index + 3] = byte.Parse(s);
          else
            numArray[index + 4] = byte.Parse(s);
        }
        numArray[numArray.Length - 1] = (byte) 35;
        return numArray;
      }
    }

    public override string ToString()
    {
      if (this.Write)
        return "Controller options updated";
      return string.Concat(new object[4]
      {
        (object) "Activated for EQOmmand: ",
        (object) (bool) (this.EQOmmand),
        (object) " SDK: ",
        (object) (bool) (this.SDK)
      });
    }

    public override object Clone()
    {
      return (object) new ControllerOptions()
      {
        ActivateString = this.ActivateString
      };
    }

    public override bool EqualAddress(Command cmd)
    {
      int type = this.Type;
      return cmd.Type == this.Type;
    }
  }
}
