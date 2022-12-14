using Gossip4Net.Http.Builder.Request;

namespace Gossip4Net.Http.Modifier.Request.Registration
{
    public interface IRequestModifierRegistration
    {
        /// <summary>
        /// This method is called when a request method parameter is found. 
        /// </summary>
        /// <param name="parameterContext">Information about the parameter being processed.</param>
        /// <param name="attributes">The attributes present/active for the current parameter. The enumerable might be empty (but not null), if there is no attribute.</param>
        /// <returns>A list of request modifiers to be registered. The method might return null to indicate that there are no modifiers to be registered.</returns>
        public IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext, IEnumerable<Attribute> attributes);

        /// <summary>
        /// This method is called when a request method is found. 
        /// </summary>
        /// <param name="methodContext">Information about the method being processed.</param>
        /// <param name="attributes">The attributes present/active for the current method. The enumerable might be empty (but not null), if there is no attribute.</param>
        /// <returns>A list of request modifiers to be registered. The method might return null to indicate that there are no modifiers to be registered.</returns>
        public IList<IHttpRequestModifier>? ForMethod(RequestMethodContext methodContext, IEnumerable<Attribute> attributes);

        /// <summary>
        /// This method is called when building an implementation for a given interface.
        /// </summary>
        /// <param name="typeContext">Information about the type being processed.</param>
        /// <param name="attributes">The attributes present/active for the current type. The enumerable migt be empty (but not null), if there is no attribute.</param>
        /// <returns>A list of request modifiers to be registered. The method might return null to indicate that there are no modifiers to be registered.</returns>
        public IList<IHttpRequestModifier>? ForType(RequestTypeContext typeContext, IEnumerable<Attribute> attributes);
    }
}
