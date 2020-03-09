using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>An implementation of IRepository that works with .csv files</summary>
public abstract class RepositoryBase : IRepository
{
    protected RepositoryBase(FileLocations fileLocations, string configPath)
    {
        this.FileLocations = fileLocations ?? throw new ArgumentNullException(nameof(fileLocations));
        this.ConfigPath = configPath ?? throw new ArgumentNullException(nameof(configPath));
    }

    protected FileLocations FileLocations { get; }
    protected string ConfigPath { get; }
    public IEnumerable<Skill> Skills { get; private set; } = Enumerable.Empty<Skill>();
    public IEnumerable<Person> People { get; private set; } = Enumerable.Empty<Person>();
    public IEnumerable<Task> Tasks { get; private set; } = Enumerable.Empty<Task>();

    public abstract void SaveAssignments(IEnumerable<Assignment> assignments, string saveToPath);

    public virtual void LoadData(string taskListPath)
    {
        this.Skills = this.LoadSkills();
        Dictionary<int, Skill> skillsIndex = this.Skills.ToDictionary(i => i.Id);

        this.People = this.LoadPeople(skillsIndex);
        this.Tasks = this.LoadTasks(taskListPath, skillsIndex);
    }

    private IEnumerable<Skill> LoadSkills()
    {
        using (StreamReader reader = new StreamReader(this.GetPath(this.FileLocations.Skills)))
        using (CsvReader csv = new CsvReader(reader))
        {
            return csv.GetRecords<Skill>().ToList();
        }
    }

    private IEnumerable<Person> LoadPeople(Dictionary<int, Skill> skillsIndex)
    {
        List<Person> people;
        Dictionary<int, Person> peopleIndex;

        using (StreamReader reader = new StreamReader(this.GetPath(this.FileLocations.People)))
        using (CsvReader csv = new CsvReader(reader))
        {
            people = csv.GetRecords<Person>().ToList();
            peopleIndex = people.ToDictionary(i => i.Id);
        }

        using (StreamReader reader = new StreamReader(this.GetPath(this.FileLocations.SkillMatrix)))
        using (CsvReader csv = new CsvReader(reader))
        {
            var skillMatrixTypeDefinition = new
            {
                PersonId = default(int),
                SkillId = default(int)
            };

            var skillMatrix = csv.GetRecords(skillMatrixTypeDefinition);

            foreach (var item in skillMatrix)
            {
                if (!peopleIndex.TryGetValue(item.PersonId, out Person person))
                {
                    throw new InvalidOperationException($"Invalid skills matrix - no person found with id {item.PersonId}");
                }

                if (!skillsIndex.TryGetValue(item.SkillId, out Skill skill))
                {
                    throw new InvalidOperationException($"Invalid skills matrix - no skill found with id {item.SkillId}");
                }

                // add skill to person
                person.Skills.Add(skill);
            }
        }

        return people;
    }

    private IEnumerable<Task> LoadTasks(string datasetPath, Dictionary<int, Skill> skillsIndex)
    {
        using (StreamReader reader = new StreamReader(this.GetPath(datasetPath)))
        using (CsvReader csv = new CsvReader(reader))
        {
            var rawTaskDefinition = new
            {
                Id = default(int),
                SkillRequired = default(int),
                IsPriority = default(bool)
            };

            return csv
              .GetRecords(rawTaskDefinition)
              .Select(item =>
              {
                  if (!skillsIndex.TryGetValue(item.SkillRequired, out Skill skill))
                  {
                      throw new InvalidOperationException($"Invalid task list - no skill found with id {item.SkillRequired}");
                  }

                  return new Task { Id = item.Id, SkillRequired = skill, IsPriority = item.IsPriority };
              })
              .ToList();
        }
    }

    public IEnumerable<Assignment> Execute()
    {
        IOrderedEnumerable<Task> tasks = this.Tasks.OrderByDescending(x => x.IsPriority);
        List<Assignment> listAssignment = new List<Assignment>();

        foreach (Task task in tasks)
        {
            int day = 0;
            Person person = null;
            while (person is null)
            {
                day = day + 1;
                person = (from p in this.People
                          join listAssig in listAssignment.Where(x => x.Day == day) 
                          on p.Id equals listAssig.Person.Id
                          into temp
                          from assign in temp.DefaultIfEmpty()
                          where assign == null && p.Skills.Any(x => x.Id == task.SkillRequired.Id)
                          orderby p.Skills.Count
                          select p).FirstOrDefault();
            }
            listAssignment.Add(new Assignment
            {
                Day = day,
                Person = person,
                Task = task
            });
        }
        return listAssignment;
    }

    protected string GetPath(string fileName)
    {
        return Path.Combine(this.ConfigPath, fileName);
    }
}