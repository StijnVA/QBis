// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.WorkText
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qbus.Communication.Protocol.Commands
{
  public class WorkText : Command
  {
    public override int Type
    {
      get
      {
        return 3;
      }
    }

    public byte TextCount { get; set; }

    public List<string> Texts { get; set; }

    public byte StartTextNum
    {
      get
      {
        return this.Instruction1;
      }
      set
      {
        this.Instruction1 = value;
      }
    }

    public override void Parse(byte[] data)
    {
      if (data.Length < 5)
        return;
      if ((int) data[0] != (int) (byte) this.Type && (int) data[0] - 128 != (int) (byte) this.Type)
        throw new Exception("Tried to parse a Worktext command while it is not");
      this.Instruction1 = data[1];
      this.Instruction2 = data[2];
      this.TextCount = (byte) ((uint) data[3] / 15U);
      if (data.Length <= 5)
        return;
      if ((int) data[4] != 0)
        throw new Exception("Tried to parse a Worktext command: echo is not 0");
      int length = (int) data[5] + 1;
      if (length > data.Length + 6)
        throw new Exception("Tried to parse a Worktext command: Length is not right");
      this.Data = new byte[length];
      for (int index = 0; index < length; ++index)
        this.Data[index] = data[6 + index];
      Encoding encoding = Encoding.GetEncoding(1252);
      this.Texts = new List<string>();
      for (int index = 0; index < length / 16; ++index)
      {
        try
        {
          this.Texts.Add(encoding.GetString(this.Data, index * 16, 16).Trim().ToLower());
        }
        catch
        {
        }
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("WorkText: \n");
      for (int index = 0; index < this.Texts.Count; ++index)
        stringBuilder.Append(((object) this.Texts[index]).ToString()).Append("\n");
      return ((object) stringBuilder).ToString();
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
      numArray[4] = (byte) ((int) this.TextCount * 16 - 1);
      numArray[5] = (byte) 35;
      return numArray;
    }

    public override bool EqualAddress(Command cmd)
    {
      return this.Type == cmd.Type && cmd.GetType() == typeof (WorkText) && ((int) this.Instruction1 == (int) cmd.Instruction1 && (int) this.Instruction2 == (int) cmd.Instruction2);
    }

    public override object Clone()
    {
      WorkText workText = new WorkText();
      workText.Data = this.Data;
      workText.Instruction1 = this.Instruction1;
      workText.Instruction2 = this.Instruction2;
      workText.Write = this.Write;
      return (object) workText;
    }
  }
}
