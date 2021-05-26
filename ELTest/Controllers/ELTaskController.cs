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

        public async Task<IActionResult> Index()
        {
            var list = await _context.ELTasks.ToListAsync();
            return View(list);
        }

        //pro filtrovani
        public async Task<IActionResult> Index2(string selectedActivityType, string searchString)
        {
            var activityTypes = _context.ActivityTypes as IQueryable<Models.ActivityType>;

            var tasks = _context.ELTasks as IQueryable<Models.ELTask>;

            if (!string.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(s => s.Name.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(selectedActivityType))
            {
                tasks = tasks.Where(x => x.ActivityType.Name == selectedActivityType);
            }

            var vm = new Models.ELTaskActivityTypeViewModel2
            {
                ActivityTypes = new SelectList(await activityTypes.Distinct().ToListAsync()),
                ELTasks = await tasks.ToListAsync()
            };

            return View(vm);
        }

        public IActionResult Create()
        {
            var activityTypes = _context.ActivityTypes.ToList();

            var vm = new Models.ELTaskActivityTypeViewModel();
            vm.SetActivityTypes(activityTypes);
          
            return View(vm);
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //použije se Bind atribut proti over-posting, alternativní přístup je použít ViewModels
        [HttpPost]
        [ValidateAntiForgeryToken] //is used to prevent forgery of a request and is paired up with an anti-forgery token generated in the edit view file
        public async Task<IActionResult> Create(Models.ELTaskActivityTypeViewModel vm)
        {

            //jen pokus ale to by stejne bylo uplne blbe
            vm.ELTask.ActivityType.Name = _context.ActivityTypes.Where(_ => _.ActivityTypeID == vm.ELTask.ActivityType.ActivityTypeID).FirstOrDefault().Name;

            if (ModelState.IsValid)
            {
                _context.Add(vm.ELTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vm.ELTask);
        }
    }
}
