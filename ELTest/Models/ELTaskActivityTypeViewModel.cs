using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace ELTest.Models
{
    //Viewmodel for Create page
    public class ELTaskActivityTypeViewModel
    {
        public ELTask ELTask { get; set; }
        public List<SelectListItem> ActivityTypes { get; set; } 

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
