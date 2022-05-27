using FarmManager.Domain.Interfaces;

namespace FarmManager.Domain.Entities
{
    public class FarmEntity : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
    }
}
