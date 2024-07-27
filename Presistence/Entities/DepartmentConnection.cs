namespace Departments.Presistence.Entities
{
    public class DepartmentConnection
    {
        public int ParentId { get; set; }
        public int ChildId { get; set; }
        public Department Parent { get; set; }
        public Department Child { get; set; }
    }
}
