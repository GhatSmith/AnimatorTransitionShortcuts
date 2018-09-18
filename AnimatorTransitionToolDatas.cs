using System.Linq;
using UnityEngine;
using UnityEditor.Presets;
using UnityEditor.Animations;
using UnityEditor;
using System.Collections.Generic;


public class AnimatorTransitionToolDatas : ScriptableObject
{
    private static AnimatorTransitionToolDatas instance = null;
    public static AnimatorTransitionToolDatas Instance
    {
        get
        {
            if (instance == null) instance = GetAllInstances<AnimatorTransitionToolDatas>().FirstOrDefault();
            if (instance == null)
            {
                instance = CreateInstance<AnimatorTransitionToolDatas>();
                System.IO.DirectoryInfo scriptDirectoryInfo = new System.IO.DirectoryInfo(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(instance))).Parent;
                string assetPath = scriptDirectoryInfo.FullName.Substring(scriptDirectoryInfo.FullName.IndexOf("Assets\\")) + "/" + typeof(AnimatorTransitionToolDatas).Name + ".asset";
                AssetDatabase.CreateAsset(instance, assetPath);
            }
            return instance;
        }
    }


    public Preset TransitionPreset = null;
    public bool UnselectAfterShortcut = true; // Unselect everything after using shortcut
    public bool OverrideConditions = false; // Chose to override or not transitions conditions


    private bool toolEnabledForThisSession = false;
    public bool ToolEnabledForThisSession { get { return toolEnabledForThisSession; } set { toolEnabledForThisSession = value; } }

    public bool ToolEnabled = false;


    /// <summary> Apply preset to AnimatorStateTransition without overriding destination state (and conditions if OverrideConditions is false) /// </summary>
    public void ApplyPresetTo(AnimatorStateTransition stateTransition)
    {
        if (TransitionPreset == null) return;

        AnimatorState destinationState = stateTransition.destinationState;
        AnimatorCondition[] conditions = stateTransition.conditions;
        TransitionPreset.ApplyTo(stateTransition);
        stateTransition.destinationState = destinationState;
        if (!OverrideConditions) stateTransition.conditions = conditions;
    }

    public static T[] GetAllInstances<T>(bool excludeSubAsset = true) where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name); // FindAssets uses tags check documentation for more info
        List<T> instances = new List<T>(guids.Length);

        for (int i = 0; i < guids.Length; i++) // Probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            T instance = AssetDatabase.LoadAssetAtPath<T>(path);
            if (excludeSubAsset && !AssetDatabase.IsMainAsset(instance)) continue;
            instances.Add(instance);
        }

        return instances.ToArray();
    }
}