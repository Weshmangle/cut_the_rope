using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using static UnityEngine.Quaternion;

namespace Physics.Runtime
{
    public class ChainGenerator : MonoBehaviour
    {
        #region Exposed

        [Range(1, 250)]
        public uint m_chainLinkNumber;
        public float pourcentage;
        public GameObject m_linkPrefab;
        public GameObject candy;
        public GameObject fragments;
        #endregion

        #region Unity API
        private void Start()
        {
            CreateChain();
        }

        /*
            DistanceJoint2D distanceJoint2D = previousChainLink.AddComponent<DistanceJoint2D>();
            distanceJoint2D.connectedBody = previousChainLink.GetComponent<HingeJoint2D>().connectedBody;
            distanceJoint2D.maxDistanceOnly = true;
            distanceJoint2D.distance = 1;
        */

        private void CreateChain()
        {
            if (m_linkPrefab == null) return;
            
            var previousChainLink = Instantiate(m_linkPrefab, _offsetPosition, identity, transform);
            AttachToPreviousHinge(gameObject, previousChainLink);
            
            for (var i = 0; i < m_chainLinkNumber; i++)
            {
                previousChainLink = AddNewChainLink(previousChainLink, i);
            }
            HingeJoint2D hingeJoint2D = previousChainLink.AddComponent<HingeJoint2D>();
            hingeJoint2D.autoConfigureConnectedAnchor = false;
            previousChainLink.name = m_chainLinkNumber.ToString();

            if(candy)
            {
                hingeJoint2D.connectedBody = candy.GetComponent<Rigidbody2D>();
            }
        }

        #endregion


        #region Main

        private void AttachToPreviousHinge(GameObject previousChainLink, GameObject chainLink)
        {
            if (previousChainLink != null)
            {
                chainLink.GetComponent<HingeJoint2D>().connectedBody = previousChainLink.GetComponent<Rigidbody2D>();
            }
        }

        private GameObject AddNewChainLink(GameObject previousChainLink, int i)
        {
            previousChainLink.name = i.ToString();
            _offsetPosition.y -= 1.2f;
            var chainLink = Instantiate(m_linkPrefab, _offsetPosition, identity, transform);

            AttachToPreviousHinge(previousChainLink, chainLink);

            previousChainLink = chainLink;
            return previousChainLink;
        }

        private void OnDrawGizmos()
        {
            if(Application.isPlaying) return ;
            Handles.color = new Color(.8f, .5f, .2f);
            Handles.DrawWireDisc(transform.position, Vector3.forward, m_chainLinkNumber);

            if(Vector3.Distance(candy.transform.position, transform.position) > m_chainLinkNumber || candy == null) return;
            
            for (var i = 0; i < m_chainLinkNumber; i++)
            {
                Handles.color = i % 2 == 0 ? new Color(.8f, .5f, .2f) : new Color(1f, .7f, .4f);
                Vector3 position = candy.transform.position - transform.position;
                Handles.DrawLine(transform.position + position * (i/(float)m_chainLinkNumber), transform.position + position * ((i + 1) / (float)m_chainLinkNumber), 10);
            }
        }

        #endregion


        #region Private

        private Rigidbody2D _previousRigidBody;
        private Vector2 _offsetPosition = new Vector2(0, 0);

        #endregion
    }
}