using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;


/// <summary> Add shortcuts related to transitions of animator states </summary>
public static class AnimatorTransitionShortcuts
{
    /// <summary> Security popup to ensure that user knows what he is doing </summary>
    private static bool ToolEnabled()
    {
        if (AnimatorTransitionToolDatas.Instance == null) return false;
        if (AnimatorTransitionToolDatas.Instance.ToolEnabled || AnimatorTransitionToolDatas.Instance.ToolEnabledForThisSession) return true;

        int choice = EditorUtility.DisplayDialogComplex("Animator Transition Shortcuts", "Enable Animator Transition Shortcuts ?", "Yes", "No", "Yes forever");

        if (choice == 0) AnimatorTransitionToolDatas.Instance.ToolEnabledForThisSession = true; // Shortcuts enabled for this session
        else if (choice == 1) return false; // Shortcuts not enabled
        else if (choice == 2) AnimatorTransitionToolDatas.Instance.ToolEnabled = true; // Shortcuts enabled forever (choice saved in scriptable object datas)
        else throw new System.NotSupportedException();

        return true;
    }

    /// <summary> Ctrl + T : add one way transitions between two states (using selection ordering) </summary>
    [MenuItem("Tools/Animations/Shortcuts/Animator/Add One Way Transitions %t")]
    static void AddOneWayTransition()
    {
        if (!ToolEnabled()) return;

        if (Selection.objects.Length > 0 && Selection.objects.All(x => x is AnimatorStateTransition)) ApplyPresetToTransitions();
        else if (OrderedSelection.Objects.Count == 2 && OrderedSelection.Objects.All(x => x is AnimatorState))
        {

            AnimatorState fromState = OrderedSelection.Objects[0] as AnimatorState;
            AnimatorState toState = OrderedSelection.Objects[1] as AnimatorState;

            AnimatorStateTransition addedTransition = fromState.AddTransition(toState, true);

            if (AnimatorTransitionToolDatas.Instance != null)
            {
                if (AnimatorTransitionToolDatas.Instance.TransitionPreset != null) AnimatorTransitionToolDatas.Instance.ApplyPresetTo(addedTransition);
                if (AnimatorTransitionToolDatas.Instance.UnselectAfterShortcut) Selection.objects = new Object[0];
            }
        }
    }

    /// <summary> Also called by Ctrl + T shortcut </summary>
    [MenuItem("Tools/Animations/Shortcuts/Animator/Apply Preset To Transitions %t")]
    static void ApplyPresetToTransitions()
    {
        if (!ToolEnabled()) return;

        if (AnimatorTransitionToolDatas.Instance.TransitionPreset != null)
        {
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                AnimatorTransitionToolDatas.Instance.ApplyPresetTo(Selection.objects[i] as AnimatorStateTransition);
            }
        }

        if (AnimatorTransitionToolDatas.Instance.UnselectAfterShortcut) Selection.objects = new Object[0];
    }

    /// <summary> Alt + T : add two ways transitions between two states </summary>
    [MenuItem("Tools/Animations/Shortcuts/Animator/Add Two Ways Transitions &t")]
    static void AddTwoWaysTransition()
    {
        if (!ToolEnabled()) return;

        if (OrderedSelection.Objects.Count != 2 || !OrderedSelection.Objects.All(x => x is AnimatorState)) return;

        AnimatorState fromState = OrderedSelection.Objects[0] as AnimatorState;
        AnimatorState toState = OrderedSelection.Objects[1] as AnimatorState;

        AnimatorStateTransition addedTransition1 = fromState.AddTransition(toState, true);
        AnimatorStateTransition addedTransition2 = toState.AddTransition(fromState, true);

        if (AnimatorTransitionToolDatas.Instance.TransitionPreset != null)
        {
            AnimatorTransitionToolDatas.Instance.ApplyPresetTo(addedTransition1);
            AnimatorTransitionToolDatas.Instance.ApplyPresetTo(addedTransition2);
        }

        if (AnimatorTransitionToolDatas.Instance.UnselectAfterShortcut) Selection.objects = new Object[0];
    }
}