using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using devDept.Eyeshot;
using devDept.Graphics;
using devDept.Eyeshot.Entities;

namespace Aura.Controls.Util
{
    public class AssemblyTree
    {
        public static int LEVEL_LIMIT { get; set; } = 3;

        public static void PopulateTree(TreeView tv, List<Entity> entList, BlockKeyedCollection blocks, int currentLevel, TreeNode parentNode = null)
        {
            ItemCollection nodes;
            if (parentNode == null)
            {
                //tv.Items.Clear();
                nodes = tv.Items;
            }
            else
            {
                nodes = parentNode.Items;
            }
            for (var i = 0; i < entList.Count; i++)
            {
                var ent = entList[i];
                if (ent is BlockReference blockReference)
                {
                    var blockName = blockReference.BlockName;

                    if (blocks.TryGetValue(blockName, out var child))
                    {
                        TreeNode parentTn;
                        if (currentLevel + 1 > LEVEL_LIMIT)
                        {
                            parentTn = parentNode;
                        }
                        else
                        {
                            parentTn = new TreeNode(parentNode, GetNodeName(blockName, i), false, currentLevel) { Tag = ent };
                            nodes.Add(parentTn);
                        }

                        PopulateTree(tv, child.Entities, blocks, currentLevel + 1, parentTn);
                    }


                }
                else
                {
                    var type = ent.GetType().ToString().Split('.').LastOrDefault();
                    var node = new TreeNode(parentNode, GetNodeName(type, i), true, currentLevel) {Tag = ent};
                    nodes.Add(node);
                }
            }
        }

        private static string GetNodeName(string name, int index)
        {
            return $"{name} ({index})";
        }

        public static void CollapseAll(TreeView tv)
        {
            foreach (TreeNode i in tv.Items)
            {
                i.IsExpanded = false;
            }
        }

