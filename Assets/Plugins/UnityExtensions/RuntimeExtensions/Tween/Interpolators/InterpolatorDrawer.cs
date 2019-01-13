﻿//#if UNITY_EDITOR

//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//namespace UnityExtensions.Editor
//{
//    /// <summary>
//    /// Interpolator Drawer
//    /// </summary>
//    class InterpolatorDrawer : BasePropertyDrawer
//    {
//        // 最后采样时的数据缓存
//        int _lastType;
//        float _lastStrength;

//        float _minValue, _maxValue;
//        List<Vector3> _samples = new List<Vector3>(64);


//        // 显示参数
//        static GUIContent _typeText = new GUIContent("Type");
//        const float _curveRectHeight = 123f;
//        const float _leftPartWidth = 114f;
//        const float _easeBackLabelWidth = 40f;
//        const float _easeBackInOutToggleWidth = 36f;
//        const float _easeBackInOutToggleInterval = 2f;
//        const float _balanceFactorInputWidth = 40f;
//        const float _verticalSpacing = 4f;

//        static Color _curveBorderColor = new Color(0, 0, 0);
//        static Color _curveColor = new Color(1f, 1f, 0f);
//        static Color _range01Color = new Color(0.4f, 0.4f, 0.4f);
//        static Color _curveBackgroundColor = new Color(0.3f, 0.3f, 0.3f);
//        const int _maxCurveSegments = 200;
//        const float _maxCurveError = 0.002f;


//        // 确保采样数据是有效的
//        void ValidateSamples(SerializedProperty property, int type, float strength, int maxSegments, float maxError)
//        {
//            if (_samples.Count == 0
//                    || type != _lastType
//                    || strength != _lastStrength)
//            {
//                _lastType = type;
//                _lastStrength = strength;
//                _samples.Clear();

//                property.propertyPath
//                ReflectionKit.GetPropertyInfo(fieldInfo.GetValue())
//                fieldInfo.get

//                // 添加第一个点

//                Vector3 point = new Vector3(0, Evaluate(0), 0);
//                _samples.Add(point);

//                // 添加其他点

//                Vector3 lastSample = point, lastEvaluate = point;
//                _minValue = _maxValue = point.y;

//                float minSlope = float.MinValue;
//                float maxSlope = float.MaxValue;

//                for (int i = 1; i <= maxSegments; i++)
//                {
//                    point.x = i / (float)maxSegments;
//                    point.y = Evaluate(point.x);

//                    if (_minValue > point.y) _minValue = point.y;
//                    if (_maxValue < point.y) _maxValue = point.y;

//                    maxSlope = Mathf.Min((point.y - lastSample.y + maxError) / (point.x - lastSample.x), maxSlope);
//                    minSlope = Mathf.Max((point.y - lastSample.y - maxError) / (point.x - lastSample.x), minSlope);

//                    if (minSlope >= maxSlope)
//                    {
//                        _samples.Add(lastSample = lastEvaluate);
//                        maxSlope = (point.y - lastSample.y + maxError) / (point.x - lastSample.x);
//                        minSlope = (point.y - lastSample.y - maxError) / (point.x - lastSample.x);
//                    }

//                    lastEvaluate = point;
//                }

//                // 添加最后一个点

//                _samples.Add(point);
//                if (_minValue > point.y) _minValue = point.y;
//                if (_maxValue < point.y) _maxValue = point.y;

//                // 计算绘制的边界值

//                if (_maxValue - _minValue < 1f)
//                {
//                    if (_minValue < 0f)
//                    {
//                        _maxValue = _minValue + 1f;
//                    }
//                    else if (_maxValue > 1f)
//                    {
//                        _minValue = _maxValue - 1f;
//                    }
//                    else
//                    {
//                        _minValue = 0f;
//                        _maxValue = 1f;
//                    }
//                }
//            }
//        }


//        // 绘制曲线
//        void DrawCurve(Rect rect, Color backgroundColor, Color range01Color, Color curveColor, Color borderColor)
//        {
//            EditorGUI.DrawRect(rect, backgroundColor);

//            Vector2 origin = new Vector2(rect.x + 1, rect.y + 1);
//            Vector2 scale = new Vector2(rect.width - 2, (rect.height - 2) / (_maxValue - _minValue));

//            if (_maxValue > 0f && _minValue < 1f)
//            {
//                float yMin = origin.y + (_maxValue - Mathf.Min(_maxValue, 1f)) * scale.y;
//                float yMax = origin.y + (_maxValue - Mathf.Max(_minValue, 0f)) * scale.y;
//                Rect rect01 = new Rect(rect.x, yMin, rect.width, yMax - yMin);
//                EditorGUI.DrawRect(rect01, range01Color);
//            }

//            Vector3 last = _samples[0];
//            last.x = origin.x + last.x * scale.x;
//            last.y = origin.y + (_maxValue - last.y) * scale.y;

//            EditorUI.BeginHandlesColor(curveColor);
//            Vector3 point;

//            for (int i = 1; i < _samples.Count; i++)
//            {
//                point = _samples[i];
//                point.x = origin.x + point.x * scale.x;
//                point.y = origin.y + (_maxValue - point.y) * scale.y;

//                EditorUI.HandlesDrawAALine(last, point);
//                last = point;
//            }

