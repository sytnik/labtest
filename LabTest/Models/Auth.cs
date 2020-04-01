using System.ComponentModel.DataAnnotations;

namespace LabTest.Models
{
    public class Auth
    {
        [Required] public string Login { get; set; }
        [Required] public string Password { get; set; }
    }

}