// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Downloaders.TextDownloader
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication;
using Qbus.Communication.Protocol;
using Qbus.Communication.Protocol.Commands;
using System;
using System.Collections.Generic;

namespace Qbus.Communication.Protocol.Downloaders
{
  public class TextDownloader : IDownloader
  {
    private string _nameCTD = "";
    public List<string> Texts = new List<string>();
    private ControllerCommunication _comm;
    private byte bTextDownloader;
    private DOWNLOAD_STATUS _status;

    public string NameCTD
    {
      get
      {
        return this._nameCTD;
      }
    }

    public double Progress
    {
      get
      {
        return 0.0;
      }
    }

    public DOWNLOAD_STATUS Status
    {
      get
      {
        return this._status;
      }
      set
      {
        this._status = value;
      }
    }

    public event EventHandler StatusChanged;

    public event EventHandler Progressed;

    public TextDownloader(ControllerCommunication comm)
    {
      this._comm = comm;
      this.bTextDownloader = (byte) 5;
    }

    public void ResetTextDownloader()
    {
      this.bTextDownloader = (byte) 5;
    }

    private void _comm_CommandReceived(object sender, CommandEventArgs e)
    {
      if (e.Command.GetType() != typeof (WorkText))
        return;
      WorkText workText = (WorkText) e.Command;
      int num1 = (int) workText.TextCount;
      int num2 = (int) workText.StartTextNum;
      if ((int) workText.Instruction2 == 1)
        num2 = 38;
      if ((int) workText.Instruction2 == 2)
        num2 = 42;
      if ((int) workText.Instruction1 == 40)
        num2 = 44;
      int num3 = num2 + num1 - 1;
      string[] names = Enum.GetNames(typeof (TEXTS));
      for (int index = num2; index < num3; ++index)
        this._comm.Controller.Texts[names[index - 6]] = workText.Texts[index - num2];
      this.ChangeStatus(DOWNLOAD_STATUS.FINISHED);
      if ((int) this.bTextDownloader != 0)
      {
        this._comm.Controller.TextsInited = false;
        this.Start();
      }
      else
        this._comm.CommandReceived -= new ControllerCommunication.CommandEventHandler(this._comm_CommandReceived);
      this.Texts = workText.Texts;
    }

    private void ChangeStatus(DOWNLOAD_STATUS status)
    {
      this._status = status;
      if (this.StatusChanged == null)
        return;
      this.StatusChanged((object) this, new EventArgs());
    }

    public void Start()
    {
      this._comm.CommandReceived -= new ControllerCommunication.CommandEventHandler(this._comm_CommandReceived);
      this._comm.CommandReceived += new ControllerCommunication.CommandEventHandler(this._comm_CommandReceived);
      ConnectionManager.Instance.ErrorHandle(new Exception("Check texts on " + this._comm.Controller.ToString()), WARNING_TYPES.INFO);
      if (!this._comm.Controller.TextsInited)
      {
        if ((int) this.bTextDownloader == 5)
        {
          WorkText workText = new WorkText();
          workText.TextCount = (byte) 16;
          workText.StartTextNum = (byte) 6;
          workText.Controller = this._comm.Controller;
          this._comm.Send((Command) workText);
        }
        if ((int) this.bTextDownloader == 4)
        {
          this._comm.Send((Command) new WorkText()
          {
            TextCount = 16,
            StartTextNum = 22
          });
          this.ChangeStatus(DOWNLOAD_STATUS.READY);
        }
        if ((int) this.bTextDownloader == 3)
          this.HVACText();
        if ((int) this.bTextDownloader == 2)
          this.AudioBronText();
        if ((int) this.bTextDownloader == 1)
          this.IRText();
        --this.bTextDownloader;
      }
      else
        this.ChangeStatus(DOWNLOAD_STATUS.FINISHED);
    }

    public void AudioBronText()
    {
      WorkText workText = new WorkText();
      workText.TextCount = (byte) 4;
      workText.StartTextNum = (byte) 0;
      workText.Controller = this._comm.Controller;
      workText.Instruction2 = (byte) 1;
      this._comm.Send((Command) workText);
    }

    public void IRText()
    {
      WorkText workText = new WorkText();
      workText.TextCount = (byte) 2;
      workText.StartTextNum = (byte) 0;
      workText.Controller = this._comm.Controller;
      workText.Instruction2 = (byte) 2;
      this._comm.Send((Command) workText);
    }

    public void HVACText()
    {
      WorkText workText = new WorkText();
      workText.TextCount = (byte) 6;
      workText.StartTextNum = (byte) 40;
      workText.Controller = this._comm.Controller;
      workText.Instruction2 = (byte) 0;
      this._comm.Send((Command) workText);
    }
  }
}
