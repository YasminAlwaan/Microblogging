using Microblogging.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Microblogging.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
}