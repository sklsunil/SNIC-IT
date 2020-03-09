using System.Collections.Generic;

namespace manpower
{
    public class TaskPlanner : ITaskPlanner
    {
        private readonly IRepository _repository;

        public TaskPlanner(IRepository repository)
        {
            this._repository = repository;
        }

        public IEnumerable<Assignment> Execute()
        {
            return this._repository.Execute();
        }
    }
}
