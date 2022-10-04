using Gossip4Net.Http.Modifier.Response;

namespace Gossip4Net.Http.Builder.Response
{
    internal class CombinedResponseBuilder : IResponseConstructor
    {

        private readonly IEnumerable<IResponseModifier> modifiers;
        private readonly IResponseConstructor responseBuilder;

        public CombinedResponseBuilder(
            IEnumerable<IResponseModifier> modifiers,
            IResponseConstructor responseBuilder
        )
        {
            this.modifiers = modifiers;
            this.responseBuilder = responseBuilder;
        }


        async Task<object?> IResponseConstructor.ConstructResponseAsync(HttpResponseMessage response)
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
