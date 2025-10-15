namespace FiszkiApp.EntityClasses.Models
{
    public class UserLoginResult
    {
        public int UserId { get; set; }
        public bool IsAdmin { get; set; }
        public string Message { get; set; }
    }
}
