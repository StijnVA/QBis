// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.AddressStatus
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;

namespace Qbus.Communication.Protocol.Commands
{
  public class AddressStatus : Command
  {
    public Module.MODE md { get; set; }

    public byte Address { get; set; }

    public AddressStatus.SUBADDRESS SubAddress { get; set; }

    public override byte Instruction1
    {
      get
      {
        return this.Address;
      }
      set
      {
        base.Instruction1 = value;
      }
    }

    public override byte Instruction2
    {
      get
      {
        return (byte) this.SubAddress;
      }
      set
      {
        base.Instruction2 = value;
      }
    }

    public override int Type
    {
      get
      {
        return 56;
      }
    }

    public AddressStatus()
    {
      this.Write = false;
      this.Address = (byte) 3;
      this.SubAddress = AddressStatus.SUBADDRESS.ALL;
      this.Data = new byte[1];
      this.Data[0] = (byte) 0;
    }

    public override string ToString()
    {
      string str = "ADDRESS " + this.Address + " - " +  this.SubAddress + ": ";
      for (int index = 0; index < this.Data.Length; ++index)
        str = str + (object) this.Data[index] + " - ";
      return str;
    }

    public override void Parse(byte[] data)
    {
      if ((int) data[0] != (int) (byte) this.Type && (int) data[0] - 128 != (int) (byte) this.Type)
        throw new Exception("Tried to parse a AddressStatus command while it is not");
      if ((int) data[0] > 128)
        this.Write = true;
      this.Address = data[1];
      this.SubAddress = (AddressStatus.SUBADDRESS) data[2];
      if ((int) data[0] - 128 == (int) (byte) this.Type)
      {
        this.Data = new byte[5];
        this.Data.Initialize();
        if (data.Length < 5)
          return;
        this.Data[(int) data[2] + 1] = data[4];
      }
      else
      {
        if (data.Length <= 4)
          return;
        if ((int) data[4] != 0)
          throw new Exception("Tried to parse a AddressStatus command: echo is not 0");
        byte num = data[5];
        if ((int) num + 8 > data.Length)
          throw new Exception("Tried to parse a AddressStatus command: Length is not right");
        this.Data = new byte[(int) num + 1];
        for (int index = 0; index < (int) num + 1; ++index)
          this.Data[index] = data[6 + index];
      }
    }

    public override object Clone()
    {
      AddressStatus addressStatus = new AddressStatus();
      addressStatus.Data = this.Data;
      addressStatus.Instruction1 = this.Instruction1;
      addressStatus.Instruction2 = this.Instruction2;
      addressStatus.Address = this.Address;
      addressStatus.SubAddress = this.SubAddress;
      addressStatus.Write = this.Write;
      return (object) addressStatus;
    }

    public override byte[] Serialize()
    {
      if (this.md != Module.MODE.CO2 && this.md != Module.MODE.CO2TEMP && this.md != Module.MODE.HVAC)
        return base.Serialize();
      byte[] numArray = new byte[6];
      numArray[0] = (byte) 42;
      byte num = (byte) this.Type;
      if (this.Write)
				num += MonoUtils.SByte_MIN;
      numArray[1] = num;
      numArray[2] = this.Instruction1;
      numArray[3] = this.Instruction2;
      numArray[4] = (byte) 35;
      numArray[numArray.Length - 1] = (byte) 35;
      return numArray;
    }

    public override bool EqualAddress(Command cmd)
    {
      return this.Type == cmd.Type && cmd.GetType() == typeof (AddressStatus) && ((int) this.Address == (int) ((AddressStatus) cmd).Address && this.SubAddress == ((AddressStatus) cmd).SubAddress);
    }

    public enum SUBADDRESS
    {
      FIRST = 0,
      SECOND = 1,
      THIRD = 2,
      FOURTH = 3,
      ALL = 255,
    }
  }
}
