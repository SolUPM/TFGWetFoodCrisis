using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(scTile))]
public class ShowcaseTileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        scTile tile = (scTile)target;

        GUILayout.Space(10);

        if(GUILayout.Button("Change Tile type from SELECTED")){
            tile.applyTile();
        }
    }
}
