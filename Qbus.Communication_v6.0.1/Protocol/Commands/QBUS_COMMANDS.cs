// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.Protocol.Commands.QBUS_COMMANDS
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

namespace Qbus.Communication.Protocol.Commands
{
  public enum QBUS_COMMANDS
  {
    SERVICE = 0,
    CONTROL_PARAMETERS = 1,
    ADDRESS_PARAMETERS = 2,
    WORK_TEXT = 3,
    ADDRESS_TEXT = 4,
    CHANNEL_LIST_TEXT = 5,
    RTC_CLOCK = 6,
    VERSION = 7,
    DATE = 8,
    FAT_DATA = 9,
    SWAP_CTL = 10,
    CLEAR_POWERDOWN_TIME = 11,
    FIRMWARE_UPLOAD = 12,
    CONTROLLER_OPTIONS = 13,
    CONTROLLER_CLEAR = 14,
    CONTROLLER_REBOOT = 15,
    PRESET_CLEAR = 16,
    PRESET_DATA = 17,
    MINI_PRESET_PARAMETERS = 18,
    PRESET_PARAMETERS = 19,
    SCHEDULE_DATA = 24,
    HOLIDAY = 25,
    EVENT_LOGS = 26,
    LOGIC = 32,
    LOGIC_ANALOG = 33,
    JAGA_DB = 34,
    MODULE_SRAM = 40,
    MODULE_EEPROM = 41,
    MODULE_FLASH = 42,
    SIMULATION_DATA = 44,
    CHANNEL_LIST_MENU = 48,
    CHANNEL_LIST_DATA = 49,
    EXTERNAL_CHANNEL_LIST_DATA = 50,
    ROOM = 51,
    EVENT_READ = 52,
    EVENTS = 53,
    ADDRESS_STATUS = 56,
    CHANNEL_STATUS = 57,
    EXTERNAL_CHANNEL_STATUS = 58,
    ADDRESS_MODE = 59,
    CHANNEL_MODE = 60,
    SIMULATION = 62,
    SD_SERVICE = 65,
    SD_MODULE = 66,
    SD_DATABASE = 68,
    SD_COPY_DATA = 70,
    COPY_DATA_SD = 71,
  }
}
