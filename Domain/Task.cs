using System;
using CsvHelper.Configuration.Attributes;

/// <Summary>A single job to be performed by a person that has the specified required skill</Summary>
/// <Remarks>
/// Each task takes a single person a full day to perform.
/// If a task is flagged as a priority it is expected that teh task will be completed as early as possible.
/// </Remarks>
public class Task
{
  public int Id { get; set; }
  public Skill SkillRequired { get; set; }
  public bool IsPriority { get; set; }

  public override string ToString()
    => $"Task {Id}, Requires skill {SkillRequired.Name}{ReportPriority()}";
  private string ReportPriority()
    => IsPriority ? " [Priority]" : string.Empty;
}