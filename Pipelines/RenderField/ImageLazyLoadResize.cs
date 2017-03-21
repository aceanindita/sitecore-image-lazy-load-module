using HtmlAgilityPack;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.RenderField;
using System;

namespace ImageLazyLoadModule.Pipelines.RenderField
{
    public class ImageLazyLoad
    {
        public void Process(RenderFieldArgs args)
        {
            try
            {
                if (args == null)
                    return;

                // Trigger the code to transform the img tags only for rich text and image fields
                if (!(args.FieldTypeKey != "rich text" || args.FieldTypeKey != "image") || string.IsNullOrEmpty(args.FieldValue) ||
                     !Context.PageMode.IsNormal)
                    return;

                if (!string.IsNullOrWhiteSpace(args.Result?.FirstPart))
                {
                    HtmlDocument doc = new HtmlDocument { OptionWriteEmptyNodes = true };
                    doc.LoadHtml(args.Result.FirstPart);

                    if (doc.DocumentNode != null)
                    {
                        // Search for all img tags
                        HtmlNodeCollection imgTag = doc.DocumentNode.SelectNodes("//img");
                        if (imgTag == null || imgTag.Count == 0)
                            return;

                        foreach (HtmlNode node in imgTag)
                        {
                            if (node.Attributes["src"] != null && node.ParentNode != null)
                            {
                                string imgUrl = node.Attributes["src"].Value;
                                node.Attributes.Add("responsive-src", imgUrl);
                                node.Attributes["src"].Value = "data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==";
                                string currentClass = node.Attributes["class"] != null ? node.Attributes["class"].Value : "";
                                node.Attributes.Remove("class");
                                node.Attributes.Add("class", (string.IsNullOrWhiteSpace(currentClass) ? "" : currentClass + " ") 
                                    + Settings.GetSetting("ImageLazyLoadModule.Selector", "b-lazy"));
                                node.Attributes.Remove("width");
                                node.Attributes.Remove("height");
                            }
                        }

                        args.Result.FirstPart = doc.DocumentNode.OuterHtml;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in ImageLazyLoadModule.ImageLazyLoad:" + ex.Message, ex);
            }
        }
    }
}