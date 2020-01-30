using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ypf.Mms.Node.Protocols.Tsaa.Helpers
{
    public static class Crc32Helper
    {
        private static uint[] _table;


        private static uint[] Table
        {
            get
            {
                if (_table == null)
                {
                    _table = GetTable();
                }

                return _table;
            }
        }
        

        public static uint ComputeChecksum(byte[] bytes)
        {
            uint crc = 0xffffffff;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(((crc) & 0xff) ^ bytes[i]);
                crc = (uint)((crc >> 8) ^ Table[index]);
            }

            return ~crc;
        }

        public static byte[] ComputeChecksumBytes(byte[] bytes)
        {
            var crc =  BitConverter.GetBytes(ComputeChecksum(bytes));

            //return crc.Reverse().ToArray();

            return crc.ToArray();
        }

        public static uint[] GetTable()
        {
            uint poly = 0xedb88320;
            var table = new uint[256];
            uint temp = 0;

            for (uint i = 0; i < table.Length; ++i)
            {
                temp = i;

                for (int j = 8; j > 0; --j)
                {
                    if ((temp & 1) == 1)
                    {
                        temp = (uint)((temp >> 1) ^ poly);
                    }
                    else
                    {
                        temp >>= 1;
                    }
                }

                table[i] = temp;
            }

            return table;
        }

    }
}
