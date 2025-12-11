using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

//GitHub commits track changes and enhance project collaboration

namespace ABCRetail.Models
{
    [Table("Claims")]
    public class Claims
    {
        //unique identifier for the claim
        public int Id { get; set; }

        [Required]
        public string DocumentName { get; set; }

        //description of the work done
        public string DescriptionOfWork { get; set; }

        //list of supporting documents for the claim
        //not mapped to the database, used for handling file uploads
        [Required]
        [DataType(DataType.MultilineText)]
        public string SupportingDocuments { get; set; }

        //UserID of the person submitting or associated with the claim.
        [Required]
        public string UserID { get; set; }

        //status of the claim, such as "Pending", "Approved", or "Rejected".
        [Required]
        [StringLength(20)]
        public string ClaimStatus { get; set; } = "Pending"; //set default status to "Pending"
        
        //property to store the date and time the claim was submitted
        public DateTime DateSubmitted { get; set; } = DateTime.Now;  //automatically set to the current date and time

        //collection to store the files associated with this claim.
        //a claim can have multiple related files.
        public virtual ICollection<Files> File { get; set; }

        //constructor to initialize the 'File' collection to prevent null reference exceptions
        public Claims()
        {
            File = new HashSet<Files>();
        }
    }
}
