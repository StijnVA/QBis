// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.ControllerParameters
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using Qbus.Communication.Protocol;
using System;
using System.Text;

namespace Qbus.Communication.Protocol.Commands
{
  public class ControllerParameters : Command
  {
    public override int Type
    {
      get
      {
        return 1;
      }
    }

    public byte MaxAddress
    {
      get
      {
        return this.Data[0];
      }
    }

    public byte MaxPreset
    {
      get
      {
        return this.Data[1];
      }
    }

    public float MinBusVolt
    {
      get
      {
        return (float) this.Data[2] / 14f;
      }
    }

    public float MaxBusVolt
    {
      get
      {
        return (float) this.Data[3] / 14f;
      }
    }

    public float NomBusCurr
    {
      get
      {
        return (float) this.Data[4] * 2.5f;
      }
    }

    public float MaxBusCurr
    {
      get
      {
        return (float) this.Data[5] * 2.5f;
      }
    }

    public byte NomTemp
    {
      get
      {
        return this.Data[6];
      }
    }

    public byte MaxTemp
    {
      get
      {
        return this.Data[7];
      }
    }

    public float VtMinPower
    {
      get
      {
        return (float) this.Data[8] / 10.7f;
      }
    }

    public float VtMaxPower
    {
      get
      {
        return (float) this.Data[9] / 10.7f;
      }
    }

    public float VoltBus1
    {
      get
      {
        return (float) this.Data[10] / 14f;
      }
    }

    public float VoltBus2
    {
      get
      {
        return (float) this.Data[11] / 14f;
      }
    }

    public float VoltBus3
    {
      get
      {
        return (float) this.Data[16] / 14f;
      }
    }

    public float CurrBus1
    {
      get
      {
        return (float) this.Data[12] * 2.5f;
      }
    }

    public float CurrBus2
    {
      get
      {
        return (float) this.Data[13] * 2.5f;
      }
    }

    public float CurrBus3
    {
      get
      {
        return (float) this.Data[17] * 2.5f;
      }
    }

    public byte ControllerTemp
    {
      get
      {
        return this.Data[14];
      }
    }

    public float Power
    {
      get
      {
        return (float) this.Data[15] / 10.7f;
      }
    }

    public ControllerParameters()
    {
      this.Write = false;
      this.Instruction1 = (byte) 0;
      this.Instruction2 = (byte) 0;
      this.Data = new byte[1];
      this.Data[0] = (byte) 21;
    }

    public override void Parse(byte[] data)
    {
      if (data.Length < 6)
        throw new Exception("Tried to parse a AddressStatus command while it is not");
      if (data.Length != (int) data[5] + 8)
        throw new Exception("Tried to parse a AddressStatus command while it is not");
      byte num = data[5];
      this.Data = new byte[(int) num + 1];
      for (int index = 0; index < (int) num + 1; ++index)
        this.Data[index] = data[6 + index];
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("CONTROLLER:\n");
      stringBuilder.Append("Max Address: ").Append(this.MaxAddress.ToString()).Append("\n");
      stringBuilder.Append("Max Preset Address: ").Append(this.MaxPreset.ToString()).Append("\n");
      stringBuilder.Append("Busvoltage 1: ").Append(this.VoltBus1.ToString()).Append("V\n");
      stringBuilder.Append("Busvoltage 2: ").Append(this.VoltBus2.ToString()).Append("V\n");
      stringBuilder.Append("Busvoltage 3: ").Append(this.VoltBus3.ToString()).Append("V\n");
      stringBuilder.Append("Buscurrent 1: ").Append(this.CurrBus1.ToString()).Append("mA\n");
      stringBuilder.Append("Buscurrent 2: ").Append(this.CurrBus2.ToString()).Append("mA\n");
      stringBuilder.Append("Buscurrent 3: ").Append(this.CurrBus3.ToString()).Append("mA\n");
      stringBuilder.Append("Current temp: ").Append(this.ControllerTemp.ToString()).Append("°\n");
      stringBuilder.Append("Minimum bus voltage: ").Append(this.MinBusVolt.ToString()).Append("V\n");
      stringBuilder.Append("Maximum bus voltage: ").Append(this.MaxBusVolt.ToString()).Append("V\n");
      stringBuilder.Append("Nominal bus current: ").Append(this.NomBusCurr.ToString()).Append("mA\n");
      stringBuilder.Append("Maximal bus current: ").Append(this.MaxBusCurr.ToString()).Append("mA\n");
      stringBuilder.Append("Nominal temperature: ").Append(this.NomTemp.ToString()).Append("°\n");
      stringBuilder.Append("Maximum temperature: ").Append(this.MaxTemp.ToString()).Append("°\n");
      stringBuilder.Append("Minimum power: ").Append(this.VtMinPower.ToString()).Append("V\n");
      stringBuilder.Append("Maximum power: ").Append(this.VtMaxPower.ToString()).Append("V\n");
      stringBuilder.Append("Power: ").Append(this.Power.ToString()).Append("V\n");
      return ((object) stringBuilder).ToString();
    }

    public override object Clone()
    {
      return (object) new ControllerParameters();
    }

    public override bool EqualAddress(Command cmd)
    {
      int type = this.Type;
      return cmd.Type == this.Type;
    }
  }
}
