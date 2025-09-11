using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Core;
using Elemental.Editor.Utils;
using ImGuiNET;
using MaterialIconFont;
using System.Numerics;

namespace Elemental.Editor.Panels
{
    public class InspectorPanel : Panel
    {

        static int? activeDragComponentIndex = null;

        public override void OnAttach()
        {
        }

        public override void OnGUI()
        {
            if (ImGui.Begin(BootstrapIconFont.InfoCircleFill + " Inspector"))
            {
                if (GameObjectListPanel.SelectedObject != null)
                {
                    bool openPopupTemp = false;
                    bool a = true;

                    UI.BeginPropertyGrid("OBJECT PROPS");

                    UI.BeginProperty("Add Component");
                    if (UI.DrawButton("Add " + MaterialDesign.Add))
                    {
                        openPopupTemp = true;
                    }
                    UI.EndProperty();

                    UI.BeginProperty("Name");
                    UI.PropertyString(ref GameObjectListPanel.SelectedObject.Name, false);
                    UI.EndProperty();

                    UI.EndPropertyGrid();

                    if (openPopupTemp)
                    {
                        ImGui.OpenPopup("Component Menu");
                        openPopupTemp = false;

                    }
                    ContextMenu.ComponentAddMenu();



                    List<Component> componentsToRemove = new();

                    for (int i = 0; i < GameObjectListPanel.SelectedObject.Components.Count; i++)
                    {
                        Component component = GameObjectListPanel.SelectedObject.Components[i];
                        Type componentType = component.GetType();

                        bool isTransform = componentType == typeof(Transform);
                        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(5, 5));
                        ImGui.PushID(componentType.Name + i);

                        string headerLabel = IconRegistry.GetFontIcon(componentType) + " " + component.Type;

                        // Collapsing header
                        bool open = EditorUI.DrawCollapsingHeaderStart(headerLabel, ImGuiTreeNodeFlags.DefaultOpen);

                        // === Context Menu ===
                        if (!isTransform && ImGui.BeginPopupContextItem("ComponentContext"))
                        {
                            if (ImGui.MenuItem("Remove Component"))
                            {
                                componentsToRemove.Add(component);
                            }
                            ImGui.EndPopup();
                        }

                        // === Drag Source ===
                        if (!isTransform && ImGui.BeginDragDropSource())
                        {
                            ImGui.SetDragDropPayload("COMPONENT_DRAG", IntPtr.Zero, 0); // dummy payload
                            activeDragComponentIndex = i;
                            ImGui.Text($"Move {component.Type}");
                            ImGui.EndDragDropSource();
                        }

                        // === Drop Target ===
                        if (!isTransform && ImGui.BeginDragDropTarget())
                        {
                            unsafe
                            {
                                ImGuiPayloadPtr payload = ImGui.AcceptDragDropPayload("COMPONENT_DRAG");
                                if (payload.NativePtr != null && activeDragComponentIndex.HasValue && activeDragComponentIndex.Value != i)
                                {
                                    var dragged = GameObjectListPanel.SelectedObject.Components[activeDragComponentIndex.Value];

                                    int insertIndex = i;
                                    if (activeDragComponentIndex.Value < insertIndex)
                                        insertIndex--;

                                    GameObjectListPanel.SelectedObject.Components.RemoveAt(activeDragComponentIndex.Value);
                                    GameObjectListPanel.SelectedObject.Components.Insert(insertIndex, dragged);

                                    activeDragComponentIndex = null; // clear it
                                }
                            }
                            ImGui.EndDragDropTarget();
                        }


                        // === Component UI ===
                        if (open)
                        {
                            EditorUI.DrawCollapsingHeaderContent(headerLabel, () =>
                            {
                                UI.BeginPropertyGrid("##" + i);

                                //if (ComponentTypeRegistry.CustomEditors.TryGetValue(componentType, out var editorType))
                                //{
                                //    editorType.component = component;
                                //    editorType.OnGUI();
                                //}
                                //else
                                //{
                                UI.DrawComponentProperty(componentType.GetProperties(), component);
                                UI.DrawComponentField(componentType.GetFields(), component);
                                //}

                                UI.EndPropertyGrid();
                            });
                        }
                        else
                        {
                            ImGui.EndChild(); // Close outer header window if content wasn't opened
                            ImGui.PopStyleColor();
                        }

                        ImGui.PopID();
                        ImGui.PopStyleVar();
                    }

                    // Apply deletions
                    foreach (var comp in componentsToRemove)
                    {
                        //GameObjectListPanel.SelectedObject.RemoveComponent(comp);
                    }





                }
            }
            ImGui.End();

        }


    }
}
