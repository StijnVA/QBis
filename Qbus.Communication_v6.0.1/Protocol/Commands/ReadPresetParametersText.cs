// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.ReadPresetParametersText
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication;
using Qbus.Communication.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qbus.Communication.Protocol.Commands
{
  public class ReadPresetParametersText : Command
  {
    private SortedList<byte, PresetAddressMode> _modes;

    public string Texts { get; set; }

    public byte IN_LINK { get; set; }

    public byte DELAY_PR1 { get; set; }

    public byte DELAY_NXT_PR { get; set; }

    public SortedList<byte, PresetAddressMode> Modes
    {
      get
      {
        return this._modes;
      }
    }

    public override int Type
    {
      get
      {
        return 19;
      }
    }

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

    public string Name
    {
      get
      {
        return ((object) this.Texts).ToString();
      }
    }

    public ReadPresetParametersText()
    {
      this.Write = false;
      this.Address = (byte) 1;
      this.SubAddress = (byte) 0;
      this.Data = new byte[1];
      this.Data[0] = (byte) 18;
    }

    public override void Parse(byte[] data)
    {
      if (data.Length < 6)
        throw new Exception("Tried to parse a AddressStatus command while it is not");
      if (data.Length != (int) data[5] + 8)
        throw new Exception("Tried to parse a AddressStatus command while it is not");
      this.Address = data[1];
      byte num = data[5];
      this.Data = new byte[(int) num + 1];
      for (int index = 0; index < (int) num + 1; ++index)
        this.Data[index] = data[6 + index];
      Encoding encoding = Encoding.GetEncoding(1252);
      this.Texts = string.Empty;
      this.Texts = encoding.GetString(this.Data, 0, 16).Trim().ToLower();
      this.IN_LINK = this.Data[16];
      this.DELAY_PR1 = this.Data[17];
      this.DELAY_NXT_PR = this.Data[18];
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("MODE / ADDRESS: \n");
      stringBuilder.Append("P").Append(((int) this.Address).ToString()).Append(" Name :").Append(((object) this.Texts).ToString()).Append(" | Delay Preset: ").Append((int) this.DELAY_PR1 * 100).Append("ms | Delay to next preset: ").Append((int) this.DELAY_NXT_PR * 100).Append("ms\n");
      return ((object) stringBuilder).ToString();
    }

    public override object Clone()
    {
      ReadPresetParametersText presetParametersText = new ReadPresetParametersText();
      presetParametersText.Data = this.Data;
      presetParametersText.Instruction1 = this.Instruction1;
      presetParametersText.Instruction2 = this.Instruction2;
      presetParametersText.Address = this.Address;
      presetParametersText.SubAddress = this.SubAddress;
      presetParametersText.Write = this.Write;
      return (object) presetParametersText;
    }

    public override bool EqualAddress(Command cmd)
    {
      return this.Type == cmd.Type && cmd.GetType() == typeof (ReadPresetParametersText) && ((int) this.Address == (int) ((ReadPresetParametersText) cmd).Address && (int) this.SubAddress == (int) ((ReadPresetParametersText) cmd).SubAddress);
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
      numArray[4] = (byte) 18;
      numArray[5] = (byte) 35;
      return numArray;
    }
  }
}
