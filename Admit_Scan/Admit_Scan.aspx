<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Admit_Scan.aspx.cs"
    Inherits="Admit_Scan.Admit_Scan" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <title>ADMITFLOW | Student Admission & Records Management</title>

    <link href="Admit_Scan.css" rel="stylesheet" />

    <link href="https://fonts.googleapis.com/css2?family=Cinzel:wght@400;500;600;700&family=Cormorant+Garamond:wght@400;500;600;700&family=Special+Elite&display=swap"
        rel="stylesheet" />

    <link rel="stylesheet"
        href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" />
</head>

<body>

<form id="form1" runat="server">

    <div class="main-container">

        <!-- ================= HEADER ================= -->

        <header class="main-header">

            <div class="emblem-banner">
                <img src="AdmitFlow_Emblem1-Photoroom.png"
                     class="brand-emblem"
                     alt="AdmitFlow Royal Emblem" />
            </div>

            <h1 class="brand-title">ADMITFLOW</h1>

            <h2 class="brand-subtitle">
                STUDENT ADMISSION & RECORDS MANAGEMENT SYSTEM
            </h2>

            <div class="tagline">
                <span></span>
                <p>Simplifying Admissions. Automating Verification.</p>
                <span></span>
            </div>

        </header>


        <!-- ================= DOCUMENT UPLOAD SECTION ================= -->

        <section class="upload-section">

            <div class="section-heading">
                <h2>DOCUMENT VERIFICATION</h2>
                <p>Upload the required documents to extract student information.</p>
            </div>


            <div class="upload-grid">

                <!-- Aadhaar Front -->

                <div class="upload-card">

                    <div class="upload-icon">
                        <i class="fa-solid fa-id-card"></i>
                    </div>

                    <h3>AADHAAR FRONT</h3>

                    <p>Student name, DOB and Aadhaar number</p>

                    <asp:FileUpload ID="FileUpload1"
                        runat="server"
                        CssClass="file-upload"
                        accept=".jpg,.jpeg,.png" />

                    <small>Required | JPG, JPEG, PNG</small>


                    <div class="upload-icon">
    <i class="fa-solid fa-address-card"></i>
</div>

<h3>AADHAAR BACK</h3>

<p>Student residential address</p>

<asp:FileUpload ID="FileUpload2"
    runat="server"
    CssClass="file-upload"
    accept=".jpg,.jpeg,.png" />