        /// <summary>
        /// Screen->Tree Selection. To check we are considering the correct entities, we use the Entity stored in the Tag property of the TreeView.
        /// </summary>
        /// <param name="tv">The TreeView control</param>
        /// <param name="blockReferences">The block reference stack</param>
        /// <param name="selectedEntity">The selected entity inside a block reference. Can be null when we click on a BlockReference.</param>
        /// <param name="parentTn">The parent TreeNode for searching inside its nodes. Can be null.</param>
        public static void SearchNodeInTree(TreeView tv, Stack<BlockReference> blockReferences, ViewportLayout.SelectedItem selectedEntity, TreeNode parentTn = null)
        {
            if (blockReferences.Count == 0 && selectedEntity == null) return;

            var tnc = tv.Items;
            if (parentTn != null) tnc = parentTn.Items;
            if (blockReferences.Count > 0)
            {
                // Nested BlockReferences

                var br = blockReferences.Pop();

                foreach (TreeNode tn in tnc)
                {
                    if (ReferenceEquals(br, tn.Tag))
                    {
                        if (blockReferences.Count > 0)
                        {
                            tn.IsExpanded = true;
                            SearchNodeInTree(tv, blockReferences, selectedEntity, tn);
                        }
                        else
                        {
                            if (selectedEntity != null)
                            {
                                foreach (TreeNode childNode in tn.Items)
                                {
                                    if (ReferenceEquals(selectedEntity.Item, childNode.Tag))
                                    {
                                        if (childNode.ParentNode != null) childNode.ParentNode.IsExpanded = true;
                                        childNode.IsSelected = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                tn.IsSelected = true;
                            }
                        }

                        return;
                    }
                }
            }
            else
            {
                // Root level

                if (selectedEntity != null)
                {
                    foreach (TreeNode childNode in tnc)
                    {
                        if (ReferenceEquals(selectedEntity.Item, childNode.Tag))
                        {
                            childNode.IsSelected = true;
                            break;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Clear selection for entities to avoid problems with the Tree->Screen selection
        /// </summary>
        /// <param name="vl">The ViewportLayout control</param>
        /// <param name="rootLevel">When true the CurrentBlockReference is set to null (Go back to the root level of the assembly)</param>
        public static void CleanCurrent(ViewportLayout vl, bool rootLevel = true)
        {
            vl.Entities.ClearSelection();

            if (rootLevel && vl.Entities.CurrentBlockReference != null)
                vl.Entities.SetCurrent(null);
        }

        /// <summary>
        /// Clear selection recursively for inner nodes.
        /// </summary>
        /// <param name="blocks">The blocks collection</param>
        /// <param name="parentBr">The parent block reference</param>
        public static void CleanCurrentNodes(BlockKeyedCollection blocks, BlockReference parentBr)
        {
            Block toClean;

            if (blocks.TryGetValue(parentBr.BlockName, out toClean))
            {
                for (int i = 0; i < toClean.Entities.Count; i++)
                {
                    if (toClean.Entities[i] is BlockReference)
                    {
                        toClean.Entities[i].Selected = false;
                        CleanCurrentNodes(blocks, (BlockReference)toClean.Entities[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the selected tree node and all the others nodes that are linked to the same entity instance.
        /// </summary>
        /// <param name="tv">The TreeView control</param>
        /// <param name="vl">The ViewportLayout control</param>
        public static void DeleteSelectedNode(TreeView tv, ViewportLayout vl)
        {
            if (tv.SelectedItem != null)
            {
                Entity deletedEntity = ((TreeNode)tv.SelectedItem).Tag as Entity;
                DeleteNodes(deletedEntity, tv.Items);
                CleanCurrent(vl);
            }
        }

        /// <summary>
        /// Deletes all the tree nodes that are referring to the same entity instance
        /// </summary>
        /// <param name="entity">The entity instance.</param>
        /// <param name="nodes">The TreeNode collection</param>
        private static void DeleteNodes(Entity entity, ItemCollection nodes)
        {
            int count = nodes.Count;
            while (count > 0)
            {
                count--;
                TreeNode node = (TreeNode)nodes[count];
                if (ReferenceEquals(entity, node.Tag))
                {
                    node.Remove();
                    count = -1;
                }
                else
                {
                    DeleteNodes(entity, node.Items);
                }
            }
        }

        /// <summary>
        /// Tree->Screen Selection. If the viewport entities are selected, they get marked as selected straight away.
        /// To check we are considering the correct entities we use the Entity stored in the Tag property of the TreeView
        /// </summary>
        /// <returns>The selected item.</returns>
        /// <param name="tv">The TreeView control</param>
        /// <param name="vl">The ViewportLayout control</param>        
        public static ViewportLayout.SelectedItem SynchTreeSelection(TreeView tv, ViewportLayout vl)
        {
            // Fill a stack of entities and blockreferences starting from the node tags.
            Stack<BlockReference> parents = new Stack<BlockReference>();

            TreeNode node = (TreeNode)tv.SelectedItem;

            if (node != null)
            {
                Entity entity = node.Tag as Entity;

                node = node.ParentNode;

                while (node != null)
                {
                    var ent = node.Tag as Entity;
                    if (ent != null)

                        parents.Push((BlockReference)ent);

                    node = node.ParentNode;
                }

                //tv.HideSelection = false;

                // The top most parent is the root Blockreference: must reverse the order, creating a new Stack
                var selItem = new ViewportLayout.SelectedItem(new Stack<BlockReference>(parents), entity);

                // Selects the item
                selItem.Select(vl, true);

                return selItem;
            }
            return null;
        }

        /// <summary>
        /// Screen->Tree Selection.
        /// </summary>
        /// <param name="tv">The TreeView control</param>
        /// <param name="blockReferences">The BlockReference stack</param>
        /// <param name="selectedEntity">The selected entity inside a block reference. Can be null when we click on a BlockReference.</param>
        public static void SynchScreenSelection(TreeView tv, Stack<BlockReference> blockReferences, ViewportLayout.SelectedItem selectedEntity)
        {
            CollapseAll(tv);

            if (selectedEntity != null && selectedEntity.Parents.Count > 0)
            {
                //// Add the parents of the selectedEntity to the BlockReferences stack

                // Reverse the stack so the one on top is the one at the root of the hierarchy
                var parentsReversed = selectedEntity.Parents.Reverse();

                var cumulativeStack = new Stack<BlockReference>(blockReferences);

                foreach (var br in parentsReversed)
                {
                    cumulativeStack.Push(br);
                }

                // Create a new stack with the reversed order so the one on top is the root.
                blockReferences = new Stack<BlockReference>(cumulativeStack);
            }

            SearchNodeInTree(tv, blockReferences, selectedEntity);
        }
    }


    public class TreeNode : FrameworkElement
    {
        public string Name { get; set; }
        public bool IsLeaf { get; set; }
        public TreeNode ParentNode { get; set; }
        public ItemCollection Items { get; set; }
        public int Level { get; set; }

        public TreeNode(TreeNode parent)
        {
            Items = new TreeView().Items;
            ParentNode = parent;
        }

        public TreeNode(TreeNode parent, string name, bool isPart, int level) : this(parent)
        {
            Name = name;
            IsLeaf = isPart;
            Level = level;
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(TreeNode), new PropertyMetadata(default(bool)));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            "IsExpanded", typeof(bool), typeof(TreeNode), new PropertyMetadata(default(bool)));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public override string ToString()
        {
            return Name;
        }

        public void Remove()
        {
            while (Items.Count > 0)
            {
                ((TreeNode)Items[0]).Remove();
            }

            if (ParentNode != null)
                ParentNode.Items.Remove(this);
        }
    }
}