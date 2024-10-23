using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public IEnumerable<object> User { get; internal set; }
    }
}

// dotnet ef dbcontext scaffold "Server=studentmanagement.cv24y6i8c9w5.us-east-2.rds.amazonaws.com,1433;Database=StudentManagementDB;User Id=Biswadeep;Password=Biswa_9804349341;" Microsoft.EntityFrameworkCore.SqlServer -o Models
// dotnet ef dbcontext scaffold "Server=studentmanagement.cv24y6i8c9w5.us-east-2.rds.amazonaws.com,1433;Database=StudentManagementDB;User Id=Biswadeep;Password=Biswa_9804;Encrypt=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models
