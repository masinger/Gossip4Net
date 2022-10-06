using Gossip4Net.Http.Modifier.Request;
using Gossip4Net.Http.Modifier.Request.Registration;
using Gossip4Net.Http.Modifier.Response.Registration;

namespace Gossip4Net.Http.Builder
{
    public class Registrations
    {
        public IList<IRequestModifierRegistration> RequestModifiers { get; }
        public IList<IResponseModifierRegistration> ResponseModifiers { get; }
        public IList<IResponseConstructorRegistration> ResponseConstructors { get; }

        public Registrations(
            IList<IRequestModifierRegistration> requestAttributes,
            IList<IResponseModifierRegistration> responseAttributes,
            IList<IResponseConstructorRegistration> responseConstructors)
        {
            RequestModifiers = requestAttributes;
            ResponseModifiers = responseAttributes;
            ResponseConstructors = responseConstructors;
        }

        public Registrations() : this(
            new List<IRequestModifierRegistration>(),
            new List<IResponseModifierRegistration>(),
            new List<IResponseConstructorRegistration>()
            )
        { 
        }

        public Registrations With(IRequestModifierRegistration registration)
        {
            RequestModifiers.Add(registration);
            return this;
        }

        public Registrations With(params IHttpRequestModifier[] modifiers)
        {
            return With(new GlobalRequestRegistration(modifiers));
        }

        public Registrations With(IResponseModifierRegistration registration)
        {
            ResponseModifiers.Add(registration);
            return this;
        }

        public Registrations With(IResponseConstructorRegistration registration)
        {
            ResponseConstructors.Add(registration);
            return this;
        }
    }
}
