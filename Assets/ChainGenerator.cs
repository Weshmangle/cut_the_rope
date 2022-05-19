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
        public GameObject m_linkPrefab;
        public GameObject candy;
        public GameObject fragments;
        public HingeJoint2D connectToCandy;
        public Vector3[] positions;
        #endregion

        #region Unity API
        private void Start()
        {
            CreateChain();
        }

        private void Update()
        {
            if(connectToCandy)
            {
                connectToCandy.gameObject.SetActive(candy != null);
            }

            positions = new Vector3[m_chainLinkNumber];
            GetComponent<LineRenderer>().positionCount = (int)m_chainLinkNumber;
            for (int i = 0; i < m_chainLinkNumber; i++)
            {
                positions[i] = fragments.transform.GetChild(i).transform.position;
                GetComponent<LineRenderer>().SetPosition(i, fragments.transform.GetChild(i).transform.position);
            }
        }

        private void CreateChain()
        {
            if (m_linkPrefab == null) return;
            
            var previousChainLink = Instantiate(m_linkPrefab, _offsetPosition, identity, fragments.transform);
            previousChainLink.GetComponent<HingeJoint2D>().connectedAnchor = Vector2.zero;
            AttachToPreviousHinge(gameObject, previousChainLink);
            
            for (var i = 0; i < m_chainLinkNumber; i++)
            {
                previousChainLink = AddNewChainLink(previousChainLink, i);
            }

            if(candy)
            {
                AttacAtCandy(previousChainLink);
            }
        }

        public void AttacAtCandy(GameObject lastChainLink)
        {
            
            HingeJoint2D hingeJoint2D = lastChainLink.AddComponent<HingeJoint2D>();
            hingeJoint2D.autoConfigureConnectedAnchor = false;
            lastChainLink.name = m_chainLinkNumber.ToString();
            hingeJoint2D.connectedBody = candy.GetComponent<Rigidbody2D>();
            hingeJoint2D.connectedAnchor = Vector3.zero;
            connectToCandy = hingeJoint2D;
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
            var chainLink = Instantiate(m_linkPrefab, _offsetPosition, identity, fragments.transform);

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