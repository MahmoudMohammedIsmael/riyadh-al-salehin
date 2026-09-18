using System;
using System.Collections.Generic;

namespace Riyadh_Al_Salehin.App_Start
{
    public class WhatsAppBookingSession
    {
        public string Phone { get; set; }

        public int Step { get; set; }

        public int? PatientId { get; set; }

        public int? DoctorId { get; set; }

        public int? ScheduleId { get; set; }

        public int? AppointmentId { get; set; }

        public int? CenterId { get; set; }

        public int? InvoiceId { get; set; }

        public int QueueLookupStep { get; set; }
        public string QueuePatientName { get; set; }

        public int RegistrationStep { get; set; }

        public string RegName { get; set; }

        public string RegAddress { get; set; }

        public string RegDateOfBirth { get; set; }

        public decimal? ServiceTotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? ServiceId { get; set; }

        public string VisitType { get; set; }

        public bool IsBookingForOther { get; set; } = false;
        public string RegPhone { get; set; }

        public string PendingAISpecialty { get; set; }
        public string PendingAIDoctorName { get; set; }
        public string PendingAIDate { get; set; }

        public string CurrentIntent { get; set; }
        public string PendingAITime { get; set; }

        public string ServiceName { get; set; }

        public string PatientTarget { get; set; }
        public string PatientName { get; set; }

        public List<string> AIConversation { get; set; }

        public WhatsAppBookingSession()
        {
            Step = 0;
            RegistrationStep = 0;
            CreatedAt = DateTime.Now;
            AIConversation = new List<string>();
        }
    }
}