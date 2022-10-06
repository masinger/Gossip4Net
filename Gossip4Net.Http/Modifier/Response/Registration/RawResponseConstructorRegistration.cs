using Gossip4Net.Http.Builder.Response;

namespace Gossip4Net.Http.Modifier.Response.Registration
{
    public class RawResponseConstructorRegistration : IResponseConstructorRegistration
    {
        public IList<IResponseConstructor>? ForMethod(ResponseMethodContext responseMethod)
        {
            if (responseMethod.ProxyReturnType.IsAssignableFrom(typeof(HttpResponseMessage)) || responseMethod.IsVoid)
            {
                return new List<IResponseConstructor>()
                {
                    new RawHttpResponseConstructor()
                };
            }
            return null;
        }
    }
}
