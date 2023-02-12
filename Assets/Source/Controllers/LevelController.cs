using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelController : MonoBase
{
    public bool initializeOnAwake;
#if UNITY_EDITOR
    public int forceLevelIndex = -1;
#endif
    [SerializeField] private Object jsonLevels;
    [SerializeField] private GridInitializer _gridInitializer;
    [SerializeField] private UnityEvent<LevelModel> onLevelLoaded;
    [SerializeField] private LevelList levelModels;
    [SerializeField] private LevelModel EditorLevel;

    private LevelModel activeLevel;
    private SceneController _sceneController;
    private int maxLevelCount => levelModels.list.Count;
    private static readonly string LevelsPath = $"Assets/_GameAssets/Levels/Levels.json";

    private void Awake()
    {
        if (!initializeOnAwake) return;
        Init();
    }

    public override void Initialize()
    {
        if (initializeOnAwake) return;
        Init();
    }

    private void Init()
    {
        _sceneController = SceneController.Instance;
        DeserializeLevels();
        activeLevel = null;
        ClearScene();
#if UNITY_EDITOR
        LoadLevel(forceLevelIndex >= 0 ? forceLevelIndex : PlayerDataModel.Data.LevelIndex);
#else
        LoadLevel(PlayerDataModel.Data.LevelIndex);
#endif
    }
    
    private void LoadLevel(int levelIndex)
    {
        LoadLevelHelper(levelIndex);
    }
    
    public void NextLevel()
    {
        PlayerDataModel.Data.LevelIndex++;
        LoadLevel(PlayerDataModel.Data.LevelIndex);
    }
 
    public void ReplayLevel()
    {
        LoadLevel(PlayerDataModel.Data.LevelIndex);
    }
    
    #region UTILS
    private void LoadLevelHelper(int levelIndex)
    {
        if (levelIndex < 0) levelIndex = 0;

        levelIndex %= maxLevelCount;

        if (levelModels.list.Count <= 0)
        {
            Debug.LogWarning("LevelModels Empty");
            return;
        }
        ClearScene();
        activeLevel = levelModels.list[levelIndex];
#if UNITY_EDITOR
        EditorLevel = activeLevel;
#endif
        _gridInitializer.InitializeGrid(activeLevel);
        onLevelLoaded?.Invoke(activeLevel);
    }

    #endregion
    public void ClearScene()
    {
        _gridInitializer.ClearScene();
        EditorLevel = null;
        activeLevel = null;
    }
    private void DeserializeLevels()
    {
        levelModels = JsonHelper.LoadJson<LevelList>(jsonLevels.ToString());
    }
#if UNITY_EDITOR

    public void E_LoadLevel(int levelIndex)
    {
        DeserializeLevels();

        LoadLevelHelper(levelIndex);
    }
    
    public void E_Test()
    {
        foreach (var levelModel in levelModels.list)
        {
            levelModel.SelectRandomColors();
        }
        JsonHelper.SaveJson(levelModels, LevelsPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public void E_SaveLevel()
    {
        if(EditorLevel == null || EditorLevel.M < ConstantValues.MINROWS)
        {
            Debug.LogWarning("You don't have any level to Save.");
            return;
        }
        if (activeLevel != null)
        {
            Debug.LogWarning("You have an active level. Try OverrideLevel instead.");
            return;
        }
        DeserializeLevels();
        EditorLevel.name = $"Level {levelModels.list.Count}";
        EditorLevel.index = levelModels.list.Count;
        EditorLevel.SelectRandomColors();
        SaveToJson(EditorLevel, LevelsPath);
        var asset = AssetDatabase.LoadAssetAtPath(LevelsPath, typeof(Object));
        jsonLevels = asset;
        ClearScene();
    }
    
    public void E_OverrideLevel()
    {
        if(levelModels == null) DeserializeLevels();
        if (EditorLevel == null) return;
        if (activeLevel == null) return;
        
        if (activeLevel.SelectedColors.Length <= 0)
        {
            activeLevel.SelectRandomColors();
        }
        SaveToJson(activeLevel, LevelsPath, true);
        E_LoadLevel(activeLevel.index);
    }
    
    private void SaveToJson(LevelModel level, string path, bool _override = false)
    {
        if (_override)
        {
            levelModels.list.Insert(activeLevel.index, level);
            levelModels.list.Remove(activeLevel);
        }
        else levelModels.list.Add(level);
        JsonHelper.SaveJson(levelModels, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelController))]
public class LevelControllerEditor : Editor
{
    private LevelController levelController;
    private int editorLevelIndex = -1;
    private GUIContent saveContent, loadContent, clearSceneContent, overrideContent,TestContent;

    private void OnEnable()
    {
        saveContent = new GUIContent();
        saveContent.text = "Save Level";

        loadContent = new GUIContent();
        loadContent.text = "Load Level";

        overrideContent = new GUIContent();
        overrideContent.text = "Override Level";

        clearSceneContent = new GUIContent();
        clearSceneContent.text = "Clear Scene";
        
        TestContent = new GUIContent();
        TestContent.text = "Test Method";
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorUtils.DrawUILine(Color.white);

        levelController = target as LevelController;

        EditorGUILayout.BeginVertical();

        if (GUILayout.Button(saveContent))
        {
            levelController.E_SaveLevel();
        }
        if (GUILayout.Button(overrideContent))
        {
            levelController.E_OverrideLevel();
        }
        editorLevelIndex = EditorGUILayout.IntField("Loaded Level Index", editorLevelIndex);
        if (GUILayout.Button(loadContent))
        {
            levelController.E_LoadLevel(editorLevelIndex);
        }

        if (GUILayout.Button(clearSceneContent))
        {
            editorLevelIndex = -1;
            levelController.ClearScene();
        }
        
        if (GUILayout.Button(TestContent))
        {
            levelController.E_Test();
        }
        
        EditorGUILayout.EndVertical();
    }
}

#endif


