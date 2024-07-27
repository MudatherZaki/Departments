using Departments.Application.DepartmentConnections;
using Departments.Presistence;
using Departments.Presistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NuGet.Protocol;
using System.ComponentModel;

namespace Departments.Application.Departments
{
    public class DepartmentService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public DepartmentService(ApplicationDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        private async Task<List<DepartmentConnectionDto>> GetCachedConnections()
        {
            var cachedConnections = _cache.Get("dep_connections") as List<DepartmentConnectionDto>;

            if (cachedConnections == null || cachedConnections.Count == 0)
            {
                cachedConnections = await _dbContext.DepartmentConnections
                    .Select(dc => new DepartmentConnectionDto()
                    {
                        ParentId = dc.ParentId,
                        ChildId = dc.ChildId
                    })
                    .ToListAsync();

                _cache.Set("dep_connections", cachedConnections);
            }

            return cachedConnections;
        }

        public async Task<List<DepartmentDto>> GetParents(
            int departmentId, 
            List<DepartmentConnectionDto> connections = null)
        {
            connections ??= await GetCachedConnections();
            var connectionManager = new DepartmentConncetionManager(connections);

            var parentIds = connectionManager.GetAllParentIds(departmentId);

            if(parentIds.Count == 0)
            {
                return new();
            }

            var parents = _dbContext.Departments
                .Where(d => parentIds.Contains(d.Id));

            return parents.Select(d => new DepartmentDto()
            {
                Id = d.Id,
                Logo = d.Logo,
                Name = d.Name
            }).ToList();
        }

        public async Task<List<DepartmentDto>> GetChildren(
            int departmentId,
            List<DepartmentConnectionDto> connections = null)
        {
            connections ??= await GetCachedConnections();
            var connectionManager = new DepartmentConncetionManager(connections);

            var childIds = connectionManager.GetAllChildrenIds(departmentId);

            if (childIds.Count == 0)
            {
                return new();
            }

            var children = _dbContext.Departments
                .Where(d => childIds.Contains(d.Id));

            return children.Select(d => new DepartmentDto()
            {
                Id = d.Id,
                Logo = d.Logo,
                Name = d.Name
            }).ToList();
        }

        public async Task<int> AddDepartment(DepartmentDto department)
        {
            var departmentToAdd = new Department()
            {
                Logo = department.Logo,
                Name = department.Name
            };

            _dbContext.Departments.Add(departmentToAdd);
            await _dbContext.SaveChangesAsync();

            return departmentToAdd.Id;
        }

        public async Task AddParents(int departmentId, List<int> parentIds)
        {
            var departmentIds = new List<int>
            {
                departmentId
            };
            departmentIds.AddRange(parentIds);
            await ValidateDepartmentExistience(departmentIds);

            var cachedConnections = await GetCachedConnections();

            foreach (var parentId in parentIds)
            {
                if (cachedConnections.Any(c => c.ChildId == departmentId && c.ParentId == parentId))
                {
                    throw new BadHttpRequestException($"Department of ID {parentId} is already a parent of the current department.");
                }
            }

            var subDepartments = await GetChildren(departmentId, cachedConnections);

            foreach (var parentId in parentIds)
            {
                var subdepartment = subDepartments
                    .Where(d => d.Id == parentId)
                    .FirstOrDefault();
                if (subdepartment is not null)
                {
                    throw new BadHttpRequestException($"{subdepartment.Name} is already a child of the current department, this operation will cause a loop.");
                }
            }

            var connectionsToAdd = parentIds
                .Select(pId => new DepartmentConnection()
                {
                    ChildId = departmentId,
                    ParentId = pId
                })
                .ToList();

            _dbContext.DepartmentConnections.AddRange(connectionsToAdd);
            await _dbContext.SaveChangesAsync();


            cachedConnections.AddRange(connectionsToAdd.Select(c => new DepartmentConnectionDto()
            {
                ChildId = c.ChildId,
                ParentId = c.ParentId
            }));

            _cache.Set("dep_connections", cachedConnections);

        }

        public async Task AddSubDepartments(int departmentId, List<int> subIds)
        {
            var departmentIds = new List<int>
            {
                departmentId
            };
            departmentIds.AddRange(subIds);
            await ValidateDepartmentExistience(departmentIds);

            var cachedConnections = await GetCachedConnections();

            foreach (var subId in subIds)
            {
                if(cachedConnections.Any(c => c.ChildId == subId && c.ParentId == departmentId))
                {
                    throw new BadHttpRequestException($"Department of ID {subId} is already a child of the current department.");
                }
            }

            var parentDepartments = await GetParents(departmentId, cachedConnections);

            foreach (var subId in subIds)
            {
                var parentDepartment = parentDepartments
                    .Where(d => d.Id == subId)
                    .FirstOrDefault();
                if (parentDepartment is not null)
                {
                    throw new BadHttpRequestException($"{parentDepartment.Name} is already a parent of the current department, this operation will cause a loop.");
                }
            }

            var connectionsToAdd = subIds
                .Select(subId => new DepartmentConnection()
                {
                    ChildId = subId,
                    ParentId = departmentId
                })
                .ToList();

            _dbContext.DepartmentConnections.AddRange(connectionsToAdd);
            await _dbContext.SaveChangesAsync();

            cachedConnections.AddRange(connectionsToAdd.Select(c => new DepartmentConnectionDto()
            {
                ChildId = c.ChildId,
                ParentId = c.ParentId
            }));

            _cache.Set("dep_connections", cachedConnections);
        }

        private async Task ValidateDepartmentExistience(List<int> departmentIds)
        {
            var existingDepartmentIds = await _dbContext.Departments
                .Where(d => departmentIds.Contains(d.Id))
                .Select(d => d.Id)
                .ToListAsync();

            var nonExistingDepartments = departmentIds
                .Where(id => !existingDepartmentIds.Contains(id))
                .ToList();

            if(nonExistingDepartments.Count > 0)
            {
                throw new BadHttpRequestException($"No department with the id(s) {string.Join(',', nonExistingDepartments)}");
            }
        }

        public async Task AddDepartmentConnection(int parentDepartmentId, int subDepartmentId)
        {
            var connectionToAdd = new Presistence.Entities.DepartmentConnection()
            {
                ChildId = subDepartmentId,
                ParentId = parentDepartmentId
            };
            _dbContext.DepartmentConnections.Add(connectionToAdd);
            await _dbContext.SaveChangesAsync();

            var cachedConnections = await GetCachedConnections();
            cachedConnections.Add(new DepartmentConnectionDto()
            {
                ParentId = parentDepartmentId,
                ChildId = subDepartmentId
            });

            _cache.Set("dep_connections", cachedConnections);
        }
    }
}
