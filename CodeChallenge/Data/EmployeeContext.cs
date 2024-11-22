using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Data
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {

        }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            //Create conversion for employee to Id
            var converter = new ValueConverter<Employee, string>(
                from => from.EmployeeId,
                to => Employees.FirstOrDefault(e => e.EmployeeId == to));

            //Set the Compensation's key to the employee, now that we can convert it to its Id
            modelBuilder.Entity<Compensation>()
                .HasKey(c => c.Employee);
            modelBuilder.Entity<Compensation>()
                .Property(c => c.Employee)
                .HasConversion(converter);
        }

		public DbSet<Employee> Employees { get; set; }

        public DbSet<Compensation> Compensations { get; set; }
    }
}
