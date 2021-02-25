using UnityEngine;
using System.Collections;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.ExpandingCells
{
    /// <summary>
    /// This example shows how you can expand and collapse cells
    /// </summary>
	public class Controller : MonoBehaviour, IEnhancedScrollerDelegate
    {

        /// <summary>
        /// Internal representation of our data. Note that the scroller will never see
        /// this, so it separates the data from the layout using MVC principles.
        /// </summary>
        private SmallList<Data> _data;

        /// <summary>
        /// Whether the first or last padder is being adjusted to fit the cell size change
        /// </summary>
        private bool _tweeningFirstPadder;

        /// <summary>
        /// This is our scroller we will be a delegate for
        /// </summary>
        public EnhancedScroller scroller;


        /// <summary>
        /// This will be the prefab of each cell in our scroller. Note that you can use more
        /// than one kind of cell, but this example just has the one type.
        /// </summary>
        public EnhancedScrollerCellView cellViewPrefab;

        /// <summary>
        /// Be sure to set up your references to the scroller after the Awake function. The
        /// scroller does some internal configuration in its own Awake function. If you need to
        /// do this in the Awake function, you can set up the script order through the Unity editor.
        /// In this case, be sure to set the EnhancedScroller's script before your delegate.
        ///
        /// In this example, we are calling our initializations in the delegate's Start function,
        /// but it could have been done later, perhaps in the Update function.
        /// </summary>
        void Start()
        {
            // tell the scroller that this script will be its delegate
            scroller.Delegate = this;

            // load in a large set of data
            LoadData();
        }


        /// <summary>
        /// Populates the data with a lot of records
        /// </summary>
        private void LoadData()
        {
            // set up some simple data
            _data = new SmallList<Data>();
            for (var i = 0; i < 50; i++)
            {
                if (i % 2 == 0)
                {
                    _data.Add(new Data()
                    {
                        headerText = "Multiple Expand",
                        descriptionText = "Expanding this cell will not collapse other cells. This allows you to have multiple cells expanded at once.\n\nClick the cell again to collapse.",
                        isExpanded = false,
                        expandedSize = 200f,
                        collapsedSize = 20f * ((i % 3) + 3),
                        tweenType = Tween.TweenType.immediate,
                        tweenTimeExpand = 0,
                        tweenTimeCollapse = 0
                    });
                }
                else if (i % 3 == 0)
                {
                    _data.Add(new Data()
                    {
                        headerText = "Tween Expand",
                        descriptionText = "This cell will animate its size when clicked.\n\nClick the cell again to collapse.",
                        isExpanded = false,
                        expandedSize = 200f,
                        collapsedSize = 20f * ((i % 3) + 3),
                        tweenType = Tween.TweenType.easeInOutSine,
                        tweenTimeExpand = 0.5f,
                        tweenTimeCollapse = 0.5f
                    });
                }
                else
                {
                    _data.Add(new Data()
                    {
                        headerText = "Single Expand",
                        descriptionText = "Expanding this cell will collapse other cells.\n\nClick the cell again to collapse.",
                        isExpanded = false,
                        expandedSize = 200f,
                        collapsedSize = 30f * ((i % 3) + 3),
                        tweenType = Tween.TweenType.immediate,
                        tweenTimeExpand = 0,
                        tweenTimeCollapse = 0
                    });
                }
            }

            // tell the scroller to reload now that we have the data
            scroller.ReloadData();
        }

        private void TweenStart(int dataIndex, bool isExpanding)
        {
            // reset the flag for if the first padder is being tweened to accommodate the cell view size change
            _tweeningFirstPadder = false;
        }

        private void TweenUpdated(float newValue, float delta)
        {
            if (delta < 0)
            {
                // only adjust padders when collapsing

                if (scroller.LastPadder.IsActive())
                {
                    scroller.LastPadder.minHeight -= delta;
                }
                else
                {
                    // adjusting the first padder due to the scroller being
                    // near the bottom
                    _tweeningFirstPadder = true;
                    scroller.FirstPadder.minHeight -= delta;
                }
            }
        }

        /// <summary>
        /// This is called when a cell is clicked
        /// </summary>
        /// <param name="dataIndex"></param>
        private void TweenEnd(int dataIndex)
        {
            // toggle the expanded value
            _data[dataIndex].isExpanded = !_data[dataIndex].isExpanded;

            if (dataIndex % 2 == 1)
            {
                // single expanded cell. collapse others
                for (var i = 0; i < _data.Count; i++)
                {
                    if (i != dataIndex)
                    {
                        _data[i].isExpanded = false;
                    }
                }
            }

            // Since the size of the scroller will be changing, we can't simply pass in the normalized
            // scroll position to the ReloadData method. Instead we will do some math to get the current offset
            // of the first or last visible cell and use that in the JumpToDataIndex call after the ReloadData call.
            // This can accommodate any changes in the sizes of multiple cell views.

            var jumpDataIndex = 0;
            var jumpPosition = 0f;
            var jumpCellSize = 0f;
            var cellOffset = 0f;

            if (_tweeningFirstPadder)
            {
                // get the end data index so that we can jump to this after the reload
                jumpDataIndex = scroller.EndDataIndex;
                // the actual start position of this end data index so we can calculate a cell offset when we jump
                jumpPosition = scroller.GetScrollPositionForDataIndex(jumpDataIndex, EnhancedScroller.CellViewPositionEnum.Before);
                // get the size of the cell at the end data index
                jumpCellSize = _data[jumpDataIndex].isExpanded ? _data[jumpDataIndex].expandedSize : _data[jumpDataIndex].collapsedSize;
                // get the cell offset by taking the difference of the bottom of the scroller and cell view start position (minus the bottom padding) and dividing it by the size of the cell
                cellOffset = (scroller.ScrollPosition + scroller.ScrollRectSize - jumpPosition - scroller.padding.bottom) / jumpCellSize;
            }
            else
            {
                // get the start data index so that we can jump to this after the reload
                jumpDataIndex = scroller.StartDataIndex;
                // the actual start position of this start data index so we can calculate a cell offset when we jump
                jumpPosition = scroller.GetScrollPositionForDataIndex(jumpDataIndex, EnhancedScroller.CellViewPositionEnum.Before);
                // get the size of the cell at the start data index
                jumpCellSize = _data[jumpDataIndex].isExpanded ? _data[jumpDataIndex].expandedSize : _data[jumpDataIndex].collapsedSize;
                // get the cell offset by taking the difference of the scroll position and cell view start position and dividing it by the size of the cell
                cellOffset = (scroller.ScrollPosition - jumpPosition) / jumpCellSize;
            }

            // if we are expanding the cell view, add a look ahead to the beginning and end of the
            // scroller so that it loads in extra cells. This is necessary because when we
            // start to collapse, we will need these extra cells so that the others do
            // not get stretched by the layout element of the ScrollRect container.
            scroller.lookAheadBefore = _data[dataIndex].isExpanded ? 200.0f : 0.0f;
            scroller.lookAheadAfter = _data[dataIndex].isExpanded ? 200.0f : 0.0f;

            // reload the scroller to set the new positions and sizes
            scroller.ReloadData();

            if (_tweeningFirstPadder)
            {
                // jump back to the original end data index that we cached above to make it
                // appear the scroller did not reset.
                // we use the cell offset calculated above and ignore the spacing. scroller offset is at the bottom
                scroller.JumpToDataIndex(jumpDataIndex, scrollerOffset: 1f, cellOffset: cellOffset, useSpacing: false);
            }
            else
            {
                // jump back to the original start data index that we cached above to make it
                // appear the scroller did not reset.
                // we use the cell offset calculated above and ignore the spacing
                scroller.JumpToDataIndex(jumpDataIndex, scrollerOffset: 0f, cellOffset: cellOffset, useSpacing: false);
            }
        }

        #region EnhancedScroller Handlers

        /// <summary>
        /// This tells the scroller the number of cells that should have room allocated.
        /// For this example, the count is the number of data elements divided by the number of cells per row (rounded up using Mathf.CeilToInt)
        /// </summary>
        /// <param name="scroller">The scroller that is requesting the data size</param>
        /// <returns>The number of cells</returns>
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return _data.Count;
        }

        /// <summary>
        /// This tells the scroller what the size of a given cell will be. Cells can be any size and do not have
        /// to be uniform. For vertical scrollers the cell size will be the height. For horizontal scrollers the
        /// cell size will be the width.
        /// </summary>
        /// <param name="scroller">The scroller requesting the cell size</param>
        /// <param name="dataIndex">The index of the data that the scroller is requesting</param>
        /// <returns>The size of the cell</returns>
        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            // pass the expanded size if the cell is expanded, else pass the collapsed size
            return _data[dataIndex].isExpanded ? _data[dataIndex].expandedSize : _data[dataIndex].collapsedSize;
        }

        /// <summary>
        /// Gets the cell to be displayed. You can have numerous cell types, allowing variety in your list.
        /// Some examples of this would be headers, footers, and other grouping cells.
        /// </summary>
        /// <param name="scroller">The scroller requesting the cell</param>
        /// <param name="dataIndex">The index of the data that the scroller is requesting</param>
        /// <param name="cellIndex">The index of the list. This will likely be different from the dataIndex if the scroller is looping</param>
        /// <returns>The cell for the scroller to use</returns>
        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            // first, we get a cell from the scroller by passing a prefab.
            // if the scroller finds one it can recycle it will do so, otherwise
            // it will create a new cell.
            CellView cellView = scroller.GetCellView(cellViewPrefab) as CellView;

            cellView.name = "Cell " + dataIndex.ToString();

            // pass in a reference to our data 
            cellView.SetData(_data[dataIndex], dataIndex, _data[dataIndex].collapsedSize, _data[dataIndex].expandedSize, TweenStart, TweenUpdated, TweenEnd);

            // return the cell to the scroller
            return cellView;
        }

        #endregion
    }
}
