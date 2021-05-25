using ELTest.Models;
using System.Linq;

namespace ELTest.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.ActivityTypes.Any())
            {
                return;   // DB has been seeded
            }

            var activityTypes = new ActivityType[]
            {
            new ActivityType{Name="Aktivita 1"},
            new ActivityType{Name="Aktivita 2"},
            new ActivityType{Name="Aktivita 3"}
            };

            foreach (var activityType in activityTypes)
            {
                context.ActivityTypes.Add(activityType);
            }
            context.SaveChanges();
        }
    }
}
