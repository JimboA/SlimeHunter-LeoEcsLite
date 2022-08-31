using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using JimboA.Plugins.Tween;
using TMPro;
using UnityEngine;

namespace Client.Battle.View.UI
{
    [ExecuteInEditMode]
    public class KillScoreWidget : WidgetBase, IStatView<int>
    {
        private enum ShapeType
        {
            Circle, Box, Rhombus
        };
        
        [SerializeField] private ShapeType _shape;
        [SerializeField, Range(0,1)] private float _valueNormalized;

        [Header("Fill")]
        [SerializeField] private Gradient _lowToHighTransition;

        [Header("Wave")]
        [SerializeField, Range(0,0.1f)] private float _fillWaveAmplitude;
        [SerializeField, Range(0,100f)] private float _fillWaveFrequency;
        [SerializeField, Range(0, 1f)] private float _fillWaveSpeed;

        [Header("Background")]
        [SerializeField] private Color _backgroundColor;

        [Header("Border")]
        [SerializeField, Range(0, 0.15f)] private float _borderWidth;
        [SerializeField] private Color _borderColor;

        [SerializeField] private TextMeshProUGUI scoreValue;
        [SerializeField] private TextMeshProUGUI requiredValue;
        
        private Material _matInstance;


        private int _requiredValue;

        public void OnInit(int amount, EcsWorld world)
        { 
            requiredValue.text = "/" + amount;
            _requiredValue = amount;
            _matInstance = GetComponent<Renderer>().material;
            SetMaterialData();
            SetValue(0);
        }
        
        public void OnUpdate(int amount, EcsWorld world)
        {
            scoreValue.text = amount.ToString();
            var tr = scoreValue.transform;
            tr.DoScale(world, Vector3.one, tr.localScale * 1.2f, 0.2f).Loops(2, true);
            SetValue(amount);
        }

        public override void SetIcon(Texture2D icon)
        {
            throw new System.NotImplementedException();
        }

        private void SetValue(int value)
        {
            float val = (float)value / _requiredValue;
            _matInstance.SetFloat("_valueNormalized", val);
            _matInstance.SetColor("_fillColor", _lowToHighTransition.Evaluate(val));
        }
        
        private void SetMaterialData()
        {
            if (_matInstance == null) 
                return;

            SetKeyword();
            _matInstance.SetFloat("_valueNormalized", _valueNormalized);
            _matInstance.SetFloat("_waveAmp", _fillWaveAmplitude);
            _matInstance.SetFloat("_waveFreq", _fillWaveFrequency);
            _matInstance.SetFloat("_waveSpeed", _fillWaveSpeed);

            _matInstance.SetColor("_fillColor", _lowToHighTransition.Evaluate(_valueNormalized));

            _matInstance.SetColor("_backgroundColor", _backgroundColor);
            _matInstance.SetFloat("_borderWidth", _borderWidth);
            _matInstance.SetColor("_borderColor", _borderColor);
        }
        
        private void SetKeyword()
        {
            foreach (var keyword in _matInstance.shaderKeywords)
            {
                _matInstance.DisableKeyword(keyword);
            }
            
            if (_shape == ShapeType.Circle) 
                _matInstance.EnableKeyword("_SHAPE_CIRCLE");
            else if (_shape == ShapeType.Box) 
                _matInstance.EnableKeyword("_SHAPE_BOX");
            else if (_shape == ShapeType.Rhombus) 
                _matInstance.EnableKeyword("_SHAPE_RHOMBUS");
            
            _matInstance.SetInt("_shape", (int)_shape);
        }

#if UNITY_EDITOR
      
        private void OnValidate()
        {
            SetupUniqueMaterial();
            SetMaterialData();
        }
        
        void SetupUniqueMaterial()
        {
            if (_matInstance != null) return;
            
            _matInstance = new Material(Shader.Find("SlimeHunter/WavesBar"));
            if (Application.isPlaying)
            {
                GetComponent<Renderer>().material = _matInstance;
            }
            else
            {
                GetComponent<Renderer>().sharedMaterial = _matInstance;
            }
        }
        
#endif
    }
}
