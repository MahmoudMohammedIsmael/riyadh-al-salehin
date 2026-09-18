using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Xml;

namespace Riyadh_Al_Salehin
{
    public class InvoiceTemplateManager
    {
        private static string ConfigPath => HostingEnvironment.MapPath("~/App_Data/InvoiceTemplates.xml");

        public class TemplateSettings
        {
            public string Name { get; set; }
            public string File { get; set; }
            public string PaperSize { get; set; } = "80mm";
            public int FontSize { get; set; } = 12;
            public string PrintType { get; set; } = "Thermal";
            public bool ShowHeader { get; set; } = true;
            public bool ShowBarcode { get; set; } = true;
            public bool ShowFooter { get; set; } = true;
            public bool ShowQueue { get; set; } = true;
            public bool ShowPaymentStatus { get; set; } = true;
            public bool ShowAmountDetails { get; set; } = true;
            public string HeaderExtra { get; set; } = "";
            public string FooterExtra { get; set; } = "";
            public string Watermark { get; set; } = "";
            public List<CustomField> CustomFields { get; set; } = new List<CustomField>();
        }

        public class CustomField
        {
            public string Label { get; set; }
            public string Value { get; set; }
        }

        public static TemplateSettings GetTemplate(string templateName)
        {
            if (!File.Exists(ConfigPath))
                CreateDefaultConfig();

            var doc = new XmlDocument();
            doc.Load(ConfigPath);

            var node = doc.SelectSingleNode($"//Template[@Name='{templateName}']");
            if (node == null) return GetDefaultSettings(templateName);

            return ParseTemplate(node);
        }

        public static void SaveTemplate(TemplateSettings settings)
        {
            if (!File.Exists(ConfigPath))
                CreateDefaultConfig();

            var doc = new XmlDocument();
            doc.Load(ConfigPath);

            var node = doc.SelectSingleNode($"//Template[@Name='{settings.Name}']");
            if (node == null)
            {
                node = doc.CreateElement("Template");
                var attr = doc.CreateAttribute("Name");
                attr.Value = settings.Name;
                node.Attributes.Append(attr);
                doc.DocumentElement.AppendChild(node);
            }

            UpdateTemplateNode(doc, node, settings);
            doc.Save(ConfigPath);
        }

        public static List<TemplateSettings> GetAllTemplates()
        {
            var list = new List<TemplateSettings>();
            if (!File.Exists(ConfigPath))
                CreateDefaultConfig();

            var doc = new XmlDocument();
            doc.Load(ConfigPath);

            foreach (XmlNode node in doc.SelectNodes("//Template"))
            {
                list.Add(ParseTemplate(node));
            }

            return list;
        }

