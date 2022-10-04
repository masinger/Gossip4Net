using System.Reflection;
using Gossip4Net.Http.Modifier.Response;
using Gossip4Net.Http.Modifier.Response.Registration;

namespace Gossip4Net.Http.Builder.Response
{
    internal class ResponseImplementationBuilder
    {
        private readonly IList<IResponseAttributeRegistration> responseAttributeRegistrations;
        private readonly IList<IResponseConstructorRegistration> responseConstructorRegistrations;

        public ResponseImplementationBuilder(
            IList<IResponseAttributeRegistration> responseAttributeRegistrations,
            IList<IResponseConstructorRegistration> responseConstructorRegistrations
        )
        {
            this.responseAttributeRegistrations = responseAttributeRegistrations;
            this.responseConstructorRegistrations = responseConstructorRegistrations;
        }

        public IResponseConstructor CreateResponseBuilder(MethodInfo method)
        {
            Type returnType = method.ReturnType.TaskResultType() ?? method.ReturnType;
            ResponseMethodContext responseMethodContext = new ResponseMethodContext(returnType, method);

            IList<Attribute> allMethodAttributes = method.GetCustomAttributes<Attribute>().ToList();
            List<IHttpResponseModifier> responseModifiers = responseAttributeRegistrations
                    .Select(it => it.ForMethod(responseMethodContext, allMethodAttributes))
                    .Where(it => it != null)
                    .SelectMany(it => it!)
                    .ToList();

            List<IResponseConstructor> responseConstructors = responseConstructorRegistrations
                    .Select(it => it.ForMethod(responseMethodContext))
                    .Where(it => it != null)
                    .SelectMany(it => it!)
                    .ToList();

            return new CombinedResponseConstructor(
                responseModifiers,
                responseConstructors
            );
        }
    }
}
