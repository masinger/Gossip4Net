using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gossip4Net.Model
{
    public interface IGossipBuilder<T>
    {
        T Build();
    }
}
