using System.Dynamic;

namespace Gossip4Net.Http.Client
{
    delegate object? RequestMethodImplementation(object?[] arguments);

    internal class GossipHttpClient : DynamicObject
    {
        private readonly IDictionary<ClientRegistration, RequestMethodImplementation> registrations;

        public GossipHttpClient(IDictionary<ClientRegistration, RequestMethodImplementation> registrations)
        {
            this.registrations = registrations;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
        {
            ClientRegistration registration = new ClientRegistration(binder.Name, binder.CallInfo.ArgumentCount);
            if (!registrations.ContainsKey(registration))
            {
                result = null;
                return false;
            }

            result = registrations[registration](args ?? new object[0]);
            return true;
        }
    }
}
