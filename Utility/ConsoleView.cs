using System;
using System.Collections.Generic;
using System.Linq;

internal class ConsoleView : IUIView
{
  private bool _verbose;

  public ConsoleView(bool verbose)
  {
    _verbose = verbose;
  }

  public void ReportStart(string candidate, DateTime startTime)
  {
    Console.WriteLine("\n===========================================");
    Console.WriteLine($"Manpower planner executed on {startTime:g}");
    Console.WriteLine($"\nCandidate: {candidate}");
    Console.WriteLine("===========================================");
  }

  public void ReportLoadedData(IRepository repository)
  {
    Console.WriteLine("\nLoaded data");

    if (_verbose)
    {
      ReportAllLoadedData();
    }
    else
    {
      ReportLoadedDataSummary();
    }

    void ReportAllLoadedData()
    {
      Console.WriteLine("Skills:\n");
      foreach (var skill in repository.Skills)
      {
        Console.WriteLine(skill);
      }

      Console.WriteLine("\n\nPeople:\n");
      foreach (var person in repository.People)
      {
        Console.WriteLine(person);
      }

      Console.WriteLine("\n\nTasks:\n");
      foreach (var task in repository.Tasks)
      {
        Console.WriteLine(task);
      }
    }

    void ReportLoadedDataSummary()
    {
      Console.WriteLine($"{repository.Skills.Count()} Skills");
      Console.WriteLine($"{repository.People.Count()} People");
      Console.WriteLine($"{repository.Tasks.Count()} Tasks");
    }
  }

  public TaskDataset GetDatasetSelection(IEnumerable<TaskDataset> choices)
  {
    Console.WriteLine("\n\nSelect the set of Tasks to load:\n");

    var choiceIndex = choices
          .Select((choice, index) => new { choice, index })
          .ToDictionary(i => i.index + 1, i => i.choice);

    foreach (var choice in choiceIndex)
    {
      Console.WriteLine($"{choice.Key} - {choice.Value.DatasetName}");
    }

    Console.Write("\nEnter the number of the dataset to load: ");
    var text = Console.ReadLine();

    if (!int.TryParse(text, out var selection) ||
        !choiceIndex.TryGetValue(selection, out var dataset))
    {
      Console.WriteLine("\nInvalid selection.\n");
      return null;
    }

    Console.WriteLine($"\nLoading '{dataset.DatasetName}'.\n");
    return dataset;
  }

  public void ReportResults(string candidate, TimeSpan elapsedTime, IEnumerable<Assignment> assignments)
  {
    Console.WriteLine("\n\n===========================================");
    Console.WriteLine($"RESULTS for candidate {candidate}\n");
    Console.WriteLine($"{assignments.Count()} allocation(s).");

    if (_verbose)
    {
      Console.WriteLine();

      foreach (var assignment in assignments)
      {
        Console.WriteLine(assignment);
      }
    }

    Console.WriteLine($"\nExecution time: {elapsedTime.TotalMilliseconds} ms");
    Console.WriteLine("===========================================\n");
  }

  public void ReportError(Exception e)
  {
    Console.WriteLine("\nSomething went wrong :(\n");
    Console.WriteLine(e.Message);
    Console.WriteLine("\n\n");
  }
}
