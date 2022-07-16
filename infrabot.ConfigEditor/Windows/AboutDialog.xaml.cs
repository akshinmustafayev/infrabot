﻿using HtmlAgilityPack;
using infrabot.ConfigEditor.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace infrabot.ConfigEditor.Windows
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutDialog"/> class.
        /// </summary>
        public AboutDialog()
        {
            InitializeComponent();
            LoadDataInfoIntoTheForm();
        }

        public void LoadDataInfoIntoTheForm()
        {
            AboutTitle.Content = "infrabot Config Editor " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            AboutInfo.Content = "Author: Akshin Mustafayev";
            string readme = CommonUtils.ReadLicenseFile();
            RichTextBox1.Document.Blocks.Clear();
            var plainText = ConvertToPlainText(readme);
            RichTextBox1.AppendText(plainText);
        }

        private static void ConvertContentTo(HtmlNode node, TextWriter outText)
        {
            foreach (HtmlNode subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText);
            }
        }

        /// <summary>
        /// Converts to plain text.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns></returns>
        public static string ConvertToPlainText(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            StringWriter sw = new StringWriter();
            ConvertTo(doc.DocumentNode, sw);
            sw.Flush();
            return sw.ToString();
        }

        private static void ConvertTo(HtmlNode node, TextWriter outText)
        {
            string html;
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // don't output comments
                    break;

                case HtmlNodeType.Document:
                    ConvertContentTo(node, outText);
                    break;

                case HtmlNodeType.Text:
                    // script and style must not be output
                    string parentName = node.ParentNode.Name;
                    if ((parentName == "script") || (parentName == "style"))
                        break;

                    // get text
                    html = ((HtmlTextNode)node).Text;

                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                        break;

                    // check the text is meaningful and not a bunch of whitespaces
                    if (html.Trim().Length > 0)
                    {
                        outText.Write(HtmlEntity.DeEntitize(html));
                    }
                    break;

                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        case "p":
                            // treat paragraphs as crlf
                            outText.Write("\r\n");
                            break;
                        case "br":
                            outText.Write("\r\n");
                            break;
                    }

                    if (node.HasChildNodes)
                    {
                        ConvertContentTo(node, outText);
                    }
                    break;
            }

        }

        private void GetInspirationButton_Click(object sender, RoutedEventArgs e)
        {
            CommonUtils.OpenLinkInBrowser("https://www.youtube.com/watch?v=l0U7SxXHkPY");
        }

        private void CheckNewReleasesButton_Click(object sender, RoutedEventArgs e)
        {
            CommonUtils.OpenLinkInBrowser("https://github.com/infrabot-io/infrabot/releases");
        }

        private void GithubImage_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CommonUtils.OpenLinkInBrowser("https://github.com/infrabot-io/infrabot");
        }
    }
}
