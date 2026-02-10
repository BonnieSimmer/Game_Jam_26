using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace One_Tap_UI.EditorExperience
{
    [CustomEditor(typeof(DisplayObjects))]
    public class DisplayObjectsEditor : Editor
    {
        private DisplayObjects displayObjects;
        private VisualElement container;

        public void OnEnable()
        {
            displayObjects = target as DisplayObjects;
        }

        public override VisualElement CreateInspectorGUI()
        {
            // Create a container for the custom inspector
            container = new VisualElement();

            // Generate the UI
            Regenerate();

            return container;
        }

        private void Regenerate()
        {
            // Clear existing content
            container.Clear();

            // Iterate over the list and create editors
            foreach (var obj in displayObjects.list)
            {
                if (obj == null) continue;

                // Create a foldout for each object
                var foldout = new Foldout { text = obj.name };
                container.Add(foldout);

                // Create an editor for the object
                var editor = Editor.CreateEditor(obj);

                // Create a container for the editor's UI
                var editorContainer = new VisualElement();
                foldout.Add(editorContainer);

                // Get the editor's UI and add it to the container
                var editorUI = editor.CreateInspectorGUI();
                if (editorUI != null)
                {
                    editorContainer.Add(editorUI);
                }
                else
                {
                    // Fallback to IMGUI if CreateInspectorGUI is not implemented
                    var imguiContainer = new IMGUIContainer(editor.OnInspectorGUI);
                    editorContainer.Add(imguiContainer);
                }
            }
        }
    }
}
