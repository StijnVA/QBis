// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.AddressText
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;
using System.Text;

namespace Qbus.Communication.Protocol.Commands
{
  public class AddressText : Command
  {
    public byte TextCount { get; set; }

    public string Texts { get; set; }

    public byte Address { get; set; }

    public byte SubAddress { get; set; }

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
        return this.SubAddress;
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
        return 4;
      }
    }

    public AddressText()
    {
      this.Write = false;
      this.Address = (byte) 3;
      this.SubAddress = (byte) 0;
      this.Data = new byte[1];
      this.Data[0] = (byte) 15;
    }

    public override string ToString()
    {
      return "ADDRESS " +  this.Address + " - " +  this.SubAddress + ": " + this.Texts;
    }

    public override void Parse(byte[] data)
    {
      if (data.Length < 5)
        return;
      if ((int) data[0] != (int) (byte) this.Type && (int) data[0] - 128 != (int) (byte) this.Type)
        throw new Exception("Tried to parse a Worktext command while it is not");
      this.Address = data[1];
      this.SubAddress = data[2];
      if (data.Length == 6 && (int) data[4] == 0)
      {
        this.Texts = "Write text complete on address: " + data[1].ToString() + "." + data[2].ToString();
      }
      else
      {
        this.TextCount = (byte) ((uint) data[3] / 15U);
        if (data.Length <= 5)
          return;
        if ((int) data[4] != 0)
          throw new Exception("Tried to parse a Worktext command: echo is not 0");
        int length = (int) data[3] + 1;
        if (length > data.Length + 6)
          throw new Exception("Tried to parse a Worktext command: Length is not right");
        this.Data = new byte[length];
        for (int index = 0; index < length; ++index)
          this.Data[index] = data[6 + index];
        Encoding encoding = Encoding.GetEncoding(1252);
        this.Texts = string.Empty;
        this.Texts = encoding.GetString(this.Data).Trim().ToLower();
      }
    }

    public override object Clone()
    {
      AddressText addressText = new AddressText();
      addressText.Data = this.Data;
      addressText.Instruction1 = this.Instruction1;
      addressText.Instruction2 = this.Instruction2;
      addressText.Address = this.Address;
      addressText.SubAddress = this.SubAddress;
      addressText.Write = this.Write;
      return (object) addressText;
    }

    public override bool EqualAddress(Command cmd)
    {
      return this.Type == cmd.Type && cmd.GetType() == typeof (AddressText) && ((int) this.Address == (int) ((AddressText) cmd).Address && (int) this.SubAddress == (int) ((AddressText) cmd).SubAddress);
    }

    public override byte[] Serialize()
    {
      byte[] numArray = this.Write ? new byte[22] : new byte[6];
      numArray[0] = (byte) 42;
      byte num = (byte) this.Type;
      if (this.Write)
				num += MonoUtils.SByte_MIN;
      numArray[1] = num;
      numArray[2] = this.Instruction1;
      numArray[3] = this.Instruction2;
      numArray[4] = (byte) 15;
      if (this.Write)
      {
        for (int index = 0; index < this.Texts.Length; ++index)
          numArray[index + 5] = (byte) this.Texts[index];
        for (int index = this.Texts.Length + 5; index < numArray.Length; ++index)
          numArray[index] = (byte) 32;
      }
      numArray[numArray.Length - 1] = (byte) 35;
      return numArray;
    }
  }
}
