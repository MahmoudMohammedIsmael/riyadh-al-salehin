<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="PatientDoctorSchedule.aspx.cs"
    Inherits="Riyadh_Al_Salehin.PatientDoctorSchedule"
    MasterPageFile="~/Riyadh.Master" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="schedule-page" dir="rtl">

        <!-- عنوان الصفحة -->
        <div class="page-title">
            <div class="title-icon">👨‍⚕️</div>
            <div>
                <h2>اختيار الطبيب</h2>
                <p>اختر الطبيب المناسب لحجز موعد الكشف</p>
            </div>
        </div>


         <!-- المواعيد -->
 <div class="appointments-section">

     <div class="appointments-title">

         <span class="calendar-icon">📅</span>

         <div>
             <h3>المواعيد المتاحة</h3>
             <span>اختر الموعد المناسب</span>
         </div>

     </div>


     <div class="table-responsive">

         <asp:GridView
             ID="gvSlots"
             runat="server"
             CssClass="slots-table"
             AutoGenerateColumns="False"
             DataKeyNames="Id"
             OnSelectedIndexChanged="gvSlots_SelectedIndexChanged">

             <Columns>

                 <asp:BoundField
                     DataField="StartTime"
                     HeaderText="بداية الموعد"
                     DataFormatString="{0:yyyy/MM/dd HH:mm}" />

                 <asp:BoundField
                     DataField="EndTime"
                     HeaderText="نهاية الموعد"
                     DataFormatString="{0:yyyy/MM/dd HH:mm}" />

                 <asp:BoundField
                     DataField="Status"
                     HeaderText="الحالة" />

                 <asp:CommandField
                     ShowSelectButton="True"
                     SelectText="حجز الموعد" />

             </Columns>

         </asp:GridView>

     </div>

 </div>

        <!-- الأطباء -->
        <div class="doctors-section">

          <asp:Repeater ID="rptDoctors"
    runat="server"
    OnItemCommand="rptDoctors_ItemCommand">
                <ItemTemplate>

                    <div class="doctor-card">

                        <!-- صورة الطبيب -->
                        <div class="doctor-image">

                            <img
                                src='<%# ResolveUrl("~/Uploads/Doctors/") + Eval("Id") + ".jpg" %>'
                                onerror="this.onerror=null;this.src='<%= ResolveUrl("~/Images/no-image.png") %>';" />

                            <!-- حالة الطبيب -->
                          <div class='<%# IsDoctorAvailableToday(Eval("Id")) ? "availability available" : "availability unavailable" %>'>

    <span class="status-dot"></span>

    <%# IsDoctorAvailableToday(Eval("Id"))
        ? "متاح اليوم"
        : "غير متاح اليوم" %>

