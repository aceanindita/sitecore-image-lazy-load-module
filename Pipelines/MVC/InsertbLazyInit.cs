using Sitecore.Configuration;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Mvc.Presentation;

namespace ImageLazyLoadModule.Pipelines.MVC
{
    public class InsertbLazyInit : RenderRenderingProcessor
    {
        public override void Process(RenderRenderingArgs args)
        {
            Renderer renderer = args.Rendering.Renderer;
            if (renderer == null)
                return;

            bool isLayout = renderer is ViewRenderer &&
                               ((ViewRenderer)renderer).Rendering.RenderingType == "Layout";

            if (isLayout)
            {
                args.Writer.Write("<script src=\"../../sitecore modules/Lazy Load/blazy.min.js\"></script>"
                                  + "<script>"
                                  + "var bLazy = new Blazy({ selector: '."
                                  + Settings.GetSetting("ImageLazyLoadModule.Selector", "b-lazy")
                                  + "', offset: " 
                                  + Settings.GetIntSetting("ImageLazyLoadModule.Offset", 200)
                                  + ", src: 'responsive-src'});"
                                  + "</script>");
            }
        }
    }
}