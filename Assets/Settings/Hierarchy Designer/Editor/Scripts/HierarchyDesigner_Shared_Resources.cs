#if UNITY_EDITOR
using UnityEngine;

namespace Verpha.HierarchyDesigner
{
    public static class HierarchyDesigner_Shared_Resources
    {
        #region General
        private static readonly string defaultTextureName = "Hierarchy Designer Default Texture";
        private static readonly string lockIconName = "Hierarchy Designer Lock Icon";

        public static readonly Texture2D DefaultTexture = HierarchyDesigner_Shared_TextureLoader.LoadTexture(defaultTextureName);
        public static readonly Texture2D LockIcon = HierarchyDesigner_Shared_TextureLoader.LoadTexture(lockIconName);
        #endregion

        #region Tree Branches
        private static readonly string treeBranchIconIName = "Hierarchy Designer Tree Branch Icon I";
        private static readonly string treeBranchIconLName = "Hierarchy Designer Tree Branch Icon L";
        private static readonly string treeBranchIconTName = "Hierarchy Designer Tree Branch Icon T";
        private static readonly string treeBranchIconTerminalBudName = "Hierarchy Designer Tree Branch Icon Terminal Bud";

        public static readonly Texture2D TreeBranchIcon_I = HierarchyDesigner_Shared_TextureLoader.LoadTexture(treeBranchIconIName);
        public static readonly Texture2D TreeBranchIcon_L = HierarchyDesigner_Shared_TextureLoader.LoadTexture(treeBranchIconLName);
        public static readonly Texture2D TreeBranchIcon_T = HierarchyDesigner_Shared_TextureLoader.LoadTexture(treeBranchIconTName);
        public static readonly Texture2D TreeBranchIcon_TerminalBud = HierarchyDesigner_Shared_TextureLoader.LoadTexture(treeBranchIconTerminalBudName);
        #endregion

        #region Folder Images
        private static readonly string folderDefaultIconName = "Hierarchy Designer Folder Icon Default";
        private static readonly string folderDefaultOutlineIconName = "Hierarchy Designer Folder Icon Default Outline";
        private static readonly string folderModernIIconName = "Hierarchy Designer Folder Icon Modern I";
        private static readonly string folderModernIIIconName = "Hierarchy Designer Folder Icon Modern II";
        private static readonly string folderModernIIIIconName = "Hierarchy Designer Folder Icon Modern III";
        private static readonly string folderModernOutlineIconName = "Hierarchy Designer Folder Icon Modern Outline";
        private static readonly string folderInspectorIconName = "Hierarchy Designer Folder Icon Inspector";

        public static readonly Texture2D FolderDefaultIcon = HierarchyDesigner_Shared_TextureLoader.LoadTexture(folderDefaultIconName);
        public static readonly Texture2D FolderDefaultOutlineIcon = HierarchyDesigner_Shared_TextureLoader.LoadTexture(folderDefaultOutlineIconName);
        public static readonly Texture2D FolderModernIIcon = HierarchyDesigner_Shared_TextureLoader.LoadTexture(folderModernIIconName);
        public static readonly Texture2D FolderModernIIIcon = HierarchyDesigner_Shared_TextureLoader.LoadTexture(folderModernIIIconName);
        public static readonly Texture2D FolderModernIIIIcon = HierarchyDesigner_Shared_TextureLoader.LoadTexture(folderModernIIIIconName);
        public static readonly Texture2D FolderModernOutlineIcon = HierarchyDesigner_Shared_TextureLoader.LoadTexture(folderModernOutlineIconName);
        public static readonly Texture2D FolderInspectorIcon = HierarchyDesigner_Shared_TextureLoader.LoadTexture(folderInspectorIconName);

