using System.Collections.Generic;
using System.Linq;
using MoreLinq;

/// <Summary>A named individual with a set of skills.</Summary>
public class Person
{
  public int Id { get; set; }
  public string Name { get; set; }
  public HashSet<Skill> Skills { get; set; } = new HashSet<Skill>();

  public override string ToString()
    => $"[{Id}] {Name}, Skills: {SkillsToString()}";
  private string SkillsToString()
    => Skills.Select(i => i.Name).ToDelimitedString(",");
}
