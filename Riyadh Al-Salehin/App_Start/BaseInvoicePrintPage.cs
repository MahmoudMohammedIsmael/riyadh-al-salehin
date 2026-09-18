using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.Common;
using WebImage = System.Web.UI.WebControls.Image;

namespace Riyadh_Al_Salehin
{
    public class BaseInvoicePrintPage : Page
    {
        protected InvoiceTemplateManager.TemplateSettings TemplateSettings { get; private set; }
        protected string TemplateName { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!string.IsNullOrEmpty(TemplateName))
            {
                TemplateSettings = InvoiceTemplateManager.GetTemplate(TemplateName);
            }
        }

        /// <summary>
        /// تطبيق إعدادات القالب على الصفحة (CSS، إظهار/إخفاء عناصر)
        /// </summary>
        protected void ApplyTemplateSettings()
        {
            if (TemplateSettings == null) return;

            ApplyDynamicStyles();

            ToggleControlsVisibility();

            UpdateExtraTexts();
        }

        private void ApplyDynamicStyles()
        {
            var style = new HtmlGenericControl("style");
            style.InnerHtml = GenerateDynamicCss();
            Page.Header.Controls.Add(style);
        }

        private string GenerateDynamicCss()
        {
            var css = "";

            var width = TemplateSettings.PaperSize == "58mm" ? "58mm" :
                        TemplateSettings.PaperSize == "A4" ? "190mm" : "80mm";

            css += $@"
                html, body {{ width: {width}; }}
                .invoice-box {{ width: {width}; max-width: {width}; }}
                .invoice-box * {{ font-size: {TemplateSettings.FontSize}px !important; }}
                .invoice-title h2 {{ font-size: {TemplateSettings.FontSize + 4}px !important; }}
                .inv-queue-big {{ font-size: {Math.Max(24, TemplateSettings.FontSize * 2)}px !important; }}
            ";

            if (TemplateSettings.PrintType == "Thermal")
            {
                css += "@page { size: " + TemplateSettings.PaperSize + " auto; margin: 0; }";
            }
            else
            {
                css += "@page { size: A4; margin: 10mm; }";
            }

            return css;
        }

        private void ToggleControlsVisibility()
        {
            var controls = new Dictionary<string, Action<bool>>
            {
                { "Header", v => ToggleControl("invoiceTitle", v) },
                { "Barcode", v => ToggleControl("barcodeArea", v) },
                { "Footer", v => ToggleControl("footerNote", v) },
                { "Queue", v => ToggleControl("queueArea", v) },
                { "PaymentStatus", v => ToggleControl("paymentStatusRow", v) },
                { "AmountDetails", v => ToggleControl("amountDetails", v) }
            };

            foreach (var kv in controls)
            {
                var prop = TemplateSettings.GetType().GetProperty("Show" + kv.Key);
                if (prop != null)
                {
                    bool val = (bool)prop.GetValue(TemplateSettings);
                    kv.Value(val);
                }
            }
        }

        private void ToggleControl(string controlId, bool visible)
        {
            var ctrl = FindControlRecursive(this, controlId);
            if (ctrl != null) ctrl.Visible = visible;
        }

        private void UpdateExtraTexts()
        {
            var headerExtra = FindControlRecursive(this, "headerExtraText") as Label;
            if (headerExtra != null)
            {
                headerExtra.Text = TemplateSettings.HeaderExtra;
                headerExtra.Visible = !string.IsNullOrEmpty(TemplateSettings.HeaderExtra);
            }

            var footerExtra = FindControlRecursive(this, "footerExtraText") as Label;
            if (footerExtra != null)
            {
                footerExtra.Text = TemplateSettings.FooterExtra;
                footerExtra.Visible = !string.IsNullOrEmpty(TemplateSettings.FooterExtra);
            }

            var watermark = FindControlRecursive(this, "watermarkText") as Label;
            if (watermark != null)
            {
                watermark.Text = TemplateSettings.Watermark;
                watermark.Visible = !string.IsNullOrEmpty(TemplateSettings.Watermark);
            }
        }

        /// <summary>
        /// توليد باركود Base64
        /// </summary>
        protected string GenerateBarcodeBase64(string text, int width = 300, int height = 80)
        {
            if (string.IsNullOrEmpty(text)) return null;

            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = width,
                    Height = height,
                    Margin = 2
                }
            };

            using (var bmp = writer.Write(text))
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        /// <summary>
        /// تعيين صورة الباركود في عنصر Image
        /// </summary>
        protected void SetBarcodeImage(WebImage imgCtrl, string text)
        {
            var base64 = GenerateBarcodeBase64(text);
            if (!string.IsNullOrEmpty(base64))
            {
                imgCtrl.ImageUrl = "data:image/png;base64," + base64;
                imgCtrl.Visible = true;
            }
            else
            {
                imgCtrl.Visible = false;
            }
        }

        private Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id) return root;
            foreach (Control c in root.Controls)
            {
                var found = FindControlRecursive(c, id);
                if (found != null) return found;
            }
            return null;
        }
    }
}