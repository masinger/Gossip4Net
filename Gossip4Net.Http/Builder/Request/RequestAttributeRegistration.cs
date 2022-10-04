namespace Gossip4Net.Http.Builder.Request
{

    public interface IRequestAttributeRegistration
    {
        /// <summary>
        /// This method is called when a request method parameter is found. 
        /// </summary>
        /// <param name="parameterContext">Information about the parameter being processed.</param>
        /// <param name="attributes">The attributes present/active for the current parameter. The enumerable might be empty (but not null), if there are no attributes.</param>
        /// <returns>A list of request modifiers to be registered. The method might return null to indicate that there are no modifiers to be registered.</returns>
        public IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext, IEnumerable<Attribute> attributes);
    }

    /// <summary>
    /// This class provides default implementations for <see cref="IRequestAttributeRegistration"/>. It can be inherited in order to register custom request modifiers.
    /// </summary>
    /// <typeparam name="T">The attribute type, for which this registration will return request modifiers.</typeparam>
    public abstract class RequestAttributeRegistration<T> : IRequestAttributeRegistration where T : Attribute
    {

        /// <summary>
        /// This method is called when a request method parameter is found and has at least one active attribute of type <typeparamref name="T"/>.
        /// In order to register modifiers for parameters without attributes, one might override <see cref="ForParameter(RequestParameterContext)"/>.
        /// </summary>
        /// <param name="parameterContext">Information about the parameter being processed.</param>
        /// <param name="attributes">The list of attributes present/active for the current parameter. It is never null and never empty.</param>
        /// <returns>A list of request modifiers to be registered. The method might return null to indicate that there are no modifiers to be registered.</returns>
        public virtual IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext, IList<T> attributes)
        {
            return null;
        }

        /// <summary>
        /// This method is called when a request method parameter is found and has no active attributes of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// The parameter might as well have other active attributes (unassignable to <typeparamref name="T"/>).
        /// It is called, if it has no attributes of type <typeparamref name="T"/>.
        /// In order to process parameters having no attributes at all, consider implementing <see cref="IRequestAttributeRegistration.ForParameter(RequestParameterContext, IEnumerable{Attribute})"/>.
        /// </remarks>
        /// <param name="parameterContext">Information about the parameter being processed.</param>
        /// <returns>A list of request modifiers to be registered. The method might return null to indicate that there are no modifiers to be registered.</returns>
        public virtual IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext)
        {
            return null;
        }

        public IList<IHttpRequestModifier>? ForParameter(RequestParameterContext parameterContext, IEnumerable<Attribute> attributes)
        {
            IList<T> relevantAttributes = attributes.Select(it => it as T)
                .Where(it => it != null)
                .Select(it => it!)
                .ToList();

            if (relevantAttributes.Count == 0)
            {
                return ForParameter(parameterContext);
            }

            return ForParameter(parameterContext, relevantAttributes);
        }
    }

}
