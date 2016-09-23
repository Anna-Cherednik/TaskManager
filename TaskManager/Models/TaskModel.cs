using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class TaskModel
    {
        [Key]
        public int TaskId { get; set; }

        [UIHint("MultilineText")]
        public string Todo { get; set; }

        public bool IsPerfomed { get; set; }

        [Display(Name = "Shared by")]
        public string AutorId { get; set; }
        [ForeignKey("AutorId")]
        public ApplicationUser Autor { get; set; }

        public virtual ICollection<ApplicationUser> Managers { get; set; }
        public TaskModel()
        {
            Managers = new List<ApplicationUser>();
        }
    }
}