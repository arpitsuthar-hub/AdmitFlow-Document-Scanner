<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Admit_Scan.aspx.cs"
    Inherits="Admit_Scan.Admit_Scan" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <title>AdmitFlow | Student Admission & Records Management</title>

    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <link href="Admit_Scan.css?v=3" rel="stylesheet" />

    <!-- Modern Fonts -->
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700;800&display=swap"
        rel="stylesheet" />

    <!-- Font Awesome -->
    <link rel="stylesheet"
        href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.2/css/all.min.css" />

</head>

<body>

<form id="form1" runat="server">

    <div class="main-container">

        <!-- ================= NAVIGATION ================= -->

        <nav class="top-navigation">

            <a href="#" class="nav-brand">
                <img src="AdmitFlow_Emblem1-Photoroom.png"
                    alt="AdmitFlow Logo" />

                <span>AdmitFlow</span>
            </a>

            <div class="nav-links">

                <a href="#" class="active">
                    <i class="fa-solid fa-house"></i> Home
                </a>

                <a href="#about">
                    <i class="fa-solid fa-circle-info"></i> About
                </a>

                <a href="#upload">
                    <i class="fa-solid fa-circle-question"></i> Help
                </a>

                <a href="#upload" class="login-link">
                    <i class="fa-solid fa-user"></i> Login
                </a>

            </div>

        </nav>


        <!-- ================= HERO SECTION ================= -->

        <header class="hero-section">

            <!-- LEFT HERO CARD -->

            <div class="hero-content">

                <span class="hero-kicker">
                    STUDENT ADMISSION & RECORDS MANAGEMENT SYSTEM
                </span>

                <h1>
                    Admit<span>Flow</span>
                </h1>

                <p class="hero-description">
                    Upload &nbsp;•&nbsp; Scan &nbsp;•&nbsp;
                    Verify &nbsp;•&nbsp; Confirm
                </p>

                <div class="hero-divider"></div>

                <p class="hero-small-text">
                    Your Documents. Our Precision.
                </p>

            </div>


            <!-- RIGHT HERO QUOTE -->

            <div class="hero-right">

                <div class="hero-quote">

                    <span>
                        “Dream Big...
                    </span>

                    <span>
                        Apply Smart...”
                    </span>

                </div>


                <!-- GRADUATION CAP IMAGE SPACE -->

                <div class="cap-art" aria-label="Graduation Cap Illustration">

                    <!-- Add your graduation cap image later.
                         Example filename: Graduation-Cap.png -->

                </div>

            </div>

        </header>



        <!-- ================= DOCUMENT UPLOAD ================= -->

        <section class="glass-section upload-section" id="upload">

            <div class="section-heading">

                <span class="section-kicker">
                    DOCUMENT VERIFICATION
                </span>

                <h2>
                    <i class="fa-solid fa-cloud-arrow-up"></i>
                    Upload Documents
                </h2>

                <p>
                    Upload the required documents to extract student information.
                </p>

            </div>


            <!-- UPLOAD CARDS -->

            <div class="upload-grid">


                <!-- AADHAAR -->

                <div class="upload-card">

                    <div class="upload-icon">
                        <i class="fa-solid fa-id-card"></i>
                    </div>

                    <h3>Aadhaar Card</h3>

                    <p>Identity Verification</p>

                    <asp:FileUpload ID="FileUpload1"
                        runat="server"
                        CssClass="file-input" />

                </div>



                <!-- STUDENT PHOTO -->

                <div class="upload-card">

                    <div class="upload-icon">
                        <i class="fa-solid fa-user"></i>
                    </div>

                    <h3>Student Photo</h3>

                    <p>Recent Passport Size Photo</p>

                    <asp:FileUpload ID="FileUpload2"
                        runat="server"
                        CssClass="file-input" />

                </div>



                <!-- 10TH MARKSHEET -->

                <div class="upload-card">

                    <div class="upload-icon">
                        <i class="fa-solid fa-file-lines"></i>
                    </div>

                    <h3>10th Marksheet</h3>

                    <p>Secondary Education</p>

                    <asp:FileUpload ID="FileUpload3"
                        runat="server"
                        CssClass="file-input" />

                </div>



                <!-- 12TH MARKSHEET -->

                <div class="upload-card">

                    <div class="upload-icon">
                        <i class="fa-solid fa-file-lines"></i>
                    </div>

                    <h3>12th Marksheet</h3>

                    <p>Senior Secondary Education</p>

                    <asp:FileUpload ID="FileUpload4"
                        runat="server"
                        CssClass="file-input" />

                </div>



                <!-- GRADUATION -->

                <div class="upload-card">

                    <div class="upload-icon">
                        <i class="fa-solid fa-graduation-cap"></i>
                    </div>

                    <h3>Graduation Marksheet</h3>

                    <p>Higher Education</p>

                    <asp:FileUpload ID="FileUpload5"
                        runat="server"
                        CssClass="file-input" />

                </div>

            </div>



            <!-- SCAN BUTTON -->

            <div class="scan-button-container">

                <asp:Button ID="BtnScan"
                    runat="server"
                    Text="SCAN DOCUMENTS"
                    CssClass="scan-button" OnClick="BtnScan_Click1" />

            </div>


            <!-- SCAN STATUS -->

            <div class="scan-status">

                <asp:Label ID="LblScanStatus"
                    runat="server"
                    CssClass="scan-status-label">
                </asp:Label>

            </div>

        </section>



        <!-- ================================================= -->
        <!-- THREE STUDENT INFORMATION PANELS                  -->
        <!-- ================================================= -->

        <div class="details-panels">


            <!-- ============================================= -->
            <!-- PANEL 1: STUDENT DETAILS                     -->
            <!-- ============================================= -->

            <asp:Panel ID="pnlStudentDetails"
                runat="server"
                CssClass="details-panel"
                Visible="false">

                <section class="glass-section details-section">

                    <div class="section-heading">

                        <span class="section-kicker">
                            PERSONAL INFORMATION
                        </span>

                        <h2>
                            <i class="fa-solid fa-user-graduate"></i>
                            Student Details
                        </h2>

                    </div>


                    <!-- STUDENT PHOTO -->

                    <div class="student-photo-container">

                        <div class="student-photo-frame">

                            <asp:Image ID="ImgStudentPhoto"
                                runat="server"
                                CssClass="student-photo"
                                AlternateText="Student Photograph" />

                        </div>

                        <span class="photo-caption">
                            STUDENT PHOTOGRAPH
                        </span>

                    </div>


                    <div class="form-grid">


                        <div class="form-group">

                            <label>Student Name</label>

                            <asp:TextBox ID="TxtStudentName"
                                runat="server"
                                placeholder="Student Name">
                            </asp:TextBox>

                        </div>



                        <div class="form-group">

                            <label>Date of Birth</label>

                            <asp:TextBox ID="TxtDOB"
                                runat="server"
                                TextMode="Date">
                            </asp:TextBox>

                        </div>



                        <div class="form-group">

                            <label>Aadhaar Number</label>

                            <asp:TextBox ID="TxtAadhaarNo"
                                runat="server"
                                placeholder="Aadhaar Number">
                            </asp:TextBox>

                        </div>



                        <div class="form-group">

                            <label>Father's Name</label>

                            <asp:TextBox ID="TxtFatherName"
                                runat="server"
                                placeholder="Father's Name">
                            </asp:TextBox>

                        </div>



                        <div class="form-group">

                            <label>Mother's Name</label>

                            <asp:TextBox ID="TxtMotherName"
                                runat="server"
                                placeholder="Mother's Name">
                            </asp:TextBox>

                        </div>



                        <div class="form-group">

                            <label>Contact Number</label>

                            <asp:TextBox ID="TxtContNumber"
                                runat="server"
                                placeholder="Contact Number">
                            </asp:TextBox>

                        </div>



                        <div class="form-group">

                            <label>Address</label>

                            <asp:TextBox ID="TxtAddress"
                                runat="server"
                                TextMode="MultiLine"
                                Rows="3"
                                placeholder="Student Address">
                            </asp:TextBox>

                        </div>



                        <!-- CATEGORY -->

                        <div class="form-group">

                            <label>Category</label>

                            <div class="radio-group">

                                <asp:RadioButton ID="RadioButton1"
                                    runat="server"
                                    GroupName="Category"
                                    Text="SC" />

                                <asp:RadioButton ID="RadioButton2"
                                    runat="server"
                                    GroupName="Category"
                                    Text="ST" />

                                <asp:RadioButton ID="RadioButton3"
                                    runat="server"
                                    GroupName="Category"
                                    Text="OBC" />

                                <asp:RadioButton ID="RadioButton4"
                                    runat="server"
                                    GroupName="Category"
                                    Text="Other" />

                            </div>

                        </div>


                    </div>

                </section>

            </asp:Panel>



            <!-- ============================================= -->
            <!-- PANEL 2: STUDENT MARKSHEET                    -->
            <!-- ============================================= -->

            <asp:Panel ID="pnlStudentMarksheet"
                runat="server"
                CssClass="details-panel"
                Visible="false">

                <section class="glass-section details-section">

                    <div class="section-heading">

                        <span class="section-kicker">
                            ACADEMIC INFORMATION
                        </span>

                        <h2>
                            <i class="fa-solid fa-book-open"></i>
                            Student Marksheet
                        </h2>

                    </div>


                    <div class="academic-container">


                        <!-- 10TH -->

                        <div class="academic-box">

                            <h3>
                                <i class="fa-solid fa-award"></i>
                                10th Standard
                            </h3>


                            <div class="form-group">

                                <label>Passing Year</label>

                                <asp:TextBox ID="Txt10year"
                                    runat="server"
                                    placeholder="Passing Year">
                                </asp:TextBox>

                            </div>


                            <div class="form-group">

                                <label>Percentage</label>

                                <asp:TextBox ID="Txt10percentage"
                                    runat="server"
                                    placeholder="Percentage">
                                </asp:TextBox>

                            </div>

                        </div>



                        <!-- 12TH -->

                        <div class="academic-box">

                            <h3>
                                <i class="fa-solid fa-award"></i>
                                12th Standard
                            </h3>


                            <div class="form-group">

                                <label>Passing Year</label>

                                <asp:TextBox ID="Txt12year"
                                    runat="server"
                                    placeholder="Passing Year">
                                </asp:TextBox>

                            </div>


                            <div class="form-group">

                                <label>Percentage</label>

                                <asp:TextBox ID="Txt12percentage"
                                    runat="server"
                                    placeholder="Percentage">
                                </asp:TextBox>

                            </div>

                        </div>

                    </div>



                    <!-- ADMISSION SESSION -->

                    <div class="form-group session-field">

                        <label>Admission Session</label>

                        <asp:TextBox ID="TxtSession"
                            runat="server"
                            placeholder="Example: 2026-2027">
                        </asp:TextBox>

                    </div>


                </section>

            </asp:Panel>



            <!-- ============================================= -->
            <!-- PANEL 3: GRADUATION DETAILS                   -->
            <!-- ============================================= -->

            <asp:Panel ID="pnlGraduationDetails"
                runat="server"
                CssClass="details-panel"
                Visible="false">

                <section class="glass-section details-section">

                    <div class="section-heading">

                        <span class="section-kicker">
                            HIGHER EDUCATION
                        </span>

                        <h2>
                            <i class="fa-solid fa-graduation-cap"></i>
                            Graduation Details
                        </h2>

                    </div>


                    <div class="form-grid">


                        <div class="form-group">

                            <label>University Name</label>

                            <asp:TextBox ID="TxtUniversity"
                                runat="server"
                                placeholder="University Name">
                            </asp:TextBox>

                        </div>



                        <div class="form-group">

                            <label>College Name</label>

                            <asp:TextBox ID="TxtCollegeName"
                                runat="server"
                                placeholder="College Name">
                            </asp:TextBox>

                        </div>



                        <div class="form-group">

                            <label>CGPA / Percentage</label>

                            <asp:TextBox ID="TxtCGPA"
                                runat="server"
                                placeholder="CGPA / Percentage">
                            </asp:TextBox>

                        </div>



                        <div class="form-group">

                            <label>Degree Passing Year</label>

                            <asp:TextBox ID="TxtDegreeYear"
                                runat="server"
                                placeholder="Passing Year">
                            </asp:TextBox>

                        </div>


                    </div>



                    <!-- SAVE BUTTON -->

                    <div class="save-button-container">

                        <asp:Button ID="BtnSave"
                            runat="server"
                            Text="SAVE STUDENT RECORD"
                            CssClass="save-button" />

                    </div>


                </section>

            </asp:Panel>


        </div>


        <!-- ================= FOOTER ================= -->

        <footer class="page-footer" id="about">

            <p>
                <i class="fa-solid fa-graduation-cap"></i>
                AdmitFlow &nbsp; | &nbsp;
                Simplifying Admissions. Automating Verification.
            </p>

        </footer>


    </div>

</form>

</body>

</html>