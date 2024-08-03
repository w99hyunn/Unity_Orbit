// Designed by KINEMATION, 2024.

using KINEMATION.KAnimationCore.Runtime.Rig;

using KINEMATION.FPSAnimationFramework.Runtime.Core;
using KINEMATION.FPSAnimationFramework.Runtime.Playables;
using KINEMATION.FPSAnimationFramework.Runtime.Recoil;
using KINEMATION.KAnimationCore.Runtime.Input;

using System;
using System.IO;
using KINEMATION.FPSAnimationFramework.Runtime.Camera;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace KINEMATION.FPSAnimationFramework.Editor.Tools
{
    public class ObjectEditor<T> where T : Object
    {
        public Object targetObject;
        public UnityEditor.Editor editor;
        
        public string objectName;
        public bool allowSceneObjects;

        private Object _cachedObject;
        private bool _isExpanded;
        
        public ObjectEditor(string name, bool useSceneObjects)
        {
            objectName = name;
            allowSceneObjects = useSceneObjects;
        }

        public T GetObject()
        {
            return (T) targetObject;
        }

        public void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            
            targetObject = (T) EditorGUILayout.ObjectField(objectName, targetObject, typeof(T), allowSceneObjects);
            
            if (GUILayout.Button("Create"))
            {
                targetObject = Activator.CreateInstance<T>();
                targetObject.name = objectName.Replace(" ", string.Empty);
            }

            if (editor != null && GUILayout.Button("Show"))
            {
                _isExpanded = !_isExpanded;
            }
            
            EditorGUILayout.EndHorizontal();

            if (targetObject != _cachedObject)
            {
                if (targetObject == null)
                {
                    editor = null;
                    return;
                }
                
                editor = EditorUtility.IsPersistent(targetObject) ? null : UnityEditor.Editor.CreateEditor(targetObject);
            }
            
            _cachedObject = targetObject;
            if (!_isExpanded || editor == null) return;

            var style = GUI.skin.box;
            style.padding = new RectOffset(15, 5, 5, 5);
            
            EditorGUILayout.BeginVertical(style);
            editor.OnInspectorGUI();
            EditorGUILayout.EndVertical();
        }
    }
    
    public class FPSAnimatorWizard : EditorWindow
    {
        //~ Character Tab
        private GameObject _character;
        private GameObject _skeletonRoot;
        private GameObject _cameraParent;

        private RuntimeAnimatorController _controller;
        private KRig _rigAsset;
        private UserInputConfig _inputConfig;
        
        // Dynamically created and modified properties.
        private ObjectEditor<AvatarMask> _upperBodyMask = new("Upper Body", false);
        //~ Character Tab
        
        private string _savePath = "Assets/FPS ANIMATOR";
        private Vector2 _scrollPosition;
        
        [MenuItem("Window/FPS ANIMATOR/Wizard")]
        public static void ShowWindow()
        {
            GetWindow<FPSAnimatorWizard>("FPS ANIMATOR Wizard");
        }

        private T InitializeComponent<T>(GameObject parent) where T : Behaviour
        {
            var component = parent.GetComponent<T>();

            if (component == null)
            {
                component = parent.AddComponent<T>();
            }

            return component;
        }

        private void SetupCharacter()
        {
            var animator = InitializeComponent<Animator>(_character);
            animator.runtimeAnimatorController = _controller;
            
            InitializeComponent<Camera>(_cameraParent);
            var fpsCamera = InitializeComponent<FPSCameraController>(_cameraParent);
            fpsCamera.transform.rotation = animator.transform.rotation;
            
            var rigComponent = InitializeComponent<KRigComponent>(_skeletonRoot);
            
            var fpsAnimator = InitializeComponent<FPSAnimator>(_character);
            fpsAnimator.CreateIkElements();
            
            var inputController = InitializeComponent<UserInputController>(_character);
            inputController.inputConfig = _inputConfig;
            
            var playablesController = InitializeComponent<FPSPlayablesController>(_character);
            playablesController.upperBodyMask = _upperBodyMask.GetObject();

            InitializeComponent<RecoilAnimation>(_character);

            string path = $"{_savePath}/{_character.name}_Assets";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            if (_rigAsset == null)
            {
                _rigAsset = ScriptableObject.CreateInstance<KRig>();
                _rigAsset.inputConfig = _inputConfig;
                _rigAsset.ImportRig(rigComponent);

                _rigAsset.targetAnimator = _controller;
                
                _rigAsset.rigCurves.Add(FPSANames.Curve_Overlay);
                _rigAsset.rigCurves.Add(FPSANames.Curve_WeaponBoneWeight);
                _rigAsset.rigCurves.Add(FPSANames.Curve_MaskAttachHand);
                
                string filePath = AssetDatabase.GenerateUniqueAssetPath($"{path}/Rig_{_character.name}.asset");
                AssetDatabase.CreateAsset(_rigAsset, filePath);
                AssetDatabase.SaveAssets();
            }
            
            if (!EditorUtility.IsPersistent(_upperBodyMask.GetObject()))
            {
                string filePath = AssetDatabase.GenerateUniqueAssetPath($"{path}/{_character.name}_{_upperBodyMask.objectName}.mask");
                AssetDatabase.CreateAsset(_upperBodyMask.GetObject(), filePath);
                AssetDatabase.SaveAssets();
            }
            
            EditorUtility.SetDirty(_character);
        }

        private void RenderCharacterSetup()
        {
            _character = (GameObject) EditorGUILayout.ObjectField("Character", _character, 
                typeof(GameObject), true);
            
            if (_character == null)
            {
                EditorGUILayout.HelpBox("Select Character", MessageType.Warning);
                return;
            }
            
            _skeletonRoot = (GameObject) EditorGUILayout.ObjectField("Skeleton Root", _skeletonRoot, 
                typeof(GameObject), true);

            if (_skeletonRoot == null)
            {
                EditorGUILayout.HelpBox("Select Skeleton Root", MessageType.Warning);
                return;
            }
            
            _cameraParent = (GameObject) EditorGUILayout.ObjectField("Camera Parent", _cameraParent, 
                typeof(GameObject), true);
            
            if (_cameraParent == null)
            {
                EditorGUILayout.HelpBox("Select Camera Parent", MessageType.Warning);
                return;
            }
            
            _controller = (RuntimeAnimatorController) EditorGUILayout.ObjectField("Animator Controller", _controller, 
                typeof(RuntimeAnimatorController), false);
            
            if (_controller == null)
            {
                EditorGUILayout.HelpBox("Select Animator Controller", MessageType.Warning);
                return;
            }
            
            _inputConfig = (UserInputConfig) EditorGUILayout.ObjectField("Input Config", _inputConfig, 
                typeof(UserInputConfig), false);

            if (_inputConfig == null)
            {
                EditorGUILayout.HelpBox("Select Input Config", MessageType.Warning);
                return;
            }
            
            _rigAsset = (KRig) EditorGUILayout.ObjectField("Rig Asset", _rigAsset, 
                typeof(KRig), false);
            
            _upperBodyMask.OnInspectorGUI();
            
            if (GUILayout.Button("Setup Character")) SetupCharacter();
        }

        private void OnEnable()
        {
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField("Save Path");
            
            if (GUILayout.Button(_savePath))
            {
                string path = EditorUtility.OpenFolderPanel("Select Directory", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                {
                    _savePath = path;
                    string assetsPath = Application.dataPath;
                    _savePath = $"Assets{path.Substring(assetsPath.Length)}";
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            RenderCharacterSetup();
            EditorGUILayout.EndScrollView();
        }
    }
}
