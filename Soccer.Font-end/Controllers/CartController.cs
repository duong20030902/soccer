//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
//using Soccer.Font_end.Models; // Update to use the new view model
//using System.Net.Http.Headers;

//namespace Soccer.Font_end.Controllers
//{
//    public class CartController : Controller
//    {
//        private readonly IHttpClientFactory _httpClientFactory;

//        public CartController(IHttpClientFactory httpClientFactory)
//        {
//            _httpClientFactory = httpClientFactory;
//        }

//        public async Task<IActionResult> Checkout()
//        {
//            var client = _httpClientFactory.CreateClient();
//            client.BaseAddress = new Uri("https://localhost:7237");
//            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//            // Send cookie to maintain session
//            var request = new HttpRequestMessage(HttpMethod.Get, "/api/cart/checkout");
//            var cookie = Request.Headers["Cookie"].ToString();
//            if (!string.IsNullOrEmpty(cookie))
//            {
//                request.Headers.Add("Cookie", cookie);
//            }

//            var response = await client.SendAsync(request);

//            if (!response.IsSuccessStatusCode)
//            {
//                ViewBag.Error = "Không thể tải dữ liệu giỏ hàng.";
//                return View("Checkout", new List<CartItemViewModel>());
//            }

//            var json = await response.Content.ReadAsStringAsync();
//            var cartItems = JsonConvert.DeserializeObject<List<CartItemViewModel>>(json);

//            return View(cartItems);
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Soccer.Font_end.ViewModels;
using System.Net.Http.Headers;

namespace Soccer.Font_end.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Checkout()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://api.soccer.soccertips.org/api");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Send cookie to maintain session
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/cart/checkout");
            var cookie = Request.Headers["Cookie"].ToString();
            if (!string.IsNullOrEmpty(cookie))
            {
                request.Headers.Add("Cookie", cookie);
            }

            try
            {
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = $"Không thể tải dữ liệu giỏ hàng. Mã lỗi: {response.StatusCode}";
                    return View("Checkout", new List<CartItemViewModel>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var cartItems = JsonConvert.DeserializeObject<List<CartItemViewModel>>(json) ?? new List<CartItemViewModel>();

                return View(cartItems);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi khi kết nối đến server: {ex.Message}";
                return View("Checkout", new List<CartItemViewModel>());
            }
        }
    }
}