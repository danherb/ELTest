using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELTest.Models
{
    public class ELTaskActivityTypeViewModel2
    {
        //viewmodel pro Index stranku
        public List<ELTask> ELTasks { get; set; }
        public SelectList ActivityTypes { get; set; } //This allows the user to select a genre from the list.
        public string SelectedActivityTpe { get; set; } //contains the selected genre
        public string SearchString { get; set; } //contains the text users enter in the search text box.
    }
}
