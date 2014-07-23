// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.XPort.Login
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;
using System.Text;

namespace Qbus.Communication.Protocol.Commands.XPort
{
  internal class Login : Command
  {
    private XPORT_COMMANDS _type;
    private string _user;
    private string _password;

    public override int Type
    {
      get
      {
        return (int) this._type;
      }
    }

    public bool Success { get; set; }

    public string User
    {
      get
      {
        if (this._user == null)
          this._user = "";
        return this._user;
      }
      set
      {
        if (value == null)
          return;
        if (value.Length > 16)
          throw new Exception("Username is limited to 16 characters");
        this._user = value;
      }
    }

    public string Password
    {
      get
      {
        if (this._password == null)
          this._password = "";
        return this._password;
      }
      set
      {
        if (value == null)
          return;
        if (value.Length > 16)
          throw new Exception("Password is limited to 16 characters");
        this._password = value;
      }
    }

    public override void Parse(byte[] data)
    {
      if (data.Length < 34)
      {
        if (data.Length != 3)
          return;
        this._type = (XPORT_COMMANDS) data[0];
        if ((int) data[1] == 0)
          this.Success = true;
        else
          this.Success = false;
      }
      else
      {
        Encoding encoding = Encoding.GetEncoding(1252);
        this.Data = data;
        this._type = (XPORT_COMMANDS) data[0];
        this.User = encoding.GetString(data, 1, 16);
        this.Password = encoding.GetString(data, 17, 16);
      }
    }

    public override byte[] Serialize()
    {
      byte[] numArray = new byte[35];
      numArray[0] = (byte) 42;
      numArray[1] = (byte) this.Type;
      Encoding encoding = Encoding.GetEncoding(1252);
      if (this.User.Length > 16)
        this.User = this.User.Substring(0, 16);
      if (this.Password.Length > 16)
        this.Password = this.Password.Substring(0, 16);
      encoding.GetBytes(this.User).CopyTo((Array) numArray, 2);
      encoding.GetBytes(this.Password).CopyTo((Array) numArray, 18);
      numArray[numArray.Length - 1] = (byte) 35;
      return numArray;
    }

    public override string ToString()
    {
      if (this.User != "" && this.Password != "")
        return "Login: " + this.User + " password: " + this.Password;
      else
        return "Login success: " + this.Success.ToString();
    }

    public override bool EqualAddress(Command cmd)
    {
      return false;
    }

    public override object Clone()
    {
      Login login = new Login();
      login.Parse(this.Data);
      login.User = this.User;
      login.Password = this.Password;
      return (object) login;
    }
  }
}
