// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.Version
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;

namespace Qbus.Communication.Protocol.Commands
{
  public class Version : Command
  {
    public override int Type
    {
      get
      {
        return 7;
      }
    }

    public string VersionNumber { get; set; }

    public string SerialNumber { get; set; }

    public byte[] Serial { get; set; }

    public Version()
    {
      this.Write = false;
      this.Instruction1 = (byte) 0;
      this.Instruction2 = (byte) 0;
      this.Data = new byte[1];
      this.Data[0] = (byte) 4;
      this.VersionNumber = "";
      this.SerialNumber = "";
    }

    public override string ToString()
    {
      return "VERSION: " + this.VersionNumber + " SERIAL: " + this.SerialNumber;
    }

    public override void Parse(byte[] data)
    {
      if ((int) data[0] != (int) (byte) this.Type)
        throw new Exception("Tried to parse a Version command while it is not");
      if (data.Length < 3)
        throw new Exception("Data length not valid for a Version command");
      this.Instruction1 = data[1];
      this.Instruction2 = data[2];
      if (data.Length < 13)
        return;
      this.SerialNumber = this.GetMinChar(data[6], 2) + this.GetMinChar(data[7], 2) + this.GetMinChar(data[8], 2);
      this.Serial = new byte[3];
      this.Serial[0] = data[6];
      this.Serial[1] = data[7];
      this.Serial[2] = data[8];
      this.VersionNumber = this.GetMinChar(data[11], 2) + "." + this.GetMinChar(data[12], 2);
    }

    private string GetMinChar(byte b, int len)
    {
      string str = b.ToString();
      while (len > str.Length)
        str = "0" + str;
      return str;
    }

    public override object Clone()
    {
      Version version = new Version();
      version.Instruction1 = this.Instruction1;
      version.Instruction2 = this.Instruction2;
      version.Data = this.Data;
      version.VersionNumber = this.VersionNumber;
      version.SerialNumber = this.SerialNumber;
      return (object) version;
    }

    public override bool EqualAddress(Command cmd)
    {
      return cmd.GetType() == typeof (Version);
    }
  }
}
