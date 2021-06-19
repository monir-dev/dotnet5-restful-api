using System.ComponentModel.DataAnnotations;

namespace dotnet5_jwt_starter.Api.Dtos
{
    public class CreateOrUpdateEmployeeDto
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
}
