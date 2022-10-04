using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gossip4Net.Model.Mappings
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DeleteMapping : HttpMapping
    {
        public DeleteMapping() : base(HttpMethod.Delete)
        {
        }

        public DeleteMapping(string path) : this()
        {
            Path = path;
        }
    }
}
