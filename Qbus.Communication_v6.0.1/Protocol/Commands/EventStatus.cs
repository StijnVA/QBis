// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.EventStatus
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;

namespace Qbus.Communication.Protocol.Commands
{
  public class EventStatus : Command
  {
    public override int Type
    {
      get
      {
        return 53;
      }
    }

    public bool UART { get; set; }

    public bool SD { get; set; }

    public bool CHANNEL { get; set; }

    public byte Address { get; set; }

    public byte Status1
    {
      get
      {
        if (this.Data != null && this.Data.Length > 0)
          return this.Data[0];
        else
          return (byte) 0;
      }
    }

    public byte Status2
    {
      get
      {
        if (this.Data != null && this.Data.Length > 1)
          return this.Data[1];
        else
          return (byte) 0;
      }
    }

    public byte Status3
    {
      get
      {
        if (this.Data != null && this.Data.Length > 2)
          return this.Data[2];
        else
          return (byte) 0;
      }
    }

    public byte Status4
    {
      get
      {
        if (this.Data != null && this.Data.Length > 0)
          return this.Data[3];
        else
          return (byte) 0;
      }
    }

    public override byte Instruction1
    {
      get
      {
        byte num = (byte) 0;
        if (this.UART)
          ++num;
        if (this.SD)
          num += (byte) 2;
        if (this.CHANNEL)
          num += (byte) 4;
        return num;
      }
      set
      {
        base.Instruction1 = value;
      }
    }

    public EventStatus()
    {
      this.UART = true;
      this.SD = true;
      this.CHANNEL = false;
      this.Instruction1 = (byte) 0;
      this.Instruction2 = (byte) 0;
      this.Data = new byte[1];
      this.Write = true;
    }

    public override string ToString()
    {
      string str = "EVENT";
      try
      {
        int num = (int) this.Address;
        str = str + this.Address.ToString() + " ";
      }
      catch
      {
      }
      try
      {
        int num = (int) this.Status1;
        str = str + this.Status1.ToString() + " ";
      }
      catch
      {
      }
      try
      {
        int num = (int) this.Status2;
        str = str + this.Status2.ToString() + " ";
      }
      catch
      {
      }
      try
      {
        int num = (int) this.Status3;
        str = str + this.Status3.ToString() + " ";
      }
      catch
      {
      }
      try
      {
        int num = (int) this.Status4;
        str = str + this.Status4.ToString();
      }
      catch
      {
      }
      return str;
    }

    public override void Parse(byte[] data)
    {
      this.Address = (int) data[0] != 181 ? data[1] : (byte) 0;
      byte[] numArray = new byte[4];
      if (data.Length < 6)
        throw new Exception("Tried to parse a Event status command: Length is not right");
      for (int index = 0; index < 4; ++index)
        numArray[index] = data[index + 2];
      this.Data = numArray;
    }

    public override object Clone()
    {
      EventStatus eventStatus = new EventStatus();
      eventStatus.Data = this.Data;
      eventStatus.Instruction1 = this.Instruction1;
      eventStatus.Instruction2 = this.Instruction2;
      eventStatus.UART = this.UART;
      eventStatus.SD = this.SD;
      eventStatus.CHANNEL = this.CHANNEL;
      return (object) eventStatus;
    }

    public override bool EqualAddress(Command cmd)
    {
      return cmd.GetType() == typeof (EventStatus);
    }
  }
}
