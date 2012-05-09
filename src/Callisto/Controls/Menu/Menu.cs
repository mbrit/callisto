﻿﻿//
// Copyright (c) 2012 Tim Heuer
//
// Licensed under the Microsoft Public License (Ms-PL) (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://opensource.org/licenses/Ms-PL.html
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
 
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;

namespace Callisto.Controls
{
    [ContentProperty(Name="Items")]
    public sealed class Menu : Control
    {
        private ObservableCollection<MenuItemBase> _items;
        private ItemsControl _itemContainerList;

        public ObservableCollection<MenuItemBase> Items
        {
            get { return _items; }
        }

        private void OnKeyDown(object sender, KeyEventArgs args)
        {
            switch (args.Key)
            {
                case Windows.System.VirtualKey.Escape:
                    ((Flyout)this.Parent).IsOpen = false;
                    break;
                case Windows.System.VirtualKey.Up:
                    ChangeFocusedItem(false);
                    break;
                case Windows.System.VirtualKey.Down:
                    ChangeFocusedItem(true);
                    break;
                case Windows.System.VirtualKey.Home:
                case Windows.System.VirtualKey.PageUp:
                    PageFocusedItem(true);
                    break;
                case Windows.System.VirtualKey.End:
                case Windows.System.VirtualKey.PageDown:
                    PageFocusedItem(false);
                    break;
                default:
                    break;
            }
        }

        private void PageFocusedItem(bool top)
        {
            int itemNumber = top ? 0 : _items.Count-1;

            _items[itemNumber].Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        private void ChangeFocusedItem(bool ascendIndex)
        {
            Type focusedElementType = FocusManager.GetFocusedElement().GetType();
            MenuItem item;
            int startIndex;

            if (focusedElementType == _itemContainerList.GetType() && _items.Count > 0)
            {
                // focused item is the item container list, so try to set focus to an initial item
                startIndex = GetNextItemIndex(-1, ascendIndex);
            }
            else if (focusedElementType != typeof(MenuItem))
            {
                // focused item isn't the item container list or a menu item, so ignore
                return;
            }
            else
            {
                // focused item is already a menu item, so try to set focus to next
                item = FocusManager.GetFocusedElement() as MenuItem;
                startIndex = GetNextItemIndex(_items.IndexOf(item), ascendIndex);
            }

            int index = startIndex;

            MenuItemBase nextItem = _items[index];

            // focus next item, if it's a menu item
            if (nextItem.GetType() == typeof(MenuItem))
            {
                nextItem.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            }
            else
            {
                // next element wasn't a MenuItem, so loop through once trying to find the next item
                index = GetNextItemIndex(index, ascendIndex);
                nextItem = _items[index];
                while (nextItem.GetType() != typeof(MenuItem) && index != startIndex)
                {
                    index = GetNextItemIndex(index, ascendIndex);
                    nextItem = _items[index];
                }
                if (nextItem.GetType() == typeof(MenuItem))
                {
                    nextItem.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                }
            }
        }

        private int GetNextItemIndex(int startIndex, bool ascending)
        {
            int index = startIndex + (ascending ? 1 : -1);

            if (index >= _items.Count)
            {
                // if after end of collection, go to start
                index = 0;
            }
            else if (index < 0)
            {
                // if before start of collection, go to end
                index = _items.Count - 1;
            }

            return index;
        }

        public Menu()
        {
            this.DefaultStyleKey = typeof(Menu);

            // handle other key events
            //this.AddHandler(KeyDownEvent, new KeyEventHandler(OnKeyDown), true);

            this.Loaded += OnLoaded;

            _items = new ObservableCollection<MenuItemBase>();
            _items.CollectionChanged += OnItemsCollectionChanged;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            ((Menu)sender).IsHitTestVisible = true;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _itemContainerList = GetTemplateChild("ItemList") as ItemsControl;
            _itemContainerList.ItemsSource = _items;   
        }

        void OnItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // since we can't make a custom routed event, manually attach and detach event handlers for selection event
            if (e.NewItems != null)
            {
                foreach (object o in e.NewItems)
                {
                    MenuItem newItem = o as MenuItem;
                    if (newItem != null)
                        newItem.Tapped += OnMenuItemSelected;
                }
            }

            if (e.OldItems != null)
            {
                foreach (object o in e.OldItems)
                {
                    MenuItem oldItem = o as MenuItem;
                    if (oldItem != null)
                        oldItem.Tapped -= OnMenuItemSelected;
                }
            }
        }

        void OnMenuItemSelected(object sender, TappedRoutedEventArgs args)
        {
            MenuItem item = sender as MenuItem;

            if (item == null)
                return;

            ((Flyout)this.Parent).IsOpen = false;

            if (item.Command != null)
                item.Command.Execute(item.CommandParameter);
        }
    }
}
