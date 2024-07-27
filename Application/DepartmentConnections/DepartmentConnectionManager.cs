using Departments.Presistence.Entities;
using System.Linq;

namespace Departments.Application.DepartmentConnections
{
    public class DepartmentConncetionManager
    {
        private List<DepartmentConnectionDto> _departmentConnections;

        public DepartmentConncetionManager(List<DepartmentConnectionDto> departmentConnections)
        {
            _departmentConnections = departmentConnections;
        }

        public List<int> GetAllChildrenIds(int parentId, List<int> result = null)
        {
            result ??= new();
            var children = _departmentConnections
                .Where(dc => dc.ParentId == parentId)
                .Select(dc => dc.ChildId)
                .ToList();

            if (children.Count == 0)
            {
                return result;
            }

            foreach (var childId in children)
            {
                result.Add(childId);
                GetAllChildrenIds(childId, result);
            }

            return result;
        }

        public List<int> GetAllParentIds(int childId, List<int> result = null)
        {
            result ??= new();
            var parents = _departmentConnections
                .Where(dc => dc.ChildId == childId)
                .Select(dc => dc.ParentId)
                .ToList();

            if (parents.Count == 0)
            {
                return result;
            }

            foreach (var parentId in parents)
            {
                result.Add(parentId);
                GetAllParentIds(parentId, result);
            }

            return result ?? new();
        }
    }
}
