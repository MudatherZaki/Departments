namespace Departments.Presistence.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public ICollection<DepartmentConnection> Parents { get; set; }
        public ICollection<DepartmentConnection> Children { get; set; }
    }
}
