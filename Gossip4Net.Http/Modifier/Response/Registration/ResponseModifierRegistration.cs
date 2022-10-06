using Gossip4Net.Http.Builder.Response;

namespace Gossip4Net.Http.Modifier.Response.Registration
{
    public class ResponseModifierRegistration<T> : IResponseModifierRegistration where T : Attribute
    {
        private static IList<T> FilterRelevantAttributes(IEnumerable<Attribute> attributes)
        {
            return attributes.Select(it => it as T)
                   .Where(it => it != null)
                   .Select(it => it!)
                   .ToList();
        }

        public virtual IList<IHttpResponseModifier>? ForMethod(ResponseMethodContext methodContext)
        {
            return null;
        }

        public virtual IList<IHttpResponseModifier>? ForMethod(ResponseMethodContext methodContext, IList<T> attributes)
        {
            return null;
        }

        public IList<IHttpResponseModifier>? ForMethod(ResponseMethodContext methodContext, IEnumerable<Attribute> attributes)
        {
            IList<T> relevantAttributes = FilterRelevantAttributes(attributes);
            if (relevantAttributes.Count == 0)
            {
                return ForMethod(methodContext);
            }
            return ForMethod(methodContext, relevantAttributes);
        }
    }
}
