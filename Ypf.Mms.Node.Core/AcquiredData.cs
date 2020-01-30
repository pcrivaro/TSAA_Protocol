

using System;

namespace Ypf.Mms.Node.Core
{
    public class AcquiredData : BaseEntity
    {
        public Item Item { get; set; }

        public DateTime DateTime { get; set; }

        public float? NumericValue { get; set; }

        public string StringValue { get; set; }
    }
}
