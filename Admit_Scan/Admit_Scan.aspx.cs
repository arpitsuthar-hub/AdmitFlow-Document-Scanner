using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Admit_Scan
{
    public partial class Admit_Scan : System.Web.UI.Page
    {
        private const int MAX_OCR_FILE_SIZE = 1350 * 1024;

        private class SubjectMarks
        {
            public string Name;
            public int Obtained;
            public int Max;

        }
        private string _careOfName = "";

        // =========================================================
        // 01. OCR API KEY
        // =========================================================
        private string OCR_API_KEY
        {
            get
            {
                return ConfigurationManager.AppSettings["OCRSpaceApiKey"];
            }
        }


        // =========================================================
        // 02. PAGE LOAD
        // =========================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlStudentDetails.Visible = false;
                pnlStudentMarksheet.Visible = false;
                pnlGraduationDetails.Visible = false;

                panelsContainer.Attributes["class"] = "panels-container two-panels";

                LblScanStatus.Text = "";

                GenerateAdmissionSession();
            }
        }

        private void GenerateAdmissionSession()
        {
            DateTime today = DateTime.Today;

            int StartYear = today.Month >= 4 ? today.Year : today.Year - 1;

            TxtSession.Text = StartYear + "-" + (StartYear + 1);
        }


        // =========================================================
        // 03. FILE VALIDATION
        // =========================================================
        private bool ValidateFile(FileUpload upload, string documentName, string[] allowedExtensions, int maxSizeMB)
        {
            if (upload == null || !upload.HasFile)
            {
                LblScanStatus.Text = "Please upload the " + documentName + ".";
                return false;
            }

            string extension = Path.GetExtension(upload.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                LblScanStatus.Text = "Invalid file format for " + documentName + ". Allowed formats: " + string.Join(", ", allowedExtensions);
                return false;
            }

            if (upload.PostedFile.ContentLength == 0)
            {
                LblScanStatus.Text = documentName + " is empty.";
                return false;
            }

            long maxBytes = maxSizeMB * 1024L * 1024L;

            if (upload.PostedFile.ContentLength > maxBytes)
            {
                LblScanStatus.Text = documentName + " must be smaller than " + maxSizeMB + " MB.";
                return false;
            }

            return true;
        }


        private bool ValidateGraduationMarksheet()
        {
            if (!FileUpload6.HasFile)
                return true;

            string[] allowedExtensions =
            {
                ".jpg", ".jpeg", ".png", ".pdf"
            };

            return ValidateFile(FileUpload6, "Graduation Marksheet", allowedExtensions, 5);
        }


        // =========================================================
        // 04. CLEAR PREVIOUS RESULTS
        // =========================================================

        private void ClearPreviousResult()
        {
            TxtStudentName.Text = "";
            TxtDOB.Text = "";
            TxtAadhaarNo.Text = "";

            TxtFatherName.Text = "";
            TxtMotherName.Text = "";

            TxtContNumber.Text = "";
            TxtAddress.Text = "";
            TxtCity.Text = "";
            TxtState.Text = "";
            TxtPinCode.Text = "";

            Txt10Board.Text = "";
            Txt10year.Text = "";
            Txt10Subject.Text = "";
            Txt10percentage.Text = "";

            Txt12Board.Text = "";
            Txt12year.Text = "";
            Txt12Subject.Text = "";
            Txt12percentage.Text = "";

            GenerateAdmissionSession();

            TxtUniversity.Text = "";
            TxtCollegeName.Text = "";
            TxtCGPA.Text = "";
            TxtDegreeYear.Text = "";

            ImgStudentPhoto.ImageUrl = "";

            Session["StudentPhoto"] = null;
        }


        // =========================================================
        // 05. STUDENT PHOTO
        // =========================================================
        private bool DisplayStudentPhoto()
        {
            if (!FileUpload3.HasFile)
            {
                LblScanStatus.Text = "Please upload the student photograph.";
                return false;
            }

            string extension = Path.GetExtension(FileUpload3.FileName).ToLowerInvariant();

            string[] allowedExtensions =
            {
                ".jpg", ".jpeg", ".png"
            };

            if (!allowedExtensions.Contains(extension))
            {
                LblScanStatus.Text = "Please upload a JPG, JPEG or PNG photograph.";
                return false;
            }

            if (FileUpload3.PostedFile.ContentLength > 2 * 1024 * 1024)
            {
                LblScanStatus.Text = "Student photograph must be smaller than 2 MB.";
                return false;
            }

            byte[] photoBytes = FileUpload3.FileBytes;

            string contentType = extension == ".png" ? "image/png" : "image/jpeg";

            string base64 = Convert.ToBase64String(photoBytes);

            string imageUrl = "data:" + contentType + ";base64," + base64;

            ImgStudentPhoto.ImageUrl = imageUrl;

            Session["StudentPhoto"] = imageUrl;

            return true;
        }


        // =========================================================
        // 06. IMAGE COMPRESSION FOR OCR
        // =========================================================
        private byte[] CompressImageForOCR(byte[] originalBytes, out string fileName, out string contentType)
        {
            using (MemoryStream input = new MemoryStream(originalBytes))
            using (System.Drawing.Image originalImage = System.Drawing.Image.FromStream(input))
            {
                int originalWidth = originalImage.Width;
                int originalHeight = originalImage.Height;
                int maxDimension = 2400;

                long[] qualityLevels =
                {
                    80, 70, 60, 50, 40
                };

                ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageEncoders()
                    .First(codec => codec.MimeType == "image/jpeg");

                for (int attempt = 0; attempt < 7; attempt++)
                {
                    double scale = Math.Min(1.0, (double)maxDimension / Math.Max(originalWidth, originalHeight));

                    int width = Math.Max(1, (int)(originalWidth * scale));
                    int height = Math.Max(1, (int)(originalHeight * scale));

                    using (Bitmap resizedImage = new Bitmap(width, height, PixelFormat.Format24bppRgb))
                    {
                        using (Graphics graphics = Graphics.FromImage(resizedImage))
                        {
                            graphics.Clear(Color.White);

                            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                            graphics.DrawImage(originalImage, 0, 0, width, height);
                        }

                        foreach (long quality in qualityLevels)
                        {
                            using (MemoryStream output = new MemoryStream())
                            using (EncoderParameters parameters = new EncoderParameters(1))
                            {
                                parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

                                resizedImage.Save(output, jpegCodec, parameters);

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

            throw new Exception("Unable to compress the image below 1.35 MB.Please upload a clearer image with smaller dimensions.");
        }


        // =========================================================
        // 07. OCR SPACE API
        // =========================================================
        private string ExtractTextUsingOCR(FileUpload upload, bool isTable = false)
        {
            if (string.IsNullOrWhiteSpace(OCR_API_KEY))
            {
                throw new Exception("OCR API key is missing. Please check Web.config.");
            }

            if (upload == null || !upload.HasFile)
            {
                throw new Exception("Please select a document before scanning.");
            }

            string apiUrl = "https://api.ocr.space/parse/image";

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);

            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Headers.Add("apikey", OCR_API_KEY);
            request.Timeout = 120000;
            request.ReadWriteTimeout = 120000;
            request.KeepAlive = false;

            byte[] fileBytes = upload.FileBytes;

            string extension = Path.GetExtension(upload.FileName).ToLowerInvariant();

            string fileName = Path.GetFileName(upload.FileName);

            string contentType = extension == ".pdf" ? "application/pdf" : extension == ".png" ? "image/png" : "image/jpeg";

            // Compress images before sending to OCR.Space.

            if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
            {
                fileBytes = CompressImageForOCR(fileBytes, out fileName, out contentType);
            }
            else if (fileBytes.Length > MAX_OCR_FILE_SIZE)
            {
                throw new Exception("PDF exceeds the OCR.Space Free Plan file-size limit. Please upload a smaller PDF.");
            }


            using (MemoryStream ms = new MemoryStream())
            {
                WriteFormField(
                    ms,
                    boundary,
                    "language",
                    "eng"
                );

                WriteFormField(
                    ms,
                    boundary,
                    "OCREngine",
                    "2"
                );

                WriteFormField(
                    ms,
                    boundary,
                    "isOverlayRequired",
                    "false"
                );

                WriteFormField(
                    ms,
                    boundary,
                    "detectOrientation",
                    "true"
                );

                // Table documents (marksheets): return the text row by row
                if (isTable)
                {
                    WriteFormField(
                        ms,
                        boundary,
                        "isTable",
                        "true"
                    );
                }

                string fileHeader =
                    "--" + boundary + "\r\n" +
                    "Content-Disposition: form-data; name=\"file\"; filename=\"" +
                    fileName + "\"\r\n" +
                    "Content-Type: " + contentType + "\r\n\r\n";

                byte[] headerBytes =
                    Encoding.UTF8.GetBytes(fileHeader);

                ms.Write(
                    headerBytes,
                    0,
                    headerBytes.Length
                );

                ms.Write(
                    fileBytes,
                    0,
                    fileBytes.Length
                );

                byte[] newLine =
                    Encoding.UTF8.GetBytes("\r\n");

                ms.Write(
                    newLine,
                    0,
                    newLine.Length
                );

                byte[] endBytes =
                    Encoding.UTF8.GetBytes(
                        "--" + boundary + "--\r\n"
                    );

                ms.Write(
                    endBytes,
                    0,
                    endBytes.Length
                );

                byte[] requestBytes =
                    ms.ToArray();

                request.ContentLength =
                    requestBytes.Length;

                using (Stream requestStream =
                    request.GetRequestStream())
                {
                    requestStream.Write(
                        requestBytes,
                        0,
                        requestBytes.Length
                    );
                }
            }


            // Read OCR response.

            string jsonResponse = "";

            using (HttpWebResponse response =
                (HttpWebResponse)request.GetResponse())
            using (StreamReader reader =
                new StreamReader(response.GetResponseStream()))
            {
                jsonResponse = reader.ReadToEnd();
            }


            JavaScriptSerializer serializer =
                new JavaScriptSerializer();

            Dictionary<string, object> result =
                serializer.Deserialize<Dictionary<string, object>>(
                    jsonResponse
                );

            if (result == null)
            {
                throw new Exception(
                    "Invalid response received from OCR service."
                );
            }


            // Check OCR API errors.

            if (result.ContainsKey("IsErroredOnProcessing") &&
                result["IsErroredOnProcessing"] != null &&
                Convert.ToBoolean(result["IsErroredOnProcessing"]))
            {
                throw new Exception(
                    GetOCRApiError(result)
                );
            }

            if (result.ContainsKey("ErrorMessage") &&
                result["ErrorMessage"] != null)
            {
                throw new Exception(
                    result["ErrorMessage"].ToString()
                );
            }


            // Read ParsedResults.

            if (!result.ContainsKey("ParsedResults") ||
                result["ParsedResults"] == null)
            {
                throw new Exception(
                    "OCR response does not contain ParsedResults."
                );
            }

            IEnumerable parsedResults =
                result["ParsedResults"] as IEnumerable;

            if (parsedResults == null)
            {
                throw new Exception(
                    "Unexpected OCR response format."
                );
            }

            StringBuilder extractedText =
                new StringBuilder();

            foreach (object item in parsedResults)
            {
                Dictionary<string, object> page =
                    item as Dictionary<string, object>;

                if (page == null)
                    continue;

                if (page.ContainsKey("ErrorMessage") &&
                    page["ErrorMessage"] != null &&
                    !string.IsNullOrWhiteSpace(
                        page["ErrorMessage"].ToString()))
                {
                    throw new Exception(
                        page["ErrorMessage"].ToString()
                    );
                }

                if (page.ContainsKey("ParsedText") &&
                    page["ParsedText"] != null)
                {
                    extractedText.AppendLine(
                        page["ParsedText"].ToString()
                    );
                }
            }

            string finalText =
                extractedText.ToString().Trim();

            if (string.IsNullOrWhiteSpace(finalText))
            {
                throw new Exception(
                    "No readable text was detected. " +
                    "Please check the document quality and orientation."
                );
            }

            return finalText;
        }


        // =========================================================
        // 08. MULTIPART FORM FIELD
        // =========================================================

        private void WriteFormField(
            MemoryStream stream,
            string boundary,
            string fieldName,
            string value)
        {
            string field =
                "--" + boundary + "\r\n" +
                "Content-Disposition: form-data; name=\"" +
                fieldName + "\"\r\n\r\n" +
                value + "\r\n";

            byte[] bytes =
                Encoding.UTF8.GetBytes(field);

            stream.Write(
                bytes,
                0,
                bytes.Length
            );
        }


        // =========================================================
        // 09. OCR ERROR MESSAGE
        // =========================================================

        private string GetOCRApiError(
            Dictionary<string, object> result)
        {
            StringBuilder message =
                new StringBuilder();

            if (result.ContainsKey("ErrorMessage") &&
                result["ErrorMessage"] != null)
            {
                object error =
                    result["ErrorMessage"];

                object[] errors =
                    error as object[];

                if (errors != null)
                {
                    message.Append(
                        string.Join(", ", errors)
                    );
                }
                else
                {
                    message.Append(error.ToString());
                }
            }

            if (result.ContainsKey("ErrorDetails") &&
                result["ErrorDetails"] != null)
            {
                if (message.Length > 0)
                    message.Append(" ");

                message.Append(
                    result["ErrorDetails"].ToString()
                );
            }

            if (message.Length == 0)
            {
                message.Append(
                    "OCR processing failed. Please try again."
                );
            }

            return message.ToString();
        }

        private string[] GetLines(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new string[0];

            text = text.Replace("\r\n", "\n").Replace('\r', '\n')
                       .Replace('\t', ' ').Replace('’', '\'')
                       .Replace('‘', '\'').Replace('`', '\'');

            return text.Split('\n').Select(l => Regex.Replace(l, @"\s+", " ").Trim()).Where(l => l.Length > 0).ToArray();
        }
        private string ToTitle(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return "";

            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLowerInvariant());
        }
        private string CleanPersonName(string v)
        {
            if (string.IsNullOrWhiteSpace(v))
                return "";

            v = Regex.Replace(v, @"[^A-Za-z .]", " ");
            v = Regex.Replace(v, @"\s+", " ").Trim(' ', '.');

            return v.Length < 3 ? "" : v;
        }

        private bool IsLikelyName(string v)
        {
            if (string.IsNullOrWhiteSpace(v) || v.Length < 3 || v.Length > 50)
                return false;

            if (!Regex.IsMatch(v, @"^[A-Za-z][A-Za-z .']+$"))
                return false;

            if (Regex.IsMatch(v,
               @"(?i)government|india|unique|identification|authority|aadhaar|" +
                @"aadhar|male|female|address|dob|date|birth|year|enrolment|" +
                @"enrollment|download|help|vid|issued|verify|school|board"))
                return false;

            return true;
        }

        private string FormatPercent(double value)
        {
            return value.ToString("0.00", CultureInfo.InvariantCulture) + "%";
        }

        // =========================================================
        // 10. AADHAAR FRONT EXTRACTION
        // =========================================================

        private void ScanAadhaar(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            string[] lines = GetLines(text);
            string joined = string.Join("\n", lines);

            // Aadhaar number: 12 digits, never part of the 16-digit VID
            string aadhaar = "";

            foreach (string line in lines)
            {
                if (Regex.IsMatch(line, @"(?i)\bVID\b"))
                    continue;

                Match m = Regex.Match(
                    line,
                    @"(?<!\d)(?<!\d\s)(\d{4})\s?(\d{4})\s?(\d{4})(?!\s?\d)");

                if (m.Success)
                {
                    aadhaar = m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value;
                    break;
                }
            }

            TxtAadhaarNo.Text = aadhaar == "" ? "Not Available" : aadhaar;

            // Date of birth
            string dob = FindDateOfBirth(joined);

            int dobLine = -1;

            for (int i = 0; i < lines.Length; i++)
            {
                if (Regex.IsMatch(lines[i],
                    @"(?i)\b(DOB|D\.O\.B|Date\s*of\s*Birth|Year\s*of\s*Birth)\b"))
                {
                    dobLine = i;
                    break;
                }
            }

            if (dob == "")
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    Match d = Regex.Match(
                        lines[i],
                        @"(?<!\d)(\d{2})[/\-.](\d{2})[/\-.]((?:19|20)\d{2})(?!\d)");

                    if (d.Success)
                    {
                        dob = d.Groups[1].Value + "/" + d.Groups[2].Value + "/" + d.Groups[3].Value;

                        if (dobLine < 0)
                            dobLine = i;

                        break;
                    }
                }
            }

            TxtDOB.Text = dob == "" ? "Not Available" : dob;

            string name = "";

            if (dobLine > 0)
            {
                for (int i = dobLine - 1; i >= Math.Max(0, dobLine - 3); i--)
                {
                    if (IsLikelyName(lines[i]))
                    {
                        name = lines[i];
                        break;
                    }
                }
            }

            if (name == "")
                name = CleanPersonName(FindLabeledValue(joined, new string[] { "Name" }));

            if (name == "")
                name = FindFirstLikelyName(joined);

            TxtStudentName.Text = name == "" ? "Not Available" : ToTitle(name);
        }


        // =========================================================
        // 11. FIND LABELED VALUE
        // =========================================================

        private string FindLabeledValue(string text, string[] labels)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            string[] lines = text.Split(
                new char[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries
            );

            for (int i = 0; i < lines.Length; i++)
            {
                string currentLine =
                    lines[i].Trim();

                foreach (string label in labels)
                {
                    Match match = Regex.Match(
                        currentLine,
                        @"(?i)^" +
                        Regex.Escape(label) +
                        @"\s*[:\-]?\s*(.*)$"
                    );

                    if (!match.Success)
                        continue;

                    string value =
                        match.Groups[1].Value.Trim();

                    if (!string.IsNullOrWhiteSpace(value))
                        return value;

                    if (i + 1 < lines.Length)
                    {
                        string nextLine =
                            lines[i + 1].Trim();

                        if (!string.IsNullOrWhiteSpace(nextLine))
                            return nextLine;
                    }
                }
            }

            return "";

        }


        // =========================================================
        // 12. FIND LIKELY STUDENT NAME
        // =========================================================
        private string FindFirstLikelyName(string text)
        {
            string[] lines = text.Split(
                new char[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries
            );

            foreach (string line in lines)
            {
                string value =
                    line.Trim();

                if (value.Length < 3 || value.Length > 50)
                    continue;

                if (Regex.IsMatch(value, @"\d"))
                    continue;

                if (Regex.IsMatch(
                    value,
                    @"(?i)government|india|unique|identification|" +
                    @"authority|aadhaar|male|female|address|dob|" +
                    @"date|birth|year|enrolment"
                ))
                {
                    continue;
                }

                if (Regex.IsMatch(value, @"^[A-Za-z ]+$"))
                {
                    return value;
                }
            }

            return "";

        }


        // =========================================================
        // 13. AADHAAR ADDRESS EXTRACTION
        // =========================================================
        private string ExtractAddress(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            string cleanText =
                text.Replace("\r", "\n");

            string[] lines = cleanText.Split(
                new char[] { '\n' },
                StringSplitOptions.RemoveEmptyEntries
            );

            StringBuilder address =
                new StringBuilder();

            bool addressStarted = false;

            foreach (string line in lines)
            {
                string currentLine =
                    line.Trim();

                if (Regex.IsMatch(
                    currentLine,
                    @"(?i)^Address\s*[:\-]?"
                ))
                {
                    addressStarted = true;

                    currentLine = Regex.Replace(
                        currentLine,
                        @"(?i)^Address\s*[:\-]?",
                        ""
                    ).Trim();
                }

                if (!addressStarted)
                    continue;

                if (Regex.IsMatch(
                    currentLine,
                    @"(?i)\b(VID|Mobile|Phone|DOB|Date of Birth)\b"
                ))
                {
                    break;
                }

                if (address.Length > 0)
                    address.Append(" ");

                address.Append(currentLine);

                if (Regex.IsMatch(
                    currentLine,
                    @"\b\d{6}\b"
                ))
                {
                    break;
                }
            }

            return Regex.Replace(
                address.ToString(),
                @"\s+",
                " "
            ).Trim();

        }

        private string StripAddressLabel(string s)
        {
            return Regex.Replace(s,
                @"(?i)^(?:vtc|po|post(?:\s*office)?|dist(?:rict)?|sub\s*dist(?:rict)?|" +
                @"city|town|village|vill|tehsil|taluka)\.?\s*[:\-]\s*", "").Trim();
        }

        // =========================================================
        // ADDRESS, CITY, STATE AND PIN CODE EXTRACTION
        // =========================================================
        private void ExtractAddressDetails(string text)
        {
            TxtAddress.Text = "";
            TxtCity.Text = "";
            TxtState.Text = "";
            TxtPinCode.Text = "";
            _careOfName = "";

            if (string.IsNullOrWhiteSpace(text))
                return;

            string address = ExtractAddress(text);

            if (string.IsNullOrWhiteSpace(address))
                address = text;

            address = Regex.Replace(address, @"\s+", " ").Trim();

            MatchCollection pins = Regex.Matches(address, @"(?<!\d)[1-9]\d{5}(?!\d)");

            if (pins.Count > 0)
            {
                Match pin = pins[pins.Count - 1];
                TxtPinCode.Text = pin.Value;
                address = address.Remove(pin.Index, pin.Length);
            }

            // Indian States and Union Territories
            string[] states =
            {
                "Andaman and Nicobar Islands", "Andhra Pradesh", "Arunachal Pradesh",
                "Assam", "Bihar", "Chandigarh", "Chhattisgarh",
                "Dadra and Nagar Haveli and Daman and Diu", "Delhi", "Goa", "Gujarat",
                "Haryana", "Himachal Pradesh", "Jammu and Kashmir", "Jharkhand",
                "Karnataka", "Kerala", "Ladakh", "Lakshadweep", "Madhya Pradesh",
                "Maharashtra", "Manipur", "Meghalaya", "Mizoram", "Nagaland", "Odisha",
                "Puducherry", "Punjab", "Rajasthan", "Sikkim", "Tamil Nadu",
                "Telangana", "Tripura", "Uttar Pradesh", "Uttarakhand", "West Bengal"
            };

            string foundState = null;
            int foundIdx = -1;
            int foundLen = 0;

            foreach (string state in states)
            {
                MatchCollection ms = Regex.Matches(address, @"\b" + Regex.Escape(state) + @"\b", RegexOptions.IgnoreCase);

                if (ms.Count > 0)
                {
                    Match last = ms[ms.Count - 1];

                    if (last.Index > foundIdx)
                    {
                        foundState = state;
                        foundIdx = last.Index;
                        foundLen = last.Length;
                    }
                }
            }

            if (foundState != null)
            {
                TxtState.Text = foundState;
                address = address.Remove(foundIdx, foundLen);
            }

            Match co = Regex.Match(address, @"(?i)^\s*([SDWC])\s*/\s*O\s*[:,\-]?\s*([A-Za-z][A-Za-z .]*?)\s*(?:,|$)");

            if (co.Success)
            {
                string rel = co.Groups[1].Value.ToUpperInvariant();

                if (rel == "S" || rel == "D")
                {
                    _careOfName = ToTitle(CleanPersonName(co.Groups[2].Value));
                    address = address.Remove(co.Index, co.Length).Trim();
                }
            }

            List<string> parts = address
                            .Split(',')
                            .Select(p => Regex.Replace(
                                p,
                                @"(?i)\b(?:pin\s*code|pincode)\b\s*[:\-]?",
                                " ").Trim(' ', '-', ':', '.'))
                            .Where(p => p.Length > 0)
                            .ToList();

            string city = "";
            for (int i = parts.Count - 1; i >= 0; i--)
            {
                Match lab = Regex.Match(parts[i], @"(?i)^dist(?:rict)?\.?\s*[:\-]?\s*(.+)$");

                if (lab.Success)
                {
                    city = lab.Groups[1].Value.Trim();
                    parts.RemoveAt(i);
                    break;
                }
            }

            if (city == "" && parts.Count >= 2)
            {
                city = parts[parts.Count - 1];
                parts.RemoveAt(parts.Count - 1);
            }

            city = StripAddressLabel(city);

            List<string> cleaned = new List<string>();

            foreach (string part in parts)
            {
                string p = StripAddressLabel(part);

                if (p == "")
                    continue;

                if (p.Equals(city, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (cleaned.Count > 0 && cleaned[cleaned.Count - 1].Equals(p, StringComparison.OrdinalIgnoreCase))
                    continue;

                cleaned.Add(p);
            }

            TxtCity.Text = ToTitle(city);
            TxtAddress.Text = string.Join(", ", cleaned);
        }

        // =========================================================
        // 14. DATE OF BIRTH EXTRACTION
        // =========================================================
        private string FindDateOfBirth(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            Match match = Regex.Match(
                text,
                @"(?i)(?:Date\s*of\s*Birth|DOB|D\.O\.B|Birth\s*Date)\W{0,12}" +
                @"(\d{1,2}\s*[/\-.]\s*\d{1,2}\s*[/\-.]\s*\d{4}|" +
                @"\d{4}\s*[/\-.]\s*\d{1,2}\s*[/\-.]\s*\d{1,2})");

            if (!match.Success)
                return "";

            // Return the date exactly as printed on the Aadhaar (no reformatting)
            return Regex.Replace(match.Groups[1].Value, @"\s", "");
        }


        // =========================================================
        // 15. PARENT NAME EXTRACTION
        // =========================================================

        private string ExtractParentsName(string text, string parent)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            string[] lines = GetLines(text);
            string who = parent == "Father" ? "father" : "mother";
            string pattern = @"(?i)\b" + who + @"(?:'s|s)?\s*" +
                             @"(?:/\s*guardian(?:'s|s)?\s*)?name\b\s*[:\-]?\s*(.*)$";

            for (int i = 0; i < lines.Length; i++)
            {
                Match match = Regex.Match(lines[i], pattern);

                if (!match.Success)
                    continue;

                string name = CleanPersonName(match.Groups[1].Value);

                if (name != "")
                    return ToTitle(name);

                if (i + 1 < lines.Length && Regex.IsMatch(lines[i + 1], @"^[A-Za-z .']+$") &&
                    !Regex.IsMatch(lines[i + 1], @"(?i)school|roll|name|board"))
                {
                    name = CleanPersonName(lines[i + 1]);

                    if (name != "")
                        return ToTitle(name);
                }
            }
            return "";
        }


        private void ExtractedParentNamesFromMarksheet(
            string tenthText,
            string twelfthText,
            string graduationText)
        {
            string father = "";
            string mother = "";

            string[] documents =
            {
                tenthText,
                twelfthText,
                graduationText
            };

            foreach (string document in documents)
            {
                if (string.IsNullOrWhiteSpace(document))
                    continue;

                if (father == "")
                    father = ExtractParentsName(document, "Father");

                if (mother == "")
                    mother = ExtractParentsName(document, "Mother");
            }

            if (father == "" && !string.IsNullOrWhiteSpace(_careOfName))
                father = _careOfName;

            TxtFatherName.Text = father == "" ? "Not Available" : father;
            TxtMotherName.Text = mother == "" ? "Not Available" : mother;

        }


        // =========================================================
        // 16. MARKSHEET SCANNING
        // =========================================================
        private void ScanMarksheet(
            string text,
            TextBox boardTextBox,
            TextBox yearTextBox,
            TextBox percentageTextBox,
            TextBox subjectTextBox,
            bool extractDOB)
        {
            string board = FindBoardName(text);
            boardTextBox.Text = string.IsNullOrWhiteSpace(board) ? "Not Available" : board;

            string year = FindPassingYear(text);
            yearTextBox.Text = string.IsNullOrWhiteSpace(year) ? "Not Available" : year;

            string subjects = ExtractSubjectDetails(text);
            subjectTextBox.Text = string.IsNullOrWhiteSpace(subjects) ? "Not Available" : subjects;

            // 1) Percentage printed on the marksheet
            string percentage = FindPercentage(text);

            // 2) Not printed (e.g. CBSE): total of best 5 subjects / 5
            if (percentage == "")
                percentage = CalculatePercentageFromSubjects(text);

            // CGPA is used only for the Graduation marksheet (see ScanGraduation),
            // not for 10th / 12th percentage.

            percentageTextBox.Text = percentage == "" ? "Not Available" : percentage;

            if (extractDOB)
            {
                string dob = FindDateOfBirth(text);

                if (dob != "" && (string.IsNullOrWhiteSpace(TxtDOB.Text) || TxtDOB.Text == "Not Available"))
                {
                    TxtDOB.Text = dob;
                }
            }
        }


        // =========================================================
        // 17. BOARD EXTRACTION
        // =========================================================
        private string FindBoardName(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            string t = Regex.Replace(text, @"\s+", " ");

            // Order matters: CBSE must be tested before the generic
            // "Board of Secondary Education" (Rajasthan) pattern.
            string[][] boards =
            {
                new[] { @"(?i)central\s*board\s*of\s*secondary\s*education|\bCBSE\b",
                        "Central Board of Secondary Education (CBSE)" },

                new[] { @"(?i)maharashtra\s*state\s*board|MSBSHSE",
                        "Maharashtra State Board of Secondary and Higher Secondary Education, Pune" },

                new[] { @"(?i)board\s*of\s*secondary\s*education\s*,?\s*rajasthan|rajasthan\s*board|\bRBSE\b",
                        "Board of Secondary Education, Rajasthan" },

                new[] { @"(?i)council\s*for\s*the\s*indian\s*school\s*certificate|\bCISCE\b|\bICSE\b",
                        "Council for the Indian School Certificate Examinations (CISCE)" },

                new[] { @"(?i)board\s*of\s*high\s*school\s*and\s*intermediate\s*education|\bUPMSP\b",
                        "Board of High School and Intermediate Education, Uttar Pradesh" },

                new[] { @"(?i)madhya\s*pradesh\s*board\s*of\s*secondary\s*education|\bMPBSE\b",
                        "Madhya Pradesh Board of Secondary Education" },

                new[] { @"(?i)gujarat\s*secondary\s*(?:and|&)\s*higher\s*secondary|\bGSEB\b",
                        "Gujarat Secondary and Higher Secondary Education Board" },

                new[] { @"(?i)bihar\s*school\s*examination\s*board|\bBSEB\b",
                        "Bihar School Examination Board" },

                new[] { @"(?i)board\s*of\s*school\s*education\s*,?\s*haryana|\bHBSE\b",
                        "Board of School Education, Haryana" }
            };

            foreach (string[] b in boards)
            {
                if (Regex.IsMatch(t, b[0]))
                    return b[1];
            }

            // Generic fallback: first line that looks like a board name
            foreach (string line in GetLines(text))
            {
                if (line.Length <= 90 &&
                    Regex.IsMatch(line, @"(?i)\bboard\b.*\b(education|examination)\b"))
                    return line;
            }

            return "";

        }


        // =========================================================
        // 18. PASSING YEAR EXTRACTION
        // =========================================================
        private string FindPassingYear(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            string[] patterns =
            {
                // explicit label
                @"(?i)(?:year\s*of\s*passing|passing\s*year|year\s*of\s*completion|" +
                @"exam\s*year|year\s*of\s*examination)\W{0,6}((?:19|20)\d{2})",

                // "SENIOR SCHOOL CERTIFICATE EXAMINATION, 2026" / "Examination 2023"
                @"(?i)examination\W{0,4}((?:19|20)\d{2})\b",

                // "has appeared in March 2023"
                @"(?i)(?:appeared|held)\s+in\s+[A-Za-z]+\W{0,3}((?:19|20)\d{2})\b",

                // Maharashtra: "A305654 9077 32.05.026 2021"
                @"(?m)^\S+\s+\d{3,5}\s+[\d.]+\s+((?:19|20)\d{2})\b",

                // "March 2023"
                @"(?i)\b(?:feb(?:ruary)?|mar(?:ch)?|apr(?:il)?|may|june?|nov(?:ember)?|dec(?:ember)?)" +
                @"[\s,\-]*((?:19|20)\d{2})\b",

                // result date: "Dated : 13-05-2026"
                @"(?i)dated?\W{0,4}\d{1,2}[/\-.]\d{1,2}[/\-.]((?:19|20)\d{2})\b"
            };

            foreach (string pattern in patterns)
            {
                Match match = Regex.Match(text, pattern);

                if (match.Success)
                    return match.Groups[1].Value;
            }

            return "";
        }



        // =========================================================
        // 19. PERCENTAGE EXTRACTION
        // =========================================================
        private string FindPercentage(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            string[] patterns =
                {
                  @"(?i)(?:percentage|percent|aggregate|overall|total\s*percentage)\s*[:\-]?\s*(\d{1,3}(?:\.\d{1,2})?)",
                  @"(\d{1,3}(?:\.\d{1,2})?)\s*%",

                  // Maharashtra row without a label: "69.40 500 347"
                  @"(?m)(?<![\d.])(\d{2,3}\.\d{1,2})\s+(?:500|600)\s+\d{3}\s*$"
                };

            foreach (string pattern in patterns)
            {
                foreach (Match m in Regex.Matches(text, pattern))
                {
                    double value;
                    if (double.TryParse(m.Groups[1].Value, NumberStyles.Any,
                       CultureInfo.InvariantCulture, out value) && value > 0 && value <= 100)
                    {
                        return FormatPercent(value);
                    }
                }
            }
            return "";
        }

        private string CalculatePercentageFromSubjects(string text)
        {
            List<SubjectMarks> subjects = ParseSubjects(text);

            if (subjects.Count < 5)
                return "";

            List<SubjectMarks> chosen = subjects.Count == 5
                ? subjects
                : subjects
                .OrderByDescending(s => (double)s.Obtained / s.Max).Take(5).ToList();

            double obtained = chosen.Sum(s => s.Obtained);
            double max = chosen.Sum(s => s.Max);

            if (max <= 0)
                return "";

            return FormatPercent(obtained * 100.0 / max);
        }

        private string CalculatePercentageFromCGPA(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            Match m = Regex.Match(text,
                @"(?i)(?:CGPA|C\.G\.P\.A|Cumulative\s+Grade\s+Point(?:\s+Average)?)" +
                @"\W{0,6}(\d{1,2}(?:\.\d{1,2})?)");

            double cgpa;

            if (m.Success &&
                double.TryParse(m.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out cgpa)
                && cgpa > 0 && cgpa <= 10)
            {
                return FormatPercent(cgpa * 9.5);
            }

            return "";
        }



        // =========================================================
        // 20. SUBJECT AND MARKS EXTRACTION
        //
        //   CBSE         301 ENGLISH CORE 068 019 087 EIGHTY SEVEN B1  -> 87/100
        //   Maharashtra  03 ENGLISH (1ST LANG) 100 063 SIXTYTHREE      -> 63/100
        //   Rajasthan    PHYSICS 56 14 30 23 10 18 14 32 28 060        -> 60/100
        //   Generic      English 85/100                                -> 85/100
        // =========================================================

        private static readonly Regex NumberToken = new Regex(@"^\d{1,3}(?:\.\d{1,2})?$|^\d{1,3}(?:\.\d{1,2})?/\d{1,3}$");

        private static readonly Regex GradeToken = new Regex(@"^[A-E][1-2]?\+{0,2}$");

        private List<SubjectMarks> ParseSubjects(string text)
        {
            List<SubjectMarks> result = new List<SubjectMarks>();

            foreach (string rawline in GetLines(text))
            {
                if (result.Count > 0 && Regex.IsMatch(rawline,
                    @"(?i)\b(additional\s*subject|total\s*max|total\s*marks|result)\b"))
                    break;

                // remove OCR noise: table borders, stray symbols, stray leading letters
                string cleanLine = Regex.Replace(rawline, @"[|\[\]{}@©_~=]+", " ");
                cleanLine = Regex.Replace(cleanLine, @"\s+", " ").Trim();
                cleanLine = Regex.Replace(cleanLine, @"^[^A-Za-z0-9]+", "");
                cleanLine = Regex.Replace(cleanLine, @"^[A-Za-z]{1,2}\s+(?=\d{2,3}\s+[A-Za-z])", "");

                // remove the subject code: "301 ", "03. ", "16) ", "P1 "
                string work = Regex.Replace(cleanLine, @"^[A-Za-z]?\d{1,3}[.:)\-]*\s+(?=[A-Za-z])", "");

                string[] tokens = work.Split(' ');

                // ---- 1) marks written in words at the end of the row
                //         (EIGHTY SEVEN / SIXTYTHREE); a grade such as B1 is ignored
                int tailEnd = tokens.Length;

                if (tailEnd > 0 && GradeToken.IsMatch(tokens[tailEnd - 1]))
                    tailEnd--;

                int wordsValue = -1;
                int wordsStart = -1;

                for (int k = 1; k <= 4 && tailEnd - k >= 0; k++)
                {
                    string[] seg = tokens.Skip(tailEnd - k).Take(k).ToArray();

                    if (!seg.All(z => Regex.IsMatch(z, @"^[A-Za-z]+$")))
                        break;

                    string phrase = string.Join(" ", seg);
                    int wv = WordsToNumber(phrase);

                    // a lone HUNDRED (from "THREE HUNDRED AND ...") is not 100 marks
                    if (wv == 100 &&
                        Regex.Replace(phrase.ToUpperInvariant(), @"[^A-Z]", "") != "ONEHUNDRED")
                        wv = -1;

                    if (wv >= 0 && wv <= 100)
                    {
                        wordsValue = wv;
                        wordsStart = tailEnd - k;
                    }
                }

                // ---- 2) first number written in digits
                int depth = 0;
                int firstnum = -1;

                for (int i = 0; i < tokens.Length; i++)
                {
                    string tk = tokens[i];

                    if (depth <= 0 && NumberToken.IsMatch(tk))
                    {
                        firstnum = i;
                        break;
                    }

                    depth += tk.Count(c => c == '(') - tk.Count(c => c == ')');
                }

                // the subject name ends where the digits (or the words) start
                int nameEnd = (wordsStart >= 0 && (firstnum < 0 || firstnum > wordsStart))
                    ? wordsStart
                    : firstnum;

                if (nameEnd < 1)
                    continue;

                string name = string.Join(" ", tokens.Take(nameEnd)).Trim(' ', '-', ':', '.');

                if (name.Length < 3 || name.Length > 45)
                    continue;

                if (!Regex.IsMatch(name, @"^[A-Za-z][A-Za-z0-9 &().,'/\-]*$"))
                    continue;

                if (Regex.IsMatch(name,
                    @"(?i)subject|marks|maximum|theory|total|roll|regn|seat|school|" +
                    @"candidate|mother|father|guardian|board|certificate|examination|" +
                    @"result|grade|date|centre|center|district|division|percentage|" +
                    @"code|certify|birth|dob|thousand"))
                    continue;

                // "THREE" / "HUNDRED AND" are number words, not a subject
                if (WordsToNumber(name) >= 0)
                    continue;

                int obtained = -1;
                int max = 100;
                bool haveObtained = false;

                bool digitsUsable = firstnum >= 1 && (wordsStart < 0 || firstnum < wordsStart);

                if (digitsUsable)
                {
                    List<string> nums = new List<string>();

                    for (int i = firstnum; i < tokens.Length; i++)
                    {
                        if (!NumberToken.IsMatch(tokens[i]))
                            break;

                        nums.Add(tokens[i]);
                    }

                    if (nums.Count == 1 && nums[0].Contains("/"))
                    {
                        string[] ab = nums[0].Split('/');
                        obtained = (int)Math.Round(double.Parse(ab[0], CultureInfo.InvariantCulture));
                        max = int.Parse(ab[1]);
                        haveObtained = true;
                    }
                    else
                    {
                        List<int> v = nums.Where(n => !n.Contains("/")).Select(n =>
                            (int)Math.Round(double.Parse(n, CultureInfo.InvariantCulture))).ToList();

                        if (v.Count == 2)
                        {
                            if (wordsValue >= 0 && (v[0] == wordsValue || v[1] == wordsValue))
                            {
                                // the marks in words say which number is the obtained one
                                int other = v[0] == wordsValue ? v[1] : v[0];
                                obtained = wordsValue;
                                max = other >= obtained ? other : 100;
                                haveObtained = true;
                            }
                            else if (nums[0] == "100" && nums[1].Length < 3)
                            {
                                // Maharashtra prints "100 063"; "100 6" is a misread digit
                            }
                            else
                            {
                                max = Math.Max(v[0], v[1]);
                                obtained = Math.Min(v[0], v[1]);
                                haveObtained = true;
                            }
                        }
                        else if (v.Count >= 3)
                        {
                            // CBSE: theory, internal, TOTAL  |  Rajasthan: ..., TOTAL
                            obtained = v[v.Count - 1];
                            haveObtained = true;
                        }
                        // a lone number is not trusted on its own (e.g. "... PUNE 1");
                        // the marks written in words are used instead (below)
                    }
                }

                if (!haveObtained)
                {
                    if (wordsValue < 0)
                        continue;

                    obtained = wordsValue;
                    max = 100;
                }

                if (max < 20 || max > 200 || obtained < 0 || obtained > max)
                    continue;

                name = CleanSubjectName(name);

                // drop OCR junk tokens that contain digits (e.g. "MATHEMATICS g00")
                name = string.Join(" ", name.Split(' ').Where(w => !Regex.IsMatch(w, @"\d"))).Trim();

                if (name.Length < 3)
                    continue;

                if (result.Any(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    continue;

                result.Add(new SubjectMarks { Name = name, Obtained = obtained, Max = max });
            }

            // OCR sometimes returns a table column by column instead of row by row
            // (common with photographed CBSE marksheets). Rebuild from the
            // "TOTAL (IN WORDS)" column, e.g. EIGHTY SEVEN = 87.
            if (result.Count < 3)
            {
                List<SubjectMarks> fromWords = ParseSubjectsFromWords(text);

                if (fromWords.Count > result.Count)
                    result = fromWords;
            }

            return result;
        }

        // "ENGLISH (1ST LANG)" -> "ENGLISH", "HINDI(COMP.)" -> "HINDI"
        private string CleanSubjectName(string name)
        {
            name = Regex.Replace(name, @"\([^)]*\)?", " ");
            name = Regex.Replace(name, @"\s+", " ").Trim(' ', '-', ':', '.', ',', '*');

            return name;
        }

        private static readonly string[] NumberWordList =
        {
            "ZERO","ONE","TWO","THREE","FOUR","FIVE","SIX","SEVEN","EIGHT","NINE",
            "TEN","ELEVEN","TWELVE","THIRTEEN","FOURTEEN","FIFTEEN","SIXTEEN",
            "SEVENTEEN","EIGHTEEN","NINETEEN","TWENTY","THIRTY","FORTY","FIFTY",
            "SIXTY","SEVENTY","EIGHTY","NINETY","HUNDRED","AND"
        };

        // "EIGHTY SEVEN" / "EIGHTYSEVEN" / "ONE HUNDRED" -> number, or -1
        private int WordsToNumber(string phrase)
        {
            string s = Regex.Replace(phrase.ToUpperInvariant(), @"[^A-Z]", "");

            if (s.Length == 0)
                return -1;

            // longest words first so "SIXTEEN" is not read as "SIX" + "TEEN"
            string[] words = NumberWordList.OrderByDescending(w => w.Length).ToArray();

            int total = 0;
            int current = 0;
            bool counted = false;

            while (s.Length > 0)
            {
                string hit = words.FirstOrDefault(w => s.StartsWith(w));

                if (hit == null)
                    return -1;

                s = s.Substring(hit.Length);

                int idx = Array.IndexOf(NumberWordList, hit);

                if (hit == "AND")
                    continue;

                counted = true;

                if (hit == "HUNDRED")
                {
                    current = (current == 0 ? 1 : current) * 100;
                    continue;
                }

                if (idx <= 19)
                    current += idx;
                else
                    current += (idx - 18) * 10;        // TWENTY = 20 ... NINETY = 90
            }

            if (!counted)
                return -1;

            total += current;

            return total;
        }

        private List<SubjectMarks> ParseSubjectsFromWords(string text)
        {
            List<SubjectMarks> result = new List<SubjectMarks>();

            string[] lines = GetLines(text);

            // table body: after the column header, before "Result"
            int start = 0;
            int end = lines.Length;

            for (int i = 0; i < lines.Length; i++)
            {
                if (Regex.IsMatch(lines[i], @"(?i)in\s*words|positional|grade$"))
                    start = i + 1;

                if (start > 0 && i >= start &&
                    Regex.IsMatch(lines[i], @"(?i)^(result|pass|fail)\b"))
                {
                    end = i;
                    break;
                }
            }

            List<string> names = new List<string>();
            List<int> marks = new List<int>();

            for (int i = start; i < end; i++)
            {
                string line = lines[i].ToUpperInvariant();

                // marks written in words
                if (Regex.IsMatch(line, @"^[A-Z ]+$"))
                {
                    // drop a trailing grade such as "B1" already excluded by ^[A-Z ]+$
                    int n = WordsToNumber(line);

                    if (n >= 0 && n <= 100)
                    {
                        marks.Add(n);
                        continue;
                    }
                }

                // subject name
                if (!Regex.IsMatch(line, @"^[A-Z][A-Z &.\-]{2,40}$"))
                    continue;

                if (Regex.IsMatch(line,
                    @"SUBJECT|MARKS|MAXIMUM|THEORY|TOTAL|ROLL|CERTIFY|BOARD|" +
                    @"CERTIFICATE|EXAMINATION|RESULT|GRADE|DATE|SCHOOL|PRACTICAL|" +
                    @"WORK EXPERIENCE|HEALTH|GENERAL STUDIES|ART EDUCATION|" +
                    @"SELF DEVELOPMENT|DEFENCE|CONTROLLER|PASS"))
                    continue;

                names.Add(CleanSubjectName(line));
            }

            int count = Math.Min(names.Count, marks.Count);

            for (int i = 0; i < count; i++)
            {
                result.Add(new SubjectMarks
                {
                    Name = names[i],
                    Obtained = marks[i],
                    Max = 100
                });
            }

            return result;
        }

        // One subject per line:  ENGLISH - 63
        private string ExtractSubjectDetails(string text)
        {
            return string.Join(Environment.NewLine,
                ParseSubjects(text).Select(s => s.Name + " - " + s.Obtained));
        }


        // =========================================================
        // 21. GRADUATION DETAILS
        // =========================================================

        private void ScanGraduation(string text)
        {
            string[] lines = GetLines(text);

            string university = FindUniversityName(lines);

            TxtUniversity.Text =
                string.IsNullOrWhiteSpace(university)
                ? "Not Available"
                : university;


            string college = FindCollegeName(lines, university);

            TxtCollegeName.Text =
                string.IsNullOrWhiteSpace(college)
                ? "Not Available"
                : college;


            string year =
                FindPassingYear(text);

            TxtDegreeYear.Text =
                string.IsNullOrWhiteSpace(year)
                ? "Not Available"
                : year;


            string cgpa =
                FindCGPA(text);

            TxtCGPA.Text =
                string.IsNullOrWhiteSpace(cgpa)
                ? "Not Available"
                : cgpa;
        }


        // "University Name : XYZ" or a header line such as
        // "MOHANLAL SUKHADIA UNIVERSITY, UDAIPUR"
        private string FindUniversityName(string[] lines)
        {
            foreach (string l in lines)
            {
                Match m = Regex.Match(l,
                    @"(?i)^(?:name\s*of\s*)?(?:the\s*)?university(?:\s*name)?\s*[:\-]\s*(.+)$");

                if (m.Success && m.Groups[1].Value.Trim().Length >= 4)
                    return m.Groups[1].Value.Trim();
            }

            foreach (string l in lines)
            {
                if (l.Length > 120 || Regex.IsMatch(l, @"\d"))
                    continue;

                if (!Regex.IsMatch(l, @"(?i)\buniversity\b|vishwavidyalaya|vidyapeeth"))
                    continue;

                if (Regex.IsMatch(l,
                    @"(?i)affiliated|controller|registrar|issued|verify|website|www\.|@|" +
                    @"examinations?\s*$"))
                    continue;

                // header may be merged with "Statement of Grade Obtained at the ..."
                string header = Regex.Replace(l, @"(?i)\s*statement\b.*$", "");

                return header.Trim(' ', ',', '.', '-');
            }

            return "";
        }


        // "College Name : XYZ" or a line containing College / Institute
        private string FindCollegeName(string[] lines, string university)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                // "Name of College : XYZ"  or  "Name of College" + value on next line
                Match m = Regex.Match(lines[i],
                    @"(?i)^(?:name\s*of\s*(?:the\s*)?)?(?:college|institution|institute)" +
                    @"(?:\s*/\s*institution)?(?:\s*name)?\s*(?:[:\-]\s*(.*))?$");

                if (!m.Success)
                    continue;

                string value = m.Groups[1].Success ? m.Groups[1].Value.Trim() : "";

                if (value.Length < 4 && i + 1 < lines.Length)
                    value = Regex.Replace(lines[i + 1], @"^[\s:\-]+", "").Trim();

                if (value.Length >= 4)
                    return value;
            }

            foreach (string l in lines)
            {
                if (l.Length > 120)
                    continue;

                if (!Regex.IsMatch(l, @"(?i)\b(college|institute|institution|mahavidyalaya)\b"))
                    continue;

                if (Regex.IsMatch(l, @"(?i)controller|registrar|issued|verify|website|www\.|@"))
                    continue;

                string candidate = l;

                // "XYZ COLLEGE, AFFILIATED TO ABC UNIVERSITY" -> keep the college part
                int aff = candidate.IndexOf("affiliated", StringComparison.OrdinalIgnoreCase);

                if (aff > 0)
                    candidate = candidate.Substring(0, aff);

                candidate = candidate.Trim(' ', ',', '.', '-', '(');

                if (candidate.Length < 4)
                    continue;

                if (!string.IsNullOrEmpty(university) &&
                    candidate.Equals(university, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (Regex.IsMatch(candidate, @"(?i)\buniversity\b"))
                    continue;

                return candidate;
            }

            return "";
        }


        // Graduation score: CGPA as printed (no conversion),
        // otherwise printed percentage, otherwise obtained / maximum marks.
        private string FindCGPA(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            // Grade-card table: "Cumulative Grade Point Average(CGPA) 7.80 8.52 ... 9.04"
            // has one value per semester, so the LAST value is the final CGPA.
            string[] gl = GetLines(text);

            for (int i = 0; i < gl.Length; i++)
            {
                if (!Regex.IsMatch(gl[i], @"(?i)(?<![A-Za-z])CGPA|cumulative\s+grade\s+point"))
                    continue;

                StringBuilder row = new StringBuilder(gl[i]);

                // OCR may put the semester values on the following lines:
                // keep appending lines that contain numbers only
                if (!Regex.IsMatch(gl[i], @"\d[.,]\d{2}"))
                {
                    for (int j = i + 1; j < gl.Length && j <= i + 12; j++)
                    {
                        if (Regex.IsMatch(gl[j], @"[A-Za-z]"))
                            break;

                        row.Append(' ').Append(gl[j]);
                    }
                }

                // 9,04 -> 9.04
                string rowText = Regex.Replace(row.ToString(), @"(?<=\d),(?=\d{2}(?!\d))", ".");

                // One CGPA value per semester (I ... VIII): the LAST value
                // is the final CGPA of the degree.
                string lastValue = "";

                foreach (Match n in Regex.Matches(rowText, @"(?<![\d.])\d{1,2}\.\d{1,3}(?![\d.])"))
                {
                    double d;

                    if (double.TryParse(n.Value, NumberStyles.Float,
                            CultureInfo.InvariantCulture, out d) && d > 0 && d <= 10)
                        lastValue = n.Value;
                }

                if (lastValue != "")
                    return lastValue;
            }

            double v;

            // CGPA (the (?<![A-Za-z]) guard stops "SGPA" matching as "GPA")
            foreach (string pattern in new string[]
            {
                @"(?i)(?<![A-Za-z])(?:CGPA|C\.G\.P\.A\.?|Cumulative\s+Grade\s+Point(?:\s+Average)?)\W{0,8}(\d{1,2}(?:\.\d{1,3})?)",
                @"(?i)(?<![A-Za-z])GPA\W{0,8}(\d{1,2}(?:\.\d{1,3})?)"
            })
            {
                Match m = Regex.Match(text, pattern);

                if (m.Success &&
                    double.TryParse(m.Groups[1].Value, NumberStyles.Float,
                        CultureInfo.InvariantCulture, out v) &&
                    v > 0 && v <= 10)
                {
                    return m.Groups[1].Value;
                }
            }

            string percentage = FindPercentage(text);

            if (percentage != "")
                return percentage;

            // "Grand Total 1650 / 2200"  or  "Total Marks Obtained 1650 out of 2200"
            Match t = Regex.Match(text,
                @"(?i)(?:grand\s*total|aggregate(?:\s*marks)?|total\s*marks(?:\s*obtained)?)" +
                @"\W{0,10}(\d{2,5})\s*(?:/|out\s*of|of)\s*(\d{3,5})");

            double obtained, max;

            if (t.Success &&
                double.TryParse(t.Groups[1].Value, out obtained) &&
                double.TryParse(t.Groups[2].Value, out max) &&
                max > 0 && obtained <= max)
            {
                return FormatPercent(obtained * 100.0 / max);
            }

            return "";
        }

        private void SaveOcrDebug(string label, string text)
        {
            try
            {
                string folder = Server.MapPath("~/App_Data/");

                File.WriteAllText(Path.Combine(folder, "ocr_debug_" + label + ".txt"),
                    text ?? "", new UTF8Encoding(true));
            }
            catch
            {

            }
        }

        // =========================================================
        // 22. SCAN BUTTON
        // =========================================================

        protected void BtnScan_Click1(object sender, EventArgs e)
        {
            pnlStudentDetails.Visible = false;
            pnlStudentMarksheet.Visible = false;
            pnlGraduationDetails.Visible = false;

            panelsContainer.Attributes["class"] = "panels-container two-panels";

            LblScanStatus.Text = "";

            ClearPreviousResult();

            try
            {
                string[] imageExtensions =
                {
                    ".jpg", ".jpeg", ".png"
                };

                string[] marksheetExtensions =
                {
                    ".jpg", ".jpeg", ".png", ".pdf"
                };


                // -----------------------------------------
                // Validate required documents
                // -----------------------------------------

                if (!ValidateFile(
                    FileUpload1,
                    "Aadhaar Front",
                    imageExtensions,
                    5))
                    return;


                if (!ValidateFile(
                    FileUpload2,
                    "Aadhaar Back",
                    imageExtensions,
                    5))
                    return;


                if (!ValidateFile(
                    FileUpload3,
                    "Student Photograph",
                    imageExtensions,
                    2))
                    return;


                if (!ValidateFile(
                    FileUpload4,
                    "10th Marksheet",
                    marksheetExtensions,
                    5))
                    return;


                if (!ValidateFile(
                    FileUpload5,
                    "12th Marksheet",
                    marksheetExtensions,
                    5))
                    return;


                if (!ValidateGraduationMarksheet())
                    return;


                // -----------------------------------------
                // Display student photograph
                // -----------------------------------------

                if (!DisplayStudentPhoto())
                    return;


                // -----------------------------------------
                // Scan Aadhaar Front
                // -----------------------------------------

                LblScanStatus.Text =
                    "Scanning Aadhaar Front...";

                string aadhaarFrontText =
                    ExtractTextUsingOCR(FileUpload1);

                ScanAadhaar(aadhaarFrontText);


                // -----------------------------------------
                // Scan Aadhaar Back
                // -----------------------------------------

                LblScanStatus.Text =
                    "Scanning Aadhaar Back...";

                string aadhaarBackText =
                    ExtractTextUsingOCR(FileUpload2);

                ExtractAddressDetails(aadhaarBackText);


                // -----------------------------------------
                // Scan 10th Marksheet
                // -----------------------------------------

                LblScanStatus.Text =
                    "Scanning 10th Marksheet...";

                string tenthText =
                      ExtractTextUsingOCR(FileUpload4, true);

                SaveOcrDebug("10th", tenthText);

                ScanMarksheet(
                    tenthText,
                    Txt10Board,
                    Txt10year,
                    Txt10percentage,
                    Txt10Subject,
                    false
                );


                // -----------------------------------------
                // Scan 12th Marksheet
                // -----------------------------------------

                LblScanStatus.Text =
                    "Scanning 12th Marksheet...";

                string twelfthText =
                    ExtractTextUsingOCR(FileUpload5, true);

                SaveOcrDebug("12th", twelfthText);

                ScanMarksheet(
                    twelfthText,
                    Txt12Board,
                    Txt12year,
                    Txt12percentage,
                    Txt12Subject,
                    false
                );


                // -----------------------------------------
                // Optional Graduation Marksheet
                // -----------------------------------------

                string graduationText = "";

                if (FileUpload6.HasFile)
                {
                    LblScanStatus.Text =
                        "Scanning Graduation Marksheet...";

                    graduationText =
                        ExtractTextUsingOCR(FileUpload6, true);

                    SaveOcrDebug("graduation", graduationText);

                    ScanGraduation(graduationText);

                    pnlGraduationDetails.Visible = true;
                }


                // -----------------------------------------
                // Extract Parent Names
                // -----------------------------------------

                ExtractedParentNamesFromMarksheet(
                    tenthText,
                    twelfthText,
                    graduationText
                );


                // -----------------------------------------
                // Display Result Panels
                // -----------------------------------------

                pnlStudentDetails.Visible = true;

                pnlStudentMarksheet.Visible = true;

                pnlGraduationDetails.Visible =
                    FileUpload6.HasFile;


                // -----------------------------------------
                // Dynamic Panel Layout
                // -----------------------------------------

                panelsContainer.Attributes["class"] =
                    FileUpload6.HasFile
                    ? "panels-container three-panels"
                    : "panels-container two-panels";


                // -----------------------------------------
                // Success Message
                // -----------------------------------------

                LblScanStatus.Text =
                    FileUpload6.HasFile
                    ? "All documents scanned successfully!"
                    : "Documents scanned successfully! Graduation marksheet not uploaded.";
            }
            catch (WebException ex)
            {
                string errorMessage =
                    "OCR service connection failed.";

                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    errorMessage =
                        "OCR request timed out. Please try again.";
                }
                else if (ex.Response != null)
                {
                    using (StreamReader reader =
                        new StreamReader(ex.Response.GetResponseStream()))
                    {
                        errorMessage =
                            reader.ReadToEnd();
                    }
                }

                LblScanStatus.Text =
                    "Scanning failed: " + errorMessage;
            }
            catch (Exception ex)
            {
                LblScanStatus.Text =
                    "Scanning failed: " + ex.Message;
            }
        }


        // =========================================================
        // 23. CATEGORY SELECTION
        // =========================================================

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


        // =========================================================
        // 24. CSV ESCAPE
        // =========================================================

        private string CsvEscape(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "\"\"";

            value = value.Replace("\"", "\"\"");

            return "\"" + value + "\"";
        }


        // =========================================================
        // 25. SAVE STUDENT RECORD
        // =========================================================

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtStudentName.Text))
                {
                    LblScanStatus.Text =
                        "Please enter the student name.";

                    return;
                }


                string folderPath =
                    Server.MapPath("~/App_Data/");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }


                string filePath =
                    Path.Combine(
                        folderPath,
                        "AdmitFlow_StudentRecords.csv"
                    );


                // -----------------------------------------
                // Create CSV Header
                // -----------------------------------------

                if (!File.Exists(filePath))
                {
                    string header =
                        "Date,Student Name,DOB,Aadhaar Number," +
                        "Father Name,Mother Name,Contact Number," +
                        "Address,City,State,PIN Code,Category," +

                        "10th Board,10th Year,10th Subjects," +
                        "10th Percentage," +

                        "12th Board,12th Year,12th Subjects," +
                        "12th Percentage," +

                        "Admission Session," +

                        "University,College,Graduation CGPA," +
                        "Graduation Year";


                    File.WriteAllText(
                        filePath,
                        header + Environment.NewLine,
                        new UTF8Encoding(true)
                    );
                }


                // -----------------------------------------
                // Student Data
                // -----------------------------------------

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
                    TxtCity.Text,
                    TxtState.Text,
                    TxtPinCode.Text,
                    GetSelectedCategory(),

                    Txt10Board.Text,
                    Txt10year.Text,
                    Txt10Subject.Text,
                    Txt10percentage.Text,

                    Txt12Board.Text,
                    Txt12year.Text,
                    Txt12Subject.Text,
                    Txt12percentage.Text,

                    TxtSession.Text,

                    TxtUniversity.Text,
                    TxtCollegeName.Text,
                    TxtCGPA.Text,
                    TxtDegreeYear.Text
                };


                // -----------------------------------------
                // Save CSV Row
                // -----------------------------------------

                string row = string.Join(",", values.Select(value => CsvEscape(value)));

                File.AppendAllText(
                    filePath,
                    row + Environment.NewLine,
                    new UTF8Encoding(true)
                );


                LblScanStatus.Text =
                    "Student record successfully saved!";
            }
            catch (Exception ex)
            {
                LblScanStatus.Text =
                    "Save failed: " + ex.Message;
            }
        }
    }
}