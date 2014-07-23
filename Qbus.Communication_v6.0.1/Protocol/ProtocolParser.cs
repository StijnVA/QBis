// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.ProtocolParser
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication;
using Qbus.Communication.Protocol.Commands;
using Qbus.Communication.Protocol.Commands.XPort;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qbus.Communication.Protocol
{
  internal class ProtocolParser
  {
    private static int TYPE = 1;
    private static int LENGTH1 = 4;
    private static int ERROR = 5;
    private static int LENGTH2 = 6;
    private static int STATUS = 7;
    private static int FIX_LENGTH = 6;
    private static byte[] W_SERVICE = new byte[1]
    {
			MonoUtils.SByte_MIN
    };
    private static byte[] W_VAR_LENGTH = new byte[3]
    {
      (byte) 168,
      (byte) 169,
      (byte) 170
    };
    private static byte[] W_8_LENGTH = new byte[1]
    {
      (byte) 186
    };
    private static byte[] W_STATUS = new byte[1]
    {
      (byte) 184
    };
    private static byte[] W_VAR_DOUBLE = new byte[1]
    {
      (byte) 140
    };
    private static byte[] R_7_LENGTH = new byte[7]
    {
      (byte) 38,
      (byte) 43,
      (byte) 44,
      (byte) 45,
      (byte) 46,
      (byte) 47,
      (byte) 48
    };
    private static byte[] R_VAR_DOUBLE = new byte[7]
    {
      (byte) 12,
      (byte) 52,
      (byte) 54,
      (byte) 62,
      (byte) 64,
      (byte) 65,
      (byte) 68
    };
    private static byte[] R_VAR_QUATRO = new byte[1]
    {
      (byte) 17
    };
    private static byte[] R_MODULE = new byte[3]
    {
      (byte) 40,
      (byte) 41,
      (byte) 42
    };

    public static List<Command> ParseTCP(ref byte[] buffer, byte[] header, Controller c)
    {
      List<Command> list1 = new List<Command>();
      if (buffer == null || buffer.Length == 0)
        return list1;
      int length1 = buffer.Length;
      int num1 = ByteArrayUtils.Find(buffer, header, 0, length1);
      int num2 = 0;
      if (num1 == -1 && length1 > 0)
      {
        Echo echo = new Echo();
        echo.Data = buffer;
        echo.Controller = c;
        list1.Add((Command) echo);
      }
			int startIndex;
      for (; num1 >= 0; 
        
        num1 = ByteArrayUtils.Find(buffer, header, startIndex, length1 - startIndex)
      
      )
      {
        num2 = num1;
        int index1 = num1 + 12;
        startIndex = index1;
        if (buffer.Length > index1 && (int) buffer[index1] == 42)
        {
          int length2 = ((int) buffer[index1 - 2] << 8) + (int) buffer[index1 - 1];
          int index2 = index1 + length2 - 1;
          bool flag = false;
          if ((int) buffer[index2] != 35)
          {
            for (int index3 = index1; index3 < buffer.Length; ++index3)
            {
              if ((int) buffer[index3] == 35)
              {
                index2 = index3;
                length2 = index2 - index1 + 1;
                flag = true;
              }
            }
          }
          if (buffer.Length > index2 && (int) buffer[index2] == 35)
          {
            byte[] buffer1 = new byte[length2];
            for (int index3 = 0; index3 < length2; ++index3)
              buffer1[index3] = buffer[index1 + index3];
            bool xport = (int) buffer[num1 + 9] == 250;
            List<Command> list2 = ProtocolParser.ParseCommands(ref buffer1, xport, c);
            if (list2.Count > 0)
            {
              list1.AddRange((IEnumerable<Command>) list2);
              startIndex = index1 + length2;
              num2 = index2 + 1;
            }
            else if (flag)
            {
              startIndex = index1 + length2;
              num2 = index2 + 1;
            }
          }
        }
      }
      if (num2 == -1)
      {
        buffer = new byte[0];
      }
      else
      {
        byte[] numArray = new byte[buffer.Length - num2];
        for (int index = 0; index < numArray.Length; ++index)
          numArray[index] = buffer[index + num2];
        buffer = numArray;
      }
      return list1;
    }

    public static List<Command> ParseCommands(ref byte[] buffer, bool xport, Controller c)
    {
      List<Command> list = new List<Command>();
      int startIndex = 0;
      int sourceIndex = 0;
      for (int sIdx = Array.IndexOf<byte>(buffer, (byte) 42, startIndex); sIdx != -1; sIdx = Array.IndexOf<byte>(buffer, (byte) 42, sIdx + 1))
      {
        try
        {
          if (!xport)
          {
            int commandLength = ProtocolParser.GetCommandLength(buffer, sIdx);
            if (buffer.Length > sIdx + commandLength)
            {
              if ((int) buffer[sIdx + commandLength] == 35)
              {
                byte[] numArray = new byte[commandLength];
                Array.Copy((Array) buffer, sIdx + 1, (Array) numArray, 0, commandLength);
                Command command = ProtocolParser.GetCommand(numArray);
                command.Controller = c;
                if (command != null)
                {
                  command.Parse(numArray);
                  list.Add(command);
                  sourceIndex = sIdx + commandLength + 1;
                  sIdx = sourceIndex - 1;
                }
              }
            }
          }
          else
          {
            byte[] numArray = new byte[buffer.Length - 1];
            Array.Copy((Array) buffer, 1, (Array) numArray, 0, buffer.Length - 1);
            Command xportCommand = ProtocolParser.GetXPORTCommand(numArray);
            xportCommand.Controller = c;
            if (xportCommand != null)
            {
              xportCommand.Parse(numArray);
              list.Add(xportCommand);
              sourceIndex = buffer.Length - 1;
            }
          }
        }
        catch (Exception ex)
        {
          ConnectionManager.Instance.ErrorHandle(ex, WARNING_TYPES.ERROR_MID);
        }
      }
      byte[] numArray1 = new byte[buffer.Length - sourceIndex];
      Array.Copy((Array) buffer, sourceIndex, (Array) numArray1, 0, buffer.Length - sourceIndex);
      buffer = numArray1;
      return list;
    }

    private static int GetCommandLength(byte[] buffer, int sIdx)
    {
      if (buffer.Length < sIdx + ProtocolParser.FIX_LENGTH)
        return 1024;
      byte num = buffer[sIdx + ProtocolParser.TYPE];
      return (int) num >= 128 ? (Array.IndexOf<byte>(ProtocolParser.W_SERVICE, num) == -1 ? (Array.IndexOf<byte>(ProtocolParser.W_VAR_LENGTH, num) == -1 ? (Array.IndexOf<byte>(ProtocolParser.W_8_LENGTH, num) == -1 ? (Array.IndexOf<byte>(ProtocolParser.W_STATUS, num) == -1 ? (Array.IndexOf<byte>(ProtocolParser.W_VAR_DOUBLE, num) == -1 ? ProtocolParser.FIX_LENGTH : ProtocolParser.FIX_LENGTH + ((int) buffer[sIdx + ProtocolParser.LENGTH1] + 1) * 2) : ProtocolParser.FIX_LENGTH + (int) buffer[sIdx + ProtocolParser.STATUS] + 3) : ProtocolParser.FIX_LENGTH + 1) : ProtocolParser.FIX_LENGTH + (int) buffer[sIdx + ProtocolParser.LENGTH2] + 2) : ProtocolParser.FIX_LENGTH + 1) : ((int) num != 0 ? ((int) num != 80 ? ((int) num != 53 ? (Array.IndexOf<byte>(ProtocolParser.R_7_LENGTH, num) == -1 ? (Array.IndexOf<byte>(ProtocolParser.R_MODULE, num) == -1 ? (Array.IndexOf<byte>(ProtocolParser.R_VAR_QUATRO, num) == -1 ? (Array.IndexOf<byte>(ProtocolParser.R_VAR_DOUBLE, num) == -1 ? ((int) buffer[sIdx + ProtocolParser.ERROR] == 0 ? ProtocolParser.FIX_LENGTH + (int) buffer[sIdx + ProtocolParser.LENGTH2] + 2 : ProtocolParser.FIX_LENGTH) : ((int) buffer[sIdx + ProtocolParser.ERROR] == 0 ? ((int) num != 54 ? ProtocolParser.FIX_LENGTH + (((int) buffer[sIdx + ProtocolParser.LENGTH2] + 1) * 2 + 1) : ProtocolParser.FIX_LENGTH + ((int) buffer[sIdx + ProtocolParser.LENGTH2] * 2 + 1)) : ProtocolParser.FIX_LENGTH)) : ((int) buffer[sIdx + ProtocolParser.ERROR] == 0 ? ProtocolParser.FIX_LENGTH + ((int) buffer[sIdx + ProtocolParser.LENGTH2] + 1) * 4 + 1 : ProtocolParser.FIX_LENGTH)) : ((int) buffer[sIdx + ProtocolParser.ERROR] == 0 ? ProtocolParser.FIX_LENGTH + (int) buffer[sIdx + ProtocolParser.LENGTH2] + 2 : ProtocolParser.FIX_LENGTH + (int) buffer[sIdx + ProtocolParser.LENGTH2] + 1 + 3)) : ((int) buffer[sIdx + ProtocolParser.LENGTH1] != 0 ? ProtocolParser.FIX_LENGTH + (int) buffer[sIdx + ProtocolParser.LENGTH2] + 2 : ProtocolParser.FIX_LENGTH)) : ProtocolParser.FIX_LENGTH + 1) : ProtocolParser.FIX_LENGTH) : 8 + (int) buffer[sIdx + ProtocolParser.ERROR] + 1);
    }

    public static List<Command> Parse(ref string s, Encoding enc, Controller c)
    {
      byte[] bytes = enc.GetBytes(s);
      List<Command> list = ProtocolParser.ParseCommands(ref bytes, false, c);
      s = enc.GetString(bytes);
      return list;
    }

    public static Command GetXPORTCommand(byte[] cmd)
    {
      switch (cmd[0])
      {
        case (byte) 0:
          return (Command) new Login();
        case (byte) 1:
          return (Command) new Login();
        case (byte) 2:
          return (Command) new StringData();
        case (byte) 3:
          return (Command) new EncryptionKey();
        case (byte) 4:
          return (Command) new EncryptionKey();
        case (byte) 129:
          return (Command) new Login();
        case byte.MaxValue:
          return (Command) new Error();
        default:
          return (Command) null;
      }
    }

    public static Command GetCommand(byte[] cmd)
    {
      byte num1 = cmd[0];
      if ((int) num1 > 128)
				num1 -= MonoUtils.SByte_MIN;
      byte num2 = num1;
      if ((uint) num2 <= 19U)
      {
        switch (num2)
        {
          case (byte) 1:
            return (Command) new ControllerParameters();
          case (byte) 2:
            return (Command) new ParametersAdressen();
          case (byte) 3:
            return (Command) new WorkText();
          case (byte) 4:
            return (Command) new AddressText();
          case (byte) 7:
            return (Command) new Qbus.Communication.Protocol.Commands.Version();
          case (byte) 9:
            return (Command) new FatData();
          case (byte) 13:
            return (Command) new ControllerOptions();
          case (byte) 19:
            return (Command) new ReadPresetParametersText();
        }
      }
      else
      {
        switch (num2)
        {
          case (byte) 26:
            return (Command) new EventLogs();
          case (byte) 52:
            return (Command) new EventRead();
          case (byte) 53:
            return (Command) new EventStatus();
          case (byte) 56:
            return (Command) new AddressStatus();
          case (byte) 59:
            return (Command) new ReadModeAddress();
        }
      }
      return (Command) null;
    }
  }
}
