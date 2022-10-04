namespace Gossip4Net.Http.Builder.Request
{

    public interface IRequestAttributeRegistration
    {
        public IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext, IEnumerable<Attribute> attributes);
    }

    public abstract class RequestAttributeRegistration<T> : IRequestAttributeRegistration where T : Attribute
    {

        public virtual IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext, IList<T> attributes)
        {
            return null;
        }

        public virtual IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext)
        {
            return null;
        }

        public IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext, IEnumerable<Attribute> attributes)
        {
            IList<T> relevantAttributes = attributes.Select(it => it as T)
                .Where(it => it != null)
                .Select(it => it!)
                .ToList();

            if (relevantAttributes.Count == 0)
            {
                return ForParameter(parameterContext);
            }

            return ForParameter(parameterContext, relevantAttributes);
        }
    }

}
