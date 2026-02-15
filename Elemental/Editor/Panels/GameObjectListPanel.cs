using DevoidEngine.Engine.Core;
using Elemental.Editor.Utils;
using ImGuiNET;
using System.Numerics;

namespace Elemental.Editor.Panels
{
    class GameObjectListPanel : Panel
    {
        public static GameObject SelectedObject = null;
        private static GameObject DraggedObject = null;


        private string searchBuffer = "";

        public override void OnAttach()
        {

        }

        public override void OnGUI()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

            if (ImGui.Begin(BootstrapIconFont.ArchiveFill + " GameObjects"))
            {

                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(5, 5));
                ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Vector2(0, 0.5f));
                ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0f);
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);
                ImGui.PushStyleColor(ImGuiCol.ChildBg, EditorUI.DarkChildBg);

                if (ImGui.BeginChild("gameobject_hierarchy_scroll", ImGui.GetContentRegionAvail(), ImGuiChildFlags.AlwaysUseWindowPadding, ImGuiWindowFlags.ChildWindow))
                {
                    var roots = SceneManager.MainScene.GameObjects;
                    int rowIndex = 0;
                    for (int i = 0; i < roots.Count; i++)
                    {
                        DrawDropZone(roots[i], before: true);
                        DrawGameObjectRecursive(roots[i], 0, ref rowIndex);
                    }

                    if (roots.Count > 0)
                        DrawDropZone(roots[^1], before: false);


                    if (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                    {
                        ImGui.OpenPopup("GameObjectMenu");
                    }
                    ContextMenu();
                }
                ImGui.EndChild();

                ImGui.PopStyleColor();
                ImGui.PopStyleVar(4);


            }
            ImGui.End();

            ImGui.PopStyleVar();
        }


        private void DrawDropZone(GameObject dropTarget, bool before)
        {
            ImGui.PushID($"dropzone_{dropTarget.GetHashCode()}_{before}");

            ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Vector4.Zero);
            ImGui.PushStyleColor(ImGuiCol.HeaderActive, Vector4.Zero);
            ImGui.Selectable("##DropZone", false, ImGuiSelectableFlags.AllowDoubleClick, new Vector2(ImGui.GetContentRegionAvail().X, 4));
            ImGui.PopStyleColor(2);

            // --- Allow clicking the drop zone to select the GameObject ---
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                SelectedObject = dropTarget;
            }


            if (ImGui.BeginDragDropTarget())
            {
                unsafe
                {
                    ImGuiPayloadPtr payload = ImGui.AcceptDragDropPayload("GAMEOBJECT");
                    if (payload.NativePtr != null)
                    {
                        if (DraggedObject != null && DraggedObject != dropTarget && !IsChildOf(dropTarget, DraggedObject))
                        {
                            UpdateThreadDispatcher.Queue(() =>
                            {
                                // Get the target sibling list (same parent as dropTarget)
                                var newParent = dropTarget.parentObject;
                                var targetList = newParent?.children ?? SceneManager.MainScene.GameObjects;

                                // Remove from old parent
                                var oldList = DraggedObject.parentObject?.children ?? SceneManager.MainScene.GameObjects;
                                oldList.Remove(DraggedObject);

                                // Set new parent
                                DraggedObject.parentObject = newParent;

                                // Insert at appropriate position
                                int index = targetList.IndexOf(dropTarget);
                                if (!before) index++;
                                index = Math.Clamp(index, 0, targetList.Count);
                                targetList.Insert(index, DraggedObject);
                            });
                        }

                        DraggedObject = null;
                    }
                }
                ImGui.EndDragDropTarget();
            }

            ImGui.PopID();
        }



        private void DrawGameObjectRecursive(GameObject gameObject, int depth, ref int rowIndex)
        {
            ImGui.PushID(gameObject.GetHashCode());

            float rowHeight = 30f;
            Vector2 cursorPos = ImGui.GetCursorScreenPos();
            Vector2 rowSize = new Vector2(ImGui.GetContentRegionAvail().X, rowHeight);

            // Alternating background color
            Vector4 bgColor = (rowIndex % 2 == 0)
                ? new Vector4(0.16f, 0.16f, 0.16f, 1f)
                : new Vector4(0.18f, 0.18f, 0.18f, 1f);

            ImGui.GetWindowDrawList().AddRectFilled(
                cursorPos,
                new Vector2(cursorPos.X + rowSize.X, cursorPos.Y + rowHeight),
                ImGui.ColorConvertFloat4ToU32(bgColor)
            );

            // Reserve space and reset cursor to top of row
            ImGui.Dummy(new Vector2(0, rowHeight)); // Reserve vertical space
            ImGui.SetCursorScreenPos(cursorPos);    // Go back to top of row

            bool isSelected = (SelectedObject == gameObject);
            bool hasChildren = gameObject.children != null && gameObject.children.Count > 0;

            ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.SpanFullWidth | ImGuiTreeNodeFlags.FramePadding;
            if (!hasChildren)
                flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;
            if (isSelected)
            {
                flags |= ImGuiTreeNodeFlags.Selected;
                ImGui.PushStyleColor(ImGuiCol.Header, new Vector4(0.26f, 0.45f, 0.78f, 0.8f));      // Selected
                ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Vector4(0.3f, 0.5f, 0.85f, 1.0f)); // Hovered + selected
            }

            // Adjust vertical padding to match row height
            float textHeight = ImGui.GetTextLineHeight();
            float verticalPadding = MathF.Max(0f, (rowHeight - textHeight) / 2f);
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(4f, verticalPadding));

            // Draw the tree node
            bool open = ImGui.TreeNodeEx(/*IconRegistry.GetFontIcon(typeof(GameObject), gameObject.children.Count > 0 ? 1 : 0).icon + */" " + gameObject.Name, flags);

            ImGui.PopStyleVar(); // FramePadding
            if (isSelected) ImGui.PopStyleColor(2); // Header + Hovered color

            // Click to select
            if (ImGui.IsItemClicked())
                SelectedObject = gameObject;

            // Begin drag
            if (ImGui.BeginDragDropSource())
            {
                ImGui.SetDragDropPayload("GAMEOBJECT", IntPtr.Zero, 0);
                ImGui.Text(/*IconRegistry.GetFontIcon(typeof(GameObject)).icon +*/ " " + gameObject.Name);
                DraggedObject = gameObject;
                ImGui.EndDragDropSource();
            }

            // Accept drop
            if (ImGui.BeginDragDropTarget())
            {
                ImGuiPayloadPtr payload = ImGui.AcceptDragDropPayload("GAMEOBJECT");
                unsafe
                {
                    if (payload.NativePtr != null)
                    {
                        if (DraggedObject != null && DraggedObject != gameObject && !IsChildOf(DraggedObject, gameObject))
                        {
                            DraggedObject.parentObject?.children?.Remove(DraggedObject);
                            if (DraggedObject.parentObject == null)
                                SceneManager.MainScene.GameObjects.Remove(DraggedObject);

                            DraggedObject.parentObject = gameObject;
                            gameObject.children ??= new List<GameObject>();
                            gameObject.children.Add(DraggedObject);
                        }
                        DraggedObject = null;
                    }
                }
                ImGui.EndDragDropTarget();
            }

            rowIndex++;

            // Recursively draw children if node is open
            if (open && hasChildren)
            {
                var children = gameObject.children;
                for (int i = 0; i < children.Count; i++)
                {
                    DrawDropZone(children[i], before: true);
                    DrawGameObjectRecursive(children[i], depth + 1, ref rowIndex);
                }
                DrawDropZone(children[^1], before: false);
                ImGui.TreePop();
            }

            ImGui.PopID();
        }

        private bool IsChildOf(GameObject child, GameObject potentialParent)
        {
            var current = child.parentObject;
            while (current != null)
            {
                if (current == potentialParent)
                    return true;
                current = current.parentObject;
            }
            return false;
        }


        void ContextMenu()
        {

            if (ImGui.BeginPopupContextWindow("GameObjectMenu", ImGuiPopupFlags.MouseButtonRight))
            {
                if (ImGui.MenuItem("Action 1"))
                {
                    // Handle Action 1
                }

                if (ImGui.BeginMenu("Add Object"))
                {
                    if (ImGui.MenuItem("Basic GameObject"))
                    {
                        UpdateThreadDispatcher.QueueLatest("GameObjectList_AddGameObject", () =>
                        {
                            SelectedObject = SceneManager.MainScene.addGameObject("DevoidObject");
                        });
                    }

                    ImGui.EndMenu();
                }
                ImGui.EndPopup();
            }

        }

    }
}
