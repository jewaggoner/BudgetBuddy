using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Uplift.DataAccess.Data;
using Uplift.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Web;
using System.IO;
using System.Data;
using LumenWorks.Framework.IO.Csv;
using Microsoft.AspNetCore.Http;

namespace Uplift.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class LineItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> userManager;

        public LineItemsController(ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        //public LineItemsController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        // GET: Customer/LineItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.LineItem.Include(l => l.IdentityUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Customer/LineItems/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lineItem = await _context.LineItem
                .Include(l => l.IdentityUser)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (lineItem == null)
            {
                return NotFound();
            }

            return View(lineItem);
        }

        // GET: Customer/LineItems/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = userManager.GetUserId(HttpContext.User);
            return View();
        }

        // POST: Customer/LineItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Date,Description,Amount")] LineItem lineItem)
        {
            if (ModelState.IsValid)
            {
                lineItem.ID = Guid.NewGuid();
                lineItem.CreateDate = DateTime.Now;
                lineItem.UserId = userManager.GetUserId(HttpContext.User);
                _context.Add(lineItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = "123";
            return View(lineItem);
        }

        // GET: Customer/LineItems/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lineItem = await _context.LineItem.FindAsync(id);
            if (lineItem == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", lineItem.UserId);
            return View(lineItem);
        }

        // POST: Customer/LineItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ID,CreateDate,UserId,Date,Description,Amount")] LineItem lineItem)
        {
            if (id != lineItem.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lineItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LineItemExists(lineItem.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", lineItem.UserId);
            return View(lineItem);
        }

        // GET: Customer/LineItems/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lineItem = await _context.LineItem
                .Include(l => l.IdentityUser)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (lineItem == null)
            {
                return NotFound();
            }

            return View(lineItem);
        }

        // POST: Customer/LineItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lineItem = await _context.LineItem.FindAsync(id);
            _context.LineItem.Remove(lineItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LineItemExists(Guid id)
        {
            return _context.LineItem.Any(e => e.ID == id);
        }




        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile upload)
        {
            if (ModelState.IsValid)
            {

                if (upload != null && upload.Length > 0)
                {

                    if (upload.FileName.ToLower().EndsWith(".csv"))
                    {
                        Stream stream = upload.OpenReadStream();
                        DataTable csvTable = new DataTable();
                        using (CsvReader csvReader =
                            new CsvReader(new StreamReader(stream), true))
                        {
                            csvTable.Load(csvReader);
                        }
                        
                        

                        foreach (DataRow row in csvTable.Rows)
                        {

                            //try catch here
                            DateTime date = DateTime.ParseExact(row["Date"].ToString(), "MM/dd/yyyy",
                                System.Globalization.CultureInfo.InvariantCulture);
                            string description = row["Description"].ToString();
                            double amount = double.Parse(row["Amount"].ToString());

                            var lineItem = new LineItem();

                            lineItem.Date = date;
                            lineItem.Description = description;
                            lineItem.Amount = amount;
                            lineItem.ID = Guid.NewGuid();
                            lineItem.CreateDate = DateTime.Now;
                            lineItem.UserId = userManager.GetUserId(HttpContext.User);
                            _context.Add(lineItem);
                            await _context.SaveChangesAsync();
                            
                        }
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            return View();
        }

      

    }
}
