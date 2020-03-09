using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using manpower;

/// <summary>Used to invoke the planner process and generate output</summary>
/// <remarks>The candidate is not expected to modify this class</remarks>
internal class Harness
{
  private readonly string _candidate;
  private readonly IRepository _repository;
  private readonly ITaskPlanner _taskPlanner;
  private readonly IEnumerable<TaskDataset> _datasetChoices;
  private readonly IUIView _view;

  // results of executing the task planner
  private IEnumerable<Assignment> _assignments;
  private TimeSpan _elapsedTime;
  private Exception _error;

  public Harness(
    string candidate,
    IRepository repository,
    ITaskPlanner taskPlanner,
    IEnumerable<TaskDataset> datasetChoices,
    IUIView view)
  {
    _candidate = candidate;
    _repository = repository;
    _taskPlanner = taskPlanner;
    _datasetChoices = datasetChoices;
    _view = view;
  }

  public void Execute()
  {
    try
    {
      _view.ReportStart(_candidate, DateTime.Now);

      var dataset = _view.GetDatasetSelection(_datasetChoices);
      _repository.LoadData(dataset.DatasetPath);

      _view.ReportLoadedData(_repository);

      Stopwatch stopwatch = Stopwatch.StartNew();
      _assignments = _taskPlanner.Execute();
      stopwatch.Stop();
      _elapsedTime = stopwatch.Elapsed;

      _repository.SaveAssignments(_assignments, dataset.SavePath);

      _view.ReportResults(_candidate, _elapsedTime, _assignments);
    }
    catch (Exception e)
    {
      _error = e;
      _view.ReportError(e);

      throw;
    }
  }
}