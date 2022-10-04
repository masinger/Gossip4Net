using Gossip4Net.Http.Modifier.Request;
using System.Net.Http.Json;
using System.Text.Json;

namespace Gossip4Net.Http.Builder.Request.Registrations
{
    internal class RequestBodyRegistration : RequestAttributeRegistration<Attribute>
    {

        private readonly JsonSerializerOptions jsonSerializerOptions;

        public RequestBodyRegistration(JsonSerializerOptions jsonSerializerOptions)
        {
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public override IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext)
        {
            return new List<IHttpRequestModifier>()
            {
                new RequestBodyModifier( // TODO: Allow customization
                    parameterContext.ParameterIndex,
                    o => Task.FromResult<HttpContent>(JsonContent.Create(o, parameterContext.Parameter.ParameterType, options: jsonSerializerOptions)),
                    true
                )
            };
        }
    }
}
