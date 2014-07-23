// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Downloaders.Log
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using System;

namespace Qbus.Communication.Protocol.Downloaders
{
  public class Log
  {
    public DateTime Date { get; set; }

    public int Address { get; set; }

    public byte Sub1 { get; set; }

    public byte Sub2 { get; set; }

    public byte Sub3 { get; set; }

    public byte Sub4 { get; set; }

    public Log Parse(byte[] data, int startIdx)
    {
      if (data.Length < startIdx + 10)
        return this;
      this.Date = new DateTime(DateTime.Now.Year, int.Parse(data[startIdx].ToString("X")), int.Parse(data[startIdx + 1].ToString("X")), int.Parse(data[startIdx + 2].ToString("X")), int.Parse(data[startIdx + 3].ToString("X")), int.Parse(data[startIdx + 4].ToString("X")));
      this.Address = (int) data[startIdx + 5];
      this.Sub1 = data[startIdx + 6];
      this.Sub2 = data[startIdx + 7];
      this.Sub3 = data[startIdx + 8];
      this.Sub4 = data[startIdx + 9];
      return this;
    }
  }
}
