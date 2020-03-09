/// <Summary>
/// Associates a task with the day on which it will be performed and 
/// the person who will perform it.
/// </Summary>
/// <Remarks>
/// A manpower plan consists of a collection of assignments
///   - one assignment for each task in the plan.
/// </Remarks>
public class Assignment
{
  public Task Task { get; set; }
  public Person Person { get; set; }
  public int Day { get; set; }

  public override string ToString()
    => $"Day {Day} - Task {Task.Id}{ReportPriority()} - {Person.Name}";

  private string ReportPriority()
    => Task.IsPriority ? " [Priority]" : string.Empty;
}