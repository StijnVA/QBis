// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.CommunicationEncryption
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using System;
using System.IO;
using System.Security.Cryptography;

namespace Qbus.Communication
{
  internal class CommunicationEncryption
  {
    private byte[] table1 = new byte[16]
    {
      (byte) 5,
      (byte) 4,
      (byte) 12,
      (byte) 10,
      (byte) 9,
      (byte) 1,
      (byte) 11,
      (byte) 13,
      (byte) 15,
      (byte) 2,
      (byte) 7,
      (byte) 8,
      (byte) 0,
      (byte) 6,
      (byte) 3,
      (byte) 14
    };
    private byte[] table2 = new byte[16]
    {
      (byte) 13,
      (byte) 9,
      (byte) 1,
      (byte) 7,
      (byte) 12,
      (byte) 6,
      (byte) 10,
      (byte) 15,
      (byte) 0,
      (byte) 8,
      (byte) 4,
      (byte) 2,
      (byte) 5,
      (byte) 3,
      (byte) 14,
      (byte) 11
    };
    private static CommunicationEncryption _instance;

    public static CommunicationEncryption Instance
    {
      get
      {
        if (CommunicationEncryption._instance == null)
          CommunicationEncryption._instance = new CommunicationEncryption();
        return CommunicationEncryption._instance;
      }
    }

    private CommunicationEncryption()
    {
    }

    private uint[] MakeUInt(byte[] val)
    {
      uint[] numArray = new uint[(int) Math.Ceiling((double) val.Length / 2.0)];
      for (int index = 0; index < numArray.Length; ++index)
      {
        uint num = 0U;
        if (val.Length > index * 2)
          num += (uint) val[index * 2];
        if (val.Length > index * 2 + 1)
          num += (uint) val[index * 2 + 1] << 8;
        numArray[index] = num;
      }
      return numArray;
    }

    private byte[] MakeByte(uint[] val)
    {
      byte[] numArray = new byte[val.Length * 2];
      for (int index = 0; index < val.Length; ++index)
      {
        uint num = val[index];
        numArray[index * 2] = (byte) (num % 256U);
        numArray[index * 2 + 1] = (byte) (num >> 8);
      }
      return numArray;
    }

    public byte[] DecryptKey(byte[] key)
    {
      uint[] numArray1 = this.DecryptKey(this.MakeUInt(key));
      byte[] numArray2 = new byte[numArray1.Length];
      for (int index = 0; index < numArray2.Length; ++index)
        numArray2[index] = (byte) numArray1[index];
      return numArray2;
    }

    public byte[] EncryptKey(byte[] key)
    {
      uint[] key1 = new uint[key.Length];
      for (int index = 0; index < key1.Length; ++index)
        key1[index] = (uint) key[index];
      return this.MakeByte(this.EncryptKey(key1));
    }

    private uint[] EncryptKey(uint[] key)
    {
      uint[] numArray = new uint[16];
      for (int index = 0; index < 16; ++index)
      {
        if (index == 0)
        {
          numArray[index] = this.Swap(key[index], index);
        }
        else
        {
          uint num = key[index - 1] & key[index - 1] << 8;
          numArray[index] = this.Swap(key[index], index) ^ num;
        }
      }
      return numArray;
    }

    private uint[] DecryptKey(uint[] key)
    {
      uint[] numArray = new uint[16];
      for (int index = 0; index < 16; ++index)
      {
        if (index == 0)
        {
          numArray[index] = this.Unswap(key[index], index);
        }
        else
        {
          int num = (int) numArray[index - 1] & (int) numArray[index - 1] << 8;
          uint input = key[index] ^ (uint) num;
          numArray[index] = this.Unswap(input, index);
        }
      }
      return numArray;
    }

    private uint Unswap(uint input, int index)
    {
      uint num1 = 0U;
      if (index == 0 || index % 2 != 0)
      {
        for (int index1 = 15; index1 >= 0; --index1)
        {
          uint num2 = input >> (int) this.table1[index1] & 1U;
          num1 |= num2 << (int) this.table2[index1];
        }
      }
      else
      {
        for (int index1 = 15; index1 >= 0; --index1)
        {
          uint num2 = input >> (int) this.table2[index1] & 1U;
          num1 |= num2 << (int) this.table1[index1];
        }
      }
      return num1;
    }

    private uint Swap(uint input, int index)
    {
      uint num1 = 0U;
      if (index == 0 || index % 2 != 0)
      {
        for (int index1 = 0; index1 < 16; ++index1)
        {
          uint num2 = input >> (int) this.table2[index1] & 1U;
          num1 |= num2 << (int) this.table1[index1];
        }
      }
      else
      {
        for (int index1 = 0; index1 < 16; ++index1)
        {
          uint num2 = input >> (int) this.table1[index1] & 1U;
          num1 |= num2 << (int) this.table2[index1];
        }
      }
      return num1;
    }

    public byte[] Decrypt(byte[] value, byte[] key, byte[] init)
    {
      DateTime now1 = DateTime.Now;
      RijndaelManaged rijndaelManaged = new RijndaelManaged();
      rijndaelManaged.Mode = CipherMode.CFB;
      rijndaelManaged.BlockSize = 128;
      rijndaelManaged.IV = init;
      rijndaelManaged.Key = key;
      rijndaelManaged.Padding = PaddingMode.Zeros;
      ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor();
      byte[] buffer1 = value;
      if (value.Length % 16 != 0)
      {
        buffer1 = new byte[value.Length + 16 - value.Length % 16];
        buffer1.Initialize();
        value.CopyTo((Array) buffer1, 0);
      }
      MemoryStream memoryStream = new MemoryStream(buffer1);
      CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, decryptor, CryptoStreamMode.Read);
      byte[] buffer2 = new byte[value.Length];
      cryptoStream.Read(buffer2, 0, value.Length);
      memoryStream.Close();
      cryptoStream.Close();
      DateTime now2 = DateTime.Now;
      return buffer2;
    }

    public byte[] Encrypt(byte[] value, byte[] key, byte[] init)
    {
      DateTime now1 = DateTime.Now;
      RijndaelManaged rijndaelManaged = new RijndaelManaged();
      rijndaelManaged.Mode = CipherMode.CFB;
      rijndaelManaged.BlockSize = 128;
      rijndaelManaged.IV = init;
      rijndaelManaged.Key = key;
      rijndaelManaged.Padding = PaddingMode.Zeros;
      ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor();
      MemoryStream memoryStream = new MemoryStream();
      CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, encryptor, CryptoStreamMode.Write);
      cryptoStream.Write(value, 0, value.Length);
      cryptoStream.FlushFinalBlock();
      byte[] numArray1 = memoryStream.ToArray();
      byte[] numArray2 = new byte[value.Length];
      Array.Copy((Array) numArray1, (Array) numArray2, numArray2.Length);
      memoryStream.Close();
      cryptoStream.Close();
      DateTime now2 = DateTime.Now;
      return numArray2;
    }
  }
}
