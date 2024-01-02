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
            // base.OnInspectorGUI();
            //DrawDefaultInspector();
            _tabMenus.Clear();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("playerMovementInputDataSo"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lookAtMovementDirection"));

            DrawToggles();

            EditorGUILayout.Separator();


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


            serializedObject.ApplyModifiedProperties();
        }

        private void DrawToggles()
        {
            GUILayout.BeginVertical();

            DrawGravity();
            DrawWalk();
            DrawJumpSection();
            DrawCrouch();
            DrawHook();

            GUILayout.EndVertical();
        }

        private void DrawHook()
        {
            AddToggle("Hook", ref _target.hookEnabled);
            AddHookMenu();

            void AddHookMenu()
            {
                if (!_target.hookEnabled) return;
                _tabMenus.Add("Hook");
            }
        }

        private void DrawCrouch()
        {
            AddToggle("Crouch", ref _target.crouchEnabled);
            AddCrouchMenu();

            void AddCrouchMenu()
            {
                if (!_target.crouchEnabled) return;
                _tabMenus.Add("Crouch");
            }
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

        private void DrawGravity()
        {
            AddToggle("Gravity", ref _target.gravityEnabled);
            AddGravityMenu();

            void AddGravityMenu()
            {
                if (_target.gravityEnabled) _tabMenus.Add("Gravity");
            }
        }

        private void DrawWalk()
        {
            AddToggle("Walk", ref _target.walkEnabled);
            AddWalkMenu();

            void AddWalkMenu()
            {
                if (_target.walkEnabled)
                    _tabMenus.Add("Walk");
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