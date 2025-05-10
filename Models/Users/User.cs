namespace BCA_Car_Auction.Models.Users
{
    public class User
    {
        private static int _nextId = 0;
        public int Id { get; init; }
        public string Name { get; init; }

        protected static int GetNextId() => Interlocked.Increment(ref _nextId);

        public User(string name)
        {
            this.Id = GetNextId();
            this.Name = name;
        }
    }
}
