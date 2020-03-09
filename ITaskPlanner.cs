using System.Collections.Generic;

public interface ITaskPlanner
{
  IEnumerable<Assignment> Execute();
}