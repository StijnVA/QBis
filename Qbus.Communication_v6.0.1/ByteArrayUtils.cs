// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.ByteArrayUtils
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using System;

namespace Qbus.Communication
{
  internal class ByteArrayUtils
  {
    public static bool Compare(byte[] array, byte[] needle, int startIndex)
    {
      if (array.Length < needle.Length)
        return false;
      int length = needle.Length;
      int index1 = 0;
      int index2 = startIndex;
      while (index1 < length)
      {
        if ((int) array[index2] != (int) needle[index1])
          return false;
        ++index1;
        ++index2;
      }
      return true;
    }

    public static int Find(byte[] array, byte[] needle, int startIndex, int count)
    {
      int length = needle.Length;
      if (startIndex < 0)
        startIndex = 0;
      if (array == null || needle == null || array.Length < needle.Length)
        return -1;
      while (count >= length)
      {
        int num = Array.IndexOf<byte>(array, needle[0], startIndex, count - length + 1);
        if (num == -1)
          return -1;
        int index1 = 0;
        for (int index2 = num; index1 < length && (int) array[index2] == (int) needle[index1]; ++index2)
          ++index1;
        if (index1 == length)
          return num;
        count -= num - startIndex + 1;
        startIndex = num + 1;
      }
      return -1;
    }
  }
}
