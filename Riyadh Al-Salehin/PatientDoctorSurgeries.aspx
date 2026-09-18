<%@ Page Title="" Language="C#" MasterPageFile="~/Riyadh.Master"
    AutoEventWireup="true"
    CodeBehind="PatientDoctorSurgeries.aspx.cs"
    Inherits="Riyadh_Al_Salehin.PatientDoctorSurgeries" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="container mt-4" dir="rtl">

    <div class="card shadow">

        <div class="card-header bg-primary text-white">
            <h4>اختيار طبيب العمليات</h4>
        </div>

        <div class="card-body">

            <asp:Repeater ID="rptDoctors"
                runat="server"
                OnItemCommand="rptDoctors_ItemCommand">

               <ItemTemplate>

<div class="doctor-profile"
     data-aos="zoom-in">

    <div class="profile-header">

        <div class="doctor-avatar">

           <img src='<%# ResolveUrl("~/Uploads/Doctors/") + Eval("Id") + ".jpg" %>'
     class="img-fluid"
     onerror="this.onerror=null;this.src='<%= ResolveUrl("~/Images/no-image.png") %>';" />               

            <div class="status-indicator available"></div>

        </div>

        <div class="doctor-details">

            <h4><%# Eval("DoctorName") %></h4>

            <span class="specialty-tag">
                <%# Eval("Specialty") %>
            </span>

            <div class="experience-info">

                <i class="bi bi-award"></i>

                <span>
                    جراح متخصص
                </span>

            </div>

        </div>

    </div>

    <div class="action-buttons">

        <asp:Button
            ID="btnSelect"
            runat="server"
            Text="اختيار الطبيب"
            CssClass="btn-primary"
            CommandName="SelectDoctor"
            CommandArgument='<%# Eval("Id") %>' />

    </div>

</div>

</ItemTemplate>

            </asp:Repeater>

            <asp:HiddenField ID="hfPatientId" runat="server" />

            <br />

            <asp:Label ID="lblMessage"
                runat="server"
                ForeColor="Red" />

        </div>

    </div>

</div>


    <style>
.doctors-grid{

display:grid;

grid-template-columns:repeat(auto-fill,minmax(320px,1fr));

gap:30px;

margin-top:25px;

}

.doctor-profile{

background:#fff;

border-radius:20px;

padding:25px;

box-shadow:0 10px 35px rgba(0,0,0,.08);

transition:.4s;

position:relative;

overflow:hidden;

}

.doctor-profile:hover{

transform:translateY(-12px);

box-shadow:0 18px 45px rgba(13,110,253,.25);

}

.doctor-profile:before{

content:"";

position:absolute;

top:0;

left:0;

width:100%;

height:5px;

background:linear-gradient(90deg,#0d6efd,#20c997);

}

.profile-header{

display:flex;

align-items:center;

gap:18px;

margin-bottom:20px;

}

.doctor-avatar{

position:relative;

}

.doctor-avatar img{

width:95px;

height:95px;

border-radius:50%;

object-fit:cover;

border:4px solid #0d6efd;

transition:.4s;

}

.doctor-profile:hover img{

transform:scale(1.08);

}

.status-indicator{

position:absolute;

bottom:5px;

right:5px;

width:16px;

height:16px;

border-radius:50%;

border:3px solid white;

}

.available{

background:#28a745;

}

.doctor-details h4{

margin:0;

font-size:22px;

font-weight:700;

color:#222;

}

.specialty-tag{

display:inline-block;

margin-top:8px;

padding:6px 14px;

background:#eaf4ff;

color:#0d6efd;

border-radius:25px;

font-size:14px;

font-weight:600;

}

.experience-info{

margin-top:12px;

color:#777;

font-size:15px;

}

.action-buttons{

margin-top:25px;

text-align:center;

}

.btn-primary{

width:100%;

background:#0d6efd;

border:none;

padding:12px;

border-radius:12px;

font-size:18px;

font-weight:bold;

transition:.3s;

color:white;

}

.btn-primary:hover{

background:#084298;

transform:scale(1.03);

}





    </style>

    <link href="https://unpkg.com/aos@2.3.4/dist/aos.css" rel="stylesheet">

    <link href="assets/css/aos.css" rel="stylesheet" />

    <script src="assets/css/aos.js"></script>
<script src="assets/css/aos.js"></script>

<script>
AOS.init({
duration:800,
once:true
});
</script>

</asp:Content>