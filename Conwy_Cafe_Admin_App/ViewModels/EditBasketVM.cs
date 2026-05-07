using Conwy_Cafe_Admin_App.Utilities;
using ConwyCafe.Shared.Models;
using System.Windows;
using System.Windows.Input;

namespace Conwy_Cafe_Admin_App.ViewModels
{
    public class EditBasketVM : ViewModelBase
    {
        // Constructor
        public EditBasketVM(Basket SelectedBasket, List<Item> Items)
        {
            // Work on a copy so we can "Cancel" without ruining the main list
            EditBasket = SelectedBasket;
            AllItems = Items;
            LoadData();

            CancelUpdateCommand = new RelayCommand(execute: obj => CancelUpdate(obj as Window));
            SaveUpdateCommand = new RelayCommand(execute: obj => SaveUpdate(obj as Window));
        }




        // Declaring variables and collections
        private Basket _editBasket;

        public List<Item> AllItems { get; set; } = new List<Item>();
        public List<Item> MainItems { get; set; } = new List<Item>();
        public List<Item> SideItems { get; set; } = new List<Item>();
        public List<Item> DrinkItems { get; set; } = new List<Item>();

        public Item BasketMainItem;
        public List<Item> BasketSideItems = new List<Item>();
        public Item BasketDrinkItem;

        public Dictionary<BasketCategory, string> CategoryDisplayNames { get; } = new()
        {
            { BasketCategory.Meat, "Meat" },
            { BasketCategory.Vegetarian, "Vegetarian" },
            { BasketCategory.ChildrenMeat, "Children Meat" },
            { BasketCategory.ChildrenVegetarian, "Children Vegetarian" }
        };

        public Dictionary<ItemType, string> ItemDisplayNames { get; } = new()
        {
            { ItemType.Main, "Main" },
            { ItemType.Side, "Side" },
            { ItemType.Drink, "Drink" }
        };


        public ICommand CancelUpdateCommand { get; set; }
        public ICommand SaveUpdateCommand { get; set; }

        // Properties
        public Basket EditBasket
        {
            get { return _editBasket; }
            set
            {
                _editBasket = value;
                OnPropertyChanged(nameof(EditBasket));
            }
        }

        // Creating slots for the 1-3-1 UI layout. The main slot is for the main item, the three side slots are for the side items, and the drink slot is for the drink item. These properties will be used to bind the UI elements to the corresponding items in the basket, allowing for easy display and interaction with the items in the basket.
        public BasketItems MainSlot
        {
            get
            {
                // This goes through the basket items of the basket, checks the item (null check) and checks the item type.
                // If the item type is main, it returns that item as the main slot. If no main item is found, it returns null.
                foreach (var item in EditBasket.BasketItems)
                {
                    if (item.Item != null && item.Item.ItemType == ItemType.Main) { return item; }
                }
                return null;
            }
            set { OnPropertyChanged(nameof(MainSlot)); }
        }

        // Although the logic is similar to the main slots, there are 3 sides and thus a list is used to store the side slots instead of a single variable.
        // The method goes through the basket items of the basket, checks the item (null check) and checks the item type.
        // If the item type is side, it adds that item to a list of side slots. It returns the list of side slots, which may contain 0 to 3 items depending on how many side items are in the basket.
        public IEnumerable<BasketItems> SideSlots
        {
            get
            {
                // This goes through the basket items of the basket, checks the item (null check) and checks the item type.
                // If the item type is side, it adds that item to a list of side slots. It returns the list of side slots, which may contain 0 to 3 items depending on how many side items are in the basket.
                List<BasketItems> sideSlots = new List<BasketItems>();
                foreach (var item in EditBasket.BasketItems)
                {
                    if (item.Item != null && item.Item.ItemType == ItemType.Side) { sideSlots.Add(item); }
                }
                return sideSlots;
            }
            set { OnPropertyChanged(nameof(SideSlots)); }
        }

        public BasketItems DrinkSlot
        {
            get
            {
                // This goes through the basket items of the basket, checks the item (null check) and checks the item type.
                // If the item type is drink, it returns that item as the drink slot. If no drink item is found, it returns null.
                foreach (var item in EditBasket.BasketItems)
                {
                    if (item.Item != null && item.Item.ItemType == ItemType.Drink) { return item; }
                }
                return null;
            }
            set { OnPropertyChanged(nameof(DrinkSlot)); }
        }


