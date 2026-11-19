<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Admit_Scan.aspx.cs"
    Inherits="Admit_Scan.Admit_Scan" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <meta charset="UTF-8" />

    <meta name="viewport"
        content="width=device-width, initial-scale=1.0" />

    <title>ADMITFLOW | Student Admission & Records Management</title>

    <!-- Google Fonts -->
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap"
        rel="stylesheet" />

    <!-- Font Awesome -->
    <link rel="stylesheet"
        href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" />

    <!-- Main CSS -->
    <link href="Admit_Scan.css?v=4" rel="stylesheet" />

</head>

<body>

    <form id="form1" runat="server">

        <div class="page-wrapper">

            <!-- =====================================
                 HEADER
            ====================================== -->

            <header class="main-header">

                <div class="emblem-banner">

                    <asp:Image
                        ID="ImgBrandEmblem"
                        runat="server"
                        ImageUrl="~/AdmitFlow_Emblem1-Photoroom.png"
                        CssClass="brand-emblem"
                        AlternateText="AdmitFlow Emblem" />

                </div>

                <h1 class="brand-title">
                    ADMITFLOW
                </h1>

                <h2 class="brand-subtitle">
                    STUDENT ADMISSION & RECORDS MANAGEMENT
                </h2>

                <div class="title-divider">

                    <span></span>

                    <i class="fa-solid fa-graduation-cap"></i>

                    <span></span>

                </div>

                <p class="tagline">
                    Simplifying Admissions. Automating Verification.
                </p>

            </header>


            <!-- =====================================
                 DOCUMENT UPLOAD SECTION
            ====================================== -->

            <section class="upload-section">

                <div class="section-heading">

                    <h2>
                        <i class="fa-solid fa-file-circle-check"></i>
                        DOCUMENT VERIFICATION
                    </h2>

                    <p>
                        Upload the required documents to extract student information.
                    </p>

                </div>


                <!-- DOCUMENT UPLOAD GRID -->

                <div class="upload-grid">


                    <!-- =================================
                         1. AADHAAR DOCUMENTS
                    ================================== -->

                    <div class="upload-card">

                        <div class="upload-icon">
                            <i class="fa-solid fa-id-card"></i>
                        </div>

                        <h3>
                            AADHAAR DOCUMENTS
                        </h3>

                        <p class="upload-description">
                            Upload front and back sides.
                        </p>


                        <div class="upload-field">

                            <label>
                                <i class="fa-solid fa-id-card"></i>
                                AADHAAR FRONT
                            </label>

                            <asp:FileUpload
                                ID="FileUpload1"
                                runat="server"
                                CssClass="file-upload"
                                accept=".jpg,.jpeg,.png" />

                        </div>


                        <div class="upload-field">

                            <label>
                                <i class="fa-solid fa-id-card-clip"></i>
                                AADHAAR BACK
                            </label>

                            <asp:FileUpload
                                ID="FileUpload2"
                                runat="server"
                                CssClass="file-upload"
                                accept=".jpg,.jpeg,.png" />

                        </div>

                    </div>


                    <!-- =================================
                         2. STUDENT PHOTOGRAPH
                    ================================== -->

                    <div class="upload-card">

                        <div class="upload-icon">
                            <i class="fa-solid fa-user-graduate"></i>
                        </div>

                        <h3>
                            STUDENT PHOTOGRAPH
                        </h3>

                        <p class="upload-description">
                            Upload a clear student photograph.
                        </p>


                        <div class="upload-field">

                            <label>
                                <i class="fa-solid fa-image"></i>
                                STUDENT PHOTO
                            </label>

                            <asp:FileUpload
                                ID="FileUpload3"
                                runat="server"
                                CssClass="file-upload"
                                accept=".jpg,.jpeg,.png" />

                        </div>

                    </div>


                    <!-- =================================
                         3. 10TH MARKSHEET
                    ================================== -->

                    <div class="upload-card">

                        <div class="upload-icon">
                            <i class="fa-solid fa-book-open"></i>
                        </div>

                        <h3>
                            10TH MARKSHEET
                        </h3>

                        <p class="upload-description">
                            Upload your secondary examination marksheet.
                        </p>


                        <div class="upload-field">

                            <label>
                                <i class="fa-solid fa-file-pdf"></i>
                                10TH DOCUMENT
                            </label>

                            <asp:FileUpload
                                ID="FileUpload4"
                                runat="server"
                                CssClass="file-upload"
                                accept=".jpg,.jpeg,.png,.pdf" />

                        </div>

                    </div>


                    <!-- =================================
                         4. 12TH MARKSHEET
                    ================================== -->

                    <div class="upload-card">

                        <div class="upload-icon">
                            <i class="fa-solid fa-graduation-cap"></i>
                        </div>

                        <h3>
                            12TH MARKSHEET
                        </h3>

                        <p class="upload-description">
                            Upload your senior secondary examination marksheet.
                        </p>


                        <div class="upload-field">

                            <label>
                                <i class="fa-solid fa-file-pdf"></i>
                                12TH DOCUMENT
                            </label>

                            <asp:FileUpload
                                ID="FileUpload5"
                                runat="server"
                                CssClass="file-upload"
                                accept=".jpg,.jpeg,.png,.pdf" />

                        </div>

                    </div>


                    <!-- =================================
                         5. GRADUATION MARKSHEET
                    ================================== -->

                    <div class="upload-card graduation-upload-card">

                        <div class="upload-icon">
                            <i class="fa-solid fa-award"></i>
                        </div>

                        <h3>
                            GRADUATION MARKSHEET
                        </h3>

                        <p class="upload-description">
                            Optional document for graduation details.
                        </p>


                        <div class="upload-field">

                            <label>
                                <i class="fa-solid fa-file-pdf"></i>
                                GRADUATION DOCUMENT
                            </label>

                            <asp:FileUpload
                                ID="FileUpload6"
                                runat="server"
                                CssClass="file-upload"
                                accept=".jpg,.jpeg,.png,.pdf" />

                        </div>


                        <span class="optional-label">
                            OPTIONAL
                        </span>

                    </div>


                </div>


                <!-- =================================
                     SCAN BUTTON
                ================================== -->

                <div class="scan-button-container">

                    <asp:Button
                        ID="BtnScan"
                        runat="server"
                        Text="SCAN DOCUMENTS"
                        CssClass="scan-button"
                        OnClick="BtnScan_Click1" />

                </div>


                <!-- SCANNING STATUS -->

                <div class="status-container">

                    <asp:Label
                        ID="LblScanStatus"
                        runat="server"
                        CssClass="scan-status">
                    </asp:Label>

                </div>

            </section>


            <!-- =====================================
                 RESULTS SECTION
            ====================================== -->

            <section class="results-section">


                <div class="section-heading">

                    <h2>
                        <i class="fa-solid fa-folder-open"></i>
                        EXTRACTED STUDENT INFORMATION
                    </h2>

                    <p>
                        Extracted information will appear below after document scanning.
                    </p>

                </div>


                <!-- DYNAMIC TWO / THREE PANEL CONTAINER -->

                <div id="panelsContainer"
                    runat="server"
                    class="panels-container two-panels">


                    <!-- =================================
                         PANEL 1 : STUDENT DETAILS
                    ================================== -->

                    <asp:Panel
                        ID="pnlStudentDetails"
                        runat="server"
                        CssClass="panel"
                        Visible="false">


                        <div class="panel-heading">

                            <h3>
                                <i class="fa-solid fa-user-graduate"></i>
                                STUDENT DETAILS
                            </h3>

                        </div>


                        <!-- STUDENT PHOTOGRAPH -->

                        <div class="student-photo-container">

                            <asp:Image
                                ID="ImgStudentPhoto"
                                runat="server"
                                CssClass="student-photo"
                                AlternateText="Student Photograph" />

                        </div>


                        <!-- STUDENT NAME -->

                        <div class="form-group">

                            <label>
                                STUDENT NAME
                            </label>

                            <asp:TextBox
                                ID="TxtStudentName"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Student name">
                            </asp:TextBox>

                        </div>


                        <!-- DATE OF BIRTH -->

                        <div class="form-group">

                            <label>
                                DATE OF BIRTH
                            </label>

                            <asp:TextBox
                                ID="TxtDOB"
                                runat="server"
                                CssClass="form-control"
                                placeholder="DD/MM/YYYY">
                            </asp:TextBox>

                        </div>


                        <!-- AADHAAR NUMBER -->

                        <div class="form-group">

                            <label>
                                AADHAAR NUMBER
                            </label>

                            <asp:TextBox
                                ID="TxtAadhaarNo"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Aadhaar number">
                            </asp:TextBox>

                        </div>


                        <!-- FATHER'S NAME -->

                        <div class="form-group">

                            <label>
                                FATHER'S NAME
                            </label>

                            <asp:TextBox
                                ID="TxtFatherName"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Father's name">
                            </asp:TextBox>

                        </div>


                        <!-- MOTHER'S NAME -->

                        <div class="form-group">

                            <label>
                                MOTHER'S NAME
                            </label>

                            <asp:TextBox
                                ID="TxtMotherName"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Mother's name">
                            </asp:TextBox>

                        </div>


                        <!-- CONTACT NUMBER -->

                        <div class="form-group">

                            <label>
                                CONTACT NUMBER
                            </label>

                            <asp:TextBox
                                ID="TxtContNumber"
                                runat="server"
                                CssClass="form-control"
                                TextMode="Phone"
                                MaxLength="10"
                                placeholder="Enter 10-digit mobile number">
                            </asp:TextBox>

                        </div>


                        <!-- =================================
                             RESIDENTIAL ADDRESS
                        ================================== -->

                        <div class="form-group">

                            <label>
                                <i class="fa-solid fa-location-dot"></i>
                                RESIDENTIAL ADDRESS
                            </label>

                            <asp:TextBox
                                ID="TxtAddress"
                                runat="server"
                                CssClass="form-control address-control"
                                TextMode="MultiLine"
                                Rows="3"
                                placeholder="Enter complete residential address">
                            </asp:TextBox>

                        </div>


                        <!-- =================================
                             CITY
                        ================================== -->

                        <div class="form-group">

                            <label>
                                <i class="fa-solid fa-city"></i>
                                CITY
                            </label>

                            <asp:TextBox
                                ID="TxtCity"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Enter city">
                            </asp:TextBox>

                        </div>


                        <!-- =================================
                             STATE
                        ================================== -->

                        <div class="form-group">

                            <label>
                                <i class="fa-solid fa-map"></i>
                                STATE
                            </label>

                            <asp:TextBox
                                ID="TxtState"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Enter state">
                            </asp:TextBox>

                        </div>


                        <!-- =================================
                             PIN CODE
                        ================================== -->

                        <div class="form-group">

                            <label>
                                <i class="fa-solid fa-map-pin"></i>
                                PIN CODE
                            </label>

                            <asp:TextBox
                                ID="TxtPinCode"
                                runat="server"
                                CssClass="form-control"
                                TextMode="Number"
                                MaxLength="6"
                                placeholder="Enter 6-digit PIN code">
                            </asp:TextBox>

                        </div>


                        <!-- =================================
                             CATEGORY
                        ================================== -->

                        <div class="form-group">

                            <label>
                                CATEGORY
                            </label>

                            <div class="category-options">

                                <asp:RadioButton
                                    ID="RadioButton1"
                                    runat="server"
                                    Text="SC"
                                    GroupName="Category" />

                                <asp:RadioButton
                                    ID="RadioButton2"
                                    runat="server"
                                    Text="ST"
                                    GroupName="Category" />

                                <asp:RadioButton
                                    ID="RadioButton3"
                                    runat="server"
                                    Text="OBC"
                                    GroupName="Category" />

                                <asp:RadioButton
                                    ID="RadioButton4"
                                    runat="server"
                                    Text="Other"
                                    GroupName="Category" />

                            </div>

                        </div>


                    </asp:Panel>



                    <!-- =================================
                         PANEL 2 : ACADEMIC DETAILS
                    ================================== -->

                    <asp:Panel
                        ID="pnlStudentMarksheet"
                        runat="server"
                        CssClass="panel"
                        Visible="false">


                        <div class="panel-heading">

                            <h3>
                                <i class="fa-solid fa-book-open"></i>
                                ACADEMIC DETAILS
                            </h3>

                        </div>


                        <!-- =================================
                             10TH STANDARD
                        ================================== -->

                        <div class="sub-panel-heading">

                            <h4>
                                <i class="fa-solid fa-book"></i>
                                10TH STANDARD
                            </h4>

                        </div>


                        <!-- 10TH BOARD -->

                        <div class="form-group">

                            <label>
                                BOARD
                            </label>

                            <asp:TextBox
                                ID="Txt10Board"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Board name">
                            </asp:TextBox>

                        </div>


                        <!-- 10TH PASSING YEAR -->

                        <div class="form-group">

                            <label>
                                PASSING YEAR
                            </label>

                            <asp:TextBox
                                ID="Txt10year"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Passing year">
                            </asp:TextBox>

                        </div>


                        <!-- 10TH SUBJECTS -->

                        <div class="form-group">

                            <label>
                                SUBJECTS
                            </label>

                            <asp:TextBox
                                ID="Txt10Subject"
                                runat="server"
                                CssClass="form-control subject-control"
                                TextMode="MultiLine"
                                Rows="3"
                                placeholder="Subject-wise marks">
                            </asp:TextBox>

                        </div>


                        <!-- 10TH PERCENTAGE -->

                        <div class="form-group">

                            <label>
                                PERCENTAGE
                            </label>

                            <asp:TextBox
                                ID="Txt10percentage"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Percentage">
                            </asp:TextBox>

                        </div>


                        <!-- =================================
                             12TH STANDARD
                        ================================== -->

                        <div class="sub-panel-heading">

                            <h4>
                                <i class="fa-solid fa-graduation-cap"></i>
                                12TH STANDARD
                            </h4>

                        </div>


                        <!-- 12TH BOARD -->

                        <div class="form-group">

                            <label>
                                BOARD
                            </label>

                            <asp:TextBox
                                ID="Txt12Board"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Board name">
                            </asp:TextBox>

                        </div>


                        <!-- 12TH PASSING YEAR -->

                        <div class="form-group">

                            <label>
                                PASSING YEAR
                            </label>

                            <asp:TextBox
                                ID="Txt12year"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Passing year">
                            </asp:TextBox>

                        </div>


                        <!-- 12TH SUBJECTS -->

                        <div class="form-group">

                            <label>
                                SUBJECTS
                            </label>

                            <asp:TextBox
                                ID="Txt12Subject"
                                runat="server"
                                CssClass="form-control subject-control"
                                TextMode="MultiLine"
                                Rows="3"
                                placeholder="Subject-wise marks">
                            </asp:TextBox>

                        </div>


                        <!-- 12TH PERCENTAGE -->

                        <div class="form-group">

                            <label>
                                PERCENTAGE
                            </label>

                            <asp:TextBox
                                ID="Txt12percentage"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Percentage">
                            </asp:TextBox>

                        </div>


                        <!-- =================================
                             ADMISSION INFORMATION
                        ================================== -->

                        <div class="sub-panel-heading">

                            <h4>
                                <i class="fa-solid fa-calendar-days"></i>
                                ADMISSION INFORMATION
                            </h4>

                        </div>


                        <!-- ADMISSION SESSION -->

                        <div class="form-group">

                            <label>
                                ADMISSION SESSION
                            </label>

                            <asp:Label ID="TxtSession"
    runat="server"
    CssClass="session-value">
