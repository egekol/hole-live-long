using Lib.CustomAttributes.InspectorHelpers.Scripts;
using UnityEditor;
using UnityEngine;

namespace Lib.CustomAttributes.InspectorHelpers.Editor
{
    [CustomPropertyDrawer(typeof(ShowAssetPreviewAttribute))]
    public class ShowAssetPreviewPropertyDrawer : PropertyDrawer
    {
        public sealed override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginProperty(rect, label, property);

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                Rect propertyRect = new Rect()
                {
                    x = rect.x,
                    y = rect.y,
                    width = rect.width,
                    height = EditorGUIUtility.singleLineHeight
                };

                EditorGUI.PropertyField(propertyRect, property, label);

                Texture2D previewTexture = GetAssetPreview(property);
                if (previewTexture != null)
                {
                    Rect previewRect = new Rect()
                    {
                        x = rect.x + GetIndentLength(rect),
                        y = rect.y + EditorGUIUtility.singleLineHeight,
                        width = rect.width,
                        height = GetAssetPreviewSize(property).y
                    };

                    GUI.Label(previewRect, previewTexture);
                }
            }
            else
            {
                string message = property.name + " doesn't have an asset preview";
                DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
            }

            EditorGUI.EndProperty();
            EditorGUI.EndChangeCheck();
        }
        
        public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                Texture2D previewTexture = GetAssetPreview(property);
                if (previewTexture != null)
                {
                    return GetPropertyHeight(property) + GetAssetPreviewSize(property).y;
                }
                else
                {
                    return GetPropertyHeight(property);
                }
            }
            else
            {
                return GetPropertyHeight(property) + GetHelpBoxHeight();
            }
        }
        
        private float GetIndentLength(Rect sourceRect)
        {
            Rect indentRect = EditorGUI.IndentedRect(sourceRect);
            float indentLength = indentRect.x - sourceRect.x;

            return indentLength;
        }
        
        private float GetHelpBoxHeight()
        {
            return EditorGUIUtility.singleLineHeight * 2.0f;
        }
        
        private float GetPropertyHeight(SerializedProperty property)
        {
            return EditorGUI.GetPropertyHeight(property, includeChildren: true);
        }

        private void DrawDefaultPropertyAndHelpBox(Rect rect, SerializedProperty property, string message, MessageType messageType)
        {
            float indentLength = GetIndentLength(rect);
            Rect helpBoxRect = new Rect(
                rect.x + indentLength,
                rect.y,
                rect.width - indentLength,
                GetHelpBoxHeight());

            EditorGUI.HelpBox(helpBoxRect, message, MessageType.Warning);

            Rect propertyRect = new Rect(
                rect.x,
                rect.y + GetHelpBoxHeight(),
                rect.width,
                GetPropertyHeight(property));

            EditorGUI.PropertyField(propertyRect, property, true);
        }
        
        private Texture2D GetAssetPreview(SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (property.objectReferenceValue != null)
                {
                    Texture2D previewTexture = AssetPreview.GetAssetPreview(property.objectReferenceValue);
                    return previewTexture;
                }

                return null;
            }

            return null;
        }

        private Vector2 GetAssetPreviewSize(SerializedProperty property)
        {
            Texture2D previewTexture = GetAssetPreview(property);
            if (previewTexture == null)
            {
                return Vector2.zero;
            }
            else
            {
                int targetWidth = ShowAssetPreviewAttribute.DefaultWidth;
                int targetHeight = ShowAssetPreviewAttribute.DefaultHeight;

                if (attribute is ShowAssetPreviewAttribute showAssetPreviewAttribute)
                {
                    targetWidth = showAssetPreviewAttribute.Width;
                    targetHeight = showAssetPreviewAttribute.Height;
                }

                int width = Mathf.Clamp(targetWidth, 0, previewTexture.width);
                int height = Mathf.Clamp(targetHeight, 0, previewTexture.height);

                return new Vector2(width, height);
            }
        }
    }
}
