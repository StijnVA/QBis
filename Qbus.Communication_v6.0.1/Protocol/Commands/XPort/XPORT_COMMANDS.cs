// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.XPort.XPORT_COMMANDS
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

namespace Qbus.Communication.Protocol.Commands.XPort
{
  internal enum XPORT_COMMANDS
  {
    VERIFY_PASSWORD = 0,
    READ_PASSWORD = 1,
    STRING_DATA = 2,
    INITIALISATION_VECTOR = 3,
    ENCRYPTION_KEY = 4,
    WRITE_PASSWORD = 129,
    ERROR = 255,
  }
}
