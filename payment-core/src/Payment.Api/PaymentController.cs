using Microsoft.AspNetCore.Mvc;
using Payment.Domain.Entities;
using Payment.Infrastructure.Cache;
using Payment.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace payment_core.src.Payment.Api
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : ControllerBase
    {

        private readonly PaymentDbContext _db;

        private readonly RedisService _cache;

        public PaymentController(
            PaymentDbContext db,
            RedisService cache)
        {
            _db = db;
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            Payment payment,
            [FromHeader(Name="Idempotency-Key")]
            string key)
        {

            var existing = await _cache.GetAsync<string>(key);

            if (existing != null)
                return Ok(existing);

            payment.Id = Guid.NewGuid();

            payment.CreatedAt = DateTime.UtcNow;

            payment.Status = "CREATED";

            _db.Payments.Add(payment);

            await _db.SaveChangesAsync();

            await _cache.SetAsync(key, payment);

            return Ok(payment);

        }

    }
}