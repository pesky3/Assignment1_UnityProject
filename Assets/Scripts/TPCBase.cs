using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace PGGE
{
    // The base class for all third-person camera controllers
    public abstract class TPCBase
    {
        protected Transform mCameraTransform;
        protected Transform mPlayerTransform;
        private bool isInitialised = false;
        private float distance;



        public Transform CameraTransform
        {
            get
            {
                return mCameraTransform;
            }
        }
        public Transform PlayerTransform
        {
            get
            {
                return mPlayerTransform;
            }
        }

        public TPCBase(Transform cameraTransform, Transform playerTransform)
        {
            mCameraTransform = cameraTransform;
            mPlayerTransform = playerTransform;
        }

        public void RepositionCamera()
        {   
            Vector3 direction = CameraTransform.position - mPlayerTransform.position;
            if (!isInitialised)
            {
                distance = direction.magnitude; //finds the magnitude of the Vector3 with the .magnitude function which utilises the pythagoras theorem
                isInitialised = true;
            }

            if (Physics.Raycast(mPlayerTransform.position, direction, out RaycastHit hit, distance, ObjectConstants.ObjectMask)) 
            {
                Vector3 targetPosition = Vector3.Lerp(hit.point, mPlayerTransform.position, Time.deltaTime * CameraConstants.RepositionStrength);
                mCameraTransform.position = targetPosition;
            }

            Debug.DrawRay(mPlayerTransform.position, direction, Color.red);
        }

        public abstract void Update();
    }
}
