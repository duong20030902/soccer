using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Soccer.Data_Access.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Soccer.Business_Logic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly SoccerContext _context;
        private readonly IConfiguration _configuration; // Thêm để lấy khóa JWT

        public CartController(SoccerContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("add")]
        public IActionResult AddToCart(int productId, int quantity, int? sizeId)
        {
            Console.WriteLine($"Session ID: {HttpContext.Session.Id}");
            int actualSizeId = sizeId ?? 1;

            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("Cart") ?? new List<OrderItem>();
            var product = _context.Products.Find(productId);
            if (product == null)
                return NotFound();

            var cartItem = cart.FirstOrDefault(x => x.ProductId == productId && x.SizeId == actualSizeId);
            if (cartItem == null)
            {
                cart.Add(new OrderItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.SalePrice * quantity,
                    UnitCost = product.CostPrice * quantity,
                    SizeId = actualSizeId
                });
            }
            else
            {
                cartItem.Quantity += quantity;
                cartItem.UnitPrice += product.SalePrice * quantity;
                cartItem.UnitCost += product.CostPrice * quantity;
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            Console.WriteLine($"Giỏ hàng sau khi thêm: {JsonConvert.SerializeObject(cart)}");
            return Ok(cart);
        }

        [HttpGet("checkout")]
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("Cart") ?? new List<OrderItem>();
            var cartWithDetails = cart.Select(item => new
            {
                item.ProductId,
                item.Quantity,
                item.UnitPrice,
                item.SizeId,
                ProductName = _context.Products.Find(item.ProductId)?.ProductName,
                SizeName = _context.Sizes.Find(item.SizeId)?.SizeName,
                ImageUrl = _context.ProductImages.Find(item.ProductId)?.ImageUrl,
            }).ToList();
            Console.WriteLine($"Giỏ hàng khi lấy: {JsonConvert.SerializeObject(cartWithDetails)}");
            return Ok(cartWithDetails);
        }

        [HttpGet("addresses")]
        public async Task<IActionResult> GetUserAddresses()
        {
            var userId = GetUserIdFromToken();
            Console.WriteLine($"UserId from token: {userId}");
            if (userId == null)
            {
                return Unauthorized("User ID not found or token invalid.");
            }

            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .Select(a => new
                {
                    AddressID = a.AddressId,
                    UserID = a.UserId,
                    RecipientName = a.RecipientName,
                    StreetAddress = a.StreetAddress,
                    CityProvince = a.CityProvince,
                    PostalCode = a.PostalCode
                })
                .ToListAsync();

            if (addresses == null || !addresses.Any())
            {
                Console.WriteLine($"No addresses found for UserId: {userId}");
                return NotFound("No addresses found for this user.");
            }

            return Ok(addresses);
        }
        [HttpPost("confirm-payment")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmPayment(int? addressId = null)
        {
            try
            {
                // Get UserId from token
                var userId = GetUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized("Invalid user ID or token invalid");
                }

                // Get cart from session
                var cart = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("Cart") ?? new List<OrderItem>();
                if (!cart.Any())
                {
                    return BadRequest("Cart is empty");
                }

                // Calculate total amount from cart
                var totalAmount = cart.Sum(item => item.UnitPrice);

                // Determine address to use
                Address address;
                if (addressId.HasValue && addressId.Value > 0)
                {
                    address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == addressId.Value && a.UserId == userId);
                    if (address == null)
                    {
                        return BadRequest("Invalid address ID");
                    }
                }
                else
                {
                    // Use the first address associated with the user, or create a default one if none exist
                    address = await _context.Addresses
                        .Where(a => a.UserId == userId)
                        .FirstOrDefaultAsync();

                    if (address == null)
                    {
                        // Create a default address (minimal validation for testing purposes)
                        address = new Address
                        {
                            UserId = userId.Value,
                            RecipientName = "Default Recipient",
                            StreetAddress = "Default Street",
                            CityProvince = "Default City",
                            PostalCode = "00000"
                        };
                        _context.Addresses.Add(address);
                        await _context.SaveChangesAsync();
                    }
                }

                // Create new order
                var order = new Order
                {
                    UserId = userId.Value,
                    TotalAmount = totalAmount,
                    OrderDate = DateTime.UtcNow,
                    ShippingMethodId = 1, // Default: Standard Shipping
                    AddressId = address.AddressId,
                    PaymentMethodId = 1, // Default: VietQR
                    StatusId = 1 // Default: Paid
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Add order items from cart
                foreach (var cartItem in cart)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = cartItem.ProductId,
                        SizeId = cartItem.SizeId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.UnitPrice / cartItem.Quantity, // UnitPrice per item
                        UnitCost = cartItem.UnitCost / cartItem.Quantity // UnitCost per item
                    };

                    _context.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync();

                // Clear cart after successful order
                HttpContext.Session.Remove("Cart");

                return Ok(new
                {
                    message = "Order confirmed successfully",
                    orderId = order.OrderId
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ConfirmPayment: {ex.Message}");
                return StatusCode(500, new { message = "Server error occurred" });
            }
        }

        // Phương thức phụ để lấy UserId từ token trong session
        private int? GetUserIdFromToken()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
                {
                    return userId;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Token validation error in GetUserIdFromToken: " + ex.Message);
                return null;
            }
        }
    }
}

public class ConfirmPaymentModel
{
    public decimal TotalAmount { get; set; }
    public string PaymentContent { get; set; }
    public AddressModel Address { get; set; }
    public List<OrderDetailModel> OrderDetails { get; set; }
}

public class AddressModel
{
    public int AddressId { get; set; }
    public int UserId { get; set; }
    public string RecipientName { get; set; }
    public string StreetAddress { get; set; }
    public string CityProvince { get; set; }
    public string PostalCode { get; set; }
}

public class OrderDetailModel
{
    public int ProductId { get; set; }
    public int SizeId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public static class SessionExtensions
{
    public static void SetObjectAsJson(this ISession session, string key, object value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value));
    }

    public static T GetObjectFromJson<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonConvert.DeserializeObject<T>(value);
    }
}