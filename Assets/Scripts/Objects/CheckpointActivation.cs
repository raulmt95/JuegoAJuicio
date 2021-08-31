using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointActivation : MonoBehaviour
{
    public List<GearRotation> GearsList;
    public GameObject YellowLight; //Poner particula dentro de la bombilla y cambiar la referencia. Dejar la particula desactivada.
    public GameObject RedLight; //Poner particula dentro de la bombilla y cambiar la referencia. Dejar la particula activada.


    //Cuando el player toca el checkpoint acceder a este script y llamar SetCheckpointState(true). Se puede llamar antes (false) al checkpoint anterior si se quiere desactivar.
    public void SetCheckpointState(bool state)
    {
        SetGearRotationState(state);

        YellowLight.SetActive(state);
        RedLight.SetActive(!state);
    }

    void SetGearRotationState(bool state)
    {
        foreach(GearRotation gear in GearsList)
        {
            gear.SetRotation(state);
        }
    }

}
