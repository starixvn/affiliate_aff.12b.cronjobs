using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Configurations
{
    public interface IAppSettings
    {
        int MaxTimeRange { get; }
        int MaxItemsInRange { get; }
    }
}
