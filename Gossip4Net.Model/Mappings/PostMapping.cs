using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gossip4Net.Model.Mappings
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PostMapping : HttpMapping
    {
        public PostMapping() : base(HttpMethod.Post)
        {
        }

        public PostMapping(string path) : this()
        {
            Path = path;
        }

    }
}
