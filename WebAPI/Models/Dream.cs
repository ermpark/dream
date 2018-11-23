using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Dream
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Summary { get; set; }
        public string CateName { get; set; }
        public DateTime? CreateTime { get; set; }
    }
    public class DreamInfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int FkDreamId { get; set; }
        public string DreamName { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}
