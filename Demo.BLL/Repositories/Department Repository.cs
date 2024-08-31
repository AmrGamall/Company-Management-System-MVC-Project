using Demo.BLL.Interfaces;
using Demo.DAL.Contexts;
using Demo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class Department_Repository : GenericRepository<Department>, IDepartmentRepository
    {
        public Department_Repository(MVCAppDbContext dbContext):base(dbContext)
        {
            
        }

    }
}
