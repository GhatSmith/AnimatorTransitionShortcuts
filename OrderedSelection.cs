using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[InitializeOnLoad]
public static class OrderedSelection
{
    // For Objects ordered selection
    private static HashSet<Object> objectsSelectionSet = new HashSet<Object>();
    private static HashSet<Object> objectsNewSet = new HashSet<Object>();
    private static HashSet<Object> objectsDeleteSet = new HashSet<Object>();

    private static List<Object> objects = new List<Object>();
    public static List<Object> Objects { get { return objects; } }


    // For GameObjects ordered selection
    private static readonly HashSet<GameObject> gameObjectsSelectionSet = new HashSet<GameObject>();
    private static readonly HashSet<GameObject> gameObjectsNewSet = new HashSet<GameObject>();
    private static readonly HashSet<GameObject> gameObjectsDeleteSet = new HashSet<GameObject>();

    private static List<GameObject> gameObjects = new List<GameObject>();
    public static List<GameObject> GameObjects { get { return gameObjects; } }

    public delegate void OrderedSelectionChangedAction();
    public static event OrderedSelectionChangedAction OnOrderedSelectionChanged;


    static OrderedSelection()
    {
        Selection.selectionChanged -= OnSelectionChange;
        Selection.selectionChanged += OnSelectionChange;
    }

    private static void OnSelectionChange()
    {
        UpdateSelectionObjects();
        UpdateSelectionGameObjects();
        if (OnOrderedSelectionChanged != null) OnOrderedSelectionChanged.Invoke();
    }

    private static void UpdateSelectionObjects()
    {
        objectsNewSet.Clear();
        objectsNewSet.UnionWith(Selection.objects);

        objectsDeleteSet.Clear();
        objectsDeleteSet.UnionWith(objectsSelectionSet);
        objectsDeleteSet.ExceptWith(objectsNewSet);

        foreach (var g in objectsDeleteSet)
        {
            objectsSelectionSet.Remove(g);
            objects.Remove(g);
        }

        objectsNewSet.ExceptWith(objectsSelectionSet);
        foreach (var g in objectsNewSet)
        {
            objectsSelectionSet.Add(g);
            objects.Add(g);
        }
    }

    private static void UpdateSelectionGameObjects()
    {
        gameObjectsNewSet.Clear();
        gameObjectsNewSet.UnionWith(Selection.gameObjects);

        gameObjectsDeleteSet.Clear();
        gameObjectsDeleteSet.UnionWith(gameObjectsSelectionSet);
        gameObjectsDeleteSet.ExceptWith(gameObjectsNewSet);

        foreach (var g in gameObjectsDeleteSet)
        {
            gameObjectsSelectionSet.Remove(g);
            gameObjects.Remove(g);
        }

        gameObjectsNewSet.ExceptWith(gameObjectsSelectionSet);
        foreach (var g in gameObjectsNewSet)
        {
            gameObjectsSelectionSet.Add(g);
            gameObjects.Add(g);
        }
    }
}