using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using AllupBackendProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AllupBackendProject.Controllers
{
    public class ContactController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<AppUser> _userManager;

        public ContactController(Context context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            Contact contact = _context.Contacts.FirstOrDefault();
            //List<CompanySlider> companySliders = _context.CompanySliders.ToList();
            //List<Service> services = _context.Services.ToList();
            //ViewData["CompanySliders"] = companySliders;
            //ViewData["Services"] = services;
            ContactVM contactVm = new ContactVM();
            if (User.Identity.IsAuthenticated)
            {
                contactVm.User = await _userManager.FindByIdAsync(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            }
            contactVm.Contact = contact;
            return View(contactVm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromForm] Message message)
        {
            if (User.Identity.IsAuthenticated)
            {
                var dataComment = new Message();

                dataComment.UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                dataComment.Title = message.Title;
                dataComment.Text = message.Text;

                await _context.Messages.AddAsync(dataComment);
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("LogIn", "Account");
            }

            return Ok(new { Message = "Success" });
        }
    }
}
   