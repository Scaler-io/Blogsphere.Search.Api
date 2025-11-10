using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Events;

public class ManagementUserUpdated : GenericEvent
{
    public string Id { get; set; }
    public List<string> Roles { get; set; }
    public string Status { get; set; }
    protected override GenericEventType Type { get; set; } = GenericEventType.ManagementUserUpdated;
}