        public static Texture2D FolderImageType(HierarchyDesigner_Configurable_Folder.FolderImageType folderImageType)
        {
            switch (folderImageType)
            {
                case HierarchyDesigner_Configurable_Folder.FolderImageType.DefaultOutline:
                    return FolderDefaultOutlineIcon;
                case HierarchyDesigner_Configurable_Folder.FolderImageType.ModernI:
                    return FolderModernIIcon;
                case HierarchyDesigner_Configurable_Folder.FolderImageType.ModernII:
                    return FolderModernIIIcon;
                case HierarchyDesigner_Configurable_Folder.FolderImageType.ModernIII:
                    return FolderModernIIIIcon;
                case HierarchyDesigner_Configurable_Folder.FolderImageType.ModernOutline:
                    return FolderModernOutlineIcon;
                default:
                    return FolderDefaultIcon;
            }
        }
        #endregion

        #region Separator Images
        private static readonly string separatorBackgroundImageDefaultName = "Hierarchy Designer Separator Background Image Default";
        private static readonly string separatorBackgroundImageDefaultFadedBottomName = "Hierarchy Designer Separator Background Image Default Faded Bottom";
        private static readonly string separatorBackgroundImageDefaultFadedLeftName = "Hierarchy Designer Separator Background Image Default Faded Left";
        private static readonly string separatorBackgroundImageDefaultFadedSidewaysName = "Hierarchy Designer Separator Background Image Default Faded Sideways";
        private static readonly string separatorBackgroundImageDefaultFadedRightName = "Hierarchy Designer Separator Background Image Default Faded Right";
        private static readonly string separatorBackgroundImageDefaultFadedTopName = "Hierarchy Designer Separator Background Image Default Faded Top";
        private static readonly string separatorBackgroundImageClassicIName = "Hierarchy Designer Separator Background Image Classic I";
        private static readonly string separatorBackgroundImageClassicIIName = "Hierarchy Designer Separator Background Image Classic II";
        private static readonly string separatorBackgroundImageModernIName = "Hierarchy Designer Separator Background Image Modern I";
        private static readonly string separatorBackgroundImageModernIIName = "Hierarchy Designer Separator Background Image Modern II";
        private static readonly string separatorBackgroundImageModernIIIName = "Hierarchy Designer Separator Background Image Modern III";
        private static readonly string separatorBackgroundImageNeoIName = "Hierarchy Designer Separator Background Image Neo I";
        private static readonly string separatorBackgroundImageNeoIIName = "Hierarchy Designer Separator Background Image Neo II";
        private static readonly string separatorBackgroundImageNextGenIName = "Hierarchy Designer Separator Background Image Next-Gen I";
        private static readonly string separatorBackgroundImageNextGenIIName = "Hierarchy Designer Separator Background Image Next-Gen II";
        private static readonly string separatorInspectorIconName = "Hierarchy Designer Separator Icon Inspector";

