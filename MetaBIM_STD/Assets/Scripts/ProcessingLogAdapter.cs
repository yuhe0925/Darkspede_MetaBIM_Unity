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
	public class ProcessingLogAdapter : OSA<BaseParamsWithPrefab, ProcessingLogItemViewsHolder>
	{
		// Helper that stores data and notifies the adapter when items count changes
		// Can be iterated and can also have its elements accessed by the [] operator
		public SimpleDataHelper<ProcessingLogItem> Data { get; private set; }


		#region OSA implementation
		protected override void Start()
		{
			Data = new SimpleDataHelper<ProcessingLogItem>(this);

			// Calling this initializes internal data and prepares the adapter to handle item count changes
			base.Start();

		}

		// This is called initially, as many times as needed to fill the viewport, 
		// and anytime the viewport's size grows, thus allowing more items to be displayed
		// Here you create the "ViewsHolder" instance whose views will be re-used
		// *For the method's full description check the base implementation
		protected override ProcessingLogItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new ProcessingLogItemViewsHolder();

			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
            instance.Item.SetBlock(Data[itemIndex], itemIndex);
            return instance;
		}

		// This is called anytime a previously invisible item become visible, or after it's created, 
		// or when anything that requires a refresh happens
		// Here you bind the data from the model to the item's views
		// *For the method's full description check the base implementation
		protected override void UpdateViewsHolder(ProcessingLogItemViewsHolder newOrRecycled)
		{
            ProcessingLogItem model = Data[newOrRecycled.ItemIndex];
            newOrRecycled.Item.SetBlock(model, newOrRecycled.ItemIndex);
        }


		#endregion

		// These are common data manipulation methods
		// The list containing the models is managed by you. The adapter only manages the items' sizes and the count
		// The adapter needs to be notified of any change that occurs in the data list. Methods for each
		// case are provided: Refresh, ResetItems, InsertItems, RemoveItems
		#region data manipulation
		public void AddItemsAt(int index, IList<ProcessingLogItem> items)
		{
			// Commented: the below 2 lines exemplify how you can use a plain list to manage the data, instead of a DataHelper, in case you need full control
			//YourList.InsertRange(index, items);
			//InsertItems(index, items.Length);

			Data.InsertItems(index, items);
		}

		public void RemoveItemsFrom(int index, int count)
		{
			// Commented: the below 2 lines exemplify how you can use a plain list to manage the data, instead of a DataHelper, in case you need full control
			//YourList.RemoveRange(index, count);
			//RemoveItems(index, count);

			Data.RemoveItems(index, count);
		}

		public void SetItems(IList<ProcessingLogItem> items)
		{

            if (Data != null)
            {
                Data.ResetItems(items);
            }
        }
		#endregion


		// Here, we're requesting <count> items from the data source
		void RetrieveDataAndUpdate(int count)
		{
			StartCoroutine(FetchMoreItemsFromDataSourceAndUpdate(count));
		}

		// Retrieving <count> models from the data source and calling OnDataRetrieved after.
		// In a real case scenario, you'd query your server, your database or whatever is your data source and call OnDataRetrieved after
		IEnumerator FetchMoreItemsFromDataSourceAndUpdate(int count)
		{
			// Simulating data retrieving delay
			yield return new WaitForSeconds(.5f);
			
			var newItems = new ProcessingLogItem[count];

	

			OnDataRetrieved(newItems);
		}

		void OnDataRetrieved(ProcessingLogItem[] newItems)
		{
			Data.InsertItemsAtEnd(newItems);
		}
	}




	// This class keeps references to an item's views.
	// Your views holder should extend BaseItemViewsHolder for ListViews and CellViewsHolder for GridViews
	public class ProcessingLogItemViewsHolder : BaseItemViewsHolder
	{
        /*
		public Text titleText;
		public Image backgroundImage;
		*/

        public UIBlock_BimViewer_ProcessingLogItem Item;
        // Retrieving the views from the item's root GameObject
        public override void CollectViews()
		{
			base.CollectViews();
            Item = root.GetComponent<UIBlock_BimViewer_ProcessingLogItem>();
            // GetComponentAtPath is a handy extension method from frame8.Logic.Misc.Other.Extensions
            // which infers the variable's component from its type, so you won't need to specify it yourself
            /*
			root.GetComponentAtPath("TitleText", out titleText);
			root.GetComponentAtPath("BackgroundImage", out backgroundImage);
			*/
        }


	}
}
