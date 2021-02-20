using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConstGenSettings", menuName = "Const Generator Setting")]
public class ConstGenSettings : ScriptableObject
{
    [ReadOnly] public bool regenerateOnMissing;
    [ReadOnly] public bool updateOnReload;
    // -----------------------------------------------------------------------------
    [ReadOnly] public List<string> _LAYERS = new List<string>();
    [ReadOnly] public List<string> _TAGS = new List<string>();
    [ReadOnly] public List<string> _SORTINGLAYERS = new List<string>();
    [ReadOnly] public List<string> _SCENES = new List<string>();
    // -----------------------------------------------------------------------------
    [System.Serializable]
    public class Shader_
    {
        public string name;
        public List<string> properties;

        public Shader_() 
        {
            properties = new List<string>();
        }
    }
    [ReadOnly] public List<Shader_> _SHADERPROPS;
    // -----------------------------------------------------------------------------
    [System.Serializable]
    public class ParamsCTRLR
    {
        public string name;
        public List<string> parameters;

        public ParamsCTRLR() 
        {
            parameters = new List<string>();
        }
    }
    [ReadOnly] public List<ParamsCTRLR> _ANIMPARAMS;
    // -----------------------------------------------------------------------------
    [System.Serializable]
    public class LayersCTRLR
    {
        public string name;
        public List<string> layers;

        public LayersCTRLR() 
        {
            layers = new List<string>();
        }
    }
    [ReadOnly] public List<LayersCTRLR> _ANIMLAYERS;
    // -----------------------------------------------------------------------------

    [System.Serializable]
    public class AnimState
    {
        public string name;
        public string tag;
    }

    [System.Serializable]
    public class AnimLayer
    {
        public string name;
        public List<AnimState> animStates;

        public AnimLayer() 
        {
            animStates = new List<AnimState>();
        }        
    }

    [System.Serializable]
    public class StatesCTRLR
    {
        public string name;
        public List<AnimLayer> animLayers;
 
        public StatesCTRLR() 
        {
            animLayers = new List<AnimLayer>();
        }
    }
    [ReadOnly] public List<StatesCTRLR> _ANIMSTATES;
    // -----------------------------------------------------------------------------
    [ReadOnly] public List<string> _NAVAREAS = new List<string>();
}
