using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DPSMod
{
	class DPSTeller : MonoBehaviour
	{
		void Update()
		{
			
			DPSmod.DPsText.text = "DPS: " + DPSCalculator.DPS();
		}
	}
}
