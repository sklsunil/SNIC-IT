using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace manpower
{
  class Program
  {
    // Enter your name here - this helps us keep track of your results
    const string candidateName = "Sunil Kumar";

    private static ITaskPlanner CreatePlanner(IRepository repository)
    {
      // TODO : develop a planner class that assigns tasks to people

      // Create an implementation of the ITaskPlanner interface.
      //
      // The role of this class is to generate a sequence of Assignments that
      // associate a Task with a Person and a day.
      //
      // Your logic needs to decide which Person should perform each task and
      // on which day the task should be perfomed.
      // 
      // The objective is to make the best use of the available people so as to
      // perform all tasks in as few days as possible while adhering to the
      // constraints identified in the requirements.

      return new TaskPlanner(repository);
    }

    private static IRepository CreateRepository(FileLocations fileLocations, string configPath)
    {
      // TODO : subclass RepositoryBase and extend it to save your results to disk

      // The class RepositoryBase already implements all the logic to load data
      // from the existing .csv files
      //
      // It does not, however, have logic to save the resulting assignments to disk.
      //
      // You should derive a sub-class from RepositoryBase and implement the 
      // missing save logic.
      //
      // The target path to write the file to will be provided as an argument to the save method

      return new Repository(fileLocations, configPath);
    }

    static void Main(string[] args)
    {
      var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("config.json")
        .AddCommandLine(args)
        .Build();

      var fileLocations = configuration
        .GetSection("fileLocations")
        .Get<FileLocations>();

      var verbose = bool.Parse(configuration["verbose"]);

      // create a repository used to load/save data
      var repository = CreateRepository(fileLocations, Directory.GetCurrentDirectory());

      // create an instance of the planner that contains the logic to create assignments
      var taskPlanner = CreatePlanner(repository);

      // create a UI view appropriate for a console application
      var view = new ConsoleView(verbose);

      var datasetChoices = CreateDatasetChoices(fileLocations);

      // the harness invokes the planner and reports the results
      var harness =
        new Harness(
          candidateName,
          repository,
          taskPlanner,
          datasetChoices,
          view);

      harness.Execute();
    }

    private static IEnumerable<TaskDataset> CreateDatasetChoices(FileLocations fileLocations)
    {
      yield return new TaskDataset { DatasetName = "Small dataset", DatasetPath = fileLocations.SmallTaskList, SavePath = fileLocations.SmallAssignmentList };
      yield return new TaskDataset { DatasetName = "Large dataset", DatasetPath = fileLocations.LargeTaskList, SavePath = fileLocations.LargeAssignmentList };
    }
  }
}
