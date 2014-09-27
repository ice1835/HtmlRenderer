﻿using System;
using System.Collections.Generic;

using System.Text;
using System.Diagnostics;

using HtmlRenderer.Boxes;
using HtmlRenderer.WebDom;
using LayoutFarm.Drawing;
using HtmlRenderer.ContentManagers;
using HtmlRenderer.Diagnostics;

namespace HtmlRenderer
{
    static class WinHtmlRootVisualBoxExtension
    {
        
     
        public static void RefreshHtmlDomChange(this MyHtmlIsland htmlIsland,
            HtmlRenderer.WebDom.WebDocument doc, CssActiveSheet cssData)
        {

            PartialRebuildCssTree(htmlIsland, doc);
        }
        //static void FullRebuildCssTree(MyHtmlIsland htmlIsland,
        //    HtmlRenderer.WebDom.WebDocument doc,
        //    CssActiveSheet cssData)
        //{
        //    HtmlRenderer.Composers.BoxModelBuilder builder = new Composers.BoxModelBuilder();
        //    builder.RequestStyleSheet += (e) =>
        //    {
        //        var textContentManager = htmlIsland.TextContentMan;
        //        if (textContentManager != null)
        //        {
        //            textContentManager.AddStyleSheetRequest(e);
        //        }
        //    };


        //    var rootBox = builder.BuildCssTree(doc, CurrentGraphicPlatform.P.SampleIGraphics, htmlIsland, cssData);
        //    htmlIsland.SetHtmlDoc(doc);
        //    htmlIsland.SetRootCssBox(rootBox, cssData);

        //}
        static void PartialRebuildCssTree(MyHtmlIsland htmlIsland,
            HtmlRenderer.WebDom.WebDocument doc)
        {
            HtmlRenderer.Composers.BoxModelBuilder builder = new Composers.BoxModelBuilder();
            builder.RequestStyleSheet += (e) =>
            {
                //var textContentManager = htmlIsland.TextContentMan;
                //if (textContentManager != null)
                //{
                //    textContentManager.AddStyleSheetRequest(e);
                //}
            };


            var rootBox2 = builder.RefreshCssTree(doc, CurrentGraphicPlatform.P.SampleIGraphics, htmlIsland);
        }

        
    }

}