using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Admit_Scan
{
    public partial class Admit_Scan : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private bool DisplayStudentPhoto()
        {
            if (!FileUpload2.HasFiles)
            {
                LblScanStatus.Text = "Please upload the student photo.";
                return false;
            }
            string extension = System.IO.Path.GetExtension(FileUpload2.FileName).ToLower();

            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(extension))
            {
                LblScanStatus.Text = "Please upload a JPG, JPEG or PNG student photo.";
                return false;
            }

            if (FileUpload2.PostedFile.ContentLength > 2 * 1024 * 1024)
            {
                LblScanStatus.Text = "Student photo must be smaller than 2 MB.";
                return false;
            }

            byte[] photobyte = FileUpload2.FileBytes;

            string contentType = extension == ".png" ? "image/png" : "image/jpeg";
            
            string base64 = Convert.ToBase64String(photobyte);

            string imageUrl = "data:" + contentType + ";base64," + base64;

            ImgStudentPhoto.ImageUrl = imageUrl;

            Session["StudentPhoto"] = ImgStudentPhoto.ImageUrl;

            return true;
        }

        

        protected void BtnScan_Click1(object sender, EventArgs e)
        {
            if (!DisplayStudentPhoto())
            {
                return;
            }

            pnlStudentDetails.Visible = true;
            pnlStudentMarksheet.Visible = true;
            pnlGraduationDetails.Visible = true;
        }
    }
}