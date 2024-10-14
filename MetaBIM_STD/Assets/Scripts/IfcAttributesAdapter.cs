using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;


namespace MetaBIM
{

	public class IfcAttributesAdapter : OSA<BaseParamsWithPrefab, IfcAttributeItemViewsHolder>
	{
		// Helper that stores data and notifies the adapter when items count changes
		// Can be iterated and can also have its elements accessed by the [] operator
		public SimpleDataHelper<IfcAttributeItem> Data { get; private set; }


		#region OSA implementation
		protected override void Start()
		{
			Data = new SimpleDataHelper<IfcAttributeItem>(this);

			// Calling this initializes internal data and prepares the adapter to handle item count changes
			base.Start();

			// Retrieve the models from your data source and set the items count
			/*
			RetrieveDataAndUpdate(500);
			*/
		}

		protected override IfcAttributeItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new IfcAttributeItemViewsHolder();

			// Using this shortcut spares you from:
			// - instantiating the prefab yourself
			// - enabling the instance game object
			// - setting its index 
			// - calling its CollectViews()
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			instance.Item.SetBlock(Data[itemIndex]);
			return instance;
		}

		// This is called anytime a previously invisible item become visible, or after it's created, 
		// or when anything that requires a refresh happens
		// Here you bind the data from the model to the item's views
		// *For the method's full description check the base implementation
		protected override void UpdateViewsHolder(IfcAttributeItemViewsHolder newOrRecycled)
		{
			// In this callback, "newOrRecycled.ItemIndex" is guaranteed to always reflect the
			// index of item that should be represented by this views holder. You'll use this index
			// to retrieve the model from your data set

			IfcAttributeItem model = Data[newOrRecycled.ItemIndex];
			newOrRecycled.Item.SetBlock(model);

		}

		// This is the best place to clear an item's views in order to prepare it from being recycled, but this is not always needed, 
		// especially if the views' values are being overwritten anyway. Instead, this can be used to, for example, cancel an image 
		// download request, if it's still in progress when the item goes out of the viewport.
		// <newItemIndex> will be non-negative if this item will be recycled as opposed to just being disabled
		// *For the method's full description check the base implementation
		/*
		protected override void OnBeforeRecycleOrDisableViewsHolder(MyListItemViewsHolder inRecycleBinOrVisible, int newItemIndex)
		{
			base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);
		}
		*/

		// You only need to care about this if changing the item count by other means than ResetItems, 
		// case in which the existing items will not be re-created, but only their indices will change.
		// Even if you do this, you may still not need it if your item's views don't depend on the physical position 
		// in the content, but they depend exclusively to the data inside the model (this is the most common scenario).
		// In this particular case, we want the item's index to be displayed and also to not be stored inside the model,
		// so we update its title when its index changes. At this point, the Data list is already updated and 
		// shiftedViewsHolder.ItemIndex was correctly shifted so you can use it to retrieve the associated model
		// Also check the base implementation for complementary info
		/*
		protected override void OnItemIndexChangedDueInsertOrRemove(MyListItemViewsHolder shiftedViewsHolder, int oldIndex, bool wasInsert, int removeOrInsertIndex)
		{
			base.OnItemIndexChangedDueInsertOrRemove(shiftedViewsHolder, oldIndex, wasInsert, removeOrInsertIndex);

			shiftedViewsHolder.titleText.text = Data[shiftedViewsHolder.ItemIndex].title + " #" + shiftedViewsHolder.ItemIndex;
		}
		*/
		#endregion

		// These are common data manipulation methods
		// The list containing the models is managed by you. The adapter only manages the items' sizes and the count
		// The adapter needs to be notified of any change that occurs in the data list. Methods for each
		// case are provided: Refresh, ResetItems, InsertItems, RemoveItems
		#region data manipulation
		public void AddItemsAt(int index, IList<IfcAttributeItem> items)
		{
			// Commented: the below 2 lines exemplify how you can use a plain list to manage the data, instead of a DataHelper, in case you need full control
			//YourList.InsertRange(index, items);
			//InsertItems(index, items.Length);
			if (Data != null)
				Data.InsertItems(index, items);
		}

		public void RemoveItemsFrom(int index, int count)
		{
			// Commented: the below 2 lines exemplify how you can use a plain list to manage the data, instead of a DataHelper, in case you need full control
			//YourList.RemoveRange(index, count);
			//RemoveItems(index, count);
			if (Data != null)
				Data.RemoveItems(index, count);
		}

		public void SetItems(IList<IfcAttributeItem> items)
		{
			// Commented: the below 3 lines exemplify how you can use a plain list to manage the data, instead of a DataHelper, in case you need full control
			//YourList.Clear();
			//YourList.AddRange(items);
			//ResetItems(YourList.Count);
			if (Data != null)
			{
				Data.ResetItems(items);
			}
		}
		#endregion

	}


	// This class keeps references to an item's views.
	// Your views holder should extend BaseItemViewsHolder for ListViews and CellViewsHolder for GridViews
	public class IfcAttributeItemViewsHolder : BaseItemViewsHolder
	{

		public UIBlock_BimViewer_IfcAttributeItem Item;


		// Retrieving the views from the item's root GameObject
		public override void CollectViews()
		{
			base.CollectViews();

			Item = root.GetComponent<UIBlock_BimViewer_IfcAttributeItem>();

		}

	
		
	}
}
