using Gossip4Net.Http.Modifier.Request.Registration;
using Gossip4Net.Http.Modifier.Response.Registration;
using System.Text.Json;

namespace Gossip4Net.Http
{
    public static class HttpGossipBuilderExtensions
    {

        private static readonly IList<IRequestModifierRegistration> BasicRequestAttributeRegistrations = new List<IRequestModifierRegistration>()
        {
            new HttpApiRegistration(),
            new HttpMappingRegistration(),
            new HeaderValueRegistration(),
        };

        private static readonly IList<IResponseModifierRegistration> BasicResponseAttributeRegistrations = new List<IResponseModifierRegistration>()
        {
            new ResponseSuccessRegistration()
        };

        private static readonly IList<IResponseConstructorRegistration> BasicResponseConstructorRegistrations = new List<IResponseConstructorRegistration>()
        {
            new RawResponseConstructorRegistration()
        };

        public static IHttpGossipBuilder<T> AddJsonBehavior<T>(this IHttpGossipBuilder<T> instance, JsonSerializerOptions jsonSerializerOptions)
        {
            instance.Registrations.RequestModifiers.Add(new JsonRequestBodyRegistration(jsonSerializerOptions));
            instance.Registrations.ResponseConstructors.Add(new JsonResponseConstructorRegistration(jsonSerializerOptions));
            return instance;
        }

        public static IHttpGossipBuilder<T> AddJsonBehavior<T>(this IHttpGossipBuilder<T> instance)
        {
            return instance.AddJsonBehavior(new JsonSerializerOptions());
        }

        public static IHttpGossipBuilder<T> AddBasicBehavior<T>(this IHttpGossipBuilder<T> instance)
        {
            foreach (IRequestModifierRegistration registration in BasicRequestAttributeRegistrations)
            {
                instance.Registrations.RequestModifiers.Add(registration);
            }
            foreach (IResponseModifierRegistration registration in BasicResponseAttributeRegistrations)
            {
                instance.Registrations.ResponseModifiers.Add(registration);
            }
            foreach (IResponseConstructorRegistration registration in BasicResponseConstructorRegistrations)
            {
                instance.Registrations.ResponseConstructors.Add(registration);
            }
            return instance;
        }

        public static IHttpGossipBuilder<T> AddStringifyingBehavior<T>(this IHttpGossipBuilder<T> instance, Func<object?, string> stringifier)
        {
            instance.Registrations.RequestModifiers.Add(new PathVariableRegistration(stringifier));
            instance.Registrations.RequestModifiers.Add(new QueryVariableRegistration(stringifier));
            instance.Registrations.RequestModifiers.Add(new HeaderVariableRegistration(stringifier));
            instance.Registrations.RequestModifiers.Add(new QueryValueRegistration(stringifier));
            return instance;
        }

        public static IHttpGossipBuilder<T> AddStringifyingBehavior<T>(this IHttpGossipBuilder<T> instance)
        {
            return instance.AddStringifyingBehavior(o => $"{o}");
        }

        public static IHttpGossipBuilder<T> AddDefaultBehavior<T>(
            this IHttpGossipBuilder<T> instance,
            Func<object?, string> stringifier,
            JsonSerializerOptions jsonSerializerOptions
        )
        {
            return instance
                .AddBasicBehavior()
                .AddStringifyingBehavior(stringifier)
                .AddJsonBehavior(jsonSerializerOptions);
        }

        public static IHttpGossipBuilder<T> AddDefaultBehavior<T>(this IHttpGossipBuilder<T> instance, JsonSerializerOptions jsonSerializerOptions)
        {
            return instance
                .AddBasicBehavior()
                .AddStringifyingBehavior()
                .AddJsonBehavior(jsonSerializerOptions);
        }

        public static IHttpGossipBuilder<T> AddDefaultBehavior<T>(this IHttpGossipBuilder<T> instance, Func<object?, string> stringifier)
        {
            return instance
                .AddBasicBehavior()
                .AddStringifyingBehavior(stringifier)
                .AddJsonBehavior();
        }

        public static IHttpGossipBuilder<T> AddDefaultBehavior<T>(
            this IHttpGossipBuilder<T> instance
        )
        {
            return instance
                .AddBasicBehavior()
                .AddStringifyingBehavior()
                .AddJsonBehavior();
        }

    }
}
