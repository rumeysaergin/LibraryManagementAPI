namespace LibraryManagementAPI.Domain.Entities
{
    public class MembershipPlan
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int MaxBorrowLimit { get; set; }

        public decimal Price { get; set; }

        public int DurationInDays { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<UserMembership> UserMemberships { get; set; }
            = new List<UserMembership>();
    }
}