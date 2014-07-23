// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.AddressMode
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication;
using Qbus.Communication.Protocol;
using System.Text;

namespace Qbus.Communication.Protocol.Commands
{
  public class AddressMode
  {
    public byte Address { get; set; }

    public Module.MODE Mode { get; set; }

    public Controller Controller { get; set; }

    public AddressMode(byte addr, Module.MODE m, Controller c)
    {
      this.Address = addr;
      this.Mode = m;
      this.Controller = c;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("A").Append(this.Address.ToString()).Append(" : ").Append(((object) this.Mode).ToString());
      return ((object) stringBuilder).ToString();
    }
  }
}
