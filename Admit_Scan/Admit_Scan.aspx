<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Admit_Scan.aspx.cs"
    Inherits="Admit_Scan.Admit_Scan" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>AdmitFlow | Student Admission & Records Management</title>

    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <link href="Admit_Scan.css" rel="stylesheet" />

    <link href="https://fonts.googleapis.com/css2?family=Cinzel:wght@400;500;600;700&family=Cormorant+Garamond:wght@400;500;600;700&family=Special+Elite&display=swap"
        rel="stylesheet" />

    <link rel="stylesheet"
        href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.2/css/all.min.css" />

</head>

<body>

<form id="form1" runat="server">

    <div class="main-container">

        <!-- ================= HEADER ================= -->

        <header class="page-header">

            <img src="AdmitFlow_Emblem1-Photoroom.png"
                class="brand-emblem"
                alt="AdmitFlow Royal Emblem" />

            <h1>ADMITFLOW</h1>

            <p class="brand-subtitle">
                STUDENT ADMISSION & RECORDS MANAGEMENT SYSTEM
            </p>

            <div class="header-divider"></div>

            <p class="brand-tagline">
                Simplifying Admissions. Automating Verification.
            </p>

        </header>


        <!-- ================= DOCUMENT UPLOAD ================= -->

        <section class="glass-section upload-section">

            <div class="section-heading">

                <span class="section-kicker">
                    DOCUMENT VERIFICATION
                </span>

                <h2>
                    <i class="fa-solid fa-file-arrow-up"></i>
                    UPLOAD DOCUMENTS
                </h2>

                <p>
                    Upload the required documents to extract student information.
                </p>

            </div>


            <div class="upload-grid">

                <!-- Aadhaar -->

                <div class="upload-card">

                    <i class="fa-solid fa-id-card"></i>

                    <h3>AADHAAR CARD</h3>

                    <asp:FileUpload ID="FileUpload1"
                        runat="server"
                        CssClass="file-input" />

                </div>


                <!-- Student Photo -->

                <div class="upload-card">

                    <i class="fa-solid fa-user"></i>

                    <h3>STUDENT PHOTO</h3>

                    <asp:FileUpload ID="FileUpload2"
                        runat="server"
                        CssClass="file-input" />

                </div>


                <!-- 10th Marksheet -->

                <div class="upload-card">

                    <i class="fa-solid fa-file-lines"></i>

                    <h3>10TH MARKSHEET</h3>

                    <asp:FileUpload ID="FileUpload3"
                        runat="server"
                        CssClass="file-input" />

                </div>


                <!-- 12th Marksheet -->

                <div class="upload-card">

                    <i class="fa-solid fa-file-lines"></i>

                    <h3>12TH MARKSHEET</h3>

                    <asp:FileUpload ID="FileUpload4"
                        runat="server"
                        CssClass="file-input" />

                </div>


                <!-- Graduation Marksheet -->

                <div class="upload-card">

                    <i class="fa-solid fa-graduation-cap"></i>

                    <h3>GRADUATION MARKSHEET</h3>

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
                    CssClass="scan-button"
                     />

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
        <!-- STUDENT'S DETAILS                                  -->
        <!-- HIDDEN UNTIL SUCCESSFUL SCANNING                   -->
        <!-- ================================================= -->

        <asp:Panel ID="pnlStudentDetails"
            runat="server"
            Visible="false">

            <section class="glass-section">

                <div class="section-heading">

                    <span class="section-kicker">
                        PERSONAL INFORMATION
                    </span>

                    <h2>
                        <i class="fa-solid fa-user-graduate"></i>
                        STUDENT'S DETAILS
                    </h2>

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


                    <div class="form-group full-width">
                        <label>Address</label>

                        <asp:TextBox ID="TxtAddress"
                            runat="server"
                            TextMode="MultiLine"
                            Rows="3"
                            placeholder="Student Address">
                        </asp:TextBox>
                    </div>


                    <!-- CATEGORY -->

                    <div class="form-group full-width">

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



        <!-- ================================================= -->
        <!-- STUDENT'S MARKSHEET                                -->
        <!-- HIDDEN UNTIL SUCCESSFUL SCANNING                   -->
        <!-- ================================================= -->

        <asp:Panel ID="pnlStudentMarksheet"
            runat="server"
            Visible="false">

            <section class="glass-section">

                <div class="section-heading">

                    <span class="section-kicker">
                        ACADEMIC INFORMATION
                    </span>

                    <h2>
                        <i class="fa-solid fa-book-open"></i>
                        STUDENT'S MARKSHEET
                    </h2>

                </div>


                <div class="academic-container">

                    <!-- 10TH -->

                    <div class="academic-box">

                        <h3>10TH STANDARD</h3>

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

                        <h3>12TH STANDARD</h3>

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


                <div class="form-group session-field">

                    <label>Admission Session</label>

                    <asp:TextBox ID="TxtSession"
                        runat="server"
                        placeholder="Example: 2026-2027">
                    </asp:TextBox>

                </div>

            </section>

        </asp:Panel>



        <!-- ================================================= -->
        <!-- GRADUATION DETAILS                                 -->
        <!-- HIDDEN UNTIL SUCCESSFUL SCANNING                   -->
        <!-- ================================================= -->

        <asp:Panel ID="pnlGraduationDetails"
            runat="server"
            Visible="false">

            <section class="glass-section">

                <div class="section-heading">

                    <span class="section-kicker">
                        HIGHER EDUCATION
                    </span>

                    <h2>
                        <i class="fa-solid fa-user-graduate"></i>
                        GRADUATION DETAILS
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

</form>

</body>
</html>