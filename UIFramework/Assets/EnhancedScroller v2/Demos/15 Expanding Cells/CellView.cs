using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using System;

namespace EnhancedScrollerDemos.ExpandingCells
{
    /// <summary>
    /// This is the view of our cell which handles how the cell looks.
    /// </summary>
    public class CellView : EnhancedScrollerCellView
    {
        private Tween tween;
        private LayoutElement layoutElement;

        private Data data;

        public Text dataIndexText;
        public Text headerText;
        public Text descriptionText;
        public Action<int, bool> startTween;
        public Action<float, float> updateTween;
        public Action<int> endTween;

        private void Start()
        {
            tween = GetComponent<Tween>();
            layoutElement = GetComponent<LayoutElement>();
        }

        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(Data data, int dataIndex, float collapsedSize, float expandedSize, Action<int, bool> startTween, Action<float, float> updateTween, Action<int> endTween)
        {
            this.dataIndex = dataIndex;
            this.startTween = startTween;
            this.updateTween = updateTween;
            this.endTween = endTween;

            dataIndexText.text = dataIndex.ToString();
            headerText.text = data.headerText;
            descriptionText.text = data.descriptionText;

            descriptionText.enabled = data.isExpanded;

            this.data = data;
        }

        public void CellButton_Clicked()
        {
            if (startTween != null)
            {
                startTween(dataIndex, !data.isExpanded);
            }

            if (data.isExpanded)
            {
                // collapse cell view
                descriptionText.enabled = false;
                StartCoroutine(tween.TweenPosition(data.tweenType, data.tweenTimeCollapse, data.expandedSize, data.collapsedSize, TweenUpdated, TweenCompleted));
            }
            else
            {
                // expand cell view
                StartCoroutine(tween.TweenPosition(data.tweenType, data.tweenTimeExpand, data.collapsedSize, data.expandedSize, TweenUpdated, TweenCompleted));
            }
        }

        private void TweenUpdated(float newValue, float delta)
        {
            // update the size of the cell view
            layoutElement.minHeight = newValue;

            if (updateTween != null)
            {
                // call the update tween on the controller
                // in order to update the last padder
                updateTween(newValue, delta);
            }
        }

        private void TweenCompleted()
        {
            if (endTween != null)
            {
                // tween is completed, so now we can reload the scroller
                endTween(dataIndex);
            }
        }
    }
}
