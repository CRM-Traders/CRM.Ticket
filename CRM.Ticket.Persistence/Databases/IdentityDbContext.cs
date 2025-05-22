﻿using CRM.Ticket.Domain.Entities.Permissions;
using Microsoft.EntityFrameworkCore;

namespace CRM.Ticket.Persistence.Databases;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<Permission> Permissions => Set<Permission>();
}
