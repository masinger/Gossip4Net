
namespace Gossip4Net.Http.Modifier.Response
{
    public class ConstructedResponse
    {
        private readonly object? response;
        public object? Response {
            get {
                if (IsEmpty)
                {
                    throw new InvalidOperationException("This response is empty.");
                }
                return response;
            }
        }

        public bool IsEmpty { get; }

        public static ConstructedResponse Empty { get; } = new ConstructedResponse();

        public ConstructedResponse(
            object? response
        )
        {
            this.response = response;
            IsEmpty = false;
        }

        protected ConstructedResponse() {
            response = null;
            IsEmpty = true;
        }

        public static ConstructedResponse<T> Of<T>(T? value)
        {
            return new ConstructedResponse<T>(value);
        }


    }

    public class ConstructedResponse<T> : ConstructedResponse
    {
        private readonly T? response;
        public new T? Response
        {
            get
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException("This response is empty.");
                }
                return response;
            }
        }

        public ConstructedResponse(T? response) : base(response)
        {
            this.response = response;
        }

        private ConstructedResponse() : base() {
            response = default(T);
        }

       
        public new static ConstructedResponse<T> Empty()
        {
            return new ConstructedResponse<T>();
        }

    }
}
