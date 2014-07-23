// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Downloaders.DOWNLOAD_STATUS
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

namespace Qbus.Communication.Protocol.Downloaders
{
  public enum DOWNLOAD_STATUS
  {
    READY,
    HEADER,
    WAITING,
    RECEIVING,
    RESTORING,
    FINISHED,
    ERROR,
    CANCELLED,
  }
}