        // Commands
        //public ICommand SwapItemCommand { get; }
        //public ICommand SaveCommand { get; }

        //Methods
        // Methods
        public async void LoadData()
        {
            // Waits for the data of baskets and categories to be loaded before proceeding. This ensures that the necessary data is available for the view model to function properly, such as populating the UI with the retrieved baskets and categories.
            await FilterAllItems();
        }

        // Method to filter all items
        // Loops through all the items in the AllItems list and categorizes them based on their ItemType. It checks the ItemType of each item and adds it to the corresponding list (MainItems, SideItems, or DrinkItems) based on whether it is a Main, Side, or Drink item.
        private async Task FilterAllItems()
        {
            // Clear existing categorized lists to avoid duplicates
            MainItems.Clear();
            SideItems.Clear();
            DrinkItems.Clear();

            // Populate categorized lists based on ItemType (used for the combo boxes in the item selector)
            foreach (var item in AllItems)
            {
                switch (item.ItemType)
                {
                    case ItemType.Main: MainItems.Add(item); break;
                    case ItemType.Side: SideItems.Add(item); break;
                    case ItemType.Drink: DrinkItems.Add(item); break;
                }
            }
        }

        // To save the changes made.
        private void SaveUpdate(Window editWindow)
        {
            // Adding the items (id) to a list to check if they are unique
            List<int> itemIds = new List<int>();

            foreach (var basketItem in EditBasket.BasketItems)
            {
                // Checks if it is in the list
                if (!itemIds.Contains(basketItem.Item.Id)) 
                { MessageBox.Show("Duplicate Items found"); return; }
                // If not add to the basket item ids list
                itemIds.Add(basketItem.ItemId);
            }

            //// Check if the items are valid (uniqe main, max 3 sides, unique drink)
            //foreach (var basketItem in EditBasket.BasketItems)
            //{
            //    // Checks if the item is not null
            //    if (basketItem.Item != null)
            //    {
            //        // Checks the item type and applies the corresponding validation rules:
            //        if (basketItem.Item.ItemType == ItemType.Main)
            //        {
            //            if (MainSlot != null && MainSlot.ItemId != basketItem.ItemId)
            //            {
            //                MessageBox.Show("Invalid basket: Multiple main items detected.");
            //                return;
            //            }
            //        }
            //        else if (basketItem.Item.ItemType == ItemType.Side)
            //        {
            //            // Getting the number of sides
            //            int sideCount = EditBasket.BasketItems.Count();
            //            if (sideCount > 3)
            //            {
            //                MessageBox.Show("Invalid basket: More than 3 side items detected.");
            //                return;
            //            }
            //        }
            //        else if (basketItem.Item.ItemType == ItemType.Drink)
            //        {
            //            if (DrinkSlot != null && DrinkSlot.ItemId != basketItem.ItemId)
            //            {
            //                MessageBox.Show("Invalid basket: Multiple drink items detected.");
            //                return;
            //            }
            //        }
            //    }
            //}

            //for (int i = 0; i < MainItems.Count; i++)
            //{
            //    if (MainSlot != null && MainSlot.ItemId == MainItems[i].Id)
            //    {
            //        MainSlot.Item = MainItems[i];
            //    }
            //}

            //for (int i = 0; i < SideItems.Count; i++)
            //{
            //    for (int j = 0; j < SideSlots.Count(); j++)
            //    {
            //        if (SideSlots.ElementAt(j) != null && SideSlots.ElementAt(j).ItemId == SideItems[i].Id)
            //        {
            //            SideSlots.ElementAt(j).Item = SideItems[i];
            //        }
            //    }
            //}


            //try
            //{
            //    // Call API to save the basket
            //    // await ....UpdateBasket(EditBasket);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Failed to save: " + ex.Message);
            //}
            //if (editWindow != null)
            //{
            //    // Setting DialogResult to true tells the calling code
            //    // "The user is happy, go ahead and save this to the API!"
            //    editWindow.DialogResult = true;
            //    //editWindow.Close();
            //}
        }

        // To cancel the changes made and revert to the original state.
        private void CancelUpdate(Window editWindow)
        {
            // Close the window without saving changes. This allows the user to exit the edit mode and discard any modifications made to the basket, effectively reverting to the original state of the basket before any edits were made.
            if (editWindow != null)
            {
                editWindow.DialogResult = false; // Set DialogResult to false to indicate cancellation
                editWindow.Close(); // Close the edit window
            }
        }
    }
}
