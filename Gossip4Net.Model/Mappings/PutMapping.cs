using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gossip4Net.Model.Mappings
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PutMapping : HttpMapping
    {
        public PutMapping() : base(HttpMethod.Put)
        {
        }

        public PutMapping(string path) : this()
        {
            Path = path;
        }
    }
}
