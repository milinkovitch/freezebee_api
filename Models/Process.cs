using System;
using System.Collections.Generic;

namespace freezebee_api.Models
{
    public class Process : IEntityBase
    {
        public Process()
        {
            this.Steps = new HashSet<Step>();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Step> Steps { get; set; }
    }
}