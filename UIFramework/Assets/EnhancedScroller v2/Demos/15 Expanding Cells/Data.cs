namespace EnhancedScrollerDemos.ExpandingCells
{
    /// <summary>
    /// Super simple data class to hold information for each cell.
    /// </summary>
    public class Data
    {
        public bool isExpanded;

        public string headerText;

        public string descriptionText;

        public float collapsedSize;

        public float expandedSize;

        public EnhancedUI.EnhancedScroller.Tween.TweenType tweenType;

        public float tweenTimeExpand;

        public float tweenTimeCollapse;
    }
}