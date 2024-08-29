using System.Collections.Generic;
using UnityEngine;

namespace ParallaxBackground
{
    [ExecuteInEditMode]
    public class ParallaxBackground : MonoBehaviour
    {
        public ParallaxCamera parallaxCamera;
        List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

        void Start()
        {
            if (parallaxCamera == null)
                parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();

            if (parallaxCamera != null)
                parallaxCamera.onCameraTranslate += Move;

            SetLayers();
        }

        void SetLayers()
        {
            parallaxLayers.Clear();

            Camera targetCamera = parallaxCamera.GetComponent<Camera>();

            for (int i = 0; i < transform.childCount; i++)
            {
                ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();

                if (layer != null)
                {
                    layer.name = "Layer-" + i;
                    parallaxLayers.Add(layer);
                    layer.targetCamera = targetCamera;
                }
            }
        }

        void Move(float delta)
        {
            foreach (ParallaxLayer layer in parallaxLayers)
            {
                layer.Move(delta);
            }
        }
    }
}