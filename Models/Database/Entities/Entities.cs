using System.ComponentModel.DataAnnotations;

namespace Onyx.Models.Database.Entities
{
    public class Product
    {
        [Key]
        public int ID { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public int Price { get; set; }
    }
}
