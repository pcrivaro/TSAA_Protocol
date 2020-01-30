

using System;
using Ypf.Mms.Node.Protocols.Tsaa.Enumerations;

namespace Ypf.Mms.Node.Protocols.Tsaa.Helpers
{
    public static class BinHelper
    {

        public static BinTypes GetBinTypeByAddress(string address)
        {
            return GetBinTypeByAddress(Convert.ToInt32(address));
        }


        public static bool IsBoolValue(string address)
        {
            return IsBoolValue(Convert.ToUInt16(address));
        }

        public static bool IsRealValue(string address)
        {
            return IsRealValue(Convert.ToUInt16(address));
        }


        public static bool IsBoolValue(BinTypes type)
        {
            switch (type)
            {
                case BinTypes.BoolInput:
                case BinTypes.BoolMemoryRead:
                case BinTypes.BoolMemoryReadWrite:
                case BinTypes.BoolOutput:
                case BinTypes.BoolSystemStatusRead:

                    return true;

                default:

                    return false;
            }
        }

        public static bool IsRealValue(BinTypes type)
        {
            switch (type)
            {
                case BinTypes.RealInputRead:
                case BinTypes.RealMemoryRead:
                case BinTypes.RealMemoryReadWrite:

                    return true;

                default:

                    return false;
            }
        }


        public static bool IsRealValue(ushort address)
        {
            if ((address >= 32001 && address <= 32120) || (address >= 33001 && address <= 34000) || (address >= 41001 && address <= 42000))
                return true;


            return false;
        }


        public static bool IsBoolValue (ushort address)
        {
            if ((address >= 0 && address <= 2000) || (address >= 2001 && address <= 4000) ||
                    (address >= 10001 && address <= 12000) || (address >= 12001 && address <= 14000) || (address >= 14001 && address <= 19999))
                return true;


            return false;
        }


        public static ushort GetStartPositionByBinType(BinTypes type)
        {
            switch (type)
            {
                case BinTypes.BoolOutput:
                    return 1;
                case BinTypes.BoolMemoryReadWrite:
                    return 2001;
                case BinTypes.BoolInput:
                    return 10001;
                case BinTypes.BoolMemoryRead:
                    return 12001;
                case BinTypes.DintInput:
                    return 30001;
                case BinTypes.DintMemoryRead:
                    return 31001;
                case BinTypes.RealInputRead:
                    return 32001;
                case BinTypes.RealMemoryRead:
                    return 33001;
                case BinTypes.BoolSystemStatusRead:
                    return 14001;
                case BinTypes.DintSystemStatusRead:
                    return 39631;
                case BinTypes.DintOutput:
                    return 40001;
                case BinTypes.DintMemoryReadWrite:
                    return 40251;
                case BinTypes.RealMemoryReadWrite:
                    return 41001;
            }

            return 0;
        }



        public static BinTypes GetBinTypeByAddress(int address)
        {
            if (address >= 0 && address <= 2000)
                return BinTypes.BoolOutput;

            if (address >= 2001 && address <= 4000)
                return BinTypes.BoolMemoryReadWrite;

            if (address >= 10001 && address <= 12000)
                return BinTypes.BoolInput;

            if (address >= 12001 && address <= 14000)
                return BinTypes.BoolMemoryRead;

            if (address >= 30001 && address <= 31000)
                return BinTypes.DintInput;

            if (address >= 31001 && address <= 32000)
                return BinTypes.DintMemoryRead;

            if (address >= 32001 && address <= 32120)
                return BinTypes.RealInputRead;

            if (address >= 33001 && address <= 34000)
                return BinTypes.RealMemoryRead;

            if (address >= 14001 && address <= 19999)
                return BinTypes.BoolSystemStatusRead;

            if (address >= 39631 && address <= 39999)
                return BinTypes.DintSystemStatusRead;

            if (address >= 40001 && address <= 40250)
                return BinTypes.DintOutput;

            if (address >= 40251 && address <= 41000)
                return BinTypes.DintMemoryReadWrite;

            if (address >= 41001 && address <= 42000)
                return BinTypes.RealMemoryReadWrite;


            return BinTypes.NotApplicable;
        }

    }
}
