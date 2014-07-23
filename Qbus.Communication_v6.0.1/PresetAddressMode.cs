// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.PresetAddressMode
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using System.Text;

namespace Qbus.Communication
{
  public class PresetAddressMode
  {
    public byte Address { get; set; }

    public Controller Controller { get; set; }

    public PresetAddressMode(byte addr, Controller c)
    {
      this.Address = addr;
      this.Controller = c;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("P").Append(this.Address.ToString()).Append(" : ").Append(this.Address.ToString());
      return ((object) stringBuilder).ToString();
    }
  }
}
