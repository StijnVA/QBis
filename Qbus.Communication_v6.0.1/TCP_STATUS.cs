// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.TCP_STATUS
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

namespace Qbus.Communication
{
  public enum TCP_STATUS
  {
    INITIATING = 1,
    LOGGING_IN = 2,
    LOGIN_FAILED = 3,
    CONNECTED = 4,
    SECURITY_INIT = 5,
    DISCONNECTED = 6,
    FAILED = 7,
    WRONG_XPORT_VERSION = 8,
  }
}
