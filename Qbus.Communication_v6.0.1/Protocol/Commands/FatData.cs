// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.FatData
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;

namespace Qbus.Communication.Protocol.Commands
{
  public class FatData : Command
  {
    public override int Type
    {
      get
      {
        return 9;
      }
    }

    public byte Address { get; set; }

    public byte CurrentBank
    {
      get
      {
        try
        {
          return (byte) ((uint) this.Data[5] - 2U);
        }
        catch
        {
          return (byte) 0;
        }
      }
    }

    public FatData()
    {
      this.Instruction1 = (byte) 0;
      this.Instruction2 = (byte) 0;
      this.Data = new byte[1]
      {
        (byte) 51
      };
      this.Write = false;
    }

    public override string ToString()
    {
      string str = "FAT ";
      try
      {
        str = str + "Bank: " + this.CurrentBank.ToString() + " ";
      }
      catch
      {
      }
      return str;
    }

    public override void Parse(byte[] data)
    {
      this.Address = data[1];
      byte[] numArray = new byte[52];
      if (data.Length < 54)
        throw new Exception("Tried to parse a Event status command: Length is not right");
      for (int index = 0; index < 52; ++index)
        numArray[index] = data[index + 2];
      this.Data = numArray;
    }

    public override object Clone()
    {
      FatData fatData = new FatData();
      fatData.Data = this.Data;
      fatData.Instruction1 = this.Instruction1;
      fatData.Instruction2 = this.Instruction2;
      return (object) fatData;
    }

    public override bool EqualAddress(Command cmd)
    {
      return cmd.GetType() == typeof (FatData);
    }
  }
}
