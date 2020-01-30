

using System.Collections.Generic;

namespace Ypf.Mms.Node.Core.Interfaces
{
    public interface IDriver
    {
        List<AcquiredData> Execute();

        void Initialize(Device device);
    }
}
