using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace Admit_Scan
{
    public partial class Admit_Scan : System.Web.UI.Page
    {

        private const int MAX_OCR_FILE_SIZE = 1350 * 1024;

        private byte[] CompressImageForOCR(
            byte[] originalBytes,
            out string fileName,
            out string contentType)
        {
            using (MemoryStream input = new MemoryStream(originalBytes))
            using (System.Drawing.Image originalImage = System.Drawing.Image.FromStream(input))
            {
                int originalWidth = originalImage.Width;
                int originalHeight = originalImage.Height;

                int maxDimension = 2400;

                long[] qualityLevels = { 80, 70, 60, 50 };

                for (int attempt = 0; attempt < 6; attempt++)
                {
                    double scale = Math.Min(
                        1.0,
                        (double)maxDimension /
                        Math.Max(originalWidth, originalHeight)
                    );

                    int width = Math.Max(1, (int)(originalWidth * scale));
                    int height = Math.Max(1, (int)(originalHeight * scale));

                    using (Bitmap resizedImage =
                        new Bitmap(width, height, PixelFormat.Format64bppArgb))
                    {
                        using (Graphics graphics = Graphics.FromImage(resizedImage))
                        {
                            graphics.Clear(Color.White);
                            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                            graphics.DrawImage(originalImage, 0, 0, width, height);
                        }

                        ImageCodecInfo jpegCodec =
                            ImageCodecInfo.GetImageEncoders()
                            .First(codec => codec.MimeType == "image/jpeg");

                        foreach (long quality in qualityLevels)
                        {
                            using (MemoryStream output = new MemoryStream())
                            using (EncoderParameters parameters =
                                new EncoderParameters(1))
                            {
                                parameters.Param[0] =
                                    new EncoderParameter(
                                        System.Drawing.Imaging.Encoder.Quality,
                                        quality
                                    );

                                resizedImage.Save(
                                    output,
                                    jpegCodec,
                                    parameters
                                );

                                byte[] compressedBytes = output.ToArray();

                                if (compressedBytes.Length <= MAX_OCR_FILE_SIZE)
                                {
                                    fileName = "CompressedDocument.jpg";
                                    contentType = "image/jpeg";

                                    return compressedBytes;
                                }
                            }
                        }
                    }

                    maxDimension = (int)(maxDimension * 0.80);
                }
            }

            throw new Exception(
                "Unable to compress the image below 1.35 MB. " +
                "Please upload a clearer image with smaller dimensions."
            );
        }
        private string OCR_API_KEY
        {
            get
            {
                return ConfigurationManager.AppSettings["OCRSpaceApiKey"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlStudentDetails.Visible = false;
                pnlStudentMarksheet.Visible = false;
                pnlGraduationDetails.Visible = false;
            }
        }

        private bool ValidateFile(FileUpload upload, string documentName, string[] allowedExtensions, int maxSizeMB)
        {
            if (!upload.HasFile)
            {
                LblScanStatus.Text = "Please upload the " + documentName + ".";

                return false;
            }

            string extension = Path.GetExtension(upload.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                LblScanStatus.Text = "Invalid" + documentName + ".Allowed Format" + string.Join(", ", allowedExtensions);

                return false;
            }

            long maxBytes = maxSizeMB * 1024L * 1024L;

            if (upload.PostedFile.ContentLength > maxBytes)
            {
                LblScanStatus.Text = documentName + " must be smaller than " + maxSizeMB + " MB.";

                return false;
            }

            if (upload.PostedFile.ContentLength == 0)
            {
                LblScanStatus.Text = documentName + " is empty.";

                return false;
            }

            return true;
        }

        private void ClearPreviousResult()
        {
            TxtStudentName.Text = "";
            TxtDOB.Text = "";
            TxtAadhaarNo.Text = "";
            TxtFatherName.Text = "";
            TxtMotherName.Text = "";
            TxtContNumber.Text = "";
            TxtAddress.Text = "";

            Txt10year.Text = "";
            Txt10percentage.Text = "";

            Txt12year.Text = "";
            Txt12percentage.Text = "";

            TxtSession.Text = "";

            TxtUniversity.Text = "";
            TxtCollegeName.Text = "";
            TxtCGPA.Text = "";
            TxtDegreeYear.Text = "";

            ImgStudentPhoto.ImageUrl = "";

            Session["StudentPhoto"] = null;
        }
        private bool DisplayStudentPhoto()
        {
            if (!FileUpload3.HasFile)
            {
                LblScanStatus.Text = "Please upload the student photo.";
                return false;
            }
            string extension = System.IO.Path.GetExtension(FileUpload3.FileName).ToLower();

            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(extension))
            {
                LblScanStatus.Text = "Please upload a JPG, JPEG or PNG student photo.";
                return false;
            }

            if (FileUpload3.PostedFile.ContentLength > 2 * 1024 * 1024)
            {
                LblScanStatus.Text = "Student photo must be smaller than 2 MB.";
                return false;
            }

            byte[] photobyte = FileUpload3.FileBytes;

            string contentType = extension == ".png" ? "image/png" : "image/jpeg";

            string base64 = Convert.ToBase64String(photobyte);

            string imageUrl = "data:" + contentType + ";base64," + base64;

            ImgStudentPhoto.ImageUrl = imageUrl;

            Session["StudentPhoto"] = ImgStudentPhoto.ImageUrl;

            return true;
        }

        private string ExtractTextUsingOCR(FileUpload upload)
        {
            if (string.IsNullOrWhiteSpace(OCR_API_KEY))
                throw new Exception("OCR API key is missing. Check Web.config.");

            if (upload == null || !upload.HasFile)
                throw new Exception("Please select a document before scanning.");

            string apiurl = "https://api.ocr.space/parse/image";

            string boundary = "---------------------------" +
                              DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request =
                (HttpWebRequest)WebRequest.Create(apiurl);

            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Headers.Add("apikey", OCR_API_KEY);

            request.Timeout = 120000;
            request.ReadWriteTimeout = 120000;
            request.KeepAlive = false;

            byte[] fileBytes = upload.FileBytes;

            string extension = Path.GetExtension(upload.FileName).ToLowerInvariant();

            string filename = Path.GetFileName(upload.FileName);

            string contentType =
                extension == ".pdf" ? "application/pdf" :
                extension == ".png" ? "image/png" :
                "image/jpeg";

            if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
            {
                fileBytes = CompressImageForOCR(fileBytes, out filename, out contentType);
            }
            else if (fileBytes.Length > MAX_OCR_FILE_SIZE)
            {
                throw new Exception(
        "PDF exceeds the OCR.Space Free Plan limit of 1.5 MB. " +
        "Please upload a smaller PDF."
    );
            }
            using (MemoryStream ms = new MemoryStream())
            {
                WriteFormField(ms, boundary, "language", "eng");
                WriteFormField(ms, boundary, "OCREngine", "2");
                WriteFormField(ms, boundary, "isOverlayRequired", "false");
                WriteFormField(ms, boundary, "detectOrientation", "true");

                string fileHeader =
                    "--" + boundary + "\r\n" +
                    "Content-Disposition: form-data; name=\"file\"; filename=\"" +
                    filename + "\"\r\n" +
                    "Content-Type: " + contentType + "\r\n\r\n";

                byte[] headerBytes = Encoding.UTF8.GetBytes(fileHeader);

                ms.Write(headerBytes, 0, headerBytes.Length);
                ms.Write(fileBytes, 0, fileBytes.Length);

                byte[] newLine = Encoding.UTF8.GetBytes("\r\n");
                ms.Write(newLine, 0, newLine.Length);

                byte[] endBytes = Encoding.UTF8.GetBytes(
                    "--" + boundary + "--\r\n"
                );

                ms.Write(endBytes, 0, endBytes.Length);

                byte[] requestBytes = ms.ToArray();

                request.ContentLength = requestBytes.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(requestBytes, 0, requestBytes.Length);
                }
            }

            string jsonResponse;

            using (HttpWebResponse response =
                (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader =
                    new StreamReader(response.GetResponseStream()))
                {
                    jsonResponse = reader.ReadToEnd();
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            Dictionary<string, object> result =
                serializer.Deserialize<Dictionary<string, object>>(jsonResponse);

            if (result == null)
                throw new Exception("Invalid response received from OCR service.");

            // Handle OCR.Space Free API throttling
            if (result.ContainsKey("error") && result["error"] != null)
            {
                string apiError = result["error"].ToString();

                if (apiError.Contains("E571"))
                {
                    throw new Exception(
                        "OCR.Space Free API is temporarily overloaded. " +
                        "Please wait 2-5 minutes and scan again. " +
                        "No paid plan is required."
                    );
                }

                throw new Exception(apiError);
            }

            // Check OCR processing errors
            if (result.ContainsKey("IsErroredOnProcessing") &&
                Convert.ToBoolean(result["IsErroredOnProcessing"]))
            {
                throw new Exception(GetOCRApiError(result));
            }

            // Check ParsedResults
            if (!result.ContainsKey("ParsedResults") || result["ParsedResults"] == null)
            {
                throw new Exception("OCR response does not contain ParsedResults.");
            }

            IEnumerable parsedResults = result["ParsedResults"] as IEnumerable;

            if (parsedResults == null)
            {
                throw new Exception("Unexpected OCR response format. ParsedResults is not a valid collection.");
            }

            StringBuilder extractedText = new StringBuilder();

            foreach (object item in parsedResults)
            {
                Dictionary<string, object> page =
                    item as Dictionary<string, object>;

                if (page == null)
                    continue;

                if (page.ContainsKey("ErrorMessage") &&
                    page["ErrorMessage"] != null &&
                    !string.IsNullOrWhiteSpace(page["ErrorMessage"].ToString()))
                {
                    throw new Exception("OCR page error: " + page["ErrorMessage"].ToString());
                }

                if (page.ContainsKey("ParsedText") &&
                    page["ParsedText"] != null)
                {
                    extractedText.AppendLine(page["ParsedText"].ToString());
                }
            }

            string finalText = extractedText.ToString().Trim();

            if (string.IsNullOrWhiteSpace(finalText))
            {
                throw new Exception(
                    "OCR processing completed, but no readable text was detected. " +
                    "Check document quality, orientation and file format."
                );
            }

            return finalText;
        }


        private void WriteFormField(MemoryStream stream, string boundary, string fieldName, string value)
        {
            string field =
                "--" + boundary + "\r\n" +
                "Content-Disposition: form-data; name=\"" +
                fieldName + "\"\r\n\r\n" +
                value + "\r\n";

            byte[] bytes = Encoding.UTF8.GetBytes(field);

            stream.Write(bytes, 0, bytes.Length);
        }

        private string GetOCRApiError(Dictionary<string, object> result)
        {
            StringBuilder message = new StringBuilder();

            if (result.ContainsKey("ErrorMessage"))
            {
                object error = result["ErrorMessage"];

                object[] errors = error as object[];

                if (errors != null)
                {
                    message.Append(string.Join(", ", errors));
                }
                else if (error != null)
                {
                    message.Append(error.ToString());
                }
            }

            if (result.ContainsKey("ErrorDetails") &&
                result["ErrorDetails"] != null)
            {
                message.Append(" ");
                message.Append(result["ErrorDetails"].ToString());
            }

            if (message.Length == 0)
            {
                message.Append("OCR processing failed. Please try again.");
            }

            return message.ToString();
        }

        private void ScanAadhaar(string text)
        {
            string cleanText = text.Replace("\r", "\n");

            string name = FindLabeledValue(cleanText, new string[] { "Name" });

            if (string.IsNullOrWhiteSpace(name))
            {
                name = FindFirstLikelyName(cleanText);
            }

            TxtStudentName.Text = string.IsNullOrWhiteSpace(name)
        ? "Not Available"
        : name;

            Match aadhaarMatch = Regex.Match(cleanText, @"\b\d{4}\s?\d{4}\s?\d{4}\b");

            TxtAadhaarNo.Text = aadhaarMatch.Success
            ? Regex.Replace(aadhaarMatch.Value, @"\s", "") : "Not Available";


            string dob = FindDateOfBirth(cleanText);

            TxtDOB.Text = string.IsNullOrWhiteSpace(dob)
                ? "Not Available"
                : dob;
        }

        private string FindLabeledValue(string text, string[] labels)
        {
            string[] lines = text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                string currentline = lines[i].Trim();

                foreach (string label in labels)
                {
                    int index = currentline.IndexOf(label, StringComparison.OrdinalIgnoreCase);

                    if (index >= 0)
                    {
                        string value = currentline.Substring(index + label.Length).Trim();

                        value = Regex.Replace(value, @"^[\s:: \-]+", "");

                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            return value;
                        }
                        if (i + 1 < lines.Length)
                        {
                            string nextline = lines[i + 1].Trim();

                            if (!string.IsNullOrWhiteSpace(nextline))
                            {
                                return nextline;
                            }
                        }
                    }
                }
            }

            return "";
        }
        private string FindFirstLikelyName(string text)
        {
            string[] lines = text.Split(
                new char[] { '\n' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string value = line.Trim();

                if (value.Length < 3 || value.Length > 50)
                    continue;

                if (Regex.IsMatch(value, @"\d"))
                    continue;

                if (Regex.IsMatch(
                    value,
                    @"(?i)government|india|unique|identification|" +
                    @"authority|aadhaar|male|female|address|dob"))
                    continue;

                if (Regex.IsMatch(value, @"^[A-Za-z ]+$"))
                {
                    return value;
                }
            }

            return "";

        }
        private string ExtractAddress(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            string cleanText = text.Replace("\r", "\n");

            Match match = Regex.Match(
                cleanText,
                @"(?is)Address\s*[:\-]?\s*(.*?)(?=\b\d{6}\b|VID|Mobile|Phone|$)"
            );

            if (match.Success)
            {
                return Regex.Replace(
                    match.Groups[1].Value,
                    @"\s+",
                    " "
                ).Trim();
            }

            return "";
        }


        private string ExtractParentsName(string text, string parent)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            string[] lines = text.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries
            );

            string pattern = parent == "Father"
                ? @"(?i)\b(?:father'?s?\s*name|fathers\s*name|father\s*name)\b\s*[:\-]?\s*(.*)"
                : @"(?i)\b(?:mother'?s?\s*name|mothers\s*name|mother\s*name)\b\s*[:\-]?\s*(.*)";

            for (int i = 0; i < lines.Length; i++)
            {
                Match match = Regex.Match(lines[i].Trim(), pattern);

                if (!match.Success)
                    continue;

                string name = match.Groups[1].Value.Trim();

                // Name is on the same line as the label
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }

                // Name is on the next line
                if (i + 1 < lines.Length)
                {
                    string nextLine = lines[i + 1].Trim();

                    if (!string.IsNullOrWhiteSpace(nextLine) &&
                        !Regex.IsMatch(nextLine, pattern))
                    {
                        return nextLine;
                    }
                }
            }

            return "";
        }
        private void ExtractedParentNamesFromMarksheet(string tenthText, string twelftText, string graduationText)
        {
            string father = "";
            string mother = "";

            string[] documents = { tenthText, twelftText, graduationText };

            foreach (string document in documents)
            {
                if (string.IsNullOrWhiteSpace(document)) continue;

                if (string.IsNullOrWhiteSpace(father))
                    father = ExtractParentsName(document, "Father");

                if (string.IsNullOrWhiteSpace(mother))
                    mother = ExtractParentsName(document, "Mother");
            }

            TxtFatherName.Text = string.IsNullOrWhiteSpace(father) ? "Not Available" : father;
            TxtMotherName.Text = string.IsNullOrWhiteSpace(mother) ? "Not Available" : mother;
        }

        private string FindDateOfBirth(string text)
        {
            string pattern =
        @"(?i)(?:Date\s*of\s*Birth|DOB|D\.O\.B|Birth\s*Date)\s*[:\-]?\s*" +
        @"(\d{1,2}[/-]\d{1,2}[/-]\d{4}|\d{4}[/-]\d{1,2}[/-]\d{1,2})";

            Match match = Regex.Match(text, pattern);

            return match.Success ? match.Groups[1].Value : "";
        }

        private void ScanMarksheet(string text, TextBox yearTextBox,
                                   TextBox percentageTextBox, bool extractDOB)
        {
            string year = FindPassingYear(text);

            if (!string.IsNullOrWhiteSpace(year))
            {
                yearTextBox.Text = year;
            }

            string percentage = FindPercentage(text);

            if (!string.IsNullOrWhiteSpace(percentage))
            {
                percentageTextBox.Text = percentage;
            }

            if (extractDOB)
            {
                string dob = FindDateOfBirth(text);

                TxtDOB.Text = !string.IsNullOrWhiteSpace(dob) ? dob : "Not Available";
            }
        }

        private string FindPassingYear(string text)
        {
            Match match = Regex.Match(
                text,
                @"(?i)(?:year\s*of\s*passing|passing\s*year|exam\s*year)\s*[:\-]?\s*(19\d{2}|20\d{2})"
            );

            if (match.Success)
                return match.Groups[1].Value;

            MatchCollection years = Regex.Matches(
                text,
                @"\b(?:19|20)\d{2}\b"
            );

            return years.Count > 0 ? years[0].Value : "";
        }
        private string FindPercentage(string text)
        {
            Match match = Regex.Match(
        text,
        @"(?i)(?:percentage|percent|aggregate|overall|total\s*percentage)\s*[:\-]?\s*(\d{1,3}(?:\.\d{1,2})?)\s*%?");

            if (match.Success)
                return match.Groups[1].Value + "%";

            match = Regex.Match(
                text,
                @"\b(\d{1,3}(?:\.\d{1,2})?)\s*%"
            );

            return match.Success ? match.Groups[1].Value + "%" : "";
        }

        private void ScanGraduation(string text)
        {
            string university = FindLabeledValue(text, new string[] { "University Name", "University" });
            if (!string.IsNullOrWhiteSpace(university))
            {
                TxtUniversity.Text = university;
            }

            string college = FindLabeledValue(text, new string[] { "College Name", "College", "Insitution" });
            if (!string.IsNullOrWhiteSpace(college))
            {
                TxtCollegeName.Text = college;
            }


            string year = FindPassingYear(text);
            if (!string.IsNullOrWhiteSpace(year))
            {
                TxtDegreeYear.Text = year;
            }

            string cgpa = FindCGPA(text);
            if (!string.IsNullOrWhiteSpace(cgpa))
            {
                TxtCGPA.Text = cgpa;
            }
        }


        private string FindCGPA(string text)
        {
            Match match = Regex.Match(
        text,
        @"(?i)(?:CGPA|C\.G\.P\.A|GPA)\s*[:\-]?\s*(\d{1,2}(?:\.\d{1,2})?)"
    );

            if (match.Success)
                return match.Groups[1].Value;

            return FindPercentage(text);
        }

        private bool ValidateGraduationMarksheet()
        {
            if (!FileUpload6.HasFile)
                return true;

            string[] allowedExtensions =
            {
        ".jpg", ".jpeg", ".png", ".pdf"
    };

            return ValidateFile(
                FileUpload6,
                "Graduation Marksheet",
                allowedExtensions,
                5
            );
        }
        protected void BtnScan_Click1(object sender, EventArgs e)
        {
            pnlStudentDetails.Visible = false;
            pnlStudentMarksheet.Visible = false;
            pnlGraduationDetails.Visible = false;

            LblScanStatus.Text = "";

            ClearPreviousResult();

            try
            {
                string[] imageExtensions = { ".jpg", ".jpeg", ".png" };
                string[] marksheetExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
                // Validate Aadhaar Front
                if (!ValidateFile(FileUpload1, "Aadhaar Front", imageExtensions, 5))
                    return;

                // Validate Aadhaar Back
                if (!ValidateFile(FileUpload2, "Aadhaar Back", imageExtensions, 5))
                    return;

                // Validate Student Photograph
                if (!ValidateFile(FileUpload3, "Student Photograph", imageExtensions, 2))
                    return;

                // Validate 10th Marksheet
                if (!ValidateFile(FileUpload4, "10th Marksheet", marksheetExtensions, 5))
                    return;

                // Validate 12th Marksheet
                if (!ValidateFile(FileUpload5, "12th Marksheet", marksheetExtensions, 5))
                    return;

                // Validate optional Graduation Marksheet
                if (!ValidateGraduationMarksheet())
                    return;

                // Display photograph
                if (!DisplayStudentPhoto())
                    return;

                LblScanStatus.Text = "Scanning Aadhaar Front..";
                string aadhaarfrontText = ExtractTextUsingOCR(FileUpload1);
                ScanAadhaar(aadhaarfrontText);

                LblScanStatus.Text = "Scanning Aadhaar Back..";
                string aadhaarbackText = ExtractTextUsingOCR(FileUpload2);

                string address = ExtractAddress(aadhaarbackText);
                TxtAddress.Text = string.IsNullOrWhiteSpace(address) ? "Not Available" : address;

                LblScanStatus.Text = "Scanning 10th marksheet...";
                string tenthText = ExtractTextUsingOCR(FileUpload4);
                ScanMarksheet(tenthText, Txt10year, Txt10percentage, false);

                LblScanStatus.Text = "Scanning 12th marksheet...";
                string twelftText = ExtractTextUsingOCR(FileUpload5);
                ScanMarksheet(twelftText, Txt12year, Txt12percentage, false);

                string graduationText = "";
                if (FileUpload6.HasFile)
                {
                    LblScanStatus.Text = "Scanning graduation marksheet...";
                    graduationText = ExtractTextUsingOCR(FileUpload6);
                    ScanGraduation(graduationText);
                    pnlGraduationDetails.Visible = true;
                }

                ExtractedParentNamesFromMarksheet(tenthText, twelftText, graduationText);
                pnlStudentDetails.Visible = true;
                pnlStudentMarksheet.Visible = true;
                pnlGraduationDetails.Visible = FileUpload6.HasFile;

                LblScanStatus.Text = FileUpload6.HasFile
                                     ? "All documents scanned successfully!"
                                     : "Documents scanned successfully! Graduation details not uploaded.";

            }
            catch (WebException ex)
            {
                string errormessage = "OCR service connection failed.";

                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    errormessage = "OCR request timed out. Please try again.";
                }

                else if (ex.Response != null)
                {
                    using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        errormessage = reader.ReadToEnd();
                    }
                }

                LblScanStatus.Text = "Scanning failed:" + errormessage;
            }

            catch (Exception ex)
            {
                LblScanStatus.Text = "Scanning failed: " + ex.Message;
            }
        }


        private string GetSelectedCategory()
        {
            if (RadioButton1.Checked)
                return "SC";

            if (RadioButton2.Checked)
                return "ST";

            if (RadioButton3.Checked)
                return "OBC";

            if (RadioButton4.Checked)
                return "Other";

            return "Not Selected";
        }

        private string CsvEscape(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "\"\"";

            value = value.Replace("\"", "\"\"");
            return "\"" + value + "\"";
        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtStudentName.Text))
                {
                    LblScanStatus.Text = "Please enter student name.";
                    return;
                }

                string folderpath = Server.MapPath("~/App-Data/");

                if (!Directory.Exists(folderpath))
                    Directory.CreateDirectory(folderpath);

                string filepath = Path.Combine(folderpath, "AdmitFlow_StudentRecords.csv");

                if (!File.Exists(filepath))
                {
                    string header =
                        "Date,Student Name,DOB,Aadhaar Number,Father Name," +
                        "Mother Name,Contact Number,Address,Category," +
                        "10th Year,10th Percentage,12th Year,12th Percentage," +
                        "Admission Session,University,College,Graduation CGPA," +
                        "Graduation Year";

                    File.WriteAllText(filepath, header + Environment.NewLine, new System.Text.UTF8Encoding(true));

                }

                string[] values =
                {
                    DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
                    TxtStudentName.Text,
                    TxtDOB.Text,
                    TxtAadhaarNo.Text,
                    TxtFatherName.Text,
                    TxtMotherName.Text,
                    TxtContNumber.Text,
                    TxtAddress.Text,
                    GetSelectedCategory(),
                    Txt10year.Text,
                    Txt10percentage.Text,
                    Txt12year.Text,
                    Txt12percentage.Text,
                    TxtSession.Text,
                    TxtUniversity.Text,
                    TxtCollegeName.Text,
                    TxtCGPA.Text,
                    TxtDegreeYear.Text
                };

                string row = string.Join(",", values.Select(x => CsvEscape(x)));

                File.AppendAllText(filepath,row+Environment.NewLine,new System.Text.UTF8Encoding(true));

                LblScanStatus.Text = "Student record successfully saved in Excel-compatible CSV!";
            }
            catch(Exception ex)
            {
                LblScanStatus.Text = "Save failed: " + ex.Message;
            }
        }
    }
}