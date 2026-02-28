using System;

namespace Payment.Domain.Entities
{
    public class Idempotency
    {
        public Guid Id { get; set; }

        public string Key { get; set; }

        public string Response { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}