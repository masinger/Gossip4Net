using Gossip4Net.Http.Modifier.Response;

namespace Gossip4Net.Http.Builder.Response
{
    internal class CombinedResponseBuilder : IResponseBuilder
    {

        private readonly IEnumerable<IResponseModifier> modifiers;
        private readonly IResponseBuilder responseBuilder;

        public CombinedResponseBuilder(
            IEnumerable<IResponseModifier> modifiers,
            IResponseBuilder responseBuilder
        )
        {
            this.modifiers = modifiers;
            this.responseBuilder = responseBuilder;
        }


        async Task<object?> IResponseBuilder.ConstructResponseAsync(HttpResponseMessage response)
        {
            HttpResponseMessage modifiedMessage = response;
            foreach (var modifier in modifiers)
            {
                modifiedMessage = modifier.Modify(modifiedMessage);
            }

            return await responseBuilder.ConstructResponseAsync(modifiedMessage);
        }
    }
}
