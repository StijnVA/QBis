// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.ReceivedData
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using System;
using System.Text;

namespace Qbus.Communication
{
  public class ReceivedData : EventArgs
  {
    public byte Destination { get; set; }

    public Controller Ctd { get; set; }

    public byte[] Data { get; set; }

    public byte ReadWrite { get; set; }

    public ReceivedData(Controller c, byte Destination, byte ReadWrite, byte[] Data)
    {
      this.Ctd = c;
      this.Data = Data;
      this.Destination = Destination;
      this.ReadWrite = ReadWrite;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.Ctd.ToString()).Append(":");
      if ((int) this.ReadWrite == (int) byte.MaxValue)
        stringBuilder.Append("TX: ");
      else
        stringBuilder.Append("RX: ");
      for (int index = 0; index < this.Data.Length; ++index)
      {
        if ((int) this.Data[index] < 100)
          stringBuilder.Append("0");
        if ((int) this.Data[index] < 10)
          stringBuilder.Append("0");
        stringBuilder.Append(this.Data[index]);
        if (index != this.Data.Length - 1)
          stringBuilder.Append("|");
      }
      stringBuilder.Append("\n");
      return ((object) stringBuilder).ToString();
    }
  }
}
