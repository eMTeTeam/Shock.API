using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shock.API.DataModel;

namespace Shock.API.Data
{
    public class ObservationDbContext: DbContext
    {
        public ObservationDbContext(DbContextOptions<ObservationDbContext> options) : base(options) { }

        public DbSet<Observation> Observations { get; set; }
    }
}
