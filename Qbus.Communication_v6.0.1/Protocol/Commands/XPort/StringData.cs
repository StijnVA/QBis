// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.XPort.StringData
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;
using System.Text;

namespace Qbus.Communication.Protocol.Commands.XPort
{
  internal class StringData : Command
  {
    private string _txt;

    public override int Type
    {
      get
      {
        return 2;
      }
    }

    public bool Success { get; set; }

    public string Text
    {
      get
      {
        if (this._txt == null)
          this._txt = "";
        return this._txt;
      }
      set
      {
        if (value == null)
          return;
        this._txt = value;
      }
    }

    public override void Parse(byte[] data)
    {
      if (data.Length < 1)
        return;
      this.Data = data;
      this.Text = Encoding.GetEncoding(1252).GetString(data, 1, data.Length - 1);
    }

    public override byte[] Serialize()
    {
      byte[] numArray = new byte[this.Text.Length + 3];
      numArray[0] = (byte) 42;
      numArray[1] = (byte) 2;
      Encoding.GetEncoding(1252).GetBytes(this.Text).CopyTo((Array) numArray, 2);
      numArray[numArray.Length - 1] = (byte) 35;
      return numArray;
    }

    public override string ToString()
    {
      return "STRING: " + this.Text;
    }

    public override bool EqualAddress(Command cmd)
    {
      return false;
    }

    public override object Clone()
    {
      return (object) new StringData()
      {
        Text = this.Text
      };
    }
  }
}
