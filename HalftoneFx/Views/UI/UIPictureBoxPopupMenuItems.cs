namespace HalftoneFx.UI
{
    using KWUI.Controls;

    using System.Collections.Generic;

    public static class UIPictureBoxPopupMenuItems
    {
        public static IEnumerable<UIPopupMenuItem> Get(UIPictureBox pictureBox)
        {
            yield return new UIPopupMenuItem
            {
                Text = "ZOOM IN",
                ButtonMode = true,
                Icon = Properties.Resources.IconZoomIn,
                Click = () => pictureBox?.ZoomIn(),
            };

            yield return new UIPopupMenuItem
            {
                Text = "ZOOM OUT",
                ButtonMode = true,
                Icon = Properties.Resources.IconZoomOut,
                Click = () => pictureBox?.ZoomOut(),
            };

            yield return new UIPopupMenuItem
            {
                Text = "OPTIMAL VIEW",
                ButtonMode = false,
                Icon = Properties.Resources.IconZoomToExtents,
                Click = () => pictureBox?.OptimalView(),
            };

            yield return new UIPopupMenuItem
            { 
                Text = "FULL VIEW",
                ButtonMode = false,
                Icon = Properties.Resources.IconFullView,
                Click = () => pictureBox?.FullView(),
            };
        }
    }
}
