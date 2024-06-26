using System;
using System.Collections.Generic;
using System.Reflection;
using MovementPackage.Runtime.Scripts;
using MovementPackage.Runtime.Scripts.CustomAttributes;
using UnityEditor;
using UnityEngine;

namespace MovementPackage.Editor
{
    [CustomEditor(typeof(PlayerMovementComponent))]
    public class PlayerMovementComponentEditor : UnityEditor.Editor
    {
        private readonly List<string> _tabMenus = new();
        private PlayerMovementComponent _target;
        private int _currentTab = 0;
        private bool _foldout;

        private void OnEnable()
        {
            _target = (PlayerMovementComponent)target;
        }

        public override void OnInspectorGUI()
        {
            _tabMenus.Clear();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("playerMovementInputDataSo"));
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lookAtMovementDirection"));
            EditorGUILayout.Separator();
            GUILayout.BeginVertical();

            GUILayout.Label("<color=yellow>Movement Axis</color>",
                new GUIStyle(EditorStyles.boldLabel) { fontSize = 14, richText = true });
            EditorGUILayout.Separator();

            AddToggle("Allow X", ref _target.enableXAxis);
            AddToggle("Allow Y", ref _target.enableYAxis);
            AddToggle("Allow Z", ref _target.enableZAxis);

            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            GUILayout.Label("<color=yellow>Behaviours</color>",
                new GUIStyle(EditorStyles.boldLabel) { fontSize = 14, richText = true });
            EditorGUILayout.Separator();
            DrawToggles();

            EditorGUILayout.Separator();

            DrawMenuWithProperties();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawMenuWithProperties()
        {
            FieldInfo[] fields = _target.GetType().GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            _currentTab = GUILayout.Toolbar(_currentTab, _tabMenus.ToArray());
            foreach (var field in fields)
            {
                var tabMenuAttr = Attribute.GetCustomAttribute(field, typeof(TabMenu)) as TabMenu;

                if (tabMenuAttr != null && tabMenuAttr.TabName == _tabMenus[_currentTab])
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(field.Name));
                }
            }
        }

        private void DrawToggles()
        {
            GUILayout.BeginVertical();

            DrawToggle("Gravity", ref _target.gravityEnabled);
            DrawToggle("Walk", ref _target.walkEnabled);
            DrawJumpSection();
            DrawToggle("Crouch", ref _target.crouchEnabled);
            DrawToggle("Hook", ref _target.hookEnabled);
            DrawToggle("Dash", ref _target.dashEnabled);

            GUILayout.EndVertical();
        }

        private void DrawToggle(string label, ref bool property)
        {
            AddToggle(label, ref property);

            if (!property) return;
            _tabMenus.Add(label);
        }

        private void DrawJumpSection()
        {
            AddToggle("Jump", ref _target.jumpEnabled);
            if (_target.jumpEnabled)
            {
                EditorGUILayout.LabelField("Jump Behaviours", EditorStyles.boldLabel);
                Rect rect = EditorGUILayout.BeginVertical();
                EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f)); // Draw the background color

                AddToggle("Wall Jump", ref _target.wallJumpEnabled);
                AddToggle("Wall Grab", ref _target.wallGrabEnabled);

                _tabMenus.Add("Jump");
                if (_target.wallJumpEnabled)
                    _tabMenus.Add("Wall Jump");

                if (_target.wallGrabEnabled)
                    _tabMenus.Add("Wall Grab");
                EditorGUILayout.EndVertical();
            }
        }

        private void AddToggle(string label, ref bool property)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            GUILayout.Space(30f);
            property = GUILayout.Toolbar(property ? 0 : 1, new[] { "Enabled", "Disabled" }) == 0;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}