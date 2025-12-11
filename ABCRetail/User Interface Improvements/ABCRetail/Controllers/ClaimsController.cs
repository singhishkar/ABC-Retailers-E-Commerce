using ABCRetail.Data;
using ABCRetail.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace ABCRetail.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Constructor to initialize dependencies through dependency injection
        public ClaimsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager; // Assign UserManager instance to the private field
            _context = context; // Assign the database context instance to the private field
            _webHostEnvironment = webHostEnvironment; // Assign the web hosting environment instance to the private field
        }

        ////asynchronous action to display a summary view for a specific claim based on its id
        //public async Task<IActionResult> Summary(int id)
        //{
        //    //fetch the claim record from the database using the provided id
        //    var claim = await _context.Claims.FindAsync(id);

        //    //check if the claim does not exist and return a error
        //    if (claim == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(claim); //return the summary view with the claim data
        //}

        //httpget action to render the claims view
        [HttpGet]
        public IActionResult Claims()
        {
            return View(); //return the claims view to the client
        }

        //httppost action to process claims submission with an optional supporting document
        [HttpPost]
        public async Task<IActionResult> Claims(Claims claims, IFormFile supportingDocument)
        {
            //log to console that the claims action is called
            Console.WriteLine("Claims action called");

            //retrieve the current user's id from the claims principal
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //assign the retrieved user id to the claim's userid property
            claims.UserID = id;
            Console.WriteLine($"Assigned UserID: {claims.UserID}");

            //remove validation errors related to userid, claimstatus, and datesubmitted
            ModelState.Remove("UserID");
            claims.ClaimStatus = "Pending";
            ModelState.Remove("ClaimStatus");
            ModelState.Remove("DateSubmitted");

            //check if the supporting document is provided and not empty
            if (supportingDocument != null && supportingDocument.Length > 0)
            {
                //store the document content in memory stream
                using (var ms = new MemoryStream())
                {
                    await supportingDocument.CopyToAsync(ms);

                    //convert file bytes to a base64 string and generate a short guid
                    var fileBytes = ms.ToArray();
                    string base64String = Convert.ToBase64String(fileBytes).Substring(0, 17);
                    string shortGuid = Guid.NewGuid().ToString().Substring(0, 8).Trim();

                    //generate a unique prefix using submission date, names, and guid
                    string distinctPrefix = $"[{claims.DateSubmitted}]-{claims.DocumentName}_{shortGuid}_";

                    //store the generated document name in the claim model
                    claims.SupportingDocuments = distinctPrefix + base64String;
                }
            }
            else
            {
                //set supporting documents to null and log the absence of files
                claims.SupportingDocuments = null;
                Console.WriteLine("No supporting documents provided or file is empty.");
            }

            //remove validation errors related to document fields
            ModelState.Remove("supportingDocument");
            ModelState.Remove("SupportingDocuments");

            //check if the user is not logged in and add an error if so
            if (string.IsNullOrEmpty(id))
            {
                ModelState.AddModelError("UserID", "User must be logged in to submit a claim.");
                return View(claims);
            }

            //check if the model state is valid before proceeding
            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState is valid");

                //assign the current date to datesubmitted and calculate total amount
                claims.DateSubmitted = DateTime.Now;
                //claims.TotalAmount = claims.HoursWorked * claims.RatePerHour;

                //add the claim to the database and save changes
                _context.Add(claims);
                await _context.SaveChangesAsync();
                Console.WriteLine("Claim saved successfully");

                //check if a valid supporting document was uploaded
                if (supportingDocument != null && supportingDocument.Length > 0)
                {
                    Console.WriteLine($"File '{supportingDocument.FileName}' detected. Size: {supportingDocument.Length} bytes.");

                    //define permitted file extensions and validate the file type
                    var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".docx", ".xlsx", ".pdf" };
                    var extension = Path.GetExtension(supportingDocument.FileName).ToLowerInvariant();

                    //check if the extension is invalid and return an error if so
                    if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                    {
                        Console.WriteLine("Invalid file type detected.");
                        ModelState.AddModelError("", "Invalid file type.");
                        return View(claims);
                    }

                    //define permitted mime types and validate the mime type
                    var mimeType = supportingDocument.ContentType;
                    var permittedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf" };

                    //check if the mime type is invalid and return an error if so
                    if (!permittedMimeTypes.Contains(mimeType))
                    {
                        Console.WriteLine("Invalid MIME type detected.");
                        ModelState.AddModelError("", "Invalid MIME type.");
                        return View(claims);
                    }

                    //define the path for storing uploaded files
                    var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                    //create the uploads directory if it doesn't exist
                    if (!Directory.Exists(uploadsFolderPath))
                    {
                        Directory.CreateDirectory(uploadsFolderPath);
                        Console.WriteLine("Uploads directory created.");
                    }

                    //generate a unique file name and define the file path
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(supportingDocument.FileName);
                    var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                    //save the uploaded file to disk
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await supportingDocument.CopyToAsync(stream);
                        Console.WriteLine("File saved successfully to disk.");
                    }

                    //create a new files object and populate it with file metadata
                    var files = new Files
                    {
                        FileName = uniqueFileName,
                        Length = supportingDocument.Length,
                        ContentType = mimeType,
                        Data = System.IO.File.ReadAllBytes(filePath),
                        ClaimId = claims.Id
                    };

                    //add the file object to the database and save changes
                    _context.Files.Add(files);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("File model added to the database.");
                }
                else
                {
                    //set the supportingdocuments property to an empty string if no file is uploaded
                    claims.SupportingDocuments = "";
                    Console.WriteLine("No supporting documents provided or file is empty.");
                }

                return RedirectToAction("Claims");
            }

            //log to console that the modelstate is invalid and display validation errors
            Console.WriteLine("ModelState is invalid");
            foreach (var modelStateKey in ModelState.Keys)
            {
                var modelStateVal = ModelState[modelStateKey];
                foreach (var error in modelStateVal.Errors)
                {
                    Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                }
            }
            return View(claims); //return the view with the claim model in case of validation failure
        }


        //action to display a confirmation view after a claim is submitted
        public IActionResult ClaimSubmitted()
        {
            return View(); //return the claimsubmitted view
        }

        //asynchronous action to list all claims from the database
        public async Task<IActionResult> List()
        {
            //retrieve all claims from the database asynchronously
            var claims = await _context.Claims.ToListAsync();
            return View(claims); //pass the claims list to the view for rendering
        }

        //asynchronous action to view the history of claims submitted by the logged-in user
        public async Task<IActionResult> ViewHistory()
        {
            try
            {
                //get the current user's id from the claims principal
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine($"Logged in User ID: {userId}");

                //retrieve the user's claims from the database, including associated files
                var claims = await _context.Claims
                    .Include(c => c.File)
                    .Where(c => c.UserID == userId)
                    .ToListAsync();

                //check if no claims were found and log a message if the list is empty
                if (!claims.Any())
                {
                    Console.WriteLine("No claims found for this user.");
                }
                else
                {
                    //log the total number of claims found for the user
                    Console.WriteLine($"Number of claims found: {claims.Count}");

                    //iterate through the claims and log their details
                    foreach (var claim in claims)
                    {
                        Console.WriteLine($"Claim ID: {claim.Id}, Total Files: {claim.File.Count}");

                        //iterate through the files associated with each claim and log their details
                        foreach (var file in claim.File)
                        {
                            Console.WriteLine($"File ID: {file.FileId}, File Name: {file.FileName}");
                        }
                    }
                }
                return View(claims); //return the view with the user's claims history
            }
            catch (Exception ex)
            {
                //log the error message if an exception occurs
                Console.WriteLine($"An error occurred: {ex.Message}");
                return View("Error"); //return the error view in case of exceptions
            }
        }


        //asynchronous action to approve a claim by its id
        public async Task<IActionResult> Approve(int id)
        {
            //find the claim by id in the database
            var claims = _context.Claims.Find(id);

            //check if the claim does not exist and show an error message
            if (claims == null)
            {
                TempData["Message"] = "Claim not found"; //set an error message for the user
                TempData["MessageType"] = "error"; //indicate that the message type is an error
                return RedirectToAction("Index"); //redirect to the index page
            }

            //update the claim status to 'Approve'
            claims.ClaimStatus = "Approve";

            //save changes to the database
            _context.SaveChanges();

            //set a success message to notify the user about the approval
            TempData["Message"] = "Claim has been approved";
            TempData["MessageType"] = "success"; //indicate the message is a success
            return RedirectToAction("List"); //redirect to the list of claims
        }

        //asynchronous action to reject a claim by its id
        public async Task<IActionResult> Reject(int id)
        {
            //find the claim by id in the database
            var claims = _context.Claims.Find(id);

            //check if the claim does not exist and show an error message
            if (claims == null)
            {
                TempData["Message"] = "Claim not found"; //set an error message for the user
                TempData["MessageType"] = "error"; //indicate that the message type is an error
                return RedirectToAction("Index"); //redirect to the index page
            }

            claims.ClaimStatus = "Rejected"; //update the claim status to 'Rejected'
            _context.SaveChanges(); //save changes to the database

            //set a success message to notify the user about the rejection
            TempData["Message"] = "Your claim has been rejected - contact HR";
            TempData["MessageType"] = "success"; //indicate the message is a success
            return RedirectToAction("List"); //redirect to the list of claims
        }

        //asynchronous action to display all claims for updating their status
        public async Task<IActionResult> UpdateClaimStatus()
        {
            //retrieve all claims from the database asynchronously
            var claims = await _context.Claims.ToListAsync();

            return View(claims); //pass the claims to the view for rendering
        }


        //http post action to update the status of a claim based on its id
        [HttpPost]
        public async Task<IActionResult> UpdateClaimStatus(int claimId, string newStatus)
        {
            try
            {
                //log the attempt to update the claim status
                Console.WriteLine($"Attempting to update status. Claim ID: {claimId}, New Status: {newStatus}");

                //check if the new status is not null or empty
                if (!string.IsNullOrEmpty(newStatus))
                {
                    //find the claim by its id asynchronously
                    var claim = await _context.Claims.FindAsync(claimId);
                    if (claim != null)
                    {
                        //update the claim's status
                        claim.ClaimStatus = newStatus;

                        //mark the claim as updated in the context
                        _context.Update(claim);

                        //save changes to the database asynchronously
                        await _context.SaveChangesAsync();

                        //log the successful update of the claim status
                        Console.WriteLine($"Status updated to: {claim.ClaimStatus}");

                        //return a success response in JSON format
                        return Json(new { success = true, message = "Status updated successfully." });
                    }
                    else
                    {
                        //log if the claim is not found
                        Console.WriteLine($"Claim not found with ID: {claimId}");
                    }
                }
                else
                {
                    //log if the new status is null or empty
                    Console.WriteLine("New status is null or empty.");
                }
            }
            catch (Exception ex)
            {
                //log the error message and stack trace in case of exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            //return a failure response in JSON format if the update was unsuccessful
            return Json(new { success = false, message = "Failed to update status. Please try again." });
        }

        //asynchronous action to download a file by its id
        public async Task<IActionResult> DownloadFile(int id)
        {
            //find the file by its id asynchronously
            var file = await _context.Files.FirstOrDefaultAsync(f => f.FileId == id);

            //check if the file is not found and return a not found response
            if (file == null)
            {
                return NotFound();
            }

            //return the file as a downloadable response with the appropriate content type and filename
            return File(file.Data, file.ContentType, file.FileName);
        }
        public async Task<IActionResult> FileDownload(int id)
        {
            //find the file by its id asynchronously
            var file = await _context.Files.FirstOrDefaultAsync(f => f.ClaimId == id);

            //check if the file is not found and return a not found response
            if (file == null)
            {
                return NotFound();
            }

            //return the file as a downloadable response with the appropriate content type and filename
            return File(file.Data, file.ContentType, file.FileName);
        }

    }
}
