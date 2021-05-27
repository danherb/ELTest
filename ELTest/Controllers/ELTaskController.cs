using ELTest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELTest.Controllers
{
    public class ELTaskController : Controller
    {
        private readonly AppDbContext _context;

        public ELTaskController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, Models.ELTaskActivityTypeViewModel2 vm = null)
        {
            //do ViewData ulozim aktualni stav razeni, tenhle aktualni stav mi prijde i ze sortOrder, takze udelam toggle a ulozim zas do ViewData
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["AcivityTypeSortParm"] = sortOrder == "activityType" ? "activityType_desc" : "activityType";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";

            var tasks = _context.ELTasks.Include(t => t.ActivityType) as IQueryable<Models.ELTask>;
            var activityTypes = _context.ActivityTypes.ToList();


            //filtr dle typu aktivity
            if (vm is not null && !string.IsNullOrEmpty(vm.SelectedActivityTypeID))
            {
                if (int.TryParse(vm.SelectedActivityTypeID, out int activityType))
                    tasks = tasks.Where(t => t.ActivityTypeID == activityType);
            }

            //filtr dle datumu, vm neni null a zaroven je vyplneny datum "From" nebo "To"
            if (vm is not null)
            {
                if (vm.From is not null && vm.From.Value.Year > 1)
                { 
                    tasks = tasks.Where(t => t.Date.Value.Date >= vm.From.Value.Date);
                }

                if (vm.To is not null && vm.To.Value.Year > 1)
                {
                    tasks = tasks.Where(t => t.Date.Value.Date <= vm.To.Value.Date);
                }
            }

            //razeni dle sloupcu
            tasks = sortOrder switch
            {
                "name_desc" => tasks.OrderByDescending(t => t.Name),
                "activityType" => tasks.OrderBy(t => t.ActivityType.Name),
                "activityType_desc" => tasks.OrderByDescending(t => t.ActivityType.Name),
                "date" => tasks.OrderBy(t => t.Date),
                "date_desc" => tasks.OrderByDescending(t => t.Date),
                _ => tasks.OrderBy(t => t.Name),
            };

            vm = new Models.ELTaskActivityTypeViewModel2
            { 
                ELTasks = await tasks.AsNoTracking().ToListAsync()
            };

            vm.SetActivityTypes(activityTypes);

            return View(vm);
        }


        public IActionResult Create()
        {
            var activityTypes = _context.ActivityTypes.ToList();

            var vm = new Models.ELTaskActivityTypeViewModel();
            vm.SetActivityTypes(activityTypes);
          
            return View(vm);
        }

        // POST: ELTask/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //použije se Bind atribut proti over-posting, alternativní přístup je použít ViewModels
        [HttpPost]
        [ValidateAntiForgeryToken] //is used to prevent forgery of a request and is paired up with an anti-forgery token generated in the edit view file
        public async Task<IActionResult> Create(Models.ELTaskActivityTypeViewModel vm, string stopWatch)
        {
            if (!string.IsNullOrEmpty(stopWatch) && stopWatch.Equals("stopwatchStart"))
            { 
                vm.ELTask.From = DateTime.Now;
                vm.ELTask.Date = DateTime.Now.Date;
            }

            if (ModelState.IsValid)
            {
                _context.Add(vm.ELTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vm.ELTask);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.ELTasks
                .FirstOrDefaultAsync(t => t.ID == id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.ELTasks.FindAsync(id);
            _context.ELTasks.Remove(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.ELTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            var activityTypes = _context.ActivityTypes.ToList();

            var vm = new Models.ELTaskActivityTypeViewModel();
            vm.SetActivityTypes(activityTypes);
            vm.ELTask = task;

            return View(vm);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.ELTaskActivityTypeViewModel vm, string stopWatch)
        {
            if (id != vm.ELTask.ID)
            {
                return NotFound();
            }

            if (stopWatch.Equals("stopwatchStop"))
                vm.ELTask.To = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vm.ELTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(vm.ELTask.ID))
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
            return View(vm.ELTask);
        }

        public async Task<IActionResult> StopStopwatch(int? id, string stopWatch)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.ELTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            if (stopWatch.Equals("stopwatchStart"))
                task.To = DateTime.Now;

            try
            {
                _context.Update(task);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!TaskExists(task.ID))
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

        private bool TaskExists(int id)
        {
            return _context.ELTasks.Any(e => e.ID == id);
        }
    }
}
