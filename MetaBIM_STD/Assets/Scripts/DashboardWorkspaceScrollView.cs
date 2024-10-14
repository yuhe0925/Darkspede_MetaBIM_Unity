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
    public class DashboardWorkspaceScrollView: OSA<BaseParamsWithPrefab, Dashboard_WorkspaceItem>
    {
        
        public SimpleDataHelper<Workspace> Data { get; private set; }


        #region OSA implementation
        protected override void Start()
        {
            Data = new SimpleDataHelper<Workspace>(this);

            // Calling this initializes internal data and prepares the adapter to handle item count changes
            base.Start();

        }


        protected override Dashboard_WorkspaceItem CreateViewsHolder(int itemIndex)
        {
            var instance = new Dashboard_WorkspaceItem();

            instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
            instance.Item.SetBlock(Data[itemIndex]);
            return instance;
        }

        protected override void UpdateViewsHolder(Dashboard_WorkspaceItem newOrRecycled)
        {

            Workspace model = Data[newOrRecycled.ItemIndex];
            newOrRecycled.Item.SetBlock(model);
        }


        #endregion


        #region data manipulation
        public void AddItemsAt(int index, IList<Workspace> items)
        {
            Data.InsertItems(index, items);
        }

        public void RemoveItemsFrom(int index, int count)
        {
            Data.RemoveItems(index, count);
        }

        public void SetItems(IList<Workspace> items)
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

            var newItems = new Workspace[count];
            OnDataRetrieved(newItems);
        }

        void OnDataRetrieved(Workspace[] newItems)
        {
            Data.InsertItemsAtEnd(newItems);
        }


    }
    
    
    public class Dashboard_WorkspaceItem : BaseItemViewsHolder
    {

        
        public UIBlock_Dashboard_WorkspaceItem Item;

        // Retrieving the views from the item's root GameObject
        public override void CollectViews()
        {
            base.CollectViews();
            Item = root.GetComponent<UIBlock_Dashboard_WorkspaceItem>();
        }

    }
}
