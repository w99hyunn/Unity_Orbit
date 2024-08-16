#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Verpha.HierarchyDesigner
{
    public class HierarchyDesigner_Window_Folder : EditorWindow
    {
        #region Properties
        #region GUI
        private Vector2 outerScroll;
        private Vector2 innerScroll;
        private GUIStyle headerGUIStyle;
        private GUIStyle contentGUIStyle;
        private GUIStyle messageGUIStyle;
        private GUIStyle outerBackgroundGUIStyle;
        private GUIStyle innerBackgroundGUIStyle;
        private GUIStyle contentBackgroundGUIStyle;
        #endregion
        #region Const
        private readonly int[] fontSizeOptions = new int[15];
        private const float defaultButtonWidth = 60;
        private const float moveFolderButtonWidth = 25;
        private const float folderCreationLabelWidth = 90;
        private const float extraFolderLabelWidthOffset = 20;
        #endregion
        #region Temporary Values
        private Dictionary<string, HierarchyDesigner_Configurable_Folder.HierarchyDesigner_FolderData> tempFolders;
        private List<string> foldersOrder;
        private static bool hasModifiedChanges = false;
        #endregion
        #region Folder Creation Values
        private string newFolderName = "";
        private Color newTextColor = Color.white;
        private int newFontSize = 12;
        private FontStyle newFontStyle = FontStyle.Normal;
        private Color newFolderIconColor = Color.white;
        private HierarchyDesigner_Configurable_Folder.FolderImageType newFolderImageType = HierarchyDesigner_Configurable_Folder.FolderImageType.Default;
        #endregion
        #region Folder Global Fields Values
        private Color tempGlobalTextColor = Color.white;
        private int tempGlobalFontSize = 12;
        private FontStyle tempGlobalFontStyle = FontStyle.Normal;
        private Color tempGlobalFolderIconColor = Color.white;
        private HierarchyDesigner_Configurable_Folder.FolderImageType tempGlobalFolderImageType = HierarchyDesigner_Configurable_Folder.FolderImageType.Default;
        #endregion
        #endregion

        #region Window
        [MenuItem(HierarchyDesigner_Shared_MenuItems.Group_Folder + "/Folder Manager Window", false, HierarchyDesigner_Shared_MenuItems.LayerTwo)]
        public static void OpenWindow()
        {
            HierarchyDesigner_Window_Folder window = GetWindow<HierarchyDesigner_Window_Folder>("Hierarchy Folder Manager");
            window.minSize = new Vector2(400, 200);
        }
        #endregion

        #region Initialization
        private void OnEnable()
        {
            InitFontSizeOptions();
            LoadTempValues();
        }

        private void InitFontSizeOptions()
        {
            for (int i = 0; i < fontSizeOptions.Length; i++)
            {
                fontSizeOptions[i] = 7 + i;
            }
        }

        private void LoadTempValues()
        {
            tempFolders = HierarchyDesigner_Configurable_Folder.GetAllFoldersData(true);
            foldersOrder = new List<string>(tempFolders.Keys);
        }
        #endregion

        #region Main
        private void OnGUI()
        {
            HierarchyDesigner_Shared_GUI.DrawGUIStyles(out headerGUIStyle, out contentGUIStyle, out messageGUIStyle, out outerBackgroundGUIStyle, out innerBackgroundGUIStyle, out contentBackgroundGUIStyle);

            #region Header
            EditorGUILayout.BeginVertical(outerBackgroundGUIStyle);
            EditorGUILayout.LabelField("Folders Manager", headerGUIStyle);
            GUILayout.Space(8);
            #endregion

            outerScroll = EditorGUILayout.BeginScrollView(outerScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginVertical(innerBackgroundGUIStyle);

            #region Body
            #region Folder Creation
            #region Fields
            EditorGUILayout.BeginVertical(contentBackgroundGUIStyle);
            EditorGUILayout.LabelField("Folder Creation:", contentGUIStyle);
            GUILayout.Space(4);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", GUILayout.Width(folderCreationLabelWidth));
            newFolderName = EditorGUILayout.TextField(newFolderName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Text Color", GUILayout.Width(folderCreationLabelWidth));
            newTextColor = EditorGUILayout.ColorField(newTextColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            string[] newFontSizeOptionsStrings = Array.ConvertAll(fontSizeOptions, x => x.ToString());
            int newFontSizeIndex = Array.IndexOf(fontSizeOptions, newFontSize);
            EditorGUILayout.LabelField("Font Size", GUILayout.Width(folderCreationLabelWidth));
            newFontSize = fontSizeOptions[EditorGUILayout.Popup(newFontSizeIndex, newFontSizeOptionsStrings)];
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Font Style", GUILayout.Width(folderCreationLabelWidth));
            newFontStyle = (FontStyle)EditorGUILayout.EnumPopup(newFontStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Image Color", GUILayout.Width(folderCreationLabelWidth));
            newFolderIconColor = EditorGUILayout.ColorField(newFolderIconColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Image Type", GUILayout.Width(folderCreationLabelWidth));
            if (GUILayout.Button(HierarchyDesigner_Configurable_Folder.GetFolderImageTypeDisplayName(newFolderImageType), EditorStyles.popup))
            {
                ShowFolderImageTypePopup();
            }
            EditorGUILayout.EndHorizontal();
            #endregion
            #region Button
            GUILayout.Space(4);
            if (GUILayout.Button("Create Folder", GUILayout.Height(25)))
            {
                if (IsFolderNameValid(newFolderName))
                {
                    CreateFolder(newFolderName, newTextColor, newFontSize, newFontStyle, newFolderIconColor, newFolderImageType);
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Folder Name", "Folder name is either duplicate or invalid.", "OK");
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(4);
            #endregion
            #endregion

            #region Folder's Global Fields and List
            if (tempFolders.Count > 0)
            {
                #region Global Fields
                EditorGUILayout.BeginVertical(contentBackgroundGUIStyle);
                EditorGUILayout.LabelField("Folders' Global Fields", contentGUIStyle);
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                tempGlobalTextColor = EditorGUILayout.ColorField(tempGlobalTextColor, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck()) { UpdateGlobalFolderTextColor(tempGlobalTextColor); }
                EditorGUI.BeginChangeCheck();
                string[] tempFontSizeOptionsStrings = Array.ConvertAll(fontSizeOptions, x => x.ToString());
                int tempFontSizeIndex = Array.IndexOf(fontSizeOptions, tempGlobalFontSize);
                tempGlobalFontSize = fontSizeOptions[EditorGUILayout.Popup(tempFontSizeIndex, tempFontSizeOptionsStrings, GUILayout.Width(50))];
                if (EditorGUI.EndChangeCheck()) { UpdateGlobalFolderFontSize(tempGlobalFontSize); }
                EditorGUI.BeginChangeCheck();
                tempGlobalFontStyle = (FontStyle)EditorGUILayout.EnumPopup(tempGlobalFontStyle, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck()) { UpdateGlobalFolderFontStyle(tempGlobalFontStyle); }
                EditorGUI.BeginChangeCheck();
                tempGlobalFolderIconColor = EditorGUILayout.ColorField(tempGlobalFolderIconColor, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck()) { UpdateGlobalFolderIconColor(tempGlobalFolderIconColor); }
                if (GUILayout.Button(HierarchyDesigner_Configurable_Folder.GetFolderImageTypeDisplayName(tempGlobalFolderImageType), EditorStyles.popup, GUILayout.MinWidth(125), GUILayout.ExpandWidth(true))) { ShowFolderImageTypePopupGlobal(); }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                GUILayout.Space(4);
                #endregion

                #region Folder List
                EditorGUILayout.BeginVertical(contentBackgroundGUIStyle);
                EditorGUILayout.LabelField("Folders' List", contentGUIStyle);
                innerScroll = EditorGUILayout.BeginScrollView(innerScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                GUILayout.Space(10);
                int index = 1;
                for (int i = 0; i < foldersOrder.Count; i++)
                {
                    string key = foldersOrder[i];
                    DrawFolders(index, key, tempFolders[key], i, foldersOrder.Count);
                    index++;
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
                GUILayout.Space(4);
                #endregion
            }
            else
            {
                EditorGUILayout.LabelField("No folders found. Please create a new folder.", messageGUIStyle);
            }
            EditorGUILayout.EndVertical();
            #endregion
            #endregion

            #region Footer
            if (GUILayout.Button("Update and Save Settings", GUILayout.Height(30)))
            {
                SaveSettings();
            }
            #endregion

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void OnDestroy()
        {
            if (hasModifiedChanges)
            {
                bool shouldSave = EditorUtility.DisplayDialog("Folder(s) Have Been Modified!",
                    "Do you want to save the changes you made to the folders?",
                    "Save", "Don't Save");

                if (shouldSave)
                {
                    SaveSettings();
                }
            }
            hasModifiedChanges = false;
        }

        private void SaveSettings()
        {
            HierarchyDesigner_Configurable_Folder.ApplyChangesToFolders(tempFolders, foldersOrder);
            HierarchyDesigner_Configurable_Folder.SaveSettings();
            HierarchyDesigner_Manager_GameObject.ClearFolderCache();
            hasModifiedChanges = false;
        }
        #endregion

        #region GUI
        #region Folder Image Type
        private void ShowFolderImageTypePopup()
        {
            GenericMenu menu = new GenericMenu();
            Dictionary<string, List<string>> groupedTypes = HierarchyDesigner_Configurable_Folder.GetFolderImageTypesGrouped();
            foreach (KeyValuePair<string, List<string>> group in groupedTypes)
            {
                foreach (string typeName in group.Value)
                {
                    menu.AddItem(new GUIContent($"{group.Key}/{typeName}"), typeName == HierarchyDesigner_Configurable_Folder.GetFolderImageTypeDisplayName(newFolderImageType), OnFolderImageTypeSelected, typeName);
                }
            }
            menu.ShowAsContext();
        }

        private void ShowFolderImageTypePopupGlobal()
        {
            GenericMenu menu = new GenericMenu();
            Dictionary<string, List<string>> groupedTypes = HierarchyDesigner_Configurable_Folder.GetFolderImageTypesGrouped();
            foreach (KeyValuePair<string, List<string>> group in groupedTypes)
            {
                foreach (string typeName in group.Value)
                {
                    menu.AddItem(new GUIContent($"{group.Key}/{typeName}"), typeName == HierarchyDesigner_Configurable_Folder.GetFolderImageTypeDisplayName(tempGlobalFolderImageType), OnFolderImageTypeGlobalSelected, typeName);
                }
            }
            menu.ShowAsContext();
        }

        private void ShowFolderImageTypePopupForFolder(HierarchyDesigner_Configurable_Folder.HierarchyDesigner_FolderData folderData)
        {
            GenericMenu menu = new GenericMenu();
            Dictionary<string, List<string>> groupedTypes = HierarchyDesigner_Configurable_Folder.GetFolderImageTypesGrouped();
            foreach (KeyValuePair<string, List<string>> group in groupedTypes)
            {
                foreach (string typeName in group.Value)
                {
                    menu.AddItem(new GUIContent($"{group.Key}/{typeName}"), typeName == HierarchyDesigner_Configurable_Folder.GetFolderImageTypeDisplayName(folderData.ImageType), OnFolderImageTypeForFolderSelected, new KeyValuePair<HierarchyDesigner_Configurable_Folder.HierarchyDesigner_FolderData, string>(folderData, typeName));
                }
            }
            menu.ShowAsContext();
        }

        private void OnFolderImageTypeSelected(object imageTypeObj)
        {
            string typeName = (string)imageTypeObj;
            newFolderImageType = HierarchyDesigner_Configurable_Folder.ParseFolderImageType(typeName);
        }

        private void OnFolderImageTypeGlobalSelected(object imageTypeObj)
        {
            string typeName = (string)imageTypeObj;
            tempGlobalFolderImageType = HierarchyDesigner_Configurable_Folder.ParseFolderImageType(typeName);
            UpdateGlobalFolderImageType(tempGlobalFolderImageType);
        }

        private void OnFolderImageTypeForFolderSelected(object folderDataAndTypeObj)
        {
            KeyValuePair<HierarchyDesigner_Configurable_Folder.HierarchyDesigner_FolderData, string> folderDataAndType = (KeyValuePair<HierarchyDesigner_Configurable_Folder.HierarchyDesigner_FolderData, string>)folderDataAndTypeObj;
            folderDataAndType.Key.ImageType = HierarchyDesigner_Configurable_Folder.ParseFolderImageType(folderDataAndType.Value);
        }
        #endregion
        #endregion

        #region Operations
        private bool IsFolderNameValid(string folderName)
        {
            return !string.IsNullOrEmpty(folderName) && !tempFolders.TryGetValue(folderName, out _);
        }

        private void CreateFolder(string folderName, Color textColor, int fontSize, FontStyle fontStyle, Color ImageColor, HierarchyDesigner_Configurable_Folder.FolderImageType imageType)
        {
            HierarchyDesigner_Configurable_Folder.HierarchyDesigner_FolderData newFolderData = new HierarchyDesigner_Configurable_Folder.HierarchyDesigner_FolderData
            {
                Name = folderName,
                TextColor = textColor,
                FontSize = fontSize,
                FontStyle = fontStyle,
                ImageColor = ImageColor,
                ImageType = imageType
            };
            tempFolders[folderName] = newFolderData;
            foldersOrder.Add(folderName);
            newFolderName = "";
            newTextColor = Color.white;
            newFontSize = 12;
            newFontStyle = FontStyle.Normal;
            newFolderIconColor = Color.white;
            newFolderImageType = HierarchyDesigner_Configurable_Folder.FolderImageType.Default;
            hasModifiedChanges = true;
            GUI.FocusControl(null);
        }

        private void DrawFolders(int index, string key, HierarchyDesigner_Configurable_Folder.HierarchyDesigner_FolderData folderData, int position, int totalItems)
        {
            float folderLabelWidth = HierarchyDesigner_Shared_GUI.CalculateMaxLabelWidth(tempFolders.Keys);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField($"{index}) {folderData.Name}", GUILayout.Width(folderLabelWidth + extraFolderLabelWidthOffset));
            EditorGUI.BeginChangeCheck();
            folderData.TextColor = EditorGUILayout.ColorField(folderData.TextColor, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
            string[] fontSizeOptionsStrings = Array.ConvertAll(fontSizeOptions, x => x.ToString());
            int fontSizeIndex = Array.IndexOf(fontSizeOptions, folderData.FontSize);
            if (fontSizeIndex == -1) { fontSizeIndex = 5; }
            folderData.FontSize = fontSizeOptions[EditorGUILayout.Popup(fontSizeIndex, fontSizeOptionsStrings, GUILayout.Width(50))];
            folderData.FontStyle = (FontStyle)EditorGUILayout.EnumPopup(folderData.FontStyle, GUILayout.Width(110));
            folderData.ImageColor = EditorGUILayout.ColorField(folderData.ImageColor, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
            if (GUILayout.Button(HierarchyDesigner_Configurable_Folder.GetFolderImageTypeDisplayName(folderData.ImageType), EditorStyles.popup, GUILayout.MinWidth(125), GUILayout.ExpandWidth(true))) { ShowFolderImageTypePopupForFolder(folderData); }
            if (EditorGUI.EndChangeCheck()) { hasModifiedChanges = true; }

            if (GUILayout.Button("↑", GUILayout.Width(moveFolderButtonWidth)) && position > 0)
            {
                MoveFolder(position, position - 1);
            }
            if (GUILayout.Button("↓", GUILayout.Width(moveFolderButtonWidth)) && position < totalItems - 1)
            {
                MoveFolder(position, position + 1);
            }
            if (GUILayout.Button("Create", GUILayout.Width(defaultButtonWidth)))
            {
                CreateFolderGameObject(folderData);
            }
            if (GUILayout.Button("Remove", GUILayout.Width(defaultButtonWidth)))
            {
                RemoveFolder(key);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void MoveFolder(int indexA, int indexB)
        {
            string keyA = foldersOrder[indexA];
            string keyB = foldersOrder[indexB];

            foldersOrder[indexA] = keyB;
            foldersOrder[indexB] = keyA;
            hasModifiedChanges = true;
        }

        private void CreateFolderGameObject(HierarchyDesigner_Configurable_Folder.HierarchyDesigner_FolderData folderData)
        {
            GameObject folder = new GameObject(folderData.Name);
            folder.AddComponent<HierarchyDesignerFolder>();
            Undo.RegisterCreatedObjectUndo(folder, $"Create {folderData.Name}");

            Texture2D inspectorIcon = HierarchyDesigner_Shared_Resources.FolderInspectorIcon;
            if (inspectorIcon != null)
            {
                EditorGUIUtility.SetIconForObject(folder, inspectorIcon);
            }
        }

        private void RemoveFolder(string folderName)
        {
            if (tempFolders.TryGetValue(folderName, out _))
            {
                tempFolders.Remove(folderName);
                foldersOrder.Remove(folderName);
                hasModifiedChanges = true;
                GUIUtility.ExitGUI();
            }
        }

        #region Global Fields Methods
        private void UpdateGlobalFolderTextColor(Color color)
        {
            foreach (HierarchyDesigner_Configurable_Folder.HierarchyDesigner_FolderData folder in tempFolders.Values)
            {
                folder.TextColor = color;
            }
            hasModifiedChanges = true;
        }

        private void UpdateGlobalFolderFontSize(int size)
        {
            foreach (HierarchyDesigner_Configurable_Folder.HierarchyDesigner_FolderData folder in tempFolders.Values)
            {
                folder.FontSize = size;
            }
            hasModifiedChanges = true;
        }

        private void UpdateGlobalFolderFontStyle(FontStyle style)
        {
            foreach (HierarchyDesigner_Configurable_Folder.HierarchyDesigner_FolderData folder in tempFolders.Values)
            {
                folder.FontStyle = style;
            }
            hasModifiedChanges = true;
        }

        private void UpdateGlobalFolderIconColor(Color color)
        {
            foreach (HierarchyDesigner_Configurable_Folder.HierarchyDesigner_FolderData folder in tempFolders.Values)
            {
                folder.ImageColor = color;
            }
            hasModifiedChanges = true;
        }

        private void UpdateGlobalFolderImageType(HierarchyDesigner_Configurable_Folder.FolderImageType imageType)
        {
            foreach (HierarchyDesigner_Configurable_Folder.HierarchyDesigner_FolderData folder in tempFolders.Values)
            {
                folder.ImageType = imageType;
            }
            hasModifiedChanges = true;
        }
        #endregion
        #endregion
    }
}
#endif