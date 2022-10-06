using Gossip4Net.Http.Builder.Request;

namespace Gossip4Net.Http.Modifier.Request.Registration
{
    public class GlobalRequestRegistration : IRequestModifierRegistration
    {
        private readonly IList<IHttpRequestModifier> modifiers;

        public GlobalRequestRegistration(IList<IHttpRequestModifier> modifiers)
        {
            this.modifiers = modifiers;
        }

        public GlobalRequestRegistration(params IHttpRequestModifier[] modifiers) 
            : this(modifiers.ToList()) { }

        public IList<IHttpRequestModifier>? ForType(RequestTypeContext typeContext, IEnumerable<Attribute> attributes)
        {
            return modifiers;
        }
        
        public IList<IHttpRequestModifier>? ForMethod(RequestMethodContext methodContext, IEnumerable<Attribute> attributes)
        {
            return null;
        }

        public IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext, IEnumerable<Attribute> attributes)
        {
            return null;
        }
    }
}
