using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELTest.Models
{
    public class ELTaskActivityTypeViewModel2
    {
        //viewmodel pro Index stranku
        public List<ELTask> ELTasks { get; set; }
        public List<SelectListItem> ActivityTypes { get; set; } 
        public string SelectedActivityTypeID { get; set; } 
        public string SearchString { get; set; }

        [DataType(DataType.Date)]
        public DateTime? From { get; set; }
        [DataType(DataType.Date)]
        public DateTime? To { get; set; }

        public int? NumberOfResultsPerPage { get; set; }
        public int? PageNumber { get; set; }

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
