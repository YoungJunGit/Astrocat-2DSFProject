using UnityEditor;
using UnityEngine;

namespace CSV_To_ScriptableObject.Editor.Window
{
    /// <summary> Styles for the editor UI. </summary>
    public static class EditorUIStyles
    {
        public static void InitializeStyles(ref GUIStyle headerStyle, ref GUIStyle sectionStyle, ref GUIStyle boxStyle, ref GUIStyle errorStyle)
        {
            // Inicializace stylu záhlaví
            headerStyle ??= new(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                margin = new(0, 0, 8, 12),
                normal =
                {
                    textColor = EditorGUIUtility.isProSkin ? new(0.6f, 0.75f, 1.0f) : new Color(0.2f, 0.34f, 0.82f),
                },
                hover = new()
                {
                    textColor = EditorGUIUtility.isProSkin ? new(0.6f, 0.75f, 1.0f) : new Color(0.2f, 0.34f, 0.82f)
                },
            };

            // Inicializace stylu sekce
            sectionStyle ??= new(EditorStyles.boldLabel)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                margin = new(0, 0, 12, 12),
                normal =
                {
                    textColor = EditorGUIUtility.isProSkin ? new(0.3f, 0.95f, 0.7f) : new Color(0.1f, 0.5f, 0.4f),
                },
                padding = new(4, 0, 0, 0),
                richText = true,
                stretchWidth = true,
                border = new(0, 0, 0, 1),
            };

            // Inicializace stylu boxu
            boxStyle ??= new(EditorStyles.helpBox)
            {
                padding = new(10, 10, 10, 10),
                margin = new(0, 0, 10, 10),
                normal =
                {
                    textColor = Color.black,
                },
            };

            // Inicializace stylu chybové zprávy
            errorStyle ??= new(EditorStyles.label)
            {
                normal =
                {
                    textColor = Color.red,
                },
                fontStyle = FontStyle.Bold,
                wordWrap = true,
            };
        }
        public static GUIStyle GetStatusBoxStyle()
        {
            return new(EditorStyles.helpBox)
            {
                margin = new(4, 4, 8, 8),
                padding = new(8, 8, 8, 8),
            };
        }
        public static GUIStyle GetStatusIconStyle()
        {
            return new(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fixedWidth = 24,
                fixedHeight = 24,
            };
        }
        public static GUIStyle GetStatusMessageStyle(bool isError)
        {
            return new(EditorStyles.wordWrappedLabel)
            {
                padding = new(8, 4, 4, 4),
                margin = new(0, 0, 0, 0),
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft,
                normal =
                {
                    textColor = isError
                                        ? new(0.9f, 0.3f, 0.2f)
                                        : EditorGUIUtility.isProSkin
                                                ? new(0.9f, 0.9f, 0.9f)
                                                : new Color(0.2f, 0.2f, 0.2f),
                },
            };
        }
        public static GUIStyle GetTableHeaderStyle()
        {
            return new()
            {
                padding = new(5, 5, 5, 5),
                margin = new(0, 0, 0, 0),
                border = new(1, 1, 1, 1),
                normal =
                {
                    background = EditorGUIUtility.isProSkin
                                         ? MakeTexture(2, 2, new(0.3f, 0.3f, 0.3f))
                                         : MakeTexture(2, 2, new(0.7f, 0.7f, 0.7f)),
                },
            };
        }
        public static GUIStyle GetTableHeaderCellStyle()
        {
            return new(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new(5, 5, 5, 5),
                wordWrap = true,
                clipping = TextClipping.Clip,
                normal =
                {
                    textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black,
                },
                richText = true
            };
        }
        public static GUIStyle GetTableRowEvenStyle()
        {
            return new()
            {
                padding = new(0, 0, 0, 0),
                margin = new(0, 0, 0, 0),
                normal =
                {
                    background = EditorGUIUtility.isProSkin
                                         ? MakeTexture(2, 2, new(0.22f, 0.22f, 0.22f))
                                         : MakeTexture(2, 2, new(0.93f, 0.93f, 0.93f)),
                },
            };
        }
        public static GUIStyle GetTableRowOddStyle()
        {
            return new()
            {
                padding = new(0, 0, 0, 0),
                margin = new(0, 0, 0, 0),
                normal =
                {
                    background = EditorGUIUtility.isProSkin
                                         ? MakeTexture(2, 2, new(0.25f, 0.25f, 0.25f))
                                         : MakeTexture(2, 2, new(0.97f, 0.97f, 0.97f)),
                },
            };
        }
        public static GUIStyle GetTableCellStyle()
        {
            return new(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new(5, 5, 5, 5),
                wordWrap = true,
                clipping = TextClipping.Clip,
            };
        }
        private static Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;

            Texture2D texture = new(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
        public static GUIStyle GetPaginationStyle()
        {
            return new(EditorStyles.miniButton)
            {
                fixedWidth = 25,
                alignment = TextAnchor.MiddleCenter,
                margin = new RectOffset(2, 2, 2, 2)
            };
        }
    }
}