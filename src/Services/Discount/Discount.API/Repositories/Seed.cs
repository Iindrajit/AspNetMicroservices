using Discount.API.Entities;
using Newtonsoft.Json;
using Npgsql;
using System.Collections.Generic;

namespace Discount.API.Repositories
{
    public class Seed
    {
        public static void SeedCoupons(NpgsqlCommand command)
        {
            var couponData = System.IO.File.ReadAllText("Repositories/CouponData.json");
            var coupons = JsonConvert.DeserializeObject<List<Coupon>>(couponData);
            
            if(coupons != null)
            {
                foreach(var coupon in coupons)
                {
                    command.CommandText = $@"INSERT INTO Coupon(ProductName, Description, Amount) 
                                                VALUES('{coupon.ProductName}', '{coupon.Description}', {coupon.Amount})";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
