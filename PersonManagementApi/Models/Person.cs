using System.ComponentModel.DataAnnotations;

namespace PersonManagementApi.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