//            EditorUI.EndHandlesColor();
//            EditorUI.DrawWireRect(rect, borderColor);
//        }


//        // 总显示区域高度
//        protected override float Editor_GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            return _curveRectHeight + _verticalSpacing + EditorGUIUtility.singleLineHeight;
//        }


//        // 绘制实现
//        protected override void Editor_OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            // 将存储的 index 转化为菜单项索引
//            int menuIndex = _index;
//            if (menuIndex < 0)
//            {
//                menuIndex = _menuItems.Length - 3 - menuIndex;
//            }

//            // 绘制插值方法类型选项
//            EditorGUI.BeginChangeCheck();

//            position.height = EditorGUIUtility.singleLineHeight;
//            menuIndex = EditorGUI.Popup(position, _typeText, menuIndex, _menuItems);

//            if (EditorGUI.EndChangeCheck())
//            {
//                SerializedProperty indexProperty = property.FindPropertyRelative("_index");

//                if (menuIndex > _menuItems.Length - 3)
//                {
//                    indexProperty.intValue = _menuItems.Length - menuIndex - 3;
//                }
//                else indexProperty.intValue = menuIndex;
//            }

//            position.y += position.height + _verticalSpacing;
//            Rect curveRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, _curveRectHeight);
//            position.width = _leftPartWidth;

//            if (menuIndex == _menuItems.Length - 1)
//            {
//                // 绘制自定义曲线
//                EditorGUI.LabelField(position, "Edit Custom Curve");
//                EditorGUI.PropertyField(curveRect, property.FindPropertyRelative("_curve"), GUIContent.none);
//            }
//            else
//            {
//                // 绘制自定义参数
//                if (menuIndex == _menuItems.Length - 2)
//                {
//                    // 绘制掩码
//                    EditorGUI.BeginChangeCheck();

//                    int maskValue = _adjustableMask;
//                    EditorGUI.LabelField(position, "Ease");

//                    Rect rect = new Rect(position.x + _easeBackLabelWidth, position.y, _easeBackInOutToggleWidth, position.height);
//                    maskValue = Math.SetBit(maskValue, 0, EditorGUI.ToggleLeft(rect, "In", Math.GetBit(maskValue, 0)));

//                    rect.x += _easeBackInOutToggleWidth + _easeBackInOutToggleInterval;
//                    maskValue = Math.SetBit(maskValue, 1, EditorGUI.ToggleLeft(rect, "Out", Math.GetBit(maskValue, 1)));

//                    position.y += position.height;
//                    EditorGUI.LabelField(position, "Back");

//                    rect.Set(position.x + _easeBackLabelWidth, position.y, _easeBackInOutToggleWidth, position.height);
//                    maskValue = Math.SetBit(maskValue, 2, EditorGUI.ToggleLeft(rect, "In", Math.GetBit(maskValue, 2)));

//                    rect.x += _easeBackInOutToggleWidth + _easeBackInOutToggleInterval;
//                    maskValue = Math.SetBit(maskValue, 3, EditorGUI.ToggleLeft(rect, "Out", Math.GetBit(maskValue, 3)));

//                    if (EditorGUI.EndChangeCheck())
//                    {
//                        // 如果掩码发生变化, balance 和 factor 需要重置
//                        property.FindPropertyRelative("_adjustableMask").intValue = maskValue;
//                        property.FindPropertyRelative("_adjustableBalance").floatValue = 0f;
//                        property.FindPropertyRelative("_adjustableFactor").floatValue = _adjustableFactorRanges[maskValue].z;
//                    }

//                    float originalFieldWidth = EditorGUIUtility.fieldWidth;
//                    EditorGUIUtility.fieldWidth = _balanceFactorInputWidth;

//                    // 绘制 balance
//                    EditorGUI.BeginChangeCheck();

//                    position.y += _verticalSpacing + position.height;
//                    EditorGUI.LabelField(position, "Balance");

//                    position.y += position.height;
//                    float balance = EditorGUI.Slider(position, GUIContent.none, _adjustableBalance, -1f, 1f);

//                    if (EditorGUI.EndChangeCheck())
//                    {
//                        property.FindPropertyRelative("_adjustableBalance").floatValue = balance;
//                    }

//                    // 绘制 factor
//                    EditorGUI.BeginChangeCheck();

//                    position.y += _verticalSpacing + position.height;
//                    EditorGUI.LabelField(position, "Factor");

//                    position.y += position.height;
//                    float factor = EditorGUI.Slider(position, GUIContent.none, _adjustableFactor,
//                        _adjustableFactorRanges[maskValue].x, _adjustableFactorRanges[maskValue].y);

//                    if (EditorGUI.EndChangeCheck())
//                    {
//                        property.FindPropertyRelative("_adjustableFactor").floatValue = factor;
//                    }

//                    EditorGUIUtility.fieldWidth = originalFieldWidth;
//                }

//                // 应用改变
//                property.serializedObject.ApplyModifiedProperties();
//                property.serializedObject.Update();

//                // 绘制曲线
//                ValidateSamples(_maxCurveSegments, _maxCurveError);
//                DrawCurve(curveRect, _curveBackgroundColor, _range01Color, _curveColor, _curveBorderColor);
//            }
//        }

//    } // class Interpolator

//} // namespace UnityExtensions

//#endif // UNITY_EDITOR