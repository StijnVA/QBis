// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Downloaders.LogHelper
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using System;
using System.Collections.Generic;

namespace Qbus.Communication.Protocol.Downloaders
{
  public class LogHelper
  {
    private List<Log> _logs;

    public List<Log> Logs
    {
      get
      {
        return this._logs;
      }
      set
      {
        this._logs = value;
      }
    }

    public LogHelper()
    {
      this._logs = new List<Log>();
    }

    public SortedList<int, SortedList<DateTime, Log>> GetFilteredLogs()
    {
      return this.GetFilteredLogs(DateTime.MinValue);
    }

    public SortedList<int, SortedList<DateTime, Log>> GetFilteredLogs(DateTime minDate, ref int lastMonth, ref int lastYear)
    {
      if (this._logs == null || this._logs.Count == 0)
        return new SortedList<int, SortedList<DateTime, Log>>();
      SortedList<int, SortedList<DateTime, Log>> sortedList = new SortedList<int, SortedList<DateTime, Log>>();
      this.SortLogs();
      DateTime date = this._logs[0].Date;
      int month = date.Month;
      if (month < lastMonth - 3)
        ++lastYear;
      else if (lastYear == date.Year && date > DateTime.Now.AddDays(1.0))
        --lastYear;
      if (lastYear > DateTime.Now.Year)
        lastYear = DateTime.Now.Year;
      if (lastYear == DateTime.Now.Year && month > DateTime.Now.Month)
        --lastYear;
      for (int index = 0; index < this._logs.Count; ++index)
      {
        if (this._logs.Count > index)
        {
          Log log = this._logs[index];
          DateTime key = log.Date;
          if (key.Month < month - 3)
            ++lastYear;
          else if (key.Month > month + 8)
            --lastYear;
          month = key.Month;
          if (key.Year != lastYear && Math.Abs(lastYear - key.Year) < 20)
          {
            key = key.AddYears(lastYear - key.Year);
            log.Date = key;
          }
          while (key > DateTime.Now.AddDays(1.0))
            key = key.AddYears(-1);
          lastYear = key.Year;
          lastMonth = key.Month;
          if (!sortedList.ContainsKey(log.Address))
            sortedList.Add(log.Address, new SortedList<DateTime, Log>());
          if (!sortedList[log.Address].ContainsKey(key))
            sortedList[log.Address].Add(key, log);
        }
      }
      return sortedList;
    }

    public SortedList<int, SortedList<DateTime, Log>> GetFilteredLogs(DateTime minDate)
    {
      if (this._logs == null || this._logs.Count == 0)
        return new SortedList<int, SortedList<DateTime, Log>>();
      SortedList<int, SortedList<DateTime, Log>> sortedList = new SortedList<int, SortedList<DateTime, Log>>();
      int year = DateTime.Now.Year;
      int month = this._logs[this._logs.Count - 1].Date.Month;
      if (month > DateTime.Now.Month)
        --year;
      for (int index = this._logs.Count - 1; index >= 0; --index)
      {
        if (this._logs.Count > index)
        {
          Log log = this._logs[index];
          DateTime key = log.Date;
          if (key.Month > month)
            --year;
          month = key.Month;
          if (key.Year != year)
          {
            key = key.AddYears(year - key.Year);
            log.Date = key;
          }
          if (key > minDate)
          {
            if (!sortedList.ContainsKey(log.Address))
              sortedList.Add(log.Address, new SortedList<DateTime, Log>());
            if (!sortedList[log.Address].ContainsKey(key))
              sortedList[log.Address].Add(key, log);
          }
        }
      }
      return sortedList;
    }

    public void SortLogs()
    {
      this._logs.Sort((Comparison<Log>) ((l1, l2) => l1.Date.CompareTo(l2.Date)));
    }
  }
}
