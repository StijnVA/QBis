// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.XPort.EncryptionKey
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication;
using Qbus.Communication.Protocol;

namespace Qbus.Communication.Protocol.Commands.XPort
{
  internal class EncryptionKey : Command
  {
    private byte _type = (byte) 4;

    public override int Type
    {
      get
      {
        return (int) this._type;
      }
    }

    public byte[] OriginalKey { get; set; }

    public byte[] Key { get; set; }

    public override void Parse(byte[] data)
    {
      if (data.Length == 0)
        return;
      this._type = data[0];
      if (data.Length < 33)
        return;
      byte[] key = new byte[32];
      for (int index = 0; index < 32; ++index)
        key[index] = data[index + 1];
      this.OriginalKey = key;
      this.Key = CommunicationEncryption.Instance.DecryptKey(key);
    }

    public override string ToString()
    {
      return "Encryption key: " + (object) this.Key;
    }

    public override bool EqualAddress(Command cmd)
    {
      return false;
    }

    public override object Clone()
    {
      return (object) new EncryptionKey()
      {
        OriginalKey = this.OriginalKey,
        Key = this.Key
      };
    }
  }
}
