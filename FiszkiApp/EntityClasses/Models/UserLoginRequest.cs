namespace FiszkiApp.EntityClasses.Models
{
    public class UserLoginRequest
    {
        public int UserId { get; set; }
        public byte[] EncryptedPassword { get; set; }
    }
}