        public static readonly Texture2D SeparatorBackgroundImageDefault = HierarchyDesigner_Shared_TextureLoader.LoadTexture(separatorBackgroundImageDefaultName);
        public static readonly Texture2D SeparatorBackgroundImageDefaultFadedBottom = HierarchyDesigner_Shared_TextureLoader.LoadTexture(separatorBackgroundImageDefaultFadedBottomName);
        public static readonly Texture2D SeparatorBackgroundImageDefaultFadedLeft = HierarchyDesigner_Shared_TextureLoader.LoadTexture(separatorBackgroundImageDefaultFadedLeftName);
        public static readonly Texture2D SeparatorBackgroundImageDefaultFadedSideways = HierarchyDesigner_Shared_TextureLoader.LoadTexture(separatorBackgroundImageDefaultFadedSidewaysName);
        public static readonly Texture2D SeparatorBackgroundImageDefaultFadedRight = HierarchyDesigner_Shared_TextureLoader.LoadTexture(separatorBackgroundImageDefaultFadedRightName);
        public static readonly Texture2D SeparatorBackgroundImageDefaultFadedTop = HierarchyDesigner_Shared_TextureLoader.LoadTexture(separatorBackgroundImageDefaultFadedTopName);
        public static readonly Texture2D SeparatorBackgroundImageClassicI = HierarchyDesigner_Shared_TextureLoader.LoadTexture(separatorBackgroundImageClassicIName);
        public static readonly Texture2D SeparatorBackgroundImageClassicII = HierarchyDesigner_Shared_TextureLoader.LoadTexture(separatorBackgroundImageClassicIIName);
        public static readonly Texture2D SeparatorBackgroundImageModernI = HierarchyDesigner_Shared_TextureLoader.LoadTexture(separatorBackgroundImageModernIName);
        public static readonly Texture2D SeparatorBackgroundImageModernII = HierarchyDesigner_Shared_TextureLoader.LoadTexture(separatorBackgroundImageModernIIName);
        public static readonly Texture2D SeparatorBackgroundImageModernIII = HierarchyDesigner_Shared_TextureLoader.LoadTexture(separatorBackgroundImageModernIIIName);
        public static readonly Texture2D SeparatorBackgroundImageNeoI = HierarchyDesigner_Shared_TextureLoader.LoadTexture(separatorBackgroundImageNeoIName);
        public static readonly Texture2D SeparatorBackgroundImageNeoII = HierarchyDesigner_Shared_TextureLoader.LoadTexture(separatorBackgroundImageNeoIIName);
        public static readonly Texture2D SeparatorBackgroundImageNextGenI = HierarchyDesigner_Shared_TextureLoader.LoadTexture(separatorBackgroundImageNextGenIName);
        public static readonly Texture2D SeparatorBackgroundImageNextGenII = HierarchyDesigner_Shared_TextureLoader.LoadTexture(separatorBackgroundImageNextGenIIName);
        public static readonly Texture2D SeparatorInspectorIcon = HierarchyDesigner_Shared_TextureLoader.LoadTexture(separatorInspectorIconName);

        public static Texture2D SeparatorImageType(HierarchyDesigner_Configurable_Separator.SeparatorImageType separatorImageType)
        {
            switch (separatorImageType)
            {
                case HierarchyDesigner_Configurable_Separator.SeparatorImageType.DefaultFadedBottom:
                    return SeparatorBackgroundImageDefaultFadedBottom;
                case HierarchyDesigner_Configurable_Separator.SeparatorImageType.DefaultFadedLeft:
                    return SeparatorBackgroundImageDefaultFadedLeft;
                case HierarchyDesigner_Configurable_Separator.SeparatorImageType.DefaultFadedSideways:
                    return SeparatorBackgroundImageDefaultFadedSideways;
                case HierarchyDesigner_Configurable_Separator.SeparatorImageType.DefaultFadedRight:
                    return SeparatorBackgroundImageDefaultFadedRight;
                case HierarchyDesigner_Configurable_Separator.SeparatorImageType.DefaultFadedTop:
                    return SeparatorBackgroundImageDefaultFadedTop;
                case HierarchyDesigner_Configurable_Separator.SeparatorImageType.ClassicI:
                    return SeparatorBackgroundImageClassicI;
                case HierarchyDesigner_Configurable_Separator.SeparatorImageType.ClassicII:
                    return SeparatorBackgroundImageClassicII;
                case HierarchyDesigner_Configurable_Separator.SeparatorImageType.ModernI:
                    return SeparatorBackgroundImageModernI;
                case HierarchyDesigner_Configurable_Separator.SeparatorImageType.ModernII:
                    return SeparatorBackgroundImageModernII;
                case HierarchyDesigner_Configurable_Separator.SeparatorImageType.ModernIII:
                    return SeparatorBackgroundImageModernIII;
                case HierarchyDesigner_Configurable_Separator.SeparatorImageType.NeoI:
                    return SeparatorBackgroundImageNeoI;
                case HierarchyDesigner_Configurable_Separator.SeparatorImageType.NeoII:
                    return SeparatorBackgroundImageNeoII;
                case HierarchyDesigner_Configurable_Separator.SeparatorImageType.NextGenI:
                    return SeparatorBackgroundImageNextGenI;
                case HierarchyDesigner_Configurable_Separator.SeparatorImageType.NextGenII:
                    return SeparatorBackgroundImageNextGenII;
                default:
                    return SeparatorBackgroundImageDefault;
            }
        }
        #endregion
    }
}
#endif