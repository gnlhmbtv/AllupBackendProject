using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class MessageController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<AppUser> _userManager;

        public MessageController(Context context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: MessageController
        public ActionResult Index()
        {
            List<Message> messages = _context.Messages.Include(m => m.User).ToList();
            return View(messages);
        }

        // GET: MessageController/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            List<Message> messages = _context.Messages.Include(u => u.User).ToList();
            var oneMessage = messages.Find(x => x.Id == id);

            AppUser user = await _userManager.FindByIdAsync(oneMessage.UserId);
            oneMessage.User = user;

            return View(oneMessage);
        }

        // GET: MessageController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MessageController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MessageController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MessageController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MessageController/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            List<Message> messages = _context.Messages.Include(u => u.User).ToList();
            var oneMessage = messages.Find(x => x.Id == id);

            AppUser user = await _userManager.FindByIdAsync(oneMessage.UserId);
            oneMessage.User = user;

            return View(oneMessage);
        }

        // POST: MessageController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            Message message = await _context.Messages.FindAsync(id);
            _context.Messages.Remove(message);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
