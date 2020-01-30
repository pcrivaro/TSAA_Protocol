using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ypf.Mms.Node.Protocols.Tsaa.Core;
using Ypf.Mms.Node.Protocols.Tsaa.Enumerations;

namespace Ypf.Mms.Node.Protocols.Tsaa.Messages
{
    public class BinHeader
    {
        private BitArray _boolArray;

        public BinTypes Bin { get; set; }
        public byte Rfu { get; set; }
        public short TotalLength { get; set; }
        public short Offset { get; set; }
        public short Length { get; set; }
        public byte[] Data{ get; set; }

        public BitArray BoolArray
        {
            get {

                if (_boolArray == null && Data != null && Data.Length > 0)
                {
                    _boolArray = new BitArray(Data);
                }

                return _boolArray;
            }
        }
    }


    public class TriconDataMsg : Message
    {
        public short NumberOfBlocks { get; set; }
        public byte[] Rfu { get; set; }
        public List<BinHeader> Bins { get; set; }

        public TriconDataMsg(Message message) : base(message)
        {
            Bins = new List<BinHeader>();
        }


        public int? GetDintMemoryReadWrite(int address)
        {
            if (address >= 40251)
                address -= 40251;
            else
                address -= 251;

            address *= 4;

            var bin = Bins.SingleOrDefault(x => x.Bin == BinTypes.DintMemoryReadWrite);

            if (bin != null && bin.Data != null && bin.Data.Length > address && address >= 0)
            {
                return BitConverter.ToInt32(new byte[] {    bin.Data[address],
                                                            bin.Data[address + 1 ],
                                                            bin.Data[address + 2],
                                                            bin.Data[address + 3] }, 0);
            }

            return null;
        }


   
        public bool? GetBoolOutput(int address)
        {
            address -= 1;

            var bin = Bins.SingleOrDefault(x => x.Bin == BinTypes.BoolOutput);

            if (bin != null && bin.BoolArray != null && bin.BoolArray.Length > address && address >= 0)
            {
                return bin.BoolArray[address];
            }

            return null;
        }



        public bool? GetBoolMemoryReadWrite(int address)
        {
            if (address >= 2001)
                address -= 2001;
            else
                address -= 1;

            var bin = Bins.SingleOrDefault(x => x.Bin == BinTypes.BoolMemoryReadWrite);

            if (bin != null && bin.BoolArray != null && bin.BoolArray.Length > address && address >= 0)
            {
                return bin.BoolArray[address];
            }

            return null;
        }




        public bool? GetBoolInput(int address)
        {
            if (address >= 10001)
                address -= 10001;
            else
                address -= 1;

            var bin = Bins.SingleOrDefault(x => x.Bin == BinTypes.BoolInput);

            if (bin != null && bin.BoolArray != null && bin.BoolArray.Length > address && address >= 0)
            {
                return bin.BoolArray[address];
            }

            return null;
        }


        public bool? GetBoolMemoryRead(int address)
        {
            if (address >= 12001)
                address -= 12001;
            else
                address -= 1;

            var bin = Bins.SingleOrDefault(x => x.Bin == BinTypes.BoolMemoryRead);

            if (bin != null && bin.BoolArray != null && bin.BoolArray.Length > address && address >= 0)
            {
                return bin.BoolArray[address];
            }

            return null;
        }


        public int? GetDintInput(int address)
        {
            if (address >= 30001)
                address -= 30001;
            else
                address -= 1;

            address *= 4;

            var bin = Bins.SingleOrDefault(x => x.Bin == BinTypes.DintInput);

            if (bin != null && bin.Data != null && bin.Data.Length > address && address >= 0)
            {
                return BitConverter.ToInt32(new byte[] {    bin.Data[address],
                                                            bin.Data[address + 1],
                                                            bin.Data[address + 2],
                                                            bin.Data[address + 3] }, 0);
            }

            return null;
        }


        public int? GetDintMemoryRead(int address)
        {
            if (address >= 31001)
                address -= 31001;
            else
                address -= 1;

            address *= 4;

            var bin = Bins.SingleOrDefault(x => x.Bin == BinTypes.DintMemoryRead);

            if (bin != null && bin.Data != null && bin.Data.Length > address && address >= 0)
            {
                return BitConverter.ToInt32(new byte[]{ bin.Data[address],
                                                        bin.Data[address + 1],
                                                        bin.Data[address + 2],
                                                        bin.Data[address + 3] }, 0);
            }

            return null;
        }




        public double? GetRealMemoryRead(int address)
        {
            if (address >= 33001)
                address -= 33001;
            else
                address -= 1;

            address *= 4;

            var bin = Bins.SingleOrDefault(x => x.Bin == BinTypes.RealMemoryRead);

            if (bin != null && bin.Data != null && bin.Data.Length > address && address >= 0)
            {
                return BitConverter.ToSingle(new byte[] {   bin.Data[address],
                                                            bin.Data[address + 1],
                                                            bin.Data[address + 2],
                                                            bin.Data[address + 3],
                                                            bin.Data[address + 4] }, 0);
            }

            return null;
        }


        public double? GetRealMemoryReadWrite(int address)
        {
            if (address >= 41001)
                address -= 41001;
            else
                address -= 1;

            address *= 4;

            var bin = Bins.SingleOrDefault(x => x.Bin == BinTypes.RealMemoryReadWrite);

            if (bin != null && bin.Data != null && bin.Data.Length > address && address >= 0)
            {
                return BitConverter.ToSingle(new byte[] {   bin.Data[address],
                                                            bin.Data[address + 1],
                                                            bin.Data[address + 2],
                                                            bin.Data[address + 3],
                                                            bin.Data[address + 4]}, 0);
            }

            return null;
        }


        public short? GetDintSystemStatusRead(int address)
        {
            if (address >= 39633)
                address -= 39631;
            else
                address -= 631;

            address *= 2;

            var bin = Bins.SingleOrDefault(x => x.Bin == BinTypes.DintSystemStatusRead);

            if (bin != null && bin.Data != null && bin.Data.Length > address && address >= 0)
            {
                return BitConverter.ToInt16(new byte[] {   bin.Data[address],
                                                            bin.Data[address + 1]}, 0);
            }

            return null;
        }



        public bool? GetBoolSystemStatusRead(int address)
        {
            if (address >= 14001)
                address -= 14001;
            else
                address -= 1;

            var bin = Bins.SingleOrDefault(x => x.Bin == BinTypes.BoolSystemStatusRead);

            if (bin != null && bin.BoolArray != null && bin.BoolArray.Length > address && address >= 0)
            {
                return bin.BoolArray[address];
            }

            return null;
        }


    }
}

