// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Downloaders.IDownloader
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using System;

namespace Qbus.Communication.Protocol.Downloaders
{
  public interface IDownloader
  {
    double Progress { get; }

    DOWNLOAD_STATUS Status { get; set; }

    event EventHandler StatusChanged;

    event EventHandler Progressed;
  }
}
