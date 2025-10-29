using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIFlashCard.Models
{
    public class Notifications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_notification { get; set; }
        public int? UserID { get; set; }  
        public bool? User_Is_active { get; set; }
        public int? ID_Category { get; set; }
        [MaxLength(255)]
        public string? CategoryName { get; set; }
        [MaxLength(255)]
        public string TableName { get; set; }
        [Required]
        public DateTime ActionDate {  get; set; }

        [MaxLength(255)]
        public string Action { get; set; }
        [MaxLength(255)]
        public string? UserName { get; set; }
        public bool? Is_read { get; set; }
    }
}