        private static void CreateDefaultConfig()
        {
            var dir = Path.GetDirectoryName(ConfigPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Templates>
  <Template Name=""Consultation"" File=""ConsultationInvoicePrint.aspx"">
    <PaperSize>80mm</PaperSize>
    <FontSize>12</FontSize>
    <PrintType>Thermal</PrintType>
    <ShowHeader>true</ShowHeader>
    <ShowBarcode>true</ShowBarcode>
    <ShowFooter>true</ShowFooter>
    <ShowQueue>true</ShowQueue>
    <ShowPaymentStatus>true</ShowPaymentStatus>
    <ShowAmountDetails>true</ShowAmountDetails>
    <HeaderExtra></HeaderExtra>
    <FooterExtra></FooterExtra>
    <Watermark></Watermark>
    <CustomFields />
  </Template>
  <Template Name=""Surgery"" File=""SurgeryInvoicePrint.aspx"">
    <PaperSize>80mm</PaperSize>
    <FontSize>12</FontSize>
    <PrintType>Thermal</PrintType>
    <ShowHeader>true</ShowHeader>
    <ShowBarcode>true</ShowBarcode>
    <ShowFooter>true</ShowFooter>
    <ShowQueue>true</ShowQueue>
    <ShowPaymentStatus>true</ShowPaymentStatus>
    <ShowAmountDetails>true</ShowAmountDetails>
    <HeaderExtra></HeaderExtra>
    <FooterExtra></FooterExtra>
    <Watermark></Watermark>
    <CustomFields />
  </Template>
  <Template Name=""Xray"" File=""XrayInvoicePrint.aspx"">
    <PaperSize>80mm</PaperSize>
    <FontSize>12</FontSize>
    <PrintType>Thermal</PrintType>
    <ShowHeader>true</ShowHeader>
    <ShowBarcode>true</ShowBarcode>
    <ShowFooter>true</ShowFooter>
    <ShowQueue>true</ShowQueue>
    <ShowPaymentStatus>true</ShowPaymentStatus>
    <ShowAmountDetails>true</ShowAmountDetails>
    <HeaderExtra></HeaderExtra>
    <FooterExtra></FooterExtra>
    <Watermark></Watermark>
    <CustomFields />
  </Template>
  <Template Name=""Additional"" File=""InvoiceItemsPrint.aspx"">
    <PaperSize>80mm</PaperSize>
    <FontSize>12</FontSize>
    <PrintType>Thermal</PrintType>
    <ShowHeader>true</ShowHeader>
    <ShowBarcode>true</ShowBarcode>
    <ShowFooter>true</ShowFooter>
    <ShowQueue>true</ShowQueue>
    <ShowPaymentStatus>true</ShowPaymentStatus>
    <ShowAmountDetails>true</ShowAmountDetails>
    <HeaderExtra></HeaderExtra>
    <FooterExtra></FooterExtra>
    <Watermark></Watermark>
    <CustomFields />
  </Template>
</Templates>";

            File.WriteAllText(ConfigPath, xml);
        }

        private static TemplateSettings GetDefaultSettings(string name)
        {
            return new TemplateSettings { Name = name };
        }

        private static TemplateSettings ParseTemplate(XmlNode node)
        {
            var s = new TemplateSettings
            {
                Name = node.Attributes["Name"]?.Value ?? "",
                File = node.Attributes["File"]?.Value ?? "",
                PaperSize = node.SelectSingleNode("PaperSize")?.InnerText ?? "80mm",
                FontSize = int.TryParse(node.SelectSingleNode("FontSize")?.InnerText, out int fs) ? fs : 12,
                PrintType = node.SelectSingleNode("PrintType")?.InnerText ?? "Thermal",
                ShowHeader = bool.TryParse(node.SelectSingleNode("ShowHeader")?.InnerText, out bool sh) && sh,
                ShowBarcode = bool.TryParse(node.SelectSingleNode("ShowBarcode")?.InnerText, out bool sb) && sb,
                ShowFooter = bool.TryParse(node.SelectSingleNode("ShowFooter")?.InnerText, out bool sf) && sf,
                ShowQueue = bool.TryParse(node.SelectSingleNode("ShowQueue")?.InnerText, out bool sq) && sq,
                ShowPaymentStatus = bool.TryParse(node.SelectSingleNode("ShowPaymentStatus")?.InnerText, out bool sps) && sps,
                ShowAmountDetails = bool.TryParse(node.SelectSingleNode("ShowAmountDetails")?.InnerText, out bool sad) && sad,
                HeaderExtra = node.SelectSingleNode("HeaderExtra")?.InnerText ?? "",
                FooterExtra = node.SelectSingleNode("FooterExtra")?.InnerText ?? "",
                Watermark = node.SelectSingleNode("Watermark")?.InnerText ?? ""
            };

            var cfNode = node.SelectSingleNode("CustomFields");
            if (cfNode != null)
            {
                foreach (XmlNode f in cfNode.SelectNodes("Field"))
                {
                    s.CustomFields.Add(new CustomField
                    {
                        Label = f.SelectSingleNode("Label")?.InnerText ?? "",
                        Value = f.SelectSingleNode("Value")?.InnerText ?? ""
                    });
                }
            }

            return s;
        }

        private static void UpdateTemplateNode(XmlDocument doc, XmlNode node, TemplateSettings s)
        {
            SetOrCreateNode(doc, node, "PaperSize", s.PaperSize);
            SetOrCreateNode(doc, node, "FontSize", s.FontSize.ToString());
            SetOrCreateNode(doc, node, "PrintType", s.PrintType);
            SetOrCreateNode(doc, node, "ShowHeader", s.ShowHeader.ToString().ToLower());
            SetOrCreateNode(doc, node, "ShowBarcode", s.ShowBarcode.ToString().ToLower());
            SetOrCreateNode(doc, node, "ShowFooter", s.ShowFooter.ToString().ToLower());
            SetOrCreateNode(doc, node, "ShowQueue", s.ShowQueue.ToString().ToLower());
            SetOrCreateNode(doc, node, "ShowPaymentStatus", s.ShowPaymentStatus.ToString().ToLower());
            SetOrCreateNode(doc, node, "ShowAmountDetails", s.ShowAmountDetails.ToString().ToLower());
            SetOrCreateNode(doc, node, "HeaderExtra", s.HeaderExtra);
            SetOrCreateNode(doc, node, "FooterExtra", s.FooterExtra);
            SetOrCreateNode(doc, node, "Watermark", s.Watermark);

            var cfNode = node.SelectSingleNode("CustomFields");
            if (cfNode == null)
            {
                cfNode = doc.CreateElement("CustomFields");
                node.AppendChild(cfNode);
            }
            cfNode.RemoveAll();

            foreach (var f in s.CustomFields)
            {
                var fNode = doc.CreateElement("Field");
                var lNode = doc.CreateElement("Label");
                lNode.InnerText = f.Label;
                var vNode = doc.CreateElement("Value");
                vNode.InnerText = f.Value;
                fNode.AppendChild(lNode);
                fNode.AppendChild(vNode);
                cfNode.AppendChild(fNode);
            }
        }

        private static void SetOrCreateNode(XmlDocument doc, XmlNode parent, string name, string value)
        {
            var node = parent.SelectSingleNode(name);
            if (node == null)
            {
                node = doc.CreateElement(name);
                parent.AppendChild(node);
            }
            node.InnerText = value;
        }
    }
}