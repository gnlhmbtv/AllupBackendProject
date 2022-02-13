using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using AllupBackendProject.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AllupBackendProject.Controllers
{
    public class BasketController : Controller
    {
        private readonly Context _context;
        public BasketController(Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

            if (id == null) return RedirectToAction("Index", "Home");

            Product product = await _context.Products.Include(p => p.Campaign).Include(p => p.Brand)
                .Include(p => p.ProductPhotos).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            string basket = Request.Cookies["basketcookie"];
            List<BasketProductVM> basketProducts;

            if (basket == null) basketProducts = new List<BasketProductVM>();
            else basketProducts = JsonConvert.DeserializeObject<List<BasketProductVM>>(basket);

            BasketProductVM isExsistProduct = basketProducts.FirstOrDefault(p => p.Id == product.Id);
            if (isExsistProduct == null)
            {
                BasketProductVM basketProduct = new BasketProductVM
                {
                    Id = product.Id,
                    Name = product.Name,
                    UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    Count = 1,
                    BrandId = product.BrandId,
                    Discount = product.Campaign.Discount,
                    PhotoUrl = product.ProductPhotos[0].PhotoUrl,
                    Price = product.Price
                };
                basketProducts.Add(basketProduct);
            }
            else isExsistProduct.Count++;

            Response.Cookies.Append("basketcookie", JsonConvert.SerializeObject(basketProducts), new CookieOptions { MaxAge = TimeSpan.FromDays(14) });
            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> ShowBasket()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");
            var UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            string basket = Request.Cookies["basketcookie"];

            List<BasketProductVM> basketProducts = new List<BasketProductVM>();
            if (basket != null)
            {
                basketProducts = JsonConvert.DeserializeObject<List<BasketProductVM>>(basket);
                foreach (var item in basketProducts)
                {
                    Product product = await _context.Products.Include(p => p.Campaign)
                        .Include(p => p.ColorProducts)
                        .Include(p => p.Brand)
                        .Include(p => p.ProductPhotos)
                        .FirstOrDefaultAsync(p => p.Id == item.Id);
                    item.Price = product.Price;
                    item.PhotoUrl = product.ProductPhotos[0].PhotoUrl;
                    item.Name = product.Name;
                    item.Discount = product.Campaign.Discount;
                }
                Response.Cookies.Append("basketcookie", JsonConvert.SerializeObject(basketProducts), new CookieOptions { MaxAge = TimeSpan.FromDays(14) });

            }
            ViewBag.userid = UserId;
            return View(basketProducts);
        }

        public IActionResult BasketCount([FromForm] int id, string change)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");
            var UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string basket = Request.Cookies["basketcookie"];
            List<BasketProductVM> basketProducts = new List<BasketProductVM>();
            basketProducts = JsonConvert.DeserializeObject<List<BasketProductVM>>(basket);
            Product product = _context.Products.Find(id);
            var totalcount = 0;
            foreach (var item in basketProducts)
            {
                if (item.Id == id && item.UserId == UserId)
                {
                    if (change == "sub" && (item.Count) > 1)
                    {
                        item.Count--;
                        totalcount += item.Count;

                    }
                    if (change == "add" && item.Count != product.Quantity)
                    {
                        item.Count++;
                        totalcount += item.Count;
                    }
                    if (totalcount != 0) item.Count = totalcount;
                }

            }

            Response.Cookies.Append("basketcookie", JsonConvert.SerializeObject(basketProducts), new CookieOptions { MaxAge = TimeSpan.FromDays(14) });
            if (totalcount != 0)
            {
                return Ok(totalcount);
            }
            return Ok("error");
        }

        public IActionResult BasketRemove(int id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");
            var UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            string basket = Request.Cookies["basketcookie"];
            List<BasketProductVM> basketProducts = new List<BasketProductVM>();

            basketProducts = JsonConvert.DeserializeObject<List<BasketProductVM>>(basket);
            Product product = _context.Products.Find(id);
            foreach (var item in basketProducts)
            {
                if (item.Id == id && item.UserId == UserId)
                {
                    basketProducts.Remove(item);
                    break;
                }

            }
            Response.Cookies.Append("basketcookie", JsonConvert.SerializeObject(basketProducts), new CookieOptions { MaxAge = TimeSpan.FromDays(14) });
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Index()
        {
            return View();
        }
    }

}

