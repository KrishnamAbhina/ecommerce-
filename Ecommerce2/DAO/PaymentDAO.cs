using DAL.DTO;
using ecommerce_application.DATA;
using ecommerce_application.MODELS.DB_MODELS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Services
{
    public class PaymentDAO
    {
        private readonly EcommerceDbContext _context;

        public PaymentDAO(EcommerceDbContext context)
        {
            _context = context;
        }

        // ============ CREATE ============

        public async Task<int> CreatePaymentAsync(CreatePaymentVM model)
        {
            var payment = new PaymentsModel
            {
                OrderId = model.OrderId,
                Amount = model.Amount,
                PaymentMethod = model.PaymentMethod,
                Status = model.Status,
                TransactionId = model.TransactionId,
                PaymentDate = model.PaymentDate
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return payment.Id;
        }

        // ============ READ ============

        public async Task<PaymentVM> GetPaymentByIdAsync(int id)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
                return null;

            return new PaymentVM
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                TransactionId = payment.TransactionId,
                PaymentDate = payment.PaymentDate
            };
        }

        public async Task<List<PaymentVM>> GetAllPaymentsAsync()
        {
            var payments = await _context.Payments
                .OrderByDescending(p => p.Id)
                .Select(p => new PaymentVM
                {
                    Id = p.Id,
                    OrderId = p.OrderId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    Status = p.Status,
                    TransactionId = p.TransactionId,
                    PaymentDate = p.PaymentDate
                })
                .ToListAsync();

            return payments;
        }

        public async Task<List<PaymentVM>> GetPaymentsByOrderIdAsync(int orderId)
        {
            var payments = await _context.Payments
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.Id)
                .Select(p => new PaymentVM
                {
                    Id = p.Id,
                    OrderId = p.OrderId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    Status = p.Status,
                    TransactionId = p.TransactionId,
                    PaymentDate = p.PaymentDate
                })
                .ToListAsync();

            return payments;
        }

        public async Task<PaymentVM> GetLatestPaymentByOrderIdAsync(int orderId)
        {
            var payment = await _context.Payments
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.PaymentDate)
                .FirstOrDefaultAsync();

            if (payment == null)
                return null;

            return new PaymentVM
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                TransactionId = payment.TransactionId,
                PaymentDate = payment.PaymentDate
            };
        }

        // ============ UPDATE ============

        public async Task UpdatePaymentAsync(int id, UpdatePaymentVM model)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);

            if (payment != null)
            {
                payment.Status = model.Status;
                payment.TransactionId = model.TransactionId;

                _context.Payments.Update(payment);
                await _context.SaveChangesAsync();
            }
        }

        // ============ DELETE ============

        public async Task DeletePaymentAsync(int id)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);

            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
