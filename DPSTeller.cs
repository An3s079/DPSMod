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
			bool constantlyUpdated = DPSmod.ConstantlyUpdated;
			if (constantlyUpdated)
			{
				DPSMod.DPSmod.addDamageToDPS(0f);
			}
			DPSmod.DPsText.text = "DPS: " + DPSmod.DPS();
			DPSmod.DPsText.fontSize = DPSmod.FontSize;
		}
	}
}
