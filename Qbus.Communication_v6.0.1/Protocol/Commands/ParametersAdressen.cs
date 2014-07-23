// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.ParametersAdressen
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System.Collections.Generic;
using System.Text;

namespace Qbus.Communication.Protocol.Commands
{
  public class ParametersAdressen : Command
  {
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

    public Module CreateModule { get; set; }

    public int[] ModeParamProp { get; set; }

    public List<int> ModeParamList { get; set; }

    public override int Type
    {
      get
      {
        return 2;
      }
    }

    public ParametersAdressen()
    {
      this.Write = false;
      this.Instruction2 = (byte) 0;
    }

    public override void Parse(byte[] data)
    {
      this.Write = false;
      if ((int) data[0] > 128)
        this.Write = true;
      this.Address = data[1];
      this.Instruction2 = data[2];
      int num = (int) data[5];
      this.Data = new byte[data.Length - 6];
      if (data.Length <= 6)
      {
        this.Data = data;
      }
      else
      {
        this.Address = data[1];
        this.SubAddress = (byte) 0;
        for (int index = 6; index < data.Length; ++index)
          this.Data[index - 6] = data[index];
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Parameters adressen\n");
      stringBuilder.Append("Address: ").Append(this.Instruction1).Append(":");
      foreach (byte num in this.Data)
      {
        stringBuilder.Append(num);
        stringBuilder.Append("|");
      }
      return ((object) stringBuilder).ToString();
    }

    public override bool EqualAddress(Command cmd)
    {
      return this.Type == cmd.Type && cmd.GetType() == typeof (ParametersAdressen) && ((int) this.Instruction1 == (int) cmd.Instruction1 && (int) this.Instruction2 == (int) cmd.Instruction2);
    }

    public override object Clone()
    {
      ParametersAdressen parametersAdressen = new ParametersAdressen();
      parametersAdressen.Data = this.Data;
      parametersAdressen.Instruction1 = this.Instruction1;
      parametersAdressen.Instruction2 = this.Instruction2;
      parametersAdressen.Address = this.Address;
      parametersAdressen.SubAddress = this.SubAddress;
      parametersAdressen.Write = this.Write;
      return (object) parametersAdressen;
    }

    public override byte[] Serialize()
    {
      byte num = (byte) this.Type;
      byte[] numArray = new byte[6];
      if (!this.Write)
      {
        numArray[0] = (byte) 42;
        numArray[1] = num;
        numArray[2] = this.Address;
        numArray[3] = (byte) 0;
        numArray[4] = (byte) 31;
        numArray[numArray.Length - 1] = (byte) 35;
      }
      return numArray;
    }
  }
}
