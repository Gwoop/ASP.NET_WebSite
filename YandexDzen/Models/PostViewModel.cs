using System.ComponentModel.DataAnnotations;
namespace YandexDzen.Models
{
    public class PostViewModel
    {

        [Key]
        public int id_Post { get; set; }

        public int id_User { get; set; }

        public string Namepost { get; set; }

        public string LoginUser { get; set; }
        public string path { get; set; }
        public string Main { get; set; }
    }
}
