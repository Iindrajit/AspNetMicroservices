using Dapper;
using Discount.Grpc.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Threading.Tasks;

namespace Discount.Grpc.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly string _connectionString;
        public DiscountRepository(IConfiguration config)
        {
            _connectionString = config.GetValue<string>("DatabaseSettings:ConnectionString");
        }
        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var affected = 
                await connection.ExecuteAsync("insert into Coupon(ProductName, Description, Amount) " +
                    "values(@ProductName, @Description, @Amount)",
                    new 
                    {
                        ProductName = coupon.ProductName,
                        Description = coupon.Description,
                        Amount = coupon.Amount 
                    });

                if (affected == 0)
                    return false;

                return true;
            }
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var affected =
               await connection.ExecuteAsync("delete from Coupon where ProductName= @ProductName",
                   new
                   {
                       ProductName = productName
                   });

                if (affected == 0)
                    return false;

                return true;
            }
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using(var connection = new NpgsqlConnection(_connectionString))
            {
                var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(
                    "select * from Coupon where ProductName= @ProductName",
                    new { ProductName = productName });
               
                if(coupon == null)
                {
                    return new Coupon
                    {
                        ProductName = productName,
                        Amount = 0,
                        Description = $"No discount available for the product: {productName}."
                    };
                }

                return coupon;
            }
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var affected =
                await connection.ExecuteAsync("update Coupon set ProductName= @ProductName, Description= @Description, " +
                        "Amount= @Amount where Id= @Id ",
                    new
                    {
                        ProductName = coupon.ProductName,
                        Description = coupon.Description,
                        Amount = coupon.Amount,
                        Id = coupon.Id
                    });

                if (affected == 0)
                    return false;

                return true;
            }
        }
    }
}
