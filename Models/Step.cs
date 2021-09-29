using System;

namespace freezebee_api.Models
{
    public class Step : IEntityBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public Process Process { get; set; }
    }
}