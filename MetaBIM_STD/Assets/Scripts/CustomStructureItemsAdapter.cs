using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;

// You should modify the namespace to your own or - if you're sure there won't ever be conflicts - remove it altogether
namespace MetaBIM
{
	// There are 2 important callbacks you need to implement, apart from Start(): CreateViewsHolder() and UpdateViewsHolder()
	// See explanations below
	public class CustomStructureItemsAdapter : OSA<BaseParamsWithPrefab, CustomStructureItemViewsHolder>
	{
		// Helper that stores data and notifies the adapter when items count changes
		// Can be iterated and can also have its elements accessed by the [] operator
		public SimpleDataHelper<StructureNode> Data { get; private set; }


		#region OSA implementation
		protected override void Start()
		{
			Data = new SimpleDataHelper<StructureNode>(this);

			base.Start();
			
		}

		// This is called initially, as many times as needed to fill the viewport, 
		// and anytime the viewport's size grows, thus allowing more items to be displayed
		// Here you create the "ViewsHolder" instance whose views will be re-used
		// *For the method's full description check the base implementation
		protected override CustomStructureItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new CustomStructureItemViewsHolder();
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			//Debug.Log("Create Item: ");
			instance.Item.SetBlock(Data[itemIndex], itemIndex);
			return instance;
		}

		// This is called anytime a previously invisible item become visible, or after it's created, 
		// or when anything that requires a refresh happens 
		// Here you bind the data from the model to the item's views
		// *For the method's full description check the base implementation
		protected override void UpdateViewsHolder(CustomStructureItemViewsHolder newOrRecycled)
		{
            StructureNode model = Data[newOrRecycled.ItemIndex];
            //Debug.Log("Update Item: ");
            newOrRecycled.Item.SetBlock(model, newOrRecycled.ItemIndex);
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


		#endregion

		// These are common data manipulation methods
		// The list containing the models is managed by you. The adapter only manages the items' sizes and the count
		// The adapter needs to be notified of any change that occurs in the data list. Methods for each
		// case are provided: Refresh, ResetItems, InsertItems, RemoveItems
		#region data manipulation
		public void AddItemsAt(int index, IList<StructureNode> items)
		{

			Data.InsertItems(index, items);
		}

		public void RemoveItemsFrom(int index, int count)
		{


			Data.RemoveItems(index, count);
		}

		public void SetItems(IList<StructureNode> items)
		{

			if (Data != null)
			{
				Data.ResetItems(items);
			}
		}
		#endregion


	}



	// This class keeps references to an item's views.
	// Your views holder should extend BaseItemViewsHolder for ListViews and CellViewsHolder for GridViews
	public class CustomStructureItemViewsHolder : BaseItemViewsHolder
	{

		public UIBlock_BimViewer_IfcStructureItem Item;

		// Retrieving the views from the item's root GameObject
		public override void CollectViews()
		{
			base.CollectViews();

			Item = root.GetComponent<UIBlock_BimViewer_IfcStructureItem>();
		}

	}
}
