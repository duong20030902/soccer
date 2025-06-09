

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

        public IActionResult Checkout()
        {
            return View();
        }
    }
}

