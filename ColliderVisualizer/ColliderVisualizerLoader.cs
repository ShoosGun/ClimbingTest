using OWML.ModHelper;
using OWML.Common;
using UnityEngine;
namespace ColliderVisualizer
{
    public class ColliderVisualizerLoader : ModBehaviour
    {
        ColliderVisualizer visualizer;
        int colliderAmount = 50;
        float checkRadius = 10f;
        bool isToDraw = true;

        bool isToDrawColliders = true;
        bool isToDrawTriggers = true;
        bool isToDrawBoundingBoxes = true;

        bool isToDrawShapeBounds = true;
        bool isToDrawShapeDetector = true;
        bool isToDrawShapeVolume = true;
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
            visualizer.CheckPeriod = checkPeriod;

            visualizer.DrawColliders = isToDrawColliders; 
            visualizer.DrawBoundingBoxes = isToDrawBoundingBoxes;
            visualizer.DrawTriggers = isToDrawTriggers;

            visualizer.DrawShapeBounds = isToDrawShapeBounds;
            visualizer.DrawShapeDetector = isToDrawShapeDetector;
            visualizer.DrawShapeVolume = isToDrawShapeVolume;
        }

        public override void Configure(IModConfig config)
        {
            colliderAmount = config.GetSettingsValue<int>("collidersToDraw");
            checkRadius = config.GetSettingsValue<float>("checkRadius");
            isToDraw = config.GetSettingsValue<bool>("isToDraw");

            isToDrawColliders = config.GetSettingsValue<bool>("drawPhysicalColliders");
            isToDrawBoundingBoxes = config.GetSettingsValue<bool>("drawBoundingBoxes");
            isToDrawTriggers = config.GetSettingsValue<bool>("drawTriggers");

            isToDrawShapeBounds = config.GetSettingsValue<bool>("drawShapeBounds");
            isToDrawShapeDetector = config.GetSettingsValue<bool>("drawShapeDetector");
            isToDrawShapeVolume = config.GetSettingsValue<bool>("drawShapeVolume");

            checkPeriod = 1 / config.GetSettingsValue<float>("checkFrequency");
            if (visualizer == null)
                return;

            SetupVisualiser();
        }
    }
}
