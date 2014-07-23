// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Command
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication;
using System;
using System.Web.Script.Serialization;

namespace Qbus.Communication.Protocol
{
  public abstract class Command : ICloneable
  {
    public abstract int Type { get; }

    [ScriptIgnore]
    public Controller Controller { get; set; }

    public bool Write { get; set; }

    public virtual byte Instruction1 { get; set; }

    public virtual byte Instruction2 { get; set; }

    public virtual byte[] Data { get; set; }

    public Command()
    {
    }

    public Command(byte[] data)
    {
      this.Parse(data);
    }

    public virtual byte[] Serialize()
    {
      byte[] numArray = new byte[this.Data.Length + 5];
      numArray[0] = (byte) 42;
      byte num = (byte) this.Type;
      if (this.Write)
				num += MonoUtils.SByte_MIN;
      numArray[1] = num;
      numArray[2] = this.Instruction1;
      numArray[3] = this.Instruction2;
      if (this.Data.Length > 0)
        this.Data.CopyTo((Array) numArray, 4);
      numArray[numArray.Length - 1] = (byte) 35;
      return numArray;
    }

    public abstract void Parse(byte[] data);

    public abstract override string ToString();

    public abstract bool EqualAddress(Command cmd);

    public abstract object Clone();
  }
}
