﻿//BSD, 2014-present, WinterDev 

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    public static class BoxHitUtils
    {
        public static bool HitTest(CssBox box, float x, float y, CssBoxHitChain hitChain)
        {

#if DEBUG
            //if (box.ViewportY != 0 && hitChain.debugEventPhase == CssBoxHitChain.dbugEventPhase.MouseDown)
            //{
            //}
#endif
            //--------------------------------------
            bool isPointInArea = box.IsPointInArea(x, y);
            float boxHitLocalX = x - box.LocalX;  //**
            float boxHitLocalY = y - box.LocalY; //**
            //---------------------------------------------------------------------- 
            if (isPointInArea)
            {
                hitChain.AddHit(box, (int)boxHitLocalX, (int)boxHitLocalY);
            }

            //---------------------------------------------------------------------- 
            //enter children space -> offset with its viewport
            boxHitLocalX += box.ViewportX;
            boxHitLocalY += box.ViewportY;


            //check absolute layer first ***
            if (box.HasAbsoluteLayer)
            {
                hitChain.PushContextBox(box);
                foreach (CssBox absBox in box.GetAbsoluteChildBoxBackwardIter())
                {
                    if (HitTest(absBox, boxHitLocalX, boxHitLocalY, hitChain))
                    {
                        //found hit 
                        hitChain.PopContextBox(box);
                        return true;
                    }
                }
                hitChain.PopContextBox(box);
            }
            //----------------------------------------------------------------------
            if (!isPointInArea)
            {
                switch (box.CssDisplay)
                {
                    case Css.CssDisplay.TableRow:
                        {
                            foreach (CssBox childBox in box.GetChildBoxIter())
                            {
                                if (HitTest(childBox, boxHitLocalX, boxHitLocalY, hitChain))
                                {
                                    return true;
                                }
                            }
                        }
                        break;
                }
                //exit 
                return false;
            }
            //----------------------------------------------------------------------
            //at here point is in the area***  
            hitChain.PushContextBox(box);
            if (box.IsCustomCssBox)
            {
                //custom css box
                //return true= stop here
                if (box.CustomContentHitTest(boxHitLocalX, boxHitLocalY, hitChain))
                {
                    hitChain.PopContextBox(box);
                    return true;
                }
            }

            if (box.LineBoxCount > 0)
            {

                bool foundSomeLine = false;
                foreach (CssLineBox lineBox in box.GetLineBoxIter())
                {
                    //line box not overlap
                    if (lineBox.HitTest(boxHitLocalX, boxHitLocalY))
                    {
                        foundSomeLine = true;
                        float lineBoxLocalY = boxHitLocalY - lineBox.CachedLineTop;
                        //2.
                        hitChain.AddHit(lineBox, (int)boxHitLocalX, (int)lineBoxLocalY);


                        CssRun foundRun = BoxHitUtils.GetCssRunOnLocation(lineBox, (int)boxHitLocalX, (int)lineBoxLocalY);
                        //CssRun foundRun = BoxHitUtils.GetCssRunOnLocation(lineBox, (int)x, (int)y);
                        if (foundRun != null)
                        {
                            //3.
                            hitChain.AddHit(foundRun, (int)(boxHitLocalX - foundRun.Left), (int)lineBoxLocalY);
                            //4. go deeper for block run
                            if (foundRun.Kind == CssRunKind.BlockRun)
                            {
                                CssBlockRun blockRun = (CssBlockRun)foundRun;
                                CssLineBox hostLine = blockRun.HostLine;
                                //adjust with hostline 
                                HitTest(blockRun.ContentBox,
                                    (int)(boxHitLocalX + blockRun.ContentBox.LocalX - foundRun.Left),
                                    (int)(boxHitLocalY - hostLine.CachedLineTop),
                                    hitChain);
                            }
                        }
                        //found line box
                        hitChain.PopContextBox(box);
                        return true;
                    }
                    else if (foundSomeLine)
                    {
                        return false;
                    }
                }
            }
            else
            {
                //iterate in child 
                //foreach (var childBox in box.GetChildBoxIter())
                foreach (CssBox childBox in box.GetChildBoxBackwardIter())
                {
                    if (HitTest(childBox, boxHitLocalX, boxHitLocalY, hitChain))
                    {
                        //recursive
                        hitChain.PopContextBox(box);
                        return true;
                    }
                }
            }
            hitChain.PopContextBox(box);
            return true;
        }
        internal static CssBox GetNextSibling(CssBox a)
        {
            return a.GetNextNode();
        }
        internal static CssLineBox GetNearestLine(CssBox a, Point point, out bool found)
        {
            if (a.LineBoxCount > 0)
            {
                CssLineBox latestLine = null;
                int y = point.Y;
                found = false;
                foreach (CssLineBox linebox in a.GetLineBoxIter())
                {
                    if (linebox.CachedLineBottom < y)
                    {
                        latestLine = linebox;
                    }
                    else
                    {
                        if (latestLine != null)
                        {
                            found = true;
                        }
                        break;
                    }
                }

                return latestLine;
            }
            else
            {
                bool foundExact = false;
                CssLineBox lastLine = null;
                foreach (CssBox cbox in a.GetChildBoxIter())
                {
                    CssLineBox candidateLine = GetNearestLine(cbox, point, out foundExact);
                    if (candidateLine != null)
                    {
                        if (foundExact)
                        {
                            found = true;
                            return lastLine;
                        }
                        //not exact
                        lastLine = candidateLine;
                    }
                    else
                    {
                        if (lastLine != null)
                        {
                            found = false;
                            return lastLine;
                        }
                    }
                }
                found = foundExact;
            }
            return null;
        }


        internal static IEnumerable<CssLineBox> GetDeepDownLineBoxIter(CssBox box)
        {
            if (box.LineBoxCount > 0)
            {
                foreach (CssLineBox linebox in box.GetLineBoxIter())
                {
                    yield return linebox;
                }
            }
            else
            {
                if (box.ChildCount > 0)
                {
                    foreach (CssBox child in box.GetChildBoxIter())
                    {
                        foreach (CssLineBox linebox in GetDeepDownLineBoxIter(child))
                        {
                            yield return linebox;
                        }
                    }
                }
            }
        }


        internal static CssRun GetCssRunOnLocation(CssLineBox lineBox, int x, int y)
        {
            foreach (CssRun word in lineBox.GetRunIter())
            {
                // add word spacing to word width so sentance won't have hols in it when moving the mouse
                RectangleF rect = word.Rectangle;
                //rect.Width += word.OwnerBox.ActualWordSpacing;
                if (rect.Contains(x, y))
                {
                    return word;
                }
            }
            return null;
        }
        /// <summary>
        /// Gets the containing block-box of this box. (The nearest parent box with display=block)
        /// </summary>
        internal static CssBox SearchUpForContainingBlockBox(CssBox startBox)
        {
            if (startBox.ParentBox == null)
            {
                return startBox; //This is the initial containing block.
            }

            CssBox box = startBox.ParentBox;
            while (box.HasContainerProperty && box.ParentBox != null)
            {
                box = box.ParentBox;
            }

            //Comment this following line to treat always superior box as block
            if (box == null)
                throw new Exception("There's no containing block on the chain");
            return box;
        }
    }
}