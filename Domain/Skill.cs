/// <Summary>
/// A named skill with an associated skill level
/// </Summary>
public class Skill
{
  public int Id { get; set; }
  public string Name { get; set; }
  public int SkillLevel { get; set; }

  public override string ToString()
    => $"[{Id}] {Name}, Skill Level: {SkillLevel}";
}
