// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.XPort.ERROR_TYPES
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

namespace Qbus.Communication.Protocol.Commands.XPort
{
  public enum ERROR_TYPES
  {
    UNKNOWN_TCP_PACKET = 1,
    QBUS_TCP_PACKET_SHORT = 2,
    NO_START_CHARACTER_FOUND = 3,
    NO_END_CHARACTER_FOUND = 4,
    DATALENGTH_NOT_RIGHT = 5,
    LOGIN_INCORRECT = 6,
    LOGIN_RECEIVE_TIMEOUT = 7,
    SYSTEM_MANAGER_ACTIVE = 8,
    SYSTEM_MANAGER_CONNECTED = 9,
    SYSTEM_MANAGER_DISCONNECTED = 10,
    STACK_FULL = 11,
    COMMAND_TIMEOUT = 12,
  }
}
