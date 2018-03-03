namespace  MarketPlace.Api.Entities
{
    public class Entity
    {
        public string Name { get; set; }
        public string RegistrationNumber { get; set; }
        public string Url { get; set; }
        public string PrimaryEmail { get; set; }
        public int Id { get; set; }
        public EntityStatus Status { get; set; }
    }

    public enum EntityStatus
    {
        None = 0,
        Pending,
        Active,
        Deleted,
        Suspended
    }
}
