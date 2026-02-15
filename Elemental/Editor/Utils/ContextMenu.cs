using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Core;
using Elemental.Editor.Panels;
using ImGuiNET;
using System.Numerics;

namespace Elemental.Editor.Utils
{
    static class ContextMenu
    {
        public static Component? ComponentAddMenu()
        {
            bool openState = true;
            if (ImGui.BeginPopupModal("Component Menu", ref openState))
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(4, 4)); // spacing between sections
                ImGui.PushStyleColor(ImGuiCol.ChildBg, EditorUI.DarkChildBg);

                // Left panel
                if (ImGui.BeginChild("FavouritesBar", new Vector2(300, -1)))
                {
                    // Favourites content
                }
                ImGui.EndChild();

                ImGui.SameLine();

                // Right panel
                Vector2 available = ImGui.GetContentRegionAvail();
                float spacing = ImGui.GetStyle().ItemSpacing.Y;
                float descriptionHeight = available.Y * 0.2f;
                float componentHeight = available.Y - descriptionHeight - spacing;

                if (ImGui.BeginChild("RightSide", new Vector2(available.X, available.Y), ImGuiChildFlags.None, ImGuiWindowFlags.NoBackground))
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, EditorUI.WindowPadding);
                    if (ImGui.BeginChild("ComponentBar", new Vector2(-1, componentHeight), ImGuiChildFlags.AlwaysUseWindowPadding))
                    {
                        if (ImGui.BeginChild("ComponentBar_SubChild1", new Vector2(-1, 0), ImGuiChildFlags.AutoResizeY))
                        {
                            string input_buffer = "";
                            ImGui.InputText("Search", ref input_buffer, 100);
                        }
                        ImGui.EndChild();

                        if (ImGui.BeginChild("ComponentBar_SubChild2"))
                        {
                            var grouped = ComponentTypeRegistry.ComponentTypes
                            .GroupBy(ComponentCategoryRegistry.GetCategoryFor)
                            .OrderBy(g => g.Key);

                            foreach (var group in grouped)
                            {
                                CategoryInfo info = ComponentCategoryRegistry.CategoryInfos[group.Key];

                                if (ImGui.CollapsingHeader(info.DefaultIcon + " " + info.DisplayName, ImGuiTreeNodeFlags.DefaultOpen))
                                {
                                    if (ImGui.BeginChild("ComponentMenuComponentHeaderChild" + group.Key, new Vector2(-1, 0), ImGuiChildFlags.AutoResizeY))
                                    {
                                        ImGui.TreePush("ComponentMenuComponentHeader" + group.Key);

                                        var types = group.ToList();
                                        for (int i = 0; i < types.Count; i++)
                                        {
                                            var type = types[i];
                                            bool isLast = (i == types.Count - 1);

                                            FontIconEntry icon = IconRegistry.GetFontIcon(type);
                                            string iconStr = icon.icon;
                                            string labelStr = type.Name;

                                            float height = 30.0f;
                                            float padding = 8.0f;

                                            float fullWidth = ImGui.GetContentRegionAvail().X;
                                            Vector2 fullSize = new Vector2(fullWidth, height);

                                            Vector2 cursorPos = ImGui.GetCursorScreenPos();

                                            // Button interaction
                                            if (ImGui.InvisibleButton($"##component_btn_{type}", fullSize))
                                            {
                                                UpdateThreadDispatcher.Queue(() =>
                                                {
                                                    GameObjectListPanel.SelectedObject.AddComponent((Component)Activator.CreateInstance(type));
                                                });

                                                ImGui.CloseCurrentPopup();
                                            }

                                            // Hover background
                                            Vector4 bgColor = ImGui.IsItemHovered()
                                                ? ImGui.GetStyle().Colors[(int)ImGuiCol.ButtonHovered]
                                                : Vector4.Zero;

                                            ImGui.GetWindowDrawList().AddRect(
                                                cursorPos,
                                                cursorPos + fullSize,
                                                ImGui.ColorConvertFloat4ToU32(bgColor),
                                                6,
                                                ImDrawFlags.None,
                                                1
                                            );

                                            // Tree lines
                                            float lineX = cursorPos.X - 5;
                                            float lineTop = cursorPos.Y;
                                            float lineBottom = cursorPos.Y + height + ImGui.GetStyle().ItemSpacing.Y;
                                            float midY = cursorPos.Y + height * 0.5f;

                                            uint lineColor = ImGui.GetColorU32(ImGuiCol.Border);

                                            // Vertical line
                                            if (!isLast)
                                            {
                                                ImGui.GetWindowDrawList().AddLine(
                                                    new Vector2(lineX, lineTop),
                                                    new Vector2(lineX, lineBottom),
                                                    lineColor
                                                );
                                            }
                                            else
                                            {
                                                // Short vertical line for last item
                                                ImGui.GetWindowDrawList().AddLine(
                                                    new Vector2(lineX, lineTop),
                                                    new Vector2(lineX, midY),
                                                    lineColor
                                                );
                                            }

                                            // Horizontal branch
                                            ImGui.GetWindowDrawList().AddLine(
                                                new Vector2(lineX, midY),
                                                new Vector2(cursorPos.X + padding, midY),
                                                lineColor
                                            );

                                            // Text position calculation
                                            Vector2 iconSize = ImGui.CalcTextSize(iconStr);
                                            Vector2 labelSize = ImGui.CalcTextSize(labelStr);

                                            float iconYOffset = (height - iconSize.Y) * 0.5f;
                                            float labelYOffset = (height - labelSize.Y) * 0.5f;

                                            // Draw icon
                                            ImGui.SetCursorScreenPos(new Vector2(cursorPos.X + padding, cursorPos.Y + iconYOffset));
                                            ImGui.PushStyleColor(ImGuiCol.Text, icon.iconColor);
                                            ImGui.TextUnformatted(iconStr);
                                            ImGui.PopStyleColor();

                                            // Draw label
                                            ImGui.SameLine();
                                            ImGui.SetCursorScreenPos(new Vector2(cursorPos.X + padding + iconSize.X + padding, cursorPos.Y + labelYOffset));
                                            ImGui.TextUnformatted(labelStr);

                                            // Spacing for next item
                                            ImGui.SetCursorScreenPos(new Vector2(cursorPos.X, cursorPos.Y + height + ImGui.GetStyle().ItemSpacing.Y));
                                            ImGui.Dummy(new Vector2(0, 0));
                                        }

                                        ImGui.TreePop();
                                    }
                                    ImGui.EndChild();
                                }
                            }
                        }
                        ImGui.EndChild();
                    }
                    ImGui.EndChild();

