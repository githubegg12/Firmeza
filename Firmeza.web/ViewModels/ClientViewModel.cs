using System.ComponentModel.DataAnnotations;

namespace Firmeza.web.ViewModels
{
    public class ClientViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Client name is required.")]
        [StringLength(100)]
        [Display(Name = "Full Name or Company Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Document ID is required.")]
        [StringLength(20, ErrorMessage = "Document ID cannot be longer than 20 characters.")]
        [Display(Name = "Document ID (NIT, DNI, etc.)")]
        public string Document { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100)]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(20)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200)]
        public string Address { get; set; }
    }
}
