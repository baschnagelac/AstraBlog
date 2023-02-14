using System.ComponentModel.DataAnnotations;

namespace AstraBlog.Models
{
    public class Comment
    {
        //primary key
        public int Id { get; set; }

        [Required]
        [Display(Name = "Comment")]
        [StringLength(5000, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Body { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? Updated { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? UpdateReason { get; set; }

        //navigation properties (one blog, one commenter/author) 


        //FOREIGN key 1-to-1
        public int BlogPostId { get; set; }
        public virtual BlogPost? BlogPost { get; set; }


        //Foreign Key 1-to-many
        [Required]
        public virtual string? AuthorId { get; set; }

        public virtual BlogUser? Author { get; set; }
    }
}