                    ImGui.PopStyleVar();

                    if (ImGui.BeginChild("DescriptionBar", new Vector2(-1, descriptionHeight)))
                    {
                        // Description content
                    }
                    ImGui.EndChild();
                }
                ImGui.EndChild();

                ImGui.PopStyleVar(2); // ItemSpacing + WindowPadding
                ImGui.PopStyleColor();
                ImGui.EndPopup();
            }

            return null;
        }

        static void DrawTreeNode(string label, int depth, bool hasChildren)
        {
            // Indent level
            ImGui.Indent(20.0f * depth);

            // Record position before node
            Vector2 startPos = ImGui.GetCursorScreenPos();
            bool open = ImGui.TreeNodeEx(label, ImGuiTreeNodeFlags.OpenOnArrow);

            // Draw vertical line from parent
            Vector2 endPos = ImGui.GetCursorScreenPos();
            ImDrawListPtr drawList = ImGui.GetWindowDrawList();

            float halfHeight = ImGui.GetTextLineHeight() * 0.5f;
            Vector2 lineStart = new Vector2(startPos.X - 10.0f, startPos.Y + halfHeight);
            Vector2 lineEnd = new Vector2(startPos.X, startPos.Y + halfHeight);

            if (depth > 0)
            {
                drawList.AddLine(lineStart, lineEnd, ImGui.GetColorU32(ImGuiCol.Separator));
            }

            if (open)
            {
                // Example children
                if (label == "Root")
                {
                    DrawTreeNode("Child 1", depth + 1, true);
                    DrawTreeNode("Child 2", depth + 1, false);
                }
                else if (label == "Child 1")
                {
                    DrawTreeNode("Grandchild 1", depth + 1, false);
                    DrawTreeNode("Grandchild 2", depth + 1, false);
                }

                ImGui.TreePop();
            }

            ImGui.Unindent(20.0f * depth);
        }


    }
}
