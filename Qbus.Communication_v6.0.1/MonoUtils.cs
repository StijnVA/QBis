using System;

namespace Qbus.Communication
{
	public class MonoUtils
	{
		private static Int16 MIN_128 = -128;
		public static byte SByte_MIN {
			get { 
				return (byte)MIN_128;
			}
		}

		public static bool IntToBool(int i){
			return i != 0;
		}
	}
}

