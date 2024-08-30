using System.Collections.Generic;
using UnityEngine;

namespace ParallaxBackground
{
    [ExecuteInEditMode]
    public class ParallaxLayer : MonoBehaviour
    {
        public float parallaxFactor;
        public float imageHeight; // The height of one image in the layer
        public Camera targetCamera;

        private Transform[] layerImages;
        private int topIndex;
        private int bottomIndex;

        void Start()
        {
            layerImages = new Transform[transform.childCount];
            bottomIndex = 0;
            topIndex = layerImages.Length - 1;
            imageHeight = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().bounds.size.y;
            // Get all child images and store their transforms
            for (int i = 0; i < transform.childCount; i++)
            {
                layerImages[i] = transform.GetChild(i);
            }
        }

        public void Move(float delta)
        {
            Vector3 newPos = transform.localPosition;
            newPos.y -= delta * parallaxFactor;

            transform.localPosition = newPos;

            // Check and reposition images if they go out of view
            RepositionImages();
        }

        private void RepositionImages()
        {
            int i = 0;
            while (IsOutOfViewAbove(layerImages[topIndex]) && i < 11)
            {
                Transform image = layerImages[bottomIndex];

                // Reposition the out-of-view image to the bottom
                image.localPosition = new Vector3(image.localPosition.x,
                    layerImages[topIndex].localPosition.y + imageHeight,
                    image.localPosition.z);
                topIndex = bottomIndex;
                bottomIndex++;
                if (bottomIndex == layerImages.Length) bottomIndex = 0;
                i++;
            }

            while (IsOutOfViewBelow(layerImages[bottomIndex]) && i < 11)
            {
                Transform image = layerImages[topIndex];

                // Reposition the out-of-view image to the bottom
                image.localPosition = new Vector3(image.localPosition.x,
                    layerImages[bottomIndex].localPosition.y - imageHeight,
                    image.localPosition.z);
                bottomIndex = topIndex;
                topIndex--;
                if (topIndex == -1) topIndex = layerImages.Length - 1;
                i++;
            }
        }

        private bool IsOutOfViewAbove(Transform image)
        {
            float cameraTopEdge = targetCamera.transform.position.y + targetCamera.orthographicSize * targetCamera.aspect;
            float imageBottomEdge = image.position.y - imageHeight / 2;

            return imageBottomEdge < cameraTopEdge;
        }

        private bool IsOutOfViewBelow(Transform image)
        {
            float cameraBottomEdge = targetCamera.transform.position.y - targetCamera.orthographicSize * targetCamera.aspect;
            float imageTopEdge = image.position.y + imageHeight / 2;

            return imageTopEdge > cameraBottomEdge;
        }
    }
}