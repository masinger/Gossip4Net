using System.Dynamic;

namespace Gossip4Net.Http.Client
{
    delegate object? RequestMethodImplementation(object?[] arguments);

    internal class GossipHttpClient : DynamicObject
    {
        private readonly IDictionary<MethodSignature, RequestMethodImplementation> registrations;

        public GossipHttpClient(IDictionary<MethodSignature, RequestMethodImplementation> registrations)
        {
            this.registrations = registrations;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
        {
            MethodSignature registration = new MethodSignature(binder.Name, binder.CallInfo.ArgumentCount);
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
