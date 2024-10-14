/*
 * * * * This bare-bones script was auto-generated * * * *
 * The code commented with "/ * * /" demonstrates how data is retrieved and passed to the adapter, plus other common commands. You can remove/replace it once you've got the idea
 * Complete it according to your specific use-case
 * Consult the Example scripts if you get stuck, as they provide solutions to most common scenarios
 * 
 * Main terms to understand:
 *		Model = class that contains the data associated with an item (title, content, icon etc.)
 *		Views Holder = class that contains references to your views (Text, Image, MonoBehavior, etc.)
 * 
 * Default expected UI hiererchy:
 *	  ...
 *		-Canvas
 *		  ...
 *			-MyScrollViewAdapter
 *				-Viewport
 *					-Content
 *				-Scrollbar (Optional)
 *				-ItemPrefab (Optional)
 * 
 * Note: If using Visual Studio and opening generated scripts for the first time, sometimes Intellisense (autocompletion)
 * won't work. This is a well-known bug and the solution is here: https://developercommunity.visualstudio.com/content/problem/130597/unity-intellisense-not-working-after-creating-new-1.html (or google "unity intellisense not working new script")
 * 
 * 
 * Please read the manual under "/Docs", as it contains everything you need to know in order to get started, including FAQ
 */

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
	public class ZoneManangementSelectedElementAdapter : OSA<BaseParamsWithPrefab, ZoneSelectedElementItemViewsHolder>
	{
		// Helper that stores data and notifies the adapter when items count changes
		// Can be iterated and can also have its elements accessed by the [] operator
		public SimpleDataHelper<StructureNode> Data { get; private set; }


		#region OSA implementation
		protected override void Start()
		{
			Data = new SimpleDataHelper<StructureNode>(this);

			// Calling this initializes internal data and prepares the adapter to handle item count changes
			base.Start();

		}

		protected override ZoneSelectedElementItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new ZoneSelectedElementItemViewsHolder();

			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			return instance;
		}


		protected override void UpdateViewsHolder(ZoneSelectedElementItemViewsHolder newOrRecycled)
		{
            StructureNode model = Data[newOrRecycled.ItemIndex];
            newOrRecycled.Item.SetBlock(model, newOrRecycled.ItemIndex);
        }

		#endregion

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

	// Class containing the data associated with an item



	// This class keeps references to an item's views.
	// Your views holder should extend BaseItemViewsHolder for ListViews and CellViewsHolder for GridViews
	public class ZoneSelectedElementItemViewsHolder : BaseItemViewsHolder
	{
        public UIBlock_BimViewer_ZoneManagement_ZoneSelectedItem Item;


        // Retrieving the views from the item's root GameObject
        public override void CollectViews()
		{
			base.CollectViews();

            Item = root.GetComponent<UIBlock_BimViewer_ZoneManagement_ZoneSelectedItem>();
        }


	}
}
