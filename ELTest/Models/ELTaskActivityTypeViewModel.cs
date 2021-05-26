using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELTest.Models
{
    //viewmodel pro Create stranku
    public class ELTaskActivityTypeViewModel
    {
        public ELTask ELTask { get; set; }
        public List<SelectListItem> ActivityTypes { get; set; } //This allows the user to select a genre from the list.
        public int SelectedActivityTypeID { get; set; }

        public void SetActivityTypes(IEnumerable<ActivityType> activityTypes)
        {
            ActivityTypes = activityTypes.Select(a => new SelectListItem
            {
                Value = a.ActivityTypeID.ToString(),
                Text = a.Name
            })
            .ToList();
        }
    }
}
