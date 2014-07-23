// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.SDModule
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;

namespace Qbus.Communication.Protocol.Commands
{
  public class SDModule : Command
  {
    public bool Write_All_Data { get; set; }

    public bool Read_All_Enable { get; set; }

    public bool WR_Group { get; set; }

    public bool Reserve { get; set; }

    public bool ModuleScan { get; set; }

    public bool Attempts { get; set; }

    public bool BlockSize { get; set; }

    public bool FirstTest { get; set; }

    public byte WaitTime { get; set; }

    public byte Offset { get; set; }

    public bool EEPROM { get; set; }

    public bool FLASH { get; set; }

    public byte Length { get; set; }

    public int Serial { get; set; }

    public int ModuleNr
    {
      get
      {
        return (int) this.Instruction1 * 256 + (int) this.Instruction2;
      }
      set
      {
        this.Instruction1 = (byte) (value / 256);
        this.Instruction2 = (byte) (value % 256);
      }
    }

    public override int Type
    {
      get
      {
        return 66;
      }
    }

    public override void Parse(byte[] data)
    {
      throw new NotImplementedException();
    }

    public override string ToString()
    {
      throw new NotImplementedException();
    }

    public override bool EqualAddress(Command cmd)
    {
      throw new NotImplementedException();
    }

    public override object Clone()
    {
      throw new NotImplementedException();
    }

    public override byte[] Serialize()
    {
      byte num1 = (byte) 248;
      byte[] numArray = new byte[(int) num1 + 6];
      numArray[0] = (byte) 42;
      numArray[1] = this.Write ? (byte) (this.Type + 128) : (byte) this.Type;
      numArray[2] = this.Instruction1;
      numArray[3] = this.Instruction2;
      numArray[4] = num1;
      if (this.Write)
      {
        byte num2 = (byte) ((int) (byte) ((int) (byte) (0 + (this.Write_All_Data ? 1 : 0)) + (this.Read_All_Enable ? 2 : 0)) + (this.WR_Group ? 4 : 0));
      }
      numArray[numArray.Length - 1] = (byte) 35;
      return numArray;
    }
  }
}
