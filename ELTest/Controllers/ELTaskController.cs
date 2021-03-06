using ELTest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ELTest.Models;

namespace ELTest.Controllers
{
    public class ELTaskController : Controller
    {
        private readonly AppDbContext _context;

        public ELTaskController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, int? numOfResults, ELTaskActivityTypeViewModel2 vm = null)
        {
            //do ViewData ulozim aktualni stav razeni, tenhle aktualni stav mi prijde i ze sortOrder, takze udelam toggle a ulozim zas do ViewData
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : ""; //Sort by name ascending by default (i.e. if string is empty)
            ViewData["ActivityTypeSortParm"] = sortOrder == "activityType" ? "activityType_desc" : "activityType";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["PageNumber"] = vm.PageNumber;
            ViewData["CurrentSortOrder"] = sortOrder;

            var tasks = _context.ELTasks.Include(t => t.ActivityType) as IQueryable<ELTask>;
            var activityTypes = _context.ActivityTypes.ToList();


            //Filter by activity type
            if (vm is not null && !string.IsNullOrEmpty(vm.SelectedActivityTypeID))
            {
                if (int.TryParse(vm.SelectedActivityTypeID, out int activityType))
                    tasks = tasks.Where(t => t.ActivityTypeID == activityType);
            }

            //Filter by date
            if (vm is not null)
            {
                //"From" is filled
                if (vm.From is not null && vm.From.Value.Year > 1)
                { 
                    tasks = tasks.Where(t => t.Date.Value.Date >= vm.From.Value.Date);
                }

                //"To" is filled
                if (vm.To is not null && vm.To.Value.Year > 1)
                {
                    tasks = tasks.Where(t => t.Date.Value.Date <= vm.To.Value.Date);
                }
            }

            int count = tasks.Count();

            if (vm.NumberOfResultsPerPage is null || vm.NumberOfResultsPerPage == 0)
            {
                vm.NumberOfResultsPerPage = 3; //Default is 3
            }

            if (numOfResults.GetValueOrDefault() > 0)
            {
                vm.NumberOfResultsPerPage = numOfResults;
            }

            if (vm.PageNumber is null || vm.PageNumber == 0)
            {
                vm.PageNumber = 1; //First page by default
                ViewData["PageNumber"] = 1;
            }

            ViewData["LastPageIndex"] = Math.Ceiling((double)count / vm.NumberOfResultsPerPage.GetValueOrDefault());

            //Filter by column
            tasks = sortOrder switch
            {
                "name_desc" => tasks.OrderByDescending(t => t.Name),
                "activityType" => tasks.OrderBy(t => t.ActivityType.Name),
                "activityType_desc" => tasks.OrderByDescending(t => t.ActivityType.Name),
                "date" => tasks.OrderBy(t => t.Date),
                "date_desc" => tasks.OrderByDescending(t => t.Date),
                _ => tasks.OrderBy(t => t.Name),
            };

            //Pagination
            tasks = tasks.Skip((vm.PageNumber.GetValueOrDefault() - 1) * vm.NumberOfResultsPerPage.GetValueOrDefault()).Take(vm.NumberOfResultsPerPage.GetValueOrDefault());

            vm.ELTasks = await tasks.AsNoTracking().ToListAsync();

            vm.SetActivityTypes(activityTypes);

            return View(vm);
        }


        public IActionResult Create()
        {
            var activityTypes = _context.ActivityTypes.ToList();

            var vm = new ELTaskActivityTypeViewModel();
            vm.SetActivityTypes(activityTypes);
          
            return View(vm);
        }

        // POST: ELTask/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] //is used to prevent forgery of a request and is paired up with an anti-forgery token generated in the edit view file
        public async Task<IActionResult> Create(ELTaskActivityTypeViewModel vm, string stopWatch)
        {
            if (!string.IsNullOrEmpty(stopWatch) && stopWatch.Equals("stopwatchStart"))
            { 
                vm.ELTask.From = DateTime.Now;
                vm.ELTask.Date = DateTime.Now.Date;
            }

            var activityTypes = _context.ActivityTypes.ToList();
            vm.SetActivityTypes(activityTypes);

            if (ModelState.IsValid)
            {
                //if(vm.ELTask.From > vm.ELTask.To)

                _context.Add(vm.ELTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.ELTasks.FirstOrDefaultAsync(t => t.ID == id);

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

            var vm = new ELTaskActivityTypeViewModel();
            vm.SetActivityTypes(activityTypes);
            vm.ELTask = task;

            return View(vm);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ELTaskActivityTypeViewModel vm, string stopWatch)
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
