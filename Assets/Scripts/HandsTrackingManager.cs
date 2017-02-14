// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity
{
	/// <summary>
	/// HandsManager determines if the hand is currently detected or not.
	/// </summary>
	public partial class HandsTrackingManager : Singleton<HandsTrackingManager>
	{
		public GameObject dstObj;
		public AudioClip upAud;
		public AudioClip downAud;
		public AudioClip leftAud;
		public AudioClip rightAud;
		public AudioClip forwardAud;
		public AudioClip successAud;
        public float targetTime = 1.0f;
        private float proximity_threshold = 0.07f; 

        private AudioSource audio;
		/// <summary>
		/// HandDetected tracks the hand detected state.
		/// Returns true if the list of tracked hands is not empty.
		/// </summary>
		public bool HandDetected
		{
			get { return trackedHands.Count > 0; }
		}
		
		public GameObject TrackingObject;
		
		private HashSet<uint> trackedHands = new HashSet<uint>();
		private Dictionary<uint, GameObject> trackingObject = new Dictionary<uint, GameObject>();
		
		void Awake()
		{
			InteractionManager.SourceDetected += InteractionManager_SourceDetected;
			InteractionManager.SourceLost += InteractionManager_SourceLost;
			InteractionManager.SourceUpdated += InteractionManager_SourceUpdated;
			audio = GetComponent<AudioSource>();
		}

        bool play = false;
        bool success = true;

        void Update()
        {
            if (trackedHands.Count > 1)
            {
                success = true;
            }

            targetTime -= Time.deltaTime;

            if (targetTime <= 0.0f)
            {
                play = true;
            }
        }
        
		private void InteractionManager_SourceUpdated(InteractionSourceState state)
		{
			Vector3 pos;
			if (state.source.kind == InteractionSourceKind.Hand)
			{
				if (trackingObject.ContainsKey(state.source.id))
				{
					if (state.properties.location.TryGetPosition(out pos))
					{
						trackingObject[state.source.id].transform.position = pos;
                        
                        if (success)
                        {
                            var frostedFlakePos = dstObj.transform.position;
                            var handPos = pos;
                            var heading = frostedFlakePos - handPos;
                            
                            if (Mathf.Abs(heading.x) > proximity_threshold)
                            {
                                if (handPos.x > frostedFlakePos.x)
                                {
                                    //print("left");
                                    if (play)
                                    {
                                        audio.clip = leftAud;
                                        audio.Play();
                                        play = false;
                                        targetTime = 1.0f;
                                    }
                                }
                                else if (handPos.x < frostedFlakePos.x)
                                {
                                    //print("right");
                                    if (play)
                                    {
                                        audio.clip = rightAud;
                                        audio.Play();
                                        play = false;
                                        targetTime = 1.0f;
                                    }
                                }
                            }
                            else if (Mathf.Abs(heading.y) > proximity_threshold)
                            {
                                if (handPos.y > frostedFlakePos.y)
                                {
                                    //print("down");
                                    if (play)
                                    {
                                        audio.clip = downAud;
                                        audio.Play();
                                        play = false;
                                        targetTime = 1.0f;
                                    }
                                }
                                else if (handPos.y < frostedFlakePos.y)
                                {
                                    //print("up");
                                    if (play)
                                    {
                                        audio.clip = upAud;
                                        audio.Play();
                                        play = false;
                                        targetTime = 1.0f;
                                    }
                                }
                            }
                            else if (Mathf.Abs(heading.z) > (proximity_threshold + 0.03))
                            {
                                if (handPos.z < frostedFlakePos.z)
                                {
                                    //print("straight");
                                    if (play)
                                    {
                                        audio.clip = forwardAud;
                                        audio.Play();
                                        play = false;
                                        targetTime = 2.0f;
                                    }
                                }
                            }
                            else
                            {
                                //print("done");
                                if (success)
                                {
                                    audio.clip = successAud;
                                    audio.Play();
                                    success = false;
                                }
                            }
                        }
                        
                        //float distLeft = Vector3.Distance(dstObj.transform.position, pos);
                        /*
						if (up)
                        {
                            dstObj.transform.position = pos;
                            up = false;
                        }
                        */
                        /*
                        Vector3 fwd = trackingObject[state.source.id].transform.TransformDirection(Vector3.forward);
                        Debug.DrawRay(trackingObject[state.source.id].transform.position, fwd, Color.green, 2, true);

                        RaycastHit hit;
                        if (Physics.Raycast(trackingObject[state.source.id].transform.position, fwd, out hit, 10))
                        {
                            print("Distance to gameObject: " + hit.distance);
                        }
						*/
                    }
				}
			}
		}
		
		private void InteractionManager_SourceDetected(InteractionSourceState state)
		{
			// Check to see that the source is a hand.
			if (state.source.kind != InteractionSourceKind.Hand)
			{
				return;
			}
			trackedHands.Add(state.source.id);
			
			var obj = Instantiate(TrackingObject) as GameObject;
			Vector3 pos;
			if (state.properties.location.TryGetPosition(out pos))
			{
				//print("Detected");
				obj.transform.position = pos;
            }
			
			trackingObject.Add(state.source.id, obj);            
		}
		
		private void InteractionManager_SourceLost(InteractionSourceState state)
		{
			// Check to see that the source is a hand.
			if (state.source.kind != InteractionSourceKind.Hand)
			{
				return;
			}
			
			if (trackedHands.Contains(state.source.id))
			{
				trackedHands.Remove(state.source.id);
			}
			
			if (trackingObject.ContainsKey(state.source.id))
			{
				var obj = trackingObject[state.source.id];                
				trackingObject.Remove(state.source.id);
				Destroy(obj);
			}
		}
        
		void OnDestroy()
		{
			InteractionManager.SourceDetected -= InteractionManager_SourceDetected;
			InteractionManager.SourceLost -= InteractionManager_SourceLost;
			InteractionManager.SourceUpdated -= InteractionManager_SourceUpdated;
		}
	}
}