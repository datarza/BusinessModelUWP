﻿using System;
using Database.EntityGenerator;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database
{
	public class DatabaseContext : DbContext
	{
		public DbSet<Department> Departments { get; set; }
		public DbSet<Employee> Employees { get; set; }
		public DbSet<Project> Projects { get; set; }
		public DbSet<ProjectToEmployee> ProjectsToEmployees { get; set; }
		public DbSet<ResearchProject> ResearchProjects { get; set; }
		public DbSet<ProductionProject> ProductionProjects { get; set; }

		/// <summary>
		/// Creates the data rows for demonstration
		/// </summary>
		public void EnsureInitialized()
		{
			Random rng = new Random(1234567890);
			this.GenerateDepartments(rng);
			this.GenerateProjects(rng);
			this.GenerateEmployees(rng);
			this.GenerateProjectsToEmployees(rng);
		}

#if DEBUG
		private static LoggerFactory loggerFactory;
#endif

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite(@"Data Source=database.db");
#if DEBUG
			if (loggerFactory == null)
			{
				loggerFactory = new LoggerFactory();
				loggerFactory.AddProvider(new LoggerProvider());
			}
			optionsBuilder.UseLoggerFactory(loggerFactory);
#endif
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<ProjectToEmployee>().HasKey(c => new { c.ProjectId, c.EmployeeId });
			builder.Entity<ProjectToEmployee>().HasDiscriminator<int>(DBConstants.DiscriminatorFieldName)
				.HasValue<ResearchProject>(DBConstants.ProjecType.Research)
				.HasValue<ProductionProject>(DBConstants.ProjecType.Production);
		}
	}
}
