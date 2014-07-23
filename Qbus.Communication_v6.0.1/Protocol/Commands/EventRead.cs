// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.EventRead
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Downloaders;
using System;
using System.Collections.Generic;

namespace Qbus.Communication.Protocol.Commands
{
  internal class EventRead : Command
  {
    private byte _bank;

    public override int Type
    {
      get
      {
        return 52;
      }
    }

    public List<Log> Logs { get; set; }

    public int Sector
    {
      get
      {
        return ((int) this.Instruction1 << 8) + (int) this.Instruction2;
      }
      set
      {
        this.Instruction1 = (byte) (value >> 8);
        this.Instruction2 = (byte) (value % 256);
      }
    }

    public int PointerH { get; set; }

    public int PointerL { get; set; }

    public byte Bank
    {
      get
      {
        return this._bank;
      }
      set
      {
        this._bank = value;
      }
    }

    public override void Parse(byte[] data)
    {
      if (data.Length < 5)
        return;
      if ((int) data[0] != (int) (byte) this.Type && (int) data[0] - 128 != (int) (byte) this.Type)
        throw new Exception("Tried to parse a Event log command while it is not");
      this.Logs = new List<Log>();
      this.Instruction1 = data[1];
      this.Instruction2 = data[2];
      if (data.Length <= 6)
        return;
      int num1 = (int) data[5] * 2;
      this.PointerH = (int) data[6];
      this.PointerL = (int) data[7];
      int num2 = 0;
      while (num2 < num1)
      {
        try
        {
          this.Logs.Add(new Log().Parse(data, num2 + 8));
        }
        catch (Exception ex)
        {
        }
        num2 += 10;
      }
      if (data.Length < 46)
        return;
      this.Data = new byte[40];
    }

    public override string ToString()
    {
      return "Event log sector " + this.Sector.ToString();
    }

    public override byte[] Serialize()
    {
      byte[] numArray = new byte[6];
      numArray[0] = (byte) 42;
      byte num = (byte) this.Type;
      if (this.Write)
				num += MonoUtils.SByte_MIN;
      numArray[1] = num;
      numArray[2] = this.Instruction1;
      numArray[3] = this.Instruction2;
      numArray[4] = this._bank;
      numArray[5] = (byte) 35;
      return numArray;
    }

    public override bool EqualAddress(Command cmd)
    {
      return this.Type == cmd.Type && (int) this.Instruction1 == (int) cmd.Instruction1 && (int) this.Instruction2 == (int) cmd.Instruction2;
    }

    public override object Clone()
    {
      EventRead eventRead = new EventRead();
      eventRead.Data = this.Data;
      eventRead.Instruction1 = this.Instruction1;
      eventRead.Instruction2 = this.Instruction2;
      return (object) eventRead;
    }
  }
}