</asp:Label>

                        </div>


                    </asp:Panel>



                    <!-- =================================
                         PANEL 3 : GRADUATION DETAILS
                    ================================== -->

                    <asp:Panel
                        ID="pnlGraduationDetails"
                        runat="server"
                        CssClass="panel graduation-panel"
                        Visible="false">


                        <div class="panel-heading">

                            <h3>
                                <i class="fa-solid fa-award"></i>
                                GRADUATION DETAILS
                            </h3>

                        </div>


                        <div class="graduation-icon">

                            <i class="fa-solid fa-user-graduate"></i>

                        </div>


                        <p class="graduation-description">
                            Graduation information extracted from the uploaded marksheet.
                        </p>


                        <!-- UNIVERSITY -->

                        <div class="form-group">

                            <label>
                                UNIVERSITY NAME
                            </label>

                            <asp:TextBox
                                ID="TxtUniversity"
                                runat="server"
                                CssClass="form-control"
                                placeholder="University name">
                            </asp:TextBox>

                        </div>


                        <!-- COLLEGE -->

                        <div class="form-group">

                            <label>
                                COLLEGE / INSTITUTION NAME
                            </label>

                            <asp:TextBox
                                ID="TxtCollegeName"
                                runat="server"
                                CssClass="form-control"
                                placeholder="College name">
                            </asp:TextBox>

                        </div>


                        <!-- CGPA -->

                        <div class="form-group">

                            <label>
                                CGPA / PERCENTAGE
                            </label>

                            <asp:TextBox
                                ID="TxtCGPA"
                                runat="server"
                                CssClass="form-control"
                                placeholder="CGPA or percentage">
                            </asp:TextBox>

                        </div>


                        <!-- DEGREE PASSING YEAR -->

                        <div class="form-group">

                            <label>
                                DEGREE PASSING YEAR
                            </label>

                            <asp:TextBox
                                ID="TxtDegreeYear"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Degree passing year">
                            </asp:TextBox>

                        </div>


                    </asp:Panel>


                </div>


                <!-- =================================
                     SAVE STUDENT RECORD
                ================================== -->

                <div class="save-button-container">

                    <asp:Button
                        ID="BtnSave"
                        runat="server"
                        Text="SAVE STUDENT RECORD"
                        CssClass="save-button"
                        OnClick="BtnSave_Click" />

                </div>


            </section>



            <!-- =====================================
                 FOOTER
            ====================================== -->

            <footer class="main-footer">

                <div class="footer-divider"></div>

                <p>
                    <i class="fa-solid fa-graduation-cap"></i>
                    ADMITFLOW
                </p>

                <span>
                    STUDENT ADMISSION & RECORDS MANAGEMENT SYSTEM
                </span>

                <small>
                    Simplifying Admissions. Automating Verification.
                </small>

            </footer>


        </div>

    </form>

</body>

</html>