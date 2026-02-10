using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace One_Tap_UI.EditorExperience {
    class DisplayObjectsEditorWindow : EditorWindow {
        private DisplayObjects displayObjects;
        private ReorderableList reorderableList;
        private int height = 50;

        [MenuItem("Window/One Tap UI/Display Objects")]
        public static void Open() {
            var window = GetWindow<DisplayObjectsEditorWindow>();
            window.titleContent = new GUIContent("Select `Display Objects`");
            window.Show();
        }

        private void OnEnable() {
            if (displayObjects != null) {
                InitializeReorderableList();
            }
        }

        public void OnGUI() {
            CustomDropField(HandleDrop);
            if (displayObjects == null) {
                return;
            }
            if (reorderableList == null) {
                InitializeReorderableList();
            } else {
                reorderableList.DoLayoutList();
                SaveButton();
            }
        }

        private void CustomDropField(Action<ScriptableObject> onDrop) {
            var width = EditorGUIUtility.currentViewWidth;
            var dropRect = new Rect(0, 0, width - height, height);
            var dropStyle = new GUIStyle(GUI.skin.box) {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Italic
            };
            GUI.Box(dropRect, "Drag & Drop Scriptable Objects Here", dropStyle);
            var buttonStyle = new GUIStyle(GUI.skin.textArea) {
                alignment = TextAnchor.MiddleCenter,
                
            };
            var buttonRect = new Rect(width - height, 0, height, height);
            if (GUI.Button(buttonRect, "+", buttonStyle)) {
                var path = EditorUtility.OpenFilePanel("Select Scriptable Object", "Assets", "asset");
                if (string.IsNullOrEmpty(path)) {
                    return;
                }

                var assetPath = path.Substring(path.IndexOf("Assets"));
                var obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                onDrop(obj);
            }

            var evt = Event.current;
            switch (evt.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropRect.Contains(evt.mousePosition)) {
                        break;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();

                        foreach (var draggedObject in DragAndDrop.objectReferences) {
                            onDrop(draggedObject as ScriptableObject);
                        }
                    }

                    break;
            }
            EditorGUILayout.Space(height);
        }

        private void HandleDrop(ScriptableObject obj) {
            if (obj is DisplayObjects) {
                displayObjects = obj as DisplayObjects;
                name = displayObjects.name;
                this.titleContent = new GUIContent(name);
            } else if (displayObjects != null) {
                displayObjects.AddObject(obj);
            }
        }

        private void InitializeReorderableList() {
            reorderableList = new ReorderableList(displayObjects.list, typeof(ScriptableObject), true, true, true, true)
            {
                // Header
                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Objects in List");
                },

                // Draw each element
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = displayObjects.list[index];
                    rect.y += 2;
                    displayObjects.list[index] = (ScriptableObject)EditorGUI.ObjectField(
                        new Rect(rect.x, rect.y, rect.width - 30, EditorGUIUtility.singleLineHeight),
                        element, typeof(ScriptableObject), false);

                    if (GUI.Button(new Rect(rect.x + rect.width - 25, rect.y, 25, EditorGUIUtility.singleLineHeight), "X"))
                    {
                        displayObjects.RemoveObjectAt(index);
                    }
                },

                // Add new element
                onAddCallback = (ReorderableList list) =>
                {
                    displayObjects.AddObject(null);
                },

                // Remove element
                onRemoveCallback = (ReorderableList list) =>
                {
                    displayObjects.RemoveObjectAt(list.index);
                }
            };
        }

        private void SaveButton() {
            if (GUILayout.Button("Save")) {
                EditorUtility.SetDirty(displayObjects);
                AssetDatabase.SaveAssets();
            }
        }
    }
}