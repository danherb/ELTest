using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELTest.Models
{
    public class ELTask : IValidatableObject
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
        public int ActivityTypeID { get; set; }
        public ActivityType ActivityType { get; set; }

        //Not required because it will be filled automatically in case of use stopwatch 
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [DataType(DataType.Time)]
        public DateTime? From { get; set; }

        [DataType(DataType.Time)]
        public DateTime? To { get; set; }

        public string Total => (To?.Year > 1) ? (To - From)?.ToString("hh\\:mm") : (DateTime.Now - From)?.ToString("hh\\:mm");

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (To < From)
            {
                yield return new ValidationResult("\"From\" has to be lesser than \"To\".");
            }
        }
    }
}
