using Gossip4Net.Http.Builder.Request;
using System.Net.Http.Json;
using System.Text.Json;

namespace Gossip4Net.Http.Modifier.Request.Registration
{
    internal class JsonRequestBodyRegistration : RequestAttributeRegistration<Attribute>
    {

        private readonly JsonSerializerOptions jsonSerializerOptions;

        public JsonRequestBodyRegistration(JsonSerializerOptions jsonSerializerOptions)
        {
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public override IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext)
        {
            return new List<IHttpRequestModifier>()
            {
                new RequestBodyModifier(
                    parameterContext.ParameterIndex,
                    o => Task.FromResult<HttpContent>(JsonContent.Create(o, parameterContext.Parameter.ParameterType, options: jsonSerializerOptions)),
                    true
                )
            };
        }
    }
}
