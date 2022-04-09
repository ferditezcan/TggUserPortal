namespace tggUserPortal.Objects
{
    public class User
    {
        public string UserName { get; set; } = string.Empty;
        public byte[] PwHash { get; set; }
        public byte[] PwSalt { get; set; }
    }
}