</div>

                        </div>

                        <!-- بيانات الطبيب -->
                        <div class="doctor-body">

                            <h3>
                                <%# Eval("DoctorName") %>
                            </h3>

                            <span class="doctor-specialty">
                                <%# Eval("Specialty") %>
                            </span>

                            <asp:Button
                                ID="btnSelect"
                                runat="server"
                                Text="عرض المواعيد"
                                CssClass="btn-select"
                                CommandName="SelectDoctor"
                                CommandArgument='<%# Eval("Id") %>' />

                        </div>

                    </div>

                </ItemTemplate>

            </asp:Repeater>

        </div>


       


        <asp:HiddenField
            ID="hfPatientId"
            runat="server" />

        <asp:HiddenField
            ID="hfDoctorId"
            runat="server" />

        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="message-label" />

    </div>


    <style>

        /* =========================
           الصفحة
        ========================= */

        .schedule-page {
            max-width: 1150px;
            margin: 20px auto;
            padding: 0 15px 40px;
        }


        /* =========================
           عنوان الصفحة
        ========================= */

        .page-title {
            display: flex;
            align-items: center;
            gap: 14px;
            margin-bottom: 22px;
            padding: 15px 18px;
            background: #fff;
            border-radius: 12px;
            border: 1px solid #e8eef5;
        }

        .title-icon {
            width: 48px;
            height: 48px;
            display: flex;
            align-items: center;
            justify-content: center;
            background: #eaf4ff;
            border-radius: 12px;
            font-size: 25px;
        }

        .page-title h2 {
            margin: 0;
            font-size: 22px;
            color: #125ea8;
            font-weight: 700;
        }

        .page-title p {
            margin: 4px 0 0;
            color: #777;
            font-size: 13px;
        }


        /* =========================
           شبكة الأطباء
        ========================= */

        .doctors-section {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(210px, 1fr));
            gap: 15px;
            margin-bottom: 25px;
        }


        /* =========================
           بطاقة الطبيب
        ========================= */

        .doctor-card {
            background: #fff;
            border: 1px solid #e6edf4;
            border-radius: 13px;
            overflow: hidden;
            transition: all .25s ease;
            box-shadow: 0 3px 12px rgba(0,0,0,.05);
        }

        .doctor-card:hover {
            transform: translateY(-3px);
            box-shadow: 0 8px 22px rgba(0,0,0,.09);
            border-color: #bcdcff;
        }


        /* =========================
           صورة الطبيب
        ========================= */

        .doctor-image {
            height: 145px;
            position: relative;
            background: #f5f8fb;
            overflow: hidden;
        }

        .doctor-image img {
            width: 100%;
            height: 100%;
            object-fit: cover;
            transition: transform .3s ease;
        }

        .doctor-card:hover .doctor-image img {
            transform: scale(1.03);
        }


        /* =========================
           حالة الطبيب
        ========================= */

        .availability {
            position: absolute;
            top: 9px;
            right: 9px;

            display: flex;
            align-items: center;
            gap: 6px;

            padding: 5px 9px;
            border-radius: 20px;

            font-size: 11px;
            font-weight: 700;

            background: rgba(255,255,255,.95);
            box-shadow: 0 2px 8px rgba(0,0,0,.12);
        }

        .availability.available {
            color: #16833b;
        }

        .availability.unavailable {
            color: #888;
        }

        .status-dot {
            width: 8px;
            height: 8px;
            border-radius: 50%;
            background: #aaa;
        }

        .availability.available .status-dot {
            background: #20b957;
            box-shadow: 0 0 0 3px rgba(32,185,87,.15);
        }


        /* =========================
           بيانات الطبيب
        ========================= */

        .doctor-body {
            padding: 13px;
            text-align: center;
        }

        .doctor-body h3 {
            margin: 0 0 7px;
            font-size: 17px;
            font-weight: 700;
            color: #155d9e;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .doctor-specialty {
            display: inline-block;
            background: #eef6ff;
            color: #2875b7;
            padding: 4px 11px;
            border-radius: 15px;
            font-size: 12px;
            margin-bottom: 12px;
        }


        /* =========================
           زر الطبيب
        ========================= */

        .btn-select {
            width: 100%;
            border: 0;
            border-radius: 8px;
            padding: 8px 10px;
            background: #1475c9;
            color: #fff;
            font-size: 13px;
            font-weight: 700;
            cursor: pointer;
            transition: .2s;
        }

        .btn-select:hover {
            background: #0d5fa8;
        }


        /* =========================
           قسم المواعيد
        ========================= */

        .appointments-section {
            background: #fff;
            border: 1px solid #e5ebf1;
            border-radius: 13px;
            padding: 18px;
            box-shadow: 0 3px 12px rgba(0,0,0,.04);
        }

        .appointments-title {
            display: flex;
            align-items: center;
            gap: 10px;
            margin-bottom: 15px;
        }

        .calendar-icon {
            width: 42px;
            height: 42px;
            display: flex;
            align-items: center;
            justify-content: center;
            background: #eaf8ef;
            border-radius: 10px;
            font-size: 21px;
        }

        .appointments-title h3 {
            margin: 0;
            color: #168344;
            font-size: 18px;
            font-weight: 700;
        }

        .appointments-title span {
            font-size: 12px;
            color: #888;
        }


        /* =========================
           جدول المواعيد
        ========================= */

        .slots-table {
            width: 100%;
            border-collapse: separate;
            border-spacing: 0;
            overflow: hidden;
            border-radius: 8px;
            border: 1px solid #e3e8ed;
        }

        .slots-table th {
            background: #eef6ff;
            color: #155d9e;
            padding: 10px;
            font-size: 13px;
            border-bottom: 1px solid #dce7f1;
        }

        .slots-table td {
            padding: 9px;
            font-size: 13px;
            vertical-align: middle;
        }

        .slots-table tr:hover td {
            background: #f8fbff;
        }

        .slots-table a {
            display: inline-block;
            background: #198754;
            color: #fff !important;
            padding: 6px 12px;
            border-radius: 7px;
            text-decoration: none;
            font-size: 12px;
            font-weight: 700;
        }


        /* =========================
           الرسائل
        ========================= */

        .message-label {
            display: block;
            text-align: center;
            margin-top: 15px;
            font-weight: 700;
            font-size: 14px;
        }


        /* =========================
           الموبايل
        ========================= */

        @media (max-width: 600px) {

            .schedule-page {
                margin-top: 10px;
                padding: 0 10px 30px;
            }

            .doctors-section {
                grid-template-columns: repeat(2, 1fr);
                gap: 10px;
            }

            .doctor-image {
                height: 120px;
            }

            .doctor-body {
                padding: 10px;
            }

            .doctor-body h3 {
                font-size: 15px;
            }

            .doctor-specialty {
                font-size: 11px;
                padding: 3px 8px;
            }

            .btn-select {
                font-size: 12px;
                padding: 7px;
            }

            .availability {
                font-size: 9px;
                padding: 4px 7px;
            }

        }

    </style>


    <script src="assets/Newcss/aos.js"></script>

    <link href="assets/Newcss/aos.css" rel="stylesheet" />

    <script>
        AOS.init({
            duration: 500,
            once: true
        });
    </script>

</asp:Content>