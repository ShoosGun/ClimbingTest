using OWML.ModHelper;
using OWML.Common;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace ColliderVisualizer
{
    public class ColliderVisualizerLoader : ModBehaviour
    {
        ColliderVisualizer visualizer;
        int colliderAmount = 50;
        float checkRadius = 10f;

        bool isToDraw = true;
        string toggleDrawKB = "f9";

        bool isToDrawColliders = true;
        bool isToDrawTriggers = true;
        bool isToDrawBoundingBoxes = true;

        bool isToDrawShapeBounds = true;
        bool isToDrawShapeDetector = true;
        bool isToDrawShapeVolume = true;
        string shapeLayers = "1-2-3-4";
        float checkPeriod = 0.25f;
        //TODO criar compatibilidade com o UnityExplorer para que, quando o objeto com as colisões seja selecionado, fazer com que apenas elas apareçam
        private void Start()
        {
            visualizer = Camera.main.gameObject.AddComponent<ColliderVisualizer>();
            SetupVisualiser();

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                visualizer = Camera.main.gameObject.AddComponent<ColliderVisualizer>();
                SetupVisualiser();
            };
        }

        private void SetupVisualiser() 
        {
            visualizer.ChangeColliderDrawAmount(colliderAmount);
            visualizer.Check_radius = checkRadius;
            visualizer.IsToDraw = isToDraw;
            visualizer.ToggleDrawKBCommand = toggleDrawKB;
            visualizer.CheckPeriod = checkPeriod;

            visualizer.DrawColliders = isToDrawColliders; 
            visualizer.DrawBoundingBoxes = isToDrawBoundingBoxes;
            visualizer.DrawTriggers = isToDrawTriggers;

            visualizer.DrawShapeBounds = isToDrawShapeBounds;
            visualizer.DrawShapeDetector = isToDrawShapeDetector;
            visualizer.DrawShapeVolume = isToDrawShapeVolume;
            visualizer.ShapeLayersToDraw = GetLayersFromString(shapeLayers, '-', ' ', ':', ',', '.');

        }

        public override void Configure(IModConfig config)
        {
            colliderAmount = config.GetSettingsValue<int>("collidersToDraw");
            checkRadius = config.GetSettingsValue<float>("checkRadius");
            isToDraw = config.GetSettingsValue<bool>("isToDraw");
            toggleDrawKB = config.GetSettingsValue<string>("toggleDrawKB");

            isToDrawColliders = config.GetSettingsValue<bool>("drawPhysicalColliders");
            isToDrawBoundingBoxes = config.GetSettingsValue<bool>("drawBoundingBoxes");
            isToDrawTriggers = config.GetSettingsValue<bool>("drawTriggers");

            isToDrawShapeBounds = config.GetSettingsValue<bool>("drawShapeBounds");
            isToDrawShapeDetector = config.GetSettingsValue<bool>("drawShapeDetector");
            isToDrawShapeVolume = config.GetSettingsValue<bool>("drawShapeVolume");
            shapeLayers = config.GetSettingsValue<string>("shapeLayers");

            checkPeriod = 1 / config.GetSettingsValue<float>("checkFrequency");


            if (visualizer == null)
                return;

            SetupVisualiser();
        }

        private int[] GetLayersFromString(string layersInString, params char[] separators) 
        {
            string[] layersString = layersInString.Split(separators);
            List<int> layers = new List<int>();
            for(int i = 0; i< layersString.Length; i++) 
            {
                if(int.TryParse(layersString[i], out int layer))
                {
                    layers.Add(layer);
                }
            }
            return layers.ToArray();
        }
    }
}
