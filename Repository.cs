using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace manpower
{
    public class Repository : RepositoryBase
    {
        public Repository(FileLocations fileLocations, string configPath)
            : base(fileLocations, configPath)
        {
        }
        public override void SaveAssignments(IEnumerable<Assignment> assignments, string saveToPath)
        {
            var writeObject = assignments.Select(x => new
            {
                TaskId = x.Task.Id,
                PersonId = x.Person.Id,
                Day = x.Day
            });
            using (StreamWriter writer = new StreamWriter(saveToPath))
            using (CsvWriter csv = new CsvWriter(writer))
            {
                csv.WriteRecords(writeObject);
            }
        }
    }
}
