using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;
using ZXing.Common;

namespace Riyadh_Al_Salehin
{
    /// <summary>
    /// كلاس أساسي لصفحات طباعة الفواتير يوفر وظائف مشتركة
    /// </summary>
    public class BaseInvoicePrintPage : System.Web.UI.Page
    {
        /// <summary>
        /// اسم القالب المستخدم (Consultation, Surgery, Xray, Items)
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// تطبيق إعدادات القالب (يمكن التجاوز في الصفحات المشتقة)
        /// </summary>
        protected virtual void ApplyTemplateSettings()
        {
        }

        /// <summary>
        /// إنشاء باركود Base64 من نص
        /// </summary>
        protected string GenerateBarcodeBase64(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            try
            {
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions
                    {
                        Width = 300,
                        Height = 80,
                        Margin = 2
                    }
                };

                using (Bitmap bmp = writer.Write(text))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Png);
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// تعيين صورة الباركود في عنصر Image
        /// </summary>
        protected void SetBarcodeImage(Image imgControl, string text)
        {
            if (imgControl == null || string.IsNullOrEmpty(text))
            {
                if (imgControl != null)
                    imgControl.Visible = false;
                return;
            }

            string base64 = GenerateBarcodeBase64(text);
            if (!string.IsNullOrEmpty(base64))
            {
                imgControl.ImageUrl = "data:image/png;base64," + base64;
                imgControl.Visible = true;
            }
            else
            {
                imgControl.Visible = false;
            }
        }

        /// <summary>
        /// تعيين صورة الباركود في عنصر Image مع عرض النص
        /// </summary>
        protected void SetBarcodeImage(Image imgControl, Label lblBarcodeText, string text)
        {
            SetBarcodeImage(imgControl, text);
            if (lblBarcodeText != null)
            {
                lblBarcodeText.Text = text;
                lblBarcodeText.Visible = !string.IsNullOrEmpty(text);
            }
        }
    }
}