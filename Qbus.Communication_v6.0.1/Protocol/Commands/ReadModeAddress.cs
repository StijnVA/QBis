// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.ReadModeAddress
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qbus.Communication.Protocol.Commands
{
  public class ReadModeAddress : Command
  {
    private byte _maxAddress;
    private SortedList<byte, AddressMode> _modes;

    public override int Type
    {
      get
      {
        return 59;
      }
    }

    public byte UsedAddresses
    {
      get
      {
        return this._maxAddress;
      }
    }

    public SortedList<byte, AddressMode> Modes
    {
      get
      {
        return this._modes;
      }
    }

    public ReadModeAddress()
    {
      this.Write = false;
      this.Instruction1 = (byte) 0;
      this.Instruction2 = (byte) 0;
      this.Data = new byte[1];
      this.Data[0] = (byte) 21;
    }

    public override void Parse(byte[] data)
    {
      if (data.Length < 6)
        throw new Exception("Tried to parse a AddressStatus command while it is not");
      if (data.Length != (int) data[5] + 8)
        throw new Exception("Tried to parse a AddressStatus command while it is not");
      byte num = data[5];
      this.Data = new byte[(int) num + 1];
      for (int index = 0; index < (int) num + 1; ++index)
        this.Data[index] = data[6 + index];
      this._modes = new SortedList<byte, AddressMode>();
      for (int index = 0; index < this.Data.Length; ++index)
      {
        if (this._modes.ContainsKey((byte) (index + 3)))
          throw new Exception("Multiple modes on the same address");
        this._modes.Add((byte) (index + 3), new AddressMode((byte) (index + 3), (Module.MODE) this.Data[index], this.Controller));
      }
      this._maxAddress = byte.Parse(this._modes.Count.ToString());
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this._modes.Count.ToString()).Append(" used addresses:\n");
      for (int index = 0; index < this._modes.Count; ++index)
        stringBuilder.Append(this._modes.Values[index].ToString()).Append("\n");
      return ((object) stringBuilder).ToString();
    }

    public override object Clone()
    {
      return (object) new ControllerParameters();
    }

    public override bool EqualAddress(Command cmd)
    {
      int type = this.Type;
      return cmd.Type == this.Type;
    }
  }
}
