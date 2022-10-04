using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gossip4Net.Http.Client
{
    internal record ClientRegistration(
        string name,
        int parameters
    );
}
