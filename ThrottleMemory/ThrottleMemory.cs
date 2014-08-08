/*
 * ThrottleMemory, by Technicalfool.
 * Remembers your throttle while landed.
 */
using System;
using UnityEngine;
/*
 * Remember where the throttle was between going on and off rails.
 */
namespace ThrottleMemory
{
	[KSPAddon(KSPAddon.Startup.Flight, true)]
	public class ThrottleMemory : MonoBehaviour
	{
		private static float oldThrottle = 0.0f; //Remember the throttle here.
		private static bool onRails = false; //Store whether the vessel is on rails.
		private const int DEBUG_LEVEL = 0; //Increase this number to decrease log spam.
		private static bool onFloor = true; //Set true if the vessel is splashed, landed or prelaunch.
		public ThrottleMemory ()
		{

		}
		public void Start()
		{
			/*
			 * Add some event callbacks.
			 */
			GameEvents.onVesselGoOnRails.Add(onVesselGoOnRails);
			GameEvents.onVesselGoOffRails.Add(onVesselGoOffRails);
		}
		public void Update()
		{
			Vessel v = FlightGlobals.ActiveVessel;
			/*
			 * Only remember throttle if vessel is landed, splashed down or not launched.
			 */
			if (
				(v.situation != Vessel.Situations.LANDED) && 
				(v.situation !=	Vessel.Situations.SPLASHED) &&
				(v.situation !=	Vessel.Situations.PRELAUNCH)
			)
			{
				onFloor = false;
			}
			else{
				onFloor = true;
			}
			/*
			 * Remember throttle if all conditions are met.
			 */
			if (!onRails && (TimeWarp.CurrentRate == 1.0f) && onFloor)
			{
				oldThrottle = v.ctrlState.mainThrottle;
			}

		}

		public void onVesselGoOnRails(Vessel v)
		{

			if (DEBUG_LEVEL < 1)
				Debug.Log("[ThrottleMemory] Vessel on rails.");
			if (v == FlightGlobals.ActiveVessel) //Only bother if this is the active vessel.
			{
				onRails = true;
				if (DEBUG_LEVEL < 1)
					Debug.Log("[ThrottleMemory] Vessel is active vessel. oldThrottle: " + oldThrottle);
			}
		}
		public void onVesselGoOffRails(Vessel v)
		{
			if (DEBUG_LEVEL < 1)
				Debug.Log("[ThrottleMemory] Vessel off rails. oldThrottle: " + oldThrottle);
			if ((v == FlightGlobals.ActiveVessel) && onFloor)
			{
				onRails = false;
				v.ctrlState.mainThrottle = oldThrottle; //Set throttle.
				FlightInputHandler.state.mainThrottle = oldThrottle; //Set the on-screen throttle.
			}
		}
	}
}