<small>Required | JPG, JPEG, PNG</small>

                </div>


                


                <!-- Student Photograph -->

                <div class="upload-card">

                    <div class="upload-icon">
                        <i class="fa-solid fa-user"></i>
                    </div>

                    <h3>STUDENT PHOTOGRAPH</h3>

                    <p>Upload a recent student photograph</p>

                    <asp:FileUpload ID="FileUpload3"
                        runat="server"
                        CssClass="file-upload"
                        accept=".jpg,.jpeg,.png" />

                    <small>Required | JPG, JPEG, PNG</small>

                </div>


                <!-- 10th Marksheet -->

                <div class="upload-card">

                    <div class="upload-icon">
                        <i class="fa-solid fa-certificate"></i>
                    </div>

                    <h3>10TH MARKSHEET</h3>

                    <p>Academic details and parent names</p>

                    <asp:FileUpload ID="FileUpload4"
                        runat="server"
                        CssClass="file-upload"
                        accept=".jpg,.jpeg,.png,.pdf" />

                    <small>Required | JPG, JPEG, PNG, PDF</small>

                </div>


                <!-- 12th Marksheet -->

                <div class="upload-card">

                    <div class="upload-icon">
                        <i class="fa-solid fa-graduation-cap"></i>
                    </div>

                    <h3>12TH MARKSHEET</h3>

                    <p>Academic details and parent names</p>

                    <asp:FileUpload ID="FileUpload5"
                        runat="server"
                        CssClass="file-upload"
                        accept=".jpg,.jpeg,.png,.pdf" />

                    <small>Required | JPG, JPEG, PNG, PDF</small>

                </div>


                <!-- Graduation Marksheet -->

                <div class="upload-card">

                    <div class="upload-icon">
                        <i class="fa-solid fa-university"></i>
                    </div>

                    <h3>GRADUATION MARKSHEET</h3>

                    <p>University, college and graduation details</p>

                    <asp:FileUpload ID="FileUpload6"
                        runat="server"
                        CssClass="file-upload"
                        accept=".jpg,.jpeg,.png,.pdf" />

                    <small>Optional | JPG, JPEG, PNG, PDF</small>

                </div>

            </div>


            <!-- Scan Button -->

            <div class="scan-button-container">

                <asp:Button ID="BtnScan"
                    runat="server"
                    Text="SCAN DOCUMENTS"
                    CssClass="scan-button"
                    OnClick="BtnScan_Click1" />

            </div>


            <!-- Scan Status -->

            <div class="status-container">

                <asp:Label ID="LblScanStatus"
                    runat="server"
                    CssClass="scan-status"
                    EnableViewState="false">
                </asp:Label>

            </div>

        </section>


        <!-- ================= RESULT PANELS ================= -->

        <section class="results-section">

            <div class="section-heading">
                <h2>VERIFIED STUDENT INFORMATION</h2>
                <p>Extracted information will appear below after document scanning.</p>
            </div>


            <div class="panels-container">


                <!-- ========================================= -->
                <!-- PANEL 1 : STUDENT DETAILS -->
                <!-- ========================================= -->

                <asp:Panel ID="pnlStudentDetails"
                    runat="server"
                    CssClass="panel"
                    Visible="false">

                    <div class="panel-heading">
                        <i class="fa-solid fa-user-graduate"></i>
                        <h3>STUDENT DETAILS</h3>
                    </div>


                    <!-- Student Photo -->

                    <div class="student-photo-container">

                        <asp:Image ID="ImgStudentPhoto"
                            runat="server"
                            CssClass="student-photo"
                            AlternateText="Student Photograph" />

                    </div>


                    <!-- Student Name -->

                    <div class="form-group">

                        <label>Student Name</label>

                        <asp:TextBox ID="TxtStudentName"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <!-- Date of Birth -->

                    <div class="form-group">

                        <label>Date of Birth</label>

                        <asp:TextBox ID="TxtDOB"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <!-- Aadhaar Number -->

                    <div class="form-group">

                        <label>Aadhaar Number</label>

                        <asp:TextBox ID="TxtAadhaarNo"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <!-- Father's Name -->

                    <div class="form-group">

                        <label>Father's Name</label>

                        <asp:TextBox ID="TxtFatherName"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <!-- Mother's Name -->

                    <div class="form-group">

                        <label>Mother's Name</label>

                        <asp:TextBox ID="TxtMotherName"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <!-- Contact Number -->

                    <div class="form-group">

                        <label>Contact Number</label>

                        <asp:TextBox ID="TxtContNumber"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="10"
                            TextMode="Phone"
                            placeholder="Enter 10-digit mobile number">
                        </asp:TextBox>

                    </div>


                    <!-- Address -->

                    <div class="form-group">

                        <label>Residential Address</label>

                        <asp:TextBox ID="TxtAddress"
                            runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="3"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <!-- Category -->

                    <div class="form-group">

                        <label>Category</label>

                        <div class="category-options">

                            <asp:RadioButton ID="RadioButton1"
                                runat="server"
                                Text=" SC"
                                GroupName="Category" />

                            <asp:RadioButton ID="RadioButton2"
                                runat="server"
                                Text=" ST"
                                GroupName="Category" />

                            <asp:RadioButton ID="RadioButton3"
                                runat="server"
                                Text=" OBC"
                                GroupName="Category" />

                            <asp:RadioButton ID="RadioButton4"
                                runat="server"
                                Text=" Other"
                                GroupName="Category" />

                        </div>

                    </div>

                </asp:Panel>



                <!-- ========================================= -->
                <!-- PANEL 2 : ACADEMIC DETAILS -->
                <!-- ========================================= -->

                <asp:Panel ID="pnlStudentMarksheet"
                    runat="server"
                    CssClass="panel"
                    Visible="false">

                    <div class="panel-heading">
                        <i class="fa-solid fa-book-open"></i>
                        <h3>ACADEMIC DETAILS</h3>
                    </div>


                    <!-- 10th Details -->

                    <div class="sub-panel-heading">
                        <h4>10TH STANDARD</h4>
                    </div>


                    <div class="form-group">

                        <label>Passing Year</label>

                        <asp:TextBox ID="Txt10year"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <div class="form-group">

                        <label>Percentage</label>

                        <asp:TextBox ID="Txt10percentage"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <!-- 12th Details -->

                    <div class="sub-panel-heading">
                        <h4>12TH STANDARD</h4>
                    </div>


                    <div class="form-group">

                        <label>Passing Year</label>

                        <asp:TextBox ID="Txt12year"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <div class="form-group">

                        <label>Percentage</label>

                        <asp:TextBox ID="Txt12percentage"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <!-- Admission Session -->

                    <div class="sub-panel-heading">
                        <h4>ADMISSION INFORMATION</h4>
                    </div>


                    <div class="form-group">

                        <label>Admission Session</label>

                        <asp:TextBox ID="TxtSession"
                            runat="server"
                            CssClass="form-control"
                            placeholder="Enter admission session">
                        </asp:TextBox>

                    </div>

                </asp:Panel>



                <!-- ========================================= -->
                <!-- PANEL 3 : GRADUATION DETAILS -->
                <!-- ========================================= -->

                <asp:Panel ID="pnlGraduationDetails"
                    runat="server"
                    CssClass="panel"
                    Visible="false">

                    <div class="panel-heading">
                        <i class="fa-solid fa-building-columns"></i>
                        <h3>GRADUATION DETAILS</h3>
                    </div>


                    <div class="form-group">

                        <label>University</label>

                        <asp:TextBox ID="TxtUniversity"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <div class="form-group">

                        <label>College Name</label>

                        <asp:TextBox ID="TxtCollegeName"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <div class="form-group">

                        <label>CGPA / Percentage</label>

                        <asp:TextBox ID="TxtCGPA"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>


                    <div class="form-group">

                        <label>Graduation Passing Year</label>

                        <asp:TextBox ID="TxtDegreeYear"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>

                </asp:Panel>


            </div>

        </section>


        <!-- ================= SAVE BUTTON ================= -->

        <div class="save-button-container">

            <asp:Button ID="BtnSave"
                runat="server"
                Text="SAVE STUDENT RECORD"
                CssClass="save-button" OnClick="BtnSave_Click" />

        </div>


        <!-- ================= FOOTER ================= -->

        <footer class="main-footer">

            <p>ADMITFLOW</p>

            <span>SIMPLIFYING ADMISSIONS. AUTOMATING VERIFICATION.</span>

        </footer>

    </div>

</form>

</body>
</html>
