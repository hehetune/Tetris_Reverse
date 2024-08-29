using System.Collections.Generic;
using UnityEngine;

namespace ParallaxBackground
{
    [ExecuteInEditMode]
    public class ParallaxLayer : MonoBehaviour
    {
        public float parallaxFactor;
        public float imageHeight; // The width of one image in the layer
        public Camera targetCamera;

        private Queue<Transform> layerImages = new();
        private Transform rightMostImage;

        void Start()
        {
            // Get all child images and store their transforms
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform image = transform.GetChild(i);
                layerImages.Enqueue(image);

                if (i == transform.childCount - 1)
                {
                    rightMostImage = image;
                    imageHeight = transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.y;
                }
            }
        }

        public void Move(float delta)
        {
            Vector3 newPos = transform.localPosition;
            newPos.x -= delta * parallaxFactor;

            transform.localPosition = newPos;

            // Check and reposition images if they go out of view
            RepositionImages();
        }

        private void RepositionImages()
        {
            while (IsOutOfView(layerImages.Peek()))
            {
                // Find the rightmost image
                Transform image = layerImages.Dequeue();

                // Reposition the out-of-view image to the rightmost position
                image.localPosition = new Vector3(rightMostImage.localPosition.x + imageHeight, image.localPosition.y,
                    image.localPosition.z);

                // Bring the repositioned image to the end of the list
                layerImages.Enqueue(image);
                rightMostImage = image;
            }
        }

        private bool IsOutOfView(Transform image)
        {
            float cameraLeftEdge =
                targetCamera.transform.position.y - targetCamera.orthographicSize * targetCamera.aspect;
            float imageRightEdge = image.position.x + imageHeight / 2;
            
            if(imageRightEdge < cameraLeftEdge) Debug.Log("IsOutOfView");

            return imageRightEdge < cameraLeftEdge;
        }

        // private Transform GetRightmostImage()
        // {
        //     Transform rightmostImage = layerImages[0];
        //
        //     foreach (Transform image in layerImages)
        //     {
        //         if (image.localPosition.x > rightmostImage.localPosition.x)
        //         {
        //             rightmostImage = image;
        //         }
        //     }
        //
        //     return rightmostImage;
        // }
    }
}