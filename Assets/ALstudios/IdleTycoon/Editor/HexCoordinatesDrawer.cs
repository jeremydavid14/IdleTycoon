/*
 * HexCoordinatesDrawer
 *
 * Author:
 *      Tom Wright <arclight@alstudios.co.uk>
 *
 * Copyright (c) 2017 Tom Wright
 */

using UnityEditor;
using UnityEngine;

namespace ALStudios.IdleTycoon.Editor
{

    [CustomPropertyDrawer(typeof(HexCoordinates))]
    public class HexCoordinatesDrawer : PropertyDrawer
    {

        #region PropertyDrawer

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HexCoordinates coordinates = new HexCoordinates(
                property.FindPropertyRelative("_x").intValue,
                property.FindPropertyRelative("_z").intValue
            );

            position = EditorGUI.PrefixLabel(position, label);
            GUI.Label(position, coordinates.ToString());
        }

        #endregion
    
    }
    
}