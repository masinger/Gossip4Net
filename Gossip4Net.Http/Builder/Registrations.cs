using Gossip4Net.Http.Modifier.Request.Registration;
using Gossip4Net.Http.Modifier.Response.Registration;

namespace Gossip4Net.Http.Builder
{
    public class Registrations
    {
        public IList<IRequestAttributeRegistration> RequestAttributes { get; }
        public IList<IResponseAttributeRegistration> ResponseAttributes { get; }
        public IList<IResponseConstructorRegistration> ResponseConstructors { get; }

        public Registrations(
            IList<IRequestAttributeRegistration> requestAttributes,
            IList<IResponseAttributeRegistration> responseAttributes,
            IList<IResponseConstructorRegistration> responseConstructors)
        {
            RequestAttributes = requestAttributes;
            ResponseAttributes = responseAttributes;
            ResponseConstructors = responseConstructors;
        }

        public Registrations() : this(
            new List<IRequestAttributeRegistration>(),
            new List<IResponseAttributeRegistration>(),
            new List<IResponseConstructorRegistration>()
            )
        { 
        } 
    }
}
