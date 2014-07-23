// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.EventLogs
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;

namespace Qbus.Communication.Protocol.Commands
{
  public class EventLogs : Command
  {
    public override int Type
    {
      get
      {
        return 26;
      }
    }

    public int[] EventSectors { get; set; }

    public override void Parse(byte[] data)
    {
      if (data.Length < 5)
        return;
      if ((int) data[0] != (int) (byte) this.Type && (int) data[0] - 128 != (int) (byte) this.Type)
        throw new Exception("Tried to parse a Event log command while it is not");
      this.Instruction1 = data[1];
      this.Instruction2 = data[2];
      if (data.Length < 46)
        return;
      this.Data = new byte[40];
      for (int index = 0; index < 40; ++index)
        this.Data[index] = data[6 + index];
      this.EventSectors = new int[10];
      for (int index = 0; index < 10; ++index)
        this.EventSectors[index] = (((int) this.Data[20 + index * 2] << 8) + (int) this.Data[20 + index * 2 + 1]) / 2;
    }

    public override string ToString()
    {
      string str = "Event logs: ";
      if (this.EventSectors != null)
      {
        foreach (int num in this.EventSectors)
          str = str + (object) num + " - ";
      }
      return str;
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
      numArray[4] = (byte) 39;
      numArray[5] = (byte) 35;
      return numArray;
    }

    public override bool EqualAddress(Command cmd)
    {
      return this.Type == cmd.Type && (int) this.Instruction1 == (int) cmd.Instruction1 && (int) this.Instruction2 == (int) cmd.Instruction2;
    }

    public override object Clone()
    {
      EventLogs eventLogs = new EventLogs();
      eventLogs.Data = this.Data;
      eventLogs.Instruction1 = this.Instruction1;
      eventLogs.Instruction2 = this.Instruction2;
      return (object) eventLogs;
    }
  }
}
