using Gossip4Net.Http.Modifier.Response;

namespace Gossip4Net.Http.Builder.Response
{
    internal class CombinedResponseConstructor : IResponseConstructor
    {
        private readonly IEnumerable<IHttpResponseModifier> modifiers;
        private readonly IEnumerable<IResponseConstructor> responseConstructors;

        public CombinedResponseConstructor(
            IEnumerable<IHttpResponseModifier> modifiers,
            IEnumerable<IResponseConstructor> responseConstructors
        )
        {
            this.modifiers = modifiers;
            this.responseConstructors = responseConstructors;
        }

        public async Task<ConstructedResponse> ConstructResponseAsync(HttpResponseMessage response)
        {
            HttpResponseMessage modifiedMessage = response;
            foreach (IHttpResponseModifier modifier in modifiers)
            {
                modifiedMessage = modifier.Modify(response);
            }

            ConstructedResponse constructedResponse;
            foreach (IResponseConstructor responseConstructor in responseConstructors)
            {
                constructedResponse = await responseConstructor.ConstructResponseAsync(modifiedMessage);
                if (!constructedResponse.IsEmpty)
                {
                    return constructedResponse;
                }
            }
            return ConstructedResponse.Empty;
        }
    }
}
