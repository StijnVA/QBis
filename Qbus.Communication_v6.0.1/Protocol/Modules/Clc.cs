// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Modules.Clc
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;

namespace Qbus.Communication.Protocol.Modules
{
  public class Clc : Module
  {
    public Clc()
    {
      this.Name = "";
      this.Address = (byte) 3;
      this.Subaddress = (byte) 0;
      this._m = Module.MODE.CLC;
    }

    public override bool EqualModule(Module m)
    {
      throw new NotImplementedException();
    }

    public override bool EqualState(Module m)
    {
      throw new NotImplementedException();
    }
  }
}
