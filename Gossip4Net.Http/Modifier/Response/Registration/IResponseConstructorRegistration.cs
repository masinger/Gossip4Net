using Gossip4Net.Http.Builder.Response;

namespace Gossip4Net.Http.Modifier.Response.Registration
{
    public interface IResponseConstructorRegistration
    {

        public IList<IResponseConstructor>? ForMethod(ResponseMethodContext responseMethod);

    }
}
