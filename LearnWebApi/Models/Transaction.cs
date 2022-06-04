using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LearnWebApi.Types;

namespace LearnWebApi.Models
{
    #pragma warning disable CS8618

    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        public string Recipient { get; set; } = string.Empty;

        [Required]
        public string TransactionId { get; set; } = string.Empty;

        [Required]
        public int Sum { get; set; } = 0;

        [Required]
        public int UserId { get; set; }

        public static implicit operator Transaction(List<Transaction> v)
        {
            throw new NotImplementedException();
        }
    }
}
