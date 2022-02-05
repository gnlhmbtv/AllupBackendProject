using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.ViewComponents
{
    public class MailsViewComponent : ViewComponent
    {
        private readonly Context _context;
        public MailsViewComponent(Context context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Mail> mails = _context.Mails.ToList();
            return View(await Task.FromResult(mails));
        }
    }
}
