﻿//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace LayoutFarm.Presentation
{


    public interface IArtSurfaceViewportControl
    {

        IntPtr Handle { get; }
        void viewport_HScrollChanged(object sender, ArtScrollEventArgs e);
        void viewport_HScrollRequest(object sender, ScrollSurfaceRequestEventArgs e);
        void viewport_VScrollChanged(object sender, ArtScrollEventArgs e);
        void viewport_VScrollRequest(object sender, ScrollSurfaceRequestEventArgs e);
        int WindowWidth { get; }
        int WindowHeight { get; }
        void Invoke(Delegate del, object req);
        void PaintMe();
        void WhenParentFormClosed(EventHandler<EventArgs> handler);
        void SetupWindowRoot(ArtVisualWindowImpl winroot);

#if DEBUG
        List<dbugLayoutMsg> dbug_rootDocDebugMsgs { get; }
        void dbug_InvokeVisualRootDrawMsg();
        List<dbugLayoutMsg> dbug_rootDocHitChainMsgs { get; }
        void dbug_InvokeHitChainMsg();
        void dbug_HighlightMeNow(Rectangle r);
#endif
    }
}