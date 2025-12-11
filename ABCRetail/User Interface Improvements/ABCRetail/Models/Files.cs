using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ABCRetail.Models
{
    [Table("Files")]

    //file entity associated with claims used for lecturers attaching documents
    public class Files
    {
        //primary key for the 'Files' table, uniquely identifying each file
        [Key]
        public int FileId { get; set; }

        //stores the name of the uploaded file
        public string FileName { get; set; }

        //stores the binary data of the file
        public byte[] Data { get; set; }

        //stores the size of the file in bytes
        public long Length { get; set; }

        //stores the MIME type of the file (e.g., "application/pdf", "image/png")
        public string ContentType { get; set; }

        //foreign key linking the file to a specific claim
        [ForeignKey("Claims")]
        public int ClaimId { get; set; }

        //navigation property to associate the file with a claim
        public Claims Claims { get; set; }
    }
}
