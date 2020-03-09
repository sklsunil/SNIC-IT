using System.Collections.Generic;

public interface IUIView
{
  void ReportStart(string candidate, System.DateTime startTime);
  void ReportLoadedData(IRepository repository);

  TaskDataset GetDatasetSelection(IEnumerable<TaskDataset> choices);

  void ReportResults(string candidate, System.TimeSpan elapsedTime, System.Collections.Generic.IEnumerable<Assignment> assignments);
  void ReportError(System.Exception e);
}
