namespace FarmManager.Domain.Dtos
{
    public class FarmDto
    {
        public FarmDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
