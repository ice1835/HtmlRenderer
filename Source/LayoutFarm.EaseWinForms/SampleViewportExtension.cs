﻿//Apache2, 2014-2016, WinterDev

using LayoutFarm.UI;
namespace LayoutFarm
{
    static class SampleViewportExtension
    {
        public static void AddContent(this SampleViewport viewport, UIElement ui)
        {
            viewport.ViewportControl.AddContent(
                ui.GetPrimaryRenderElement(viewport.ViewportControl.RootGfx),
                ui);
        }
    }
}