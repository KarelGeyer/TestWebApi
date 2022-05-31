using LearnWebApi.Types;

namespace LearnWebApi.Request_Types
{
    #pragma warning disable CS8618

    public class TransactionPostDto
    {
        public TransactionType Type;
        public string TransactionId;
        public string Recipient;
        public int UserId;
        public int Sum;
    }
}