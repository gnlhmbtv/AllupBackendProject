using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class ContactController : Controller
    {
        private readonly Context _context;
        public ContactController(Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            Contact contact = _context.Contacts.FirstOrDefault(); 
            return View(contact);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Contact contact)
        {
          
            Contact newContact = new Contact();

            newContact.Address = contact.Address;
            newContact.Email = contact.Email;
            newContact.Phone = contact.Phone;
            await _context.Contacts.AddAsync(newContact);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();
            Contact dbContact = await _context.Contacts.FindAsync(id);
            if (dbContact == null) return NotFound();
            return View(dbContact);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(Contact contact)
        {
            Contact dbContact = _context.Contacts.FirstOrDefault(c => c.Id == contact.Id);

            dbContact.Email = contact.Email;
            dbContact.Phone = contact.Phone;
            dbContact.Address = contact.Address;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
