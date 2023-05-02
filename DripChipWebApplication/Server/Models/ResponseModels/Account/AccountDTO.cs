namespace DripChipWebApplication.Server.Models
{
    public class AccountDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public AccountDTO(Account account)
        {
            Id = account.Id;
            LastName = account.LastName;
            FirstName = account.FirstName;
            Email = account.Email;
        }
    }
}
