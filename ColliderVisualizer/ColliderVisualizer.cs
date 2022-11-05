using System;
using System.Collections;
using UnityEngine;

using GizmosLibrary;

namespace ColliderVisualizer
{
    public class ColliderVisualizer : MonoBehaviour
    {
        public float Check_radius = 10f;

        static Color TriggerVolumeColor = Color.green;
        static Color ColliderColor = Color.blue;
        static Color BoundsColor = Color.red;
        static Color ShapeBoundColor = Color.magenta;
        static Color DetectorShapeColor = Color.white;
        static Color VolumeShapeColor = Color.cyan;

        public const int MAX_COLLIDERS_TO_DRAW = 50;
        private Collider[] collidersToDraw = new Collider[MAX_COLLIDERS_TO_DRAW];
        private int amountToDraw = 0;

        public bool IsToDraw = false;
        public string ToggleDrawKBCommand = "f9";

        public float CheckPeriod = 0.25f;

        public bool DrawBoundingBoxes = true;
        public bool DrawTriggers = true;
        public bool DrawColliders = true;

        public bool DrawShapeBounds = true;
        public bool DrawShapeDetector = true;
        public bool DrawShapeVolume = true;
        public int[] ShapeLayersToDraw = new int[] { 1, 2, 3, 4 };

        private void Start()
        {
            StartCoroutine("UpdateCollidersListWithDelay");
        }
        private void Update() 
        {
            if (Event.current != null)
            {
                if (Event.current.Equals(Event.KeyboardEvent(ToggleDrawKBCommand)))
                {
                    IsToDraw = !IsToDraw;
                }
            }
        }

        //TODO fazer um patch e atualizar essa lista com a destruição e construção do Collider
        private void UpdateCollidersList(float radius)
        {
            Transform reference;
            if (Locator.GetActiveCamera() != null)
                reference = Locator.GetActiveCamera().transform;
            else
                reference = Camera.main.transform;

            amountToDraw = Physics.OverlapSphereNonAlloc(reference.position, radius, collidersToDraw, ~0, QueryTriggerInteraction.Collide);
        }
        public void ChangeColliderDrawAmount(int amount = MAX_COLLIDERS_TO_DRAW)
        {
            if (amount > 0)
                collidersToDraw = new Collider[amount];
        }
        private IEnumerator UpdateCollidersListWithDelay()
        {
            while (true)
            {
                UpdateCollidersList(Check_radius);
                yield return new WaitForSeconds(CheckPeriod);
            }
        }

        public void OnRenderObject()
        {
            if (!IsToDraw)
                return;

            GLHelper.SetDefaultMaterialPass();

            if (DrawBoundingBoxes)
            {
                RenderBoundingBoxes(collidersToDraw, amountToDraw);
            }
            RenderColliders(collidersToDraw, amountToDraw, DrawTriggers, DrawColliders);
            if (DrawShapeBounds)
            {
                ShapeManager.Layer layer = ShapeManager._detectors;
                RenderShapeBounds(layer._array, layer.Count);

                ShapeManager.Layer[] layers = ShapeManager._volumes;
                for(int i = 0; i < layers.Length; i++) 
                {
                    RenderShapeBounds(layers[i]._array, layers[i].Count);
                }
            }
            if (DrawShapeDetector)
            {
                ShapeManager.Layer layer = ShapeManager._detectors;
                RenderShapes(layer._array, layer.Count, DetectorShapeColor);
            }
            if (DrawShapeVolume)
            {
                ShapeManager.Layer[] layers = ShapeManager._volumes;
                for (int i = 0; i < ShapeLayersToDraw.Length; i++)
                {
                    int layer = ShapeLayersToDraw[i] - 1;
                    if (layer >= 0 && layer < layers.Length)
                    {
                        RenderShapes(layers[layer]._array, layers[layer].Count, VolumeShapeColor);
                    }
                }
            }
        }
        private void RenderBoundingBoxes(Collider[] colliders, int amountToDraw) 
        {
            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one));
           
            for (int i = 0; i < amountToDraw; i++)
            {
                if (colliders[i] != null)
                    GLHelper.DrawColliderBoundingBox(colliders[i], BoundsColor);
            }
            GL.PopMatrix();
        }
    
        private void RenderColliders(Collider[] colliders, int amountToDraw, bool drawTriggers, bool drawColliders) 
        {
            for (int i = 0; i < amountToDraw; i++)
            {
                if (colliders[i] != null
                    && (colliders[i].isTrigger && drawTriggers || !colliders[i].isTrigger && drawColliders))
                {
                    Color colorToUse = colliders[i].isTrigger ? TriggerVolumeColor : ColliderColor;
                    GL.PushMatrix();
                    GL.MultMatrix(colliders[i].transform.localToWorldMatrix);
                    GLHelper.DrawCollider(colliders[i], colorToUse);
                    GL.PopMatrix();
                }
            }
        }
        
        private void RenderShapeBounds(ShapeManager.ShapeData[] shapes, int count)
        {
            Color colorToUse = ShapeBoundColor;
            for (int i = 0; i < count; i++)
            {
                if (shapes[i] != null)
                {
                    GLHelper.DrawShapeBoundingSphere(shapes[i].shape, colorToUse);
                }
            }
        }
        private void RenderShapes(ShapeManager.ShapeData[] shapes, int count, Color colorToUse)
        {
            for (int i = 0; i < count; i++)
            {
                if (shapes[i] != null)
                {
                    GLHelper.DrawShape(shapes[i].shape, colorToUse);
                }
            }
        }
    }
}
